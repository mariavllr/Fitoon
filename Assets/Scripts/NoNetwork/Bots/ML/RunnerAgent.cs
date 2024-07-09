using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RunnerAgent : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private bool training = false;

    private NPCController controller;
    private Vector3 diff;
    private Vector3 lastDiff;
    [SerializeField] private HashSet<GameObject> checkpointsPassedSet;

    public override void Initialize()
    {
        controller = GetComponent<NPCController>();
        checkpointsPassedSet = new HashSet<GameObject>();
        target = FindObjectOfType<GoalController>().transform;
    }

    public override void OnEpisodeBegin()
    {
        controller.enabled = true;
        controller.SetBoost(1f);
        target.GetComponent<Collider>().enabled = true;
        checkpointsPassedSet.Clear();

        lastDiff = (target.transform.localPosition - transform.localPosition) / 20;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        diff = (target.transform.localPosition - transform.localPosition) / 20;
        sensor.AddObservation(diff);

        if (diff.magnitude < lastDiff.magnitude) AddReward(0.1f);
        if (diff.magnitude > lastDiff.magnitude) AddReward(-0.1f);
        lastDiff = diff;

        //AddReward(-0.001f); //Encourage the player to move faster
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        float moveV = ScaleAction(actions.ContinuousActions[0], 0.0f, 1.0f);
        float moveH = ScaleAction(actions.ContinuousActions[1], -1.0f, 1.0f);
        controller.SetMovement(moveV, moveH);

        if (moveV == 0 && moveH == 0)
        {
            AddReward(-0.1f); // Penalize for staying still
        }

        if (GetCumulativeReward() < -1000)
        {
            checkpointsPassedSet.Clear();
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Vertical");
        continuousActions[1] = Input.GetAxisRaw("Horizontal");
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Goal"))
        {
            AddReward(500f);
            EndEpisode();
        }
        if (collider.gameObject.CompareTag("POI"))
        {
            AddReward(20f);
        }
        if (collider.gameObject.CompareTag("Checkpoint"))
        {
            if (checkpointsPassedSet.Contains(collider.gameObject))
            {
                AddReward(-5f); // Penalize for reaching a repeated CheckPoint
                EndEpisode();
            }
            else
            {
                checkpointsPassedSet.Add(collider.gameObject);
                AddReward(50f); // Reward for reaching a new CheckPoint
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (training)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                AddReward(-0.1f); // Apply a small penalty continuously
            }
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                AddReward(-0.05f); // Apply a small penalty continuously
            }
            if (collision.gameObject.CompareTag("Player"))
            {
                AddReward(-0.001f); // Apply a small penalty continuously
            }
        }
    }
}
