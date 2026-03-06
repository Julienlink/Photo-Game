using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PNJSpawner : MonoBehaviour
{
    [Header("Prefabs à spawn")]
    public List<GameObject> pnjPrefabs;

    [Header("Spawn Settings")]
    public int pnjCount = 10;
    public Vector3 mapMin;
    public Vector3 mapMax;
    public float maxNavMeshSample = 10f;

    void Start()
    {
        for (int i = 0; i < pnjCount; i++)
        {
            SpawnRandomPNJ();
        }
    }

    void SpawnRandomPNJ()
    {
        Debug.Log("Spawning PNJ...");
        if (pnjPrefabs.Count == 0) return;
        GameObject prefab = pnjPrefabs[Random.Range(0, pnjPrefabs.Count)];

        
        Vector3 randomPos = new Vector3(
            Random.Range(mapMin.x, mapMax.x),
            mapMin.y,
            Random.Range(mapMin.z, mapMax.z)
        );

        //Vérifier que c'est sur le NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, maxNavMeshSample, NavMesh.AllAreas))
        {
            Debug.Log("spawned");
            GameObject pnj = Instantiate(prefab, hit.position, Quaternion.identity);
        }
    }
}