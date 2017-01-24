using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UniRx;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    #region Fields
    private Dictionary<string, Pool> pools;
    #endregion Fields

    #region Methods
    #region MonoBehaviour
    private void Awake()
    {
        pools = new Dictionary<string, Pool>();
    }
    #endregion MonoBehaviour

    public Pool CreatePool(PoolData data)
    {
        return CreatePool(data.Name, data.Quantity, data.Prefab.GetComponent<Poolable>());
    }

    public Pool CreatePool(string name, int quantity, Poolable prefab)
    {
        Pool p = this[name];
        if (p != null) {
            Debug.LogErrorFormat("A pool named '{0}' already exists! Giving already existing.", name);
            return p;
        }
        p = new GameObject().AddComponent<Pool>();
        p.Init(quantity, prefab, name);
        p.transform.SetParent(transform);
        pools.Add(name, p);
		return p;
    }

    #region Getters
    public Pool this[string name] {
        get {
            return pools.ContainsKey(name) ? pools[name] : null;
        }
    }

    public Pool[] this[Regex regex] {
        get {
            return pools.Where(kvp => regex.Match(kvp.Value.name).Success)
                .Select(kvp => kvp.Value)
                .ToArray();
        }
    }

    internal void RecycleAll() {
        pools.ForEach(kvp => kvp.Value.Recycle(deep: true));
        // Debug.LogWarningFormat("Remaining actives in {0}: {1}.", name, pools.Select(kvp => kvp.Value.ActiveCount).Aggregate((a, b) => a + b));
    }
    #endregion Getters
    #endregion Methods
}
