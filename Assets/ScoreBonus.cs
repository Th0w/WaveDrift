using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Text.RegularExpressions;

[RequireComponent(typeof(CapsuleCollider))]
public class ScoreBonus : MonoBehaviour
{
    [SerializeField]
    private int scoreValue;
    // Use this for initialization
    void Start()
    {
        Regex regex = new Regex(@"^Ship_P(\w)$");
        this.OnTriggerEnterAsObservable()
            .Select(collider => regex.Match(collider.name))
            .Where(match => match.Success)
            .Subscribe(match => {
                int pid = int.Parse(match.Groups[0].Value);
                MessagingCenter.Instance.FireMessage("AddPlayerScoreMultiplier", new object[] { pid, scoreValue });
                Destroy(gameObject);
            }).AddTo(this);
    }
}
