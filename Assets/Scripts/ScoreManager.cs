using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    [SerializeField]
    private Text[] playerScores;
    private int[] playerCachedScore;
    private int[] playerScoreBonusMultiplier;
    [SerializeField]
    private Text[] playerDeath;
    private int[] playerCachedDeath;
    private int[] playerPowerUpJauge;

    [SerializeField]
    private Transform[] players;

    [SerializeField]
    private int powerUpPerPoint = 10;

    [SerializeField]
    private float multLostPercentOnDeath = 0.1f;

    [SerializeField]
    private bool debug = false;
    private PoolManager poolManager;

    [SerializeField]
    private Poolable multiplierPrefab;
    private Pool multiplierPool;

    private void Start () {
        playerScoreBonusMultiplier = new [] { 1, 1, 1, 1 };
        playerCachedScore = new[] { 0, 0, 0, 0 };
        playerCachedDeath = new[] { 0, 0, 0, 0 };
        playerPowerUpJauge = new[] { 0, 0, 0, 0 };

        MessagingCenter.Instance.RegisterMessage("UnitKilled", HandleUnitKilled);
        MessagingCenter.Instance.RegisterMessage("UnitTookDamage", HandleUnitTookDamage);
        MessagingCenter.Instance.RegisterMessage("AddPlayerScoreMultiplier", HandlePlayerScoreMultiplier);
        MessagingCenter.Instance.RegisterMessage("PlayerGainScore", GainScore);
        MessagingCenter.Instance.RegisterMessage("PlayerDeath", HandlePlayerDeath);

        poolManager = FindObjectOfType<PoolManager>();
        multiplierPool = poolManager.CreatePool("PowerUps", 5, multiplierPrefab);
    }

    private void OnDestroy()
    {
        if (MessagingCenter.Instance == null) { return; }

        MessagingCenter.Instance.UnregisterMessage(
            "UnitKilled", 
            "UnitTookDamage", 
            "AddPlayerScoreMultiplier", 
            "PlayerGainScore",
            "PlayerDeath");
    }

    private void HandlePlayerDeath(object obj)
    {
        if (obj is object[] == false) {
            if (debug) {
                Debug.LogError("Wrong arguments passed.");
            }
            return;
        }

        // Cast to retrieve arguments from polyvalent object[]
        var objs = (object[])obj;
        var id = (int)(objs[0]) - 1;
        var pos = (Vector3)objs[1];
        
        // Update Death counter
        playerCachedDeath[id]++;
        UpdateScoreMultiplier(id);

        // Penalties on dead player!
        int retr = (int)(playerScoreBonusMultiplier[id] * multLostPercentOnDeath);
        playerScoreBonusMultiplier[id] -= retr;
        multiplierPool.Spawn(new object[] { pos, retr });
    }

    private void HandlePlayerScoreMultiplier(object obj)
    {
        if (obj is object[] == false)
        {
            if (debug)
            {
                Debug.LogError("Wrong arguments passed.");
            }
            return;
        }
        var nobj = (object[])obj;
        var id = (int)nobj[0] - 1;
        var val = (int)nobj[1];
        playerScoreBonusMultiplier[id] += val;
        playerPowerUpJauge[id] += val;
        if (playerPowerUpJauge[id] >= powerUpPerPoint)
        {
            playerScoreBonusMultiplier[id]++;
            playerPowerUpJauge[id] -= powerUpPerPoint;
        }
        UpdateScoreMultiplier(id);
    }

    private void UpdateScoreMultiplier(int id)
    {
        if (playerDeath == null || playerDeath[id] == null) { return; }
        string str = "000" + playerCachedDeath[id];
        playerDeath[id].text = str.Substring(str.Length - 3);
    }

    private void HandleUnitTookDamage(object obj)
    {
        if (obj is object[] == false)
        {
            if (debug)
            {
                Debug.LogError("Wrong arguments passed.");
            }
            return;
        }
        GainScore(ObjectToValues(obj));
    }

    private void HandleUnitKilled(object obj)
    {
        if (obj is object[] == false)
        {
            if (debug)
            {
                Debug.LogError("Wrong arguments passed.");
            }
            return;
        }
        GainScore(ObjectToValues(obj));
    }

    private void GainScore(object obj)
    {
        if (obj is object[] == false)
        {
            if (debug)
            {
                Debug.LogError("Wrong arguments passed.");
            }
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
		playerScores [values.Item1].SendMessage ("PlayAnim");
    }

    private Tuple<int, int> ObjectToValues(object obj)
    {
        var nobj = (object[])obj;
        var id = (int)nobj[0] - 1;
        var score = (int)nobj[1];
        return new Tuple<int, int>(id, score);
    }
}
