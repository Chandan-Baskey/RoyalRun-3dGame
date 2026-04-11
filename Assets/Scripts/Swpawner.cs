using UnityEngine;
using System.Collections;

public class Swpawner : MonoBehaviour
{
    [SerializeField] GameObject[] obstacles;
    [SerializeField] int numToSpawn = 5;
    [SerializeField] int waitPerSpawn = 1;
    [SerializeField] float spawnWidth = 3f;
    [SerializeField] GameObject obstacleParent;
    private void Start()
    {
        StartCoroutine(SpawmObstacles());
    }
    IEnumerator SpawmObstacles()
    {
        
        while(true )
        {
            yield return new WaitForSeconds(waitPerSpawn);
            GameObject obstacle = obstacles[Random.Range(0, obstacles.Length)];
            Vector3 spawnPosition = new Vector3(Random.Range(-spawnWidth, spawnWidth), transform.position.y, transform.position.z);
            Instantiate(obstacle, transform.position, Random.rotation, obstacleParent.transform);
            numToSpawn--;
        }
    }
}
