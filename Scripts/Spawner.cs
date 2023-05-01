using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    [Header("Agents")]
    public GameObject runner;
    public GameObject goalie;
    public GameObject goal;

    [Header("Invisible Borders")]
    public Transform borderTop;
    public Transform borderBottom;
    public Transform borderRight;
    public Transform borderLeft;

    private Transform goalLoc;
    private Transform runnerLoc;

    // Start is called before the first frame update
    void Start()
    {
        goalLoc = goal.GetComponent<Transform>();
        runnerLoc = runner.GetComponent<Transform>();
        //goalieSpot = goalie.GetComponent<Transform>();
        //moveToSafePosition(goal, goalSpot);
       //moveToSafePosition(runner, runnerSpot);
        //moveToSafePosition(goalie, goalieSpot);
    }

    /*NOTE: Instantiation does not seem to be necessary as long as the gameObject already exists prior to running.
     * But I'll leave it commented is case it must be referenced later --Michael
    
    void spawnGoal() //
    {
        // Instantiate the goal at (x, y, z)
        Instantiate(goal,
                    Vector3.zero,
                    Quaternion.identity); // default rotation/no rotation

    }*/

    public void moveToSafePosition(GameObject obj, Transform t, bool spawnClose)
    {
        bool safePositionfound = false;
        int attemptsRemaining = 100; //Prevent an infinite looop
        Vector3 potentialPosition = Vector3.zero;

        //Loop until a safe positon is found to spawn, or we run out of attempts
        while (!safePositionfound && attemptsRemaining > 0)
        {
            attemptsRemaining--;
            //Debug.Log("Attempts remaining: " + attemptsRemaining);

            /*if (spawnClose)
            {
                float distanceFromObj = UnityEngine.Random.Range(.1f, .2f);

                if(obj = goalie)
                {
                    Debug.Log("spawning GOALIE CLOSE");
                    potentialPosition = runner.transform.position + new Vector3(distanceFromObj, t.localScale.y, distanceFromObj);
                }
                else if(obj = runner)
                {
                    Debug.Log("spawning GOALIE CLOSE");
                    potentialPosition = goal.transform.position + new Vector3 (distanceFromObj, t.localScale.y, distanceFromObj);
                }
                
            }
            else*/
            {
                // x position between left & right border
                float x = Random.Range(borderLeft.position.x,
                                          borderRight.position.x);

                // z position between top & bottom border
                float z = Random.Range(borderBottom.position.z,
                                          borderTop.position.z);
                //int height = 9; //Don't wanna use this, it would be hard-coding

                potentialPosition = new Vector3(x, t.localScale.y, z);
                t.position = potentialPosition;
            }

            //Check to see if the GameObject will collide with anything
            Collider[] colliders = Physics.OverlapBox(t.position, t.localScale / 2, Quaternion.identity);

            //Safe position has been found if no colliders overlap
            safePositionfound = colliders.Length == 0;
        }

        Debug.Assert(safePositionfound, "could not find a safe position to spawn " + obj);

        //Set the position and rotation
        t.position = potentialPosition;

        //If the gameObject being moved is the runner or goalie agents, their rotation will be randomized as well
        if (obj == runner)
        {
            var euler = transform.eulerAngles;
            euler.y = Random.Range(0f, 360f);
            obj.transform.eulerAngles = euler;


        }
        else if (obj == goalie)
        {
            var euler = transform.eulerAngles;
            euler.y = Random.Range(0f, 360f);
            obj.transform.eulerAngles = euler;


        }
        else
        {
            t.rotation = Quaternion.identity;
        }
    }
}
