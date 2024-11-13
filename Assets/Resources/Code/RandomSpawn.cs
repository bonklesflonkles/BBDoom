using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;

public class RandomSpawn : MonoBehaviour
{
    public static RandomSpawn Instance;

    Spawn[] spawnpoints;

    void Awake()
    {
        Instance = this;
        spawnpoints = GetComponentsInChildren<Spawn>();
    }

    public Transform GetSpawnpoint()
    {
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }

    private void Start()
    {
        PhotonNetwork.Instantiate(Path.Combine("Prefabs","SpookySpawner"), GetSpawnpoint().position, Quaternion.identity);
    }
}
