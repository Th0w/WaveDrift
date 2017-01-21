using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArenaDimensions : Singleton<ArenaDimensions>
{
    protected ArenaDimensions() { }

    [SerializeField]
    private Transform[] levelLimits;

    public Vector2 min { get; private set; }
    public Vector2 max { get; private set; }

    private void Awake()
    {
        levelLimits = FindObjectsOfType<LevelLimits>()
            .Select(ll => ll.transform)
            .ToArray();

        min = new Vector2(
            Mathf.Min(levelLimits.Select(transform => transform.position.x).ToArray()),
            Mathf.Min(levelLimits.Select(transform => transform.position.y).ToArray()));
        max = new Vector2(
            Mathf.Max(levelLimits.Select(transform => transform.position.x).ToArray()),
            Mathf.Max(levelLimits.Select(transform => transform.position.y).ToArray()));
    }
}
