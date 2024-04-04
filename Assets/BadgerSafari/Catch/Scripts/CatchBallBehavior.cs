using System.Collections;
using UnityEngine;
using UnityEngine.Events;
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
    private AudioClip audioPickup;
    [SerializeField]
    private AudioClip audioThrow;
    [SerializeField]
    private AudioClip audioBounce;
    [SerializeField]
    [Tooltip("Will be found if not set")]
    private CatchSceneManager sceneManager;
    private Rigidbody rb;
    private AudioSource audioSource;
    private bool thrown;
    private int thrownCollisionLifetime;

    void Start() {
        // find if not initialized
        sceneManager = sceneManager == null ? FindObjectOfType<CatchSceneManager>() : sceneManager;

        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        thrown = false;
        thrownCollisionLifetime = 3;

        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(args => OnControllerInteraction(args, _ => audioSource.PlayOneShot(audioPickup)));
        interactable.selectExited.AddListener(args => OnControllerInteraction(args, _ => audioSource.PlayOneShot(audioThrow)));
        interactable.selectExited.AddListener(args => OnControllerInteraction(args, TriggerHaptics));
    }

    void OnCollisionEnter(Collision collision)
    {
        // if collides with badger, set game state to End
        if (thrown && collision.gameObject.CompareTag("Badger") && sceneManager.GetGameState() == GameState.Catching)
        {
            sceneManager.isBadgerCaught = true;
            sceneManager.ChangeGameState(GameState.End);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("World Obstacles") || collision.gameObject.CompareTag("Badger"))
        {
            if (audioSource.isPlaying && audioSource.clip == audioBounce)
            {
                audioSource.Stop();
            }
            audioSource.PlayOneShot(audioBounce);
        }
    }

    private void OnControllerInteraction(BaseInteractionEventArgs args, UnityAction<XRBaseController> action) {
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor) {
            action(controllerInteractor.xrController);
        }
    }

    private void TriggerHaptics(XRBaseController controller) {
        // convert velocity and duration to haptic strength
        float hapticStrength = Mathf.Clamp(rb.velocity.magnitude / 6, 0.1f, 1.0f);
        float hapticDuration = Mathf.Clamp(rb.velocity.magnitude / 20, 0.1f, 0.3f);
        controller.SendHapticImpulse(hapticStrength, hapticDuration);

        // consider thrown, start timeout
        thrown = true;
        StartCoroutine(DestroyObjectAfterDelay());
    }

    private IEnumerator DestroyObjectAfterDelay()
    {
        // Wait for 3 seconds
        yield return new WaitForSeconds(thrownCollisionLifetime);

        // Destroy the object
        Destroy(gameObject);
    }
}
