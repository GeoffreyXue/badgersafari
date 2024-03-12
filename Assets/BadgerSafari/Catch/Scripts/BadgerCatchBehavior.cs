using UnityEngine;

public class BadgerCatchBehavior : MonoBehaviour
{
    private SceneManager sceneManager;

    void Start() {
        sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
    }

    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        // if collides with catch ball, set game state to End
        if (sceneManager.gameState == GameState.Catching)
        {
            sceneManager.isBadgerCaught = true;
            if (collision.gameObject.CompareTag("Catch Ball"))
            {
                sceneManager.gameState = GameState.End;
            }
        }
    }
}
