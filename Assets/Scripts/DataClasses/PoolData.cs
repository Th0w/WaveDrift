using System;
using UnityEngine;

[Serializable]
public class PoolData
{
    [SerializeField]
    private int quantity = -1;
    [SerializeField]
    private GameObject prefab = null;

    [SerializeField]
    private string name = "";

    public int Quantity { get { return quantity; } }
    public GameObject Prefab { get { return prefab; } }
    public string Name { get { return name; } }

    public bool IsValid { get { return quantity != -1 && prefab != null; } }

    public PoolData() { }

    public PoolData(string name, int quantity, GameObject prefab) {
        this.name = name;
        this.prefab = prefab;
        this.quantity = quantity;
    }
}
