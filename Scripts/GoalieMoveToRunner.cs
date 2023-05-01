using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoalieMoveToRunner : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject goalGameObject;
    private Transform goalTransform;
    //public GoalBehavior gb;
    private GoalBehavior goalBehavior;
    private Spawner spawner;

    [Tooltip("Whether this is training or gameplay mode")]
    public bool trainingMode;

    [Header("Spawn")]
    public GameObject environmentWithSpawn;

    [Header("Movement Speed")]
    public float moveSpeed = 1f;
    public float groundFriction;
    public LayerMask ground; //This is a layer in unity that will be assigned to objects that will cause friction

    [Header("Raycasting")]
    public GameObject targetRef;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public float radius;
    [HideInInspector]
    public bool canSeePlayer;

    [Header("Set Rewards")]
    [SerializeField] private float walkTowards = 0.02f;
    [SerializeField] private float walkAway = -0.01f;
    [SerializeField] private float win = 1f;
    [SerializeField] private float lose = -1f;
    [SerializeField] private float hitWall = -0.5f;
    [SerializeField] private float timePass = -0.001f;
    //[SerializeField] private float inSight = 0.01f;

    private int episodeCount = 0;
    private double timeTotal = 0;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool grounded;
    private float distanceToTarget;
    [HideInInspector] public bool lost;


    private void Start()
    {
        goalBehavior = goalGameObject.GetComponent<GoalBehavior>();
        goalTransform = goalGameObject.transform;
        spawner = environmentWithSpawn.GetComponent<Spawner>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; //Ensures agent isn't spinning willy nilly
    }
    public override void OnEpisodeBegin()
    {
        lost = false;
        episodeCount++;
        //Debug.Log("New Episode (GOALIE): " + episodeCount);

        timeTotal = Time.time;
        //transform.localPosition = Vector3.zero;
        bool inFrontofRunner = true;
        if (trainingMode)
        {
            //Spawn in front of goal 50% of the time during training
            inFrontofRunner = UnityEngine.Random.value > .5f;
        }

        //rb.freezeRotation = true; //Ensures agent isn't spinning willy nilly
        rb.velocity = Vector3.zero; //Make sure agent isn't moving when they spawn
        spawner.moveToSafePosition(this.gameObject, this.transform, inFrontofRunner);

    }

    /*private void FixedUpdate()
        {
            if (gb.goalReached)
            {
                SetReward(-1f);
                EndEpisode();
            }
        }*/

    private void Update()
    {
        if ((Time.time - timeTotal) > 0.1f)
        {
            SetReward(timePass);
        }
        if (goalBehavior.goalReached)
        {
            //Debug.Log("-------------------------------------------------------- MISSION FAILURE");
            SetReward(lose);
            EndEpisode();
        }

        //Check if touching "ground"
        grounded = Physics.Raycast
            (transform.position, Vector3.down, 1.5f * 0.5f + 0.2f, ground);

        SpeedControl();

        //Apply fricton from "ground" layer
        if (grounded)
        {
            rb.drag = groundFriction;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        LookForward();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(goalTransform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        //transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        moveDirection = transform.forward * moveZ + transform.right * moveX;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        Vector3 predictedPosition = rb.velocity / 24 + transform.position;
        distanceToTarget = Vector3.Distance(transform.position, targetRef.transform.position);
        float predictedDistance = Vector3.Distance(predictedPosition, targetRef.transform.position);
        if (predictedDistance < distanceToTarget)
        {
            SetReward(walkTowards);
        }
        else
        {
            SetReward(walkAway);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacle")//other.TryGetComponent<Wall>(out Wall wall)
        {
            SetReward(hitWall);
        }
        if (other.TryGetComponent<Runner>(out Runner runner))
        {
            //Debug.Log("---------------------------- TARGET AQUIRED ----------------------------");
            SetReward(win);
            EndEpisode();
        }
        /*if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(-1f);
        }*/
        //add more penalties for the goalie
    }

    //private IEnumerator FOVRoutine()
    //{
    //    WaitForSeconds wait = new WaitForSeconds(0.2f);

    //    while (true)
    //    {
    //        yield return wait;
    //        FieldOfViewCheck();
    //    }
    //}

    //private void FieldOfViewCheck()
    //{
    //    Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;

    //    float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

    //    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
    //    {
    //        SetReward(inSight);
    //    }
    //}


    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //Cap velocity if necessary
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private void LookForward()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 70f * Time.deltaTime);
        }
    }
}
