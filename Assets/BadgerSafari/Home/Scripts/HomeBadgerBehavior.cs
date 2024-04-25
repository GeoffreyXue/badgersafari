using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles home badger behavior.
/// Should be attached to a badger in the home scene.
/// https://www.youtube.com/watch?v=dYs0WRzzoRc
/// </summary>
public class HomeBadgerBehavior : MonoBehaviour {
    public float waitTimeAverage = 5f;
    public float waitTimeRange = 2f;
    public float range = 3f ; //radius of sphere

    public Transform centrePoint; //centre of the area the agent wants to move around in
    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area

    private Animator animator;
    private NavMeshAgent agent;
    private System.Diagnostics.Stopwatch stopwatchWaiting = new();
    private System.Diagnostics.Stopwatch stopwatchMoving = new();
    private float waitTime;

    private GameObject infoPanel;
    private XRBaseInteractable interactable;

    public void Init(BadgerData badgerData, GameObject infoPanelPrefab) {
        // instantiate info panel under gameobject
        infoPanel = Instantiate(infoPanelPrefab, transform);

        infoPanel.SetActive(false);
        TextMeshProUGUI[] texts = infoPanel.GetComponentsInChildren<TextMeshProUGUI>();
        texts[0].text = badgerData.name;
        // if date caught, set date caught
        texts[1].text = $"Caught: {(badgerData.dateCaught != null ? badgerData.dateCaught.ToString("MM/dd/yyyy") : "unknown")}";
        texts[2].text = $"Fav food: {badgerData.favoriteFood}";
        
        // Set up awake on hover
        interactable = gameObject.GetOrAddComponent<XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(args => OnControllerInteraction(args, _ => {
            infoPanel.SetActive(true);
            infoPanel.transform.LookAt(Camera.main.transform);
        }));
        interactable.hoverExited.AddListener(args => OnControllerInteraction(args, _ => infoPanel.SetActive(false)));
    }

    void Start()
    {
        if (!gameObject.TryGetComponent(out animator)) {
            Debug.LogError($"No {typeof(Animator)} found on badger");
        }

        if (!gameObject.TryGetComponent(out agent)) {
            Debug.LogError($"No {typeof(NavMeshAgent)} found on badger");
            return;
        }

        // animator.SetBool("Walk", false);

        waitTime = 0;
        centrePoint = agent.transform;

        stopwatchMoving.Reset();
        stopwatchWaiting.Reset();
    }

    void Update()
    {
        if (interactable.interactorsHovering.Count > 0) {
            infoPanel.transform.LookAt(Camera.main.transform);
        }

        // once finished waiting, navigate to a new random point
        if (stopwatchWaiting.Elapsed.TotalSeconds >= waitTime && stopwatchMoving.IsRunning == false)
        {
            stopwatchWaiting.Stop();
            stopwatchWaiting.Reset();

            stopwatchMoving.Start();
            // animator.SetBool("Walk", true);

            NavigateToRandomPoint();
            Debug.DrawRay(agent.transform.position, agent.destination - agent.transform.position, Color.red, 1f);
        }

        // if close enough or done with path, pause for a certain amount of time
        if (agent.remainingDistance <= (agent.stoppingDistance + (stopwatchMoving.Elapsed.TotalSeconds / 100)) && stopwatchWaiting.IsRunning == false)
        {
            stopwatchMoving.Stop();
            stopwatchMoving.Reset();
            // animator.SetBool("Walk", false);

            waitTime = Random.Range(waitTimeAverage - (waitTimeRange / 2), waitTimeAverage + (waitTimeRange / 2));
            stopwatchWaiting.Start();
        }
    }

    private void OnControllerInteraction(BaseInteractionEventArgs args, UnityAction<XRBaseController> action) {
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor) {
            action(controllerInteractor.xrController);
        }
    }

    private void NavigateToRandomPoint()
    {
        Vector3 randomPoint;
        if (RandomPoint(centrePoint.position, range, out randomPoint))
        {
            agent.SetDestination(randomPoint);
        }
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
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


