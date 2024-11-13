using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField] GameObject graphics;
    void Awake()
    {
        graphics = this.gameObject;
        graphics.SetActive(false);
    }
}
