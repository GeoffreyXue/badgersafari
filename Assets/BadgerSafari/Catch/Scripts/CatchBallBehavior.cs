using UnityEngine;

public class CatchBallBehavior : MonoBehaviour
{
    private SceneManager sceneManager;

    void Start() {
        sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        Debug.Log(sceneManager.GetGameState());
    }

    void OnCollisionEnter(Collision collision)
    {
        // if collides with badger, set game state to End
        if (collision.gameObject.CompareTag("Badger") && sceneManager.GetGameState() == GameState.Catching)
        {
            sceneManager.isBadgerCaught = true;
            sceneManager.ChangeGameState(GameState.End);
        }
    }
}
