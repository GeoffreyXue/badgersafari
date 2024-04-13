using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles home badger behavior.
/// </summary>
// https://www.youtube.com/watch?v=dYs0WRzzoRc
public class HomeBadgerBehavior : MonoBehaviour
{
    public BadgerData badgerData;
    public float waitTimeAverage = 5f;
    public float waitTimeRange = 2f;
    public float range = 3f ; //radius of sphere

    public Transform centrePoint; //centre of the area the agent wants to move around in
    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area

    private Animator animator;
    private NavMeshAgent agent;
    private System.Diagnostics.Stopwatch stopwatchWait = new();
    private System.Diagnostics.Stopwatch stopwatchMax = new();
    private float waitTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        animator.SetBool("Caught", false);
        animator.SetBool("Walk", false);

        waitTime = 0;
        centrePoint = agent.transform;
    }

    void Update()
    {
        // once finished waiting, navigate to a new random point
        if (stopwatchWait.Elapsed.TotalSeconds >= waitTime)
        {
            stopwatchWait.Stop();
            stopwatchWait.Reset();

            stopwatchMax.Start();
            animator.SetBool("Walk", true);

            NavigateToRandomPoint();
            Debug.DrawRay(agent.transform.position, agent.destination - agent.transform.position, Color.red, 1f);
        }

        // if close enough or done with path, pause for a certain amount of time
        if (agent.remainingDistance <= (agent.stoppingDistance + (stopwatchMax.Elapsed.TotalSeconds / 100)) && stopwatchWait.IsRunning == false)
        {
            stopwatchMax.Stop();
            stopwatchMax.Reset();
            animator.SetBool("Walk", false);

            waitTime = Random.Range(waitTimeAverage - (waitTimeRange / 2), waitTimeAverage + (waitTimeRange / 2));
            stopwatchWait.Start();
        }
    }

    void NavigateToRandomPoint()
    {
        Vector3 randomPoint;
        if (RandomPoint(centrePoint.position, range, out randomPoint))
        {
            agent.SetDestination(randomPoint);
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    
}