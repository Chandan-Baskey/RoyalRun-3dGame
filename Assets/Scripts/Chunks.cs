using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Chunks : MonoBehaviour
{
    [SerializeField] float[] lanes = {-2.8f, 0f, 2.8f};
    [SerializeField] GameObject fance;
    [SerializeField] GameObject coin;

    List<int> availableLance = new List<int> { 0, 1, 2 };

    private void Start()
    {
        SpawnFance();
        SpawnCoin();
    }

    private void SpawnFance()
    {
        /*int RandomNum = Random.Range(0, 3);
        for (int i = 0; i < RandomNum; i++)
        {
            Vector3 pos = new Vector3(lanes[Random.Range(0, lanes.Length)], transform.position.y, transform.position.z);
            Instantiate(fance, pos, Quaternion.identity, this.transform);
        }*/
        
        int fencesToSpawn = Random.Range(0, lanes.Length);

        for (int i = 0; i < fencesToSpawn; i++)
        {
            if(availableLance.Count == 0) break;

            int randomIndex = Random.Range(0, availableLance.Count);
            int selectedLane= availableLance[randomIndex];
            availableLance.RemoveAt(randomIndex);

            Vector3 spawnPosition = new Vector3(lanes[selectedLane], transform.position.y, transform.position.z);
            Instantiate(fance, spawnPosition, Quaternion.identity,transform);

        }
    }
    private void SpawnCoin()
    {
        int availableLanee = availableLance[0];
        Vector3 spawnPosition = new Vector3(lanes[availableLanee], transform.position.y +0.8f, transform.position.z);
        Instantiate(coin, spawnPosition, Quaternion.identity, transform);
    }
}
