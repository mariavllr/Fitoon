using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RunnerAgent : Agent
{
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private Transform target;
    //[SerializeField] private List<GameObject> spawnpointsList;
    [SerializeField] private List<GameObject> checkpointsPassedList;

    //private BotControl controller;
    private Vector3 diff;
    private Vector3 lastDiff;

    public override void Initialize()
    {
        //controller = GetComponent<BotControl>();
    }

    public override void OnEpisodeBegin()
    {
        //controller.enabled = true;
        //controller.SetBoost(1f);
        target.GetComponent<Collider>().enabled = true;
        checkpointsPassedList.Clear();

        lastDiff = (target.transform.localPosition - transform.localPosition) / 20;
        transform.localRotation = Quaternion.identity;
        //int randSP = Random.Range(0, spawnpointsList.Count - 1);
        //transform.localPosition = new Vector3(spawnpointsList[randSP].transform.localPosition.x, spawnpointsList[randSP].transform.localPosition.y + 1, spawnpointsList[randSP].transform.localPosition.z);

        transform.localPosition = new Vector3(Random.Range(-20f, 20f), 0.5f, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        diff = (target.transform.localPosition - transform.localPosition) / 20;
        sensor.AddObservation(diff);


        if (diff.magnitude < lastDiff.magnitude) AddReward(0.0001f);
        if (diff.magnitude > lastDiff.magnitude) AddReward(-0.01f);
        lastDiff = diff;


        //AddReward(-0.001f); //Encourage the player to move faster
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        float moveV = ScaleAction(actions.ContinuousActions[0], 0.0f, 1.0f);
        float moveH = ScaleAction(actions.ContinuousActions[1], -1.0f, 1.0f);
        //controller.SetMovement(moveV, moveH);

        transform.localPosition += new Vector3(moveH, 0, moveV) * Time.deltaTime * movementSpeed;

        //Penalize for staying still
        if (moveV <= 0)
        {
            AddReward(-10f);
        }

        if (GetCumulativeReward() < -10) EndEpisode();
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
        if (collider.gameObject.CompareTag("Checkpoint"))
        {
            int counter = 0;

            foreach (GameObject go in checkpointsPassedList)
            {
                if (go == collider.gameObject)
                {
                    AddReward(-100f); //Penalize for reaching a repeated CheckPoint
                    EndEpisode();
                    //Debug.Log("REPEATED CP");
                }
                else
                {
                    counter++;
                }
            }

            if (counter == checkpointsPassedList.Count)
            {
                checkpointsPassedList.Add(collider.gameObject);
                AddReward(100f); //Reward for reaching a CheckPoint
                //Debug.Log("NEW CP");
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-10f); //Penalization for colliding with a wall
        }
    }
}
