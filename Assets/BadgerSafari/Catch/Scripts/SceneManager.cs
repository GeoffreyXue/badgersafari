using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject badgerPrefab;

    private float spawnXRange = 10;
    private float spawnZRange = 10;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-spawnXRange, spawnXRange), 0, Random.Range(-spawnZRange, spawnZRange));
        Instantiate(badgerPrefab, randomPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
