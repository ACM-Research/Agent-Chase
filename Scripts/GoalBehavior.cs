using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class GoalBehavior : Agent
{
    public static GoalBehavior Instance;
    public GameObject environmentWithSpawn;
    private Spawner spawner;
    private int episodeCount = 0;
    [HideInInspector]
    public bool goalReached;

    private void Start()
    {
        Instance= this;
        spawner = environmentWithSpawn.GetComponent<Spawner>();
    }
    public override void OnEpisodeBegin()
    {
        episodeCount++;
        
        spawner.moveToSafePosition(gameObject, transform, false);
        //Debug.Log("New Episode (Goal): " + episodeCount);
        goalReached = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Runner>(out Runner runner))
        {
            Debug.Log("-------------------------------------------------------- GOAL REACHED ");
            goalReached = true;
            EndEpisode();
        }
        //add more penalties for the goalie
    }
}
