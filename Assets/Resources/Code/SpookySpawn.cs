using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class SpookySpawn : MonoBehaviour
{
    public GameObject dood;

    private void OnTriggerEnter(Collider other)
    {
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "NUGGET"), Vector3.zero, Quaternion.identity);
        Destroy(gameObject);
    }
}
