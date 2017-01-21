using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class RandomTP : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.OnTriggerEnterAsObservable()
            .Subscribe(collider =>
            {
                var colliders = FindObjectsOfType<Collider>();
                Vector3 pos;
                do
                {
                    pos = GetRandomPos();
                } while (false);
            })
            .AddTo(this);
	}

    private Vector3 GetRandomPos()
    {
        var dim = ArenaDimensions.Instance;

        return new Vector3(Random.Range(dim.min.x, dim.max.x), 0.0f, Random.Range(dim.min.y, dim.max.y));
    }
}
