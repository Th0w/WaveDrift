using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    [SerializeField]
    private Text[] playerScores;
    private int[] playerCachedScore;
    private int[] playerScoreBonusMultiplier;
    
	private void Start () {
        playerScoreBonusMultiplier = new [] { 1, 1, 1, 1 };
        playerCachedScore = new[] { 0, 0, 0, 0 };

        MessagingCenter.Instance.RegisterMessage("UnitKilled", HandleUnitKilled);
        MessagingCenter.Instance.RegisterMessage("UnitTookDamage", HandleUnitTookDamage);
        MessagingCenter.Instance.RegisterMessage("AddPlayerScoreMultiplier", HandlePlayerScoreMultiplier);
        MessagingCenter.Instance.RegisterMessage("PlayerGainScore", GainScore);
    }

    private void OnDestroy()
    {
        MessagingCenter.Instance.UnregisterMessage("UnitKilled", "UnitTookDamage", "AddPlayerScoreMultiplier");
    }

    private void HandlePlayerScoreMultiplier(object obj)
    {
        if (obj is object[] == false)
        {
            Debug.LogError("Wrong arguments passed.");
            return;
        }
        var nobj = (object[])obj;
        playerScoreBonusMultiplier[(int)nobj[0] - 1] += (int)nobj[1];
    }

    private void HandleUnitTookDamage(object obj)
    {
        if (obj is object[] == false)
        {
            Debug.LogError("Wrong arguments passed.");
            return;
        }
        GainScore(ObjectToValues(obj));
    }

    private void HandleUnitKilled(object obj)
    {
        if (obj is object[] == false)
        {
            Debug.LogWarning("Wrong arguments passed.");
            return;
        }
        GainScore(ObjectToValues(obj));
    }

    private void GainScore(object obj)
    {
        if (obj is object[] == false)
        {
            Debug.LogWarning("Wrong arguments passed.");
            return;
        }
        GainScore(ObjectToValues(obj));
    }

    private void GainScore(Tuple<int, int> values)
    {
        playerCachedScore[values.Item1] += values.Item2 * playerScoreBonusMultiplier[values.Item1];
        string str = ("000000" + playerCachedScore[values.Item1].ToString());
        str = str.Substring(str.Length - 6);
        playerScores[values.Item1].text = str;
    }

    private Tuple<int, int> ObjectToValues(object obj)
    {
        var nobj = (object[])obj;
        var id = (int)nobj[0] - 1;
        var score = (int)nobj[1];
        return new Tuple<int, int>(id, score);
    }
}
