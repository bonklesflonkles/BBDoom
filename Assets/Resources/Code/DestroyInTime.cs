using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInTime : MonoBehaviour
{
    public bool longest;

    private void Start()
    {
        if (longest)
        {
            Destroy(gameObject, 15f);
            return;
        }
        Destroy(gameObject, 1f);
    }
}
