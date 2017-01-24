using System;
using UnityEngine;

[Serializable]
public class SpawnData
{
    public GameObject prefab;
    public float timer;
    public int quantity;
    public float distFromSpawn;
}