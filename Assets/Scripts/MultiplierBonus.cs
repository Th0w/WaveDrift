using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Text.RegularExpressions;

[RequireComponent(typeof(CapsuleCollider))]
public class MultiplierBonus : MonoBehaviour {
    [SerializeField]
    private int multiplierValue;
	// Use this for initialization
	void Start () {
        Regex regex = new Regex(@"^Ship_P(\w)$");
        this.OnTriggerEnterAsObservable()
            .Select(collider => regex.Match(collider.name))
            .Where(match => match.Success)
            .Subscribe(match => {
                int pid = int.Parse(match.Groups[0].Value);
                MessagingCenter.Instance.FireMessage("AddPlayerScoreMultiplier", new object[] { pid, multiplierValue });
                Destroy(gameObject);
            }).AddTo(this);
	}
}
