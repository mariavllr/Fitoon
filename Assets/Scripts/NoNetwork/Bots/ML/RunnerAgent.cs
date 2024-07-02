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
    //[SerializeField] private CreateBots createBots;
    private Vector3 diff;
    private Vector3 lastDiff;
    [SerializeField] private HashSet<GameObject> checkpointsPassedSet;

    public override void Initialize()
    {
        controller = GetComponent<NPCController>();
        checkpointsPassedSet = new HashSet<GameObject>();
        target = FindObjectOfType<GoalController>().transform;
        //createBots = GetComponentInParent<CreateBots>();
    }

    public override void OnEpisodeBegin()
    {
        //if (training)
        //{
        //    transform.localRotation = Quaternion.identity;
        //    transform.localPosition = new Vector3(Random.Range(-20f, 20f), 1f, 0f);
        //}
        //createBots.SpawnBots();
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

        if (diff.magnitude < lastDiff.magnitude) AddReward(0.0001f);
        if (diff.magnitude > lastDiff.magnitude) AddReward(-0.01f);
        lastDiff = diff;

        AddReward(-0.001f); //Encourage the player to move faster
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        float moveV = ScaleAction(actions.ContinuousActions[0], 0.0f, 1.0f);
        float moveH = ScaleAction(actions.ContinuousActions[1], -1.0f, 1.0f);
        controller.SetMovement(moveV, moveH);

        //Penalize for staying still
        if (moveV <= 0)
        {
            AddReward(-10f);
        }

        //Reiniciar si va mal
        if (GetCumulativeReward() < -2000)
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
            AddReward(1000f);
            EndEpisode();
        }
        if (collider.gameObject.CompareTag("POI"))
        {
            AddReward(200f);
        }
        if (collider.gameObject.CompareTag("Checkpoint"))
        {
            if (checkpointsPassedSet.Contains(collider.gameObject))
            {
                AddReward(-100f); // Penalize for reaching a repeated CheckPoint
                EndEpisode();
            }
            else
            {
                checkpointsPassedSet.Add(collider.gameObject);
                AddReward(100f); // Reward for reaching a new CheckPoint
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (training)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                AddReward(-10f); //Penalization for colliding with a wall
            }
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                AddReward(-5f); //Penalization for colliding with an obstacle
            }
            if (collision.gameObject.CompareTag("Character"))
            {
                AddReward(-1f); //Penalization for colliding with another NPC too much time
            }
        }
    }
}
