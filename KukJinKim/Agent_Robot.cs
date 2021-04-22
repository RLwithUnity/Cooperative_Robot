using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.Barracuda;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Agent_Robot : Agent
{
    // Start is called before the first frame update
    public Rigidbody rb_robot;
    public GameObject robot;
    public GameObject box;
    public GameObject SpawnPos;
    public Bounds areaBounds;

    public Transform target;
    public int step;
    public bool collider;
    public bool target_ground;
    public float agent_speed;
    public override void Initialize()
    {
        areaBounds = SpawnPos.GetComponent<Collider>().bounds;
        agent_speed = 5;
    }

public override void OnEpisodeBegin()
    {
        agent_speed = 5;
        step = 0;
        target_ground = false;
        collider = false;
        ResetAgent();
        ResetBoxes();
    }
    public void ResetAgent()
    {
        var RandomPosX = Random.Range(-50f, 50f);
        var RandomPosZ = Random.Range(-50f, 50f);
        var PosY = 6.0f;

        robot.transform.localPosition = new Vector3(RandomPosX, PosY, RandomPosZ);
        robot.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        rb_robot.velocity = Vector3.zero;
        rb_robot.angularVelocity = Vector3.zero;
    }
    public void ResetBoxes()
    {
        var RandomPosX = Random.Range(-areaBounds.extents.x, areaBounds.extents.x);
        var RandomPosZ = Random.Range(-areaBounds.extents.z + 90f, areaBounds.extents.z + 90f);
        var PosY = 3.0f;
        box.transform.localPosition = new Vector3(RandomPosX, PosY, RandomPosZ);
        box.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

    }
        
    public override void CollectObservations(VectorSensor sensor)
    { 
        sensor.AddObservation(robot.transform.localPosition);
        sensor.AddObservation(box.transform.localPosition);
        sensor.AddObservation(target.localPosition);
        }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;   

        var move_action = act[0];

        switch (move_action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
                break;
        }
        robot.transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        rb_robot.AddForce(dirToGo * agent_speed,
            ForceMode.VelocityChange);
    }

    public void MoveBox(ActionSegment<int> act)
    {
        var box_action = act[1];
        var x = robot.transform.localPosition.x;
        var y = robot.transform.localPosition.y;
        var z = robot.transform.localPosition.z;
        switch (box_action)
        {
            //case 1:
            //    box.transform.position = new Vector3(x, y + 10f, z);
            //    // 박스를 에이전트 머리 위로 이동시키기
            //    break;
            case 1:

                // 박스를 에이전트 앞으로 내려놓기
                box.transform.localPosition = new Vector3(x, y+8f, z-8f);
                AddReward(0.001f);
                break;
            default:
                break;
        }
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        step += 1;
        MoveAgent(actionBuffers.DiscreteActions);
        if (collider == true)
        {
            MoveBox(actionBuffers.DiscreteActions);
        }
        if(box.transform.position.y < 0)
        {
            Debug.Log("Box isn't in area");
            SetReward(-1f);
            EndEpisode();
        }
        AddReward(-1f / MaxStep);

        if (target_ground == true)
        {
            Debug.Log("Sucess!!!");
            SetReward(1.0f);
            EndEpisode();
        }
    }
    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "box")
        {

            collider = true;
        }
        else
        {
            collider = false;
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            discreteActionsOut[1] = 1;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            //  discreteActionsOut[1] = 2;
        }

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
