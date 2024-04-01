using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles catch ball behavior.
/// - Triggers haptics when the ball is thrown.
/// - Destroys the ball after a certain number of collisions.
/// - Ends the game when the ball collides with the badger.
/// </summary>
public class CatchBallBehavior : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Will be found if not set")]
    private CatchSceneManager sceneManager;
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool thrown;
    private int thrownCollisionLifetime;

    void Awake() {
        CatchSceneManager.GameStateChanged += OnGameStateChanged;
    }

    void Start() {
        // find if not initialized
        sceneManager = sceneManager == null ? FindObjectOfType<CatchSceneManager>() : sceneManager;

        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        thrown = false;
        thrownCollisionLifetime = 3;

        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        interactable.selectExited.AddListener(TriggerHaptics);
    }

    void OnCollisionEnter(Collision collision)
    {
        // if collides with badger, set game state to End
        if (collision.gameObject.CompareTag("Badger") && sceneManager.GetGameState() == GameState.Catching)
        {
            sceneManager.isBadgerCaught = true;
            sceneManager.ChangeGameState(GameState.End);
        }

        if (thrown) {
            thrownCollisionLifetime -= 1;

            if (thrownCollisionLifetime <= 0) {
                Destroy(gameObject);
            }
        }
    }

    public void TriggerHaptics(BaseInteractionEventArgs args) {
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor) {
            TriggerHaptics(controllerInteractor.xrController);
            thrown = true;
        }
    }

    public void TriggerHaptics(XRBaseController controller) {
        // convert velocity and duration to haptic strength
        float hapticStrength = Mathf.Clamp(rb.velocity.magnitude / 6, 0.1f, 1.0f);
        float hapticDuration = Mathf.Clamp(rb.velocity.magnitude / 20, 0.1f, 0.3f);
        controller.SendHapticImpulse(hapticStrength, hapticDuration);
    }

    public void OnGameStateChanged(GameState newState) {
        // prevent grabbing during start phase
        // TODO: Currently doesn't work, catch balls detach
        // switch (newState) {
        //     case GameState.Start:
        //         grabInteractable.interactionLayers = LayerMask.GetMask("Nothing");
        //         break;
        //     default:
        //         grabInteractable.interactionLayers = LayerMask.GetMask("Default");
        //         break;
        // }
    }
}
