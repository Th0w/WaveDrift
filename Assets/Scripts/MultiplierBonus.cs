using System.Text.RegularExpressions;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class MultiplierBonus : MonoBehaviour {
    [SerializeField]
    private int multiplierValue;
	// Use this for initialization
	void Start () {
        Regex regex = new Regex(@"^Ship_P(\w)$");
        this.OnTriggerEnterAsObservable()
            .Subscribe(coll => Debug.Log(coll.name));
        this.OnTriggerEnterAsObservable()
            .Select(collider => regex.Match(collider.name))
            .Where(match => match.Success)
            .Subscribe(match => {
                int pid = int.Parse(match.Groups[1].Value);
                MessagingCenter.Instance.FireMessage("AddPlayerScoreMultiplier", new object[] { pid, multiplierValue });
                Destroy(gameObject);
            }).AddTo(this);
	}
}
