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
    private SceneManager sceneManager;
    private Rigidbody rb;
    private bool thrown;
    private int thrownCollisionLifetime = 3;

    void Start() {
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        interactable.selectExited.AddListener(TriggerHaptics);

        sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        rb = GetComponent<Rigidbody>();
        thrown = false;
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
}
