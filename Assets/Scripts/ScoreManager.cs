using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    #region Fields
    #region Serialized
    [SerializeField]
    private Text[] playerScores;

    [SerializeField]
    private Text[] playerDeath;

    [SerializeField]
    private Text[] playerMultText;

    [SerializeField]
    private int powerUpPerPoint = 10;

    [SerializeField]
    private float multLostPercentOnDeath = 0.1f;

    [SerializeField]
    private bool debug = false;

    [SerializeField]
    private Image[] gauges;

    [SerializeField]
    private float mapDimensions = 170.0f;
    #endregion Serialized
    private int[] playerCachedScore;
    private int[] playerScoreBonusMultiplier;
    private int[] playerCachedDeath;
    private int[] playerPowerUpJauge;

    private GameManager gameManager;
    private MessagingCenter messagingCenter;
    #endregion Fields
    #region Methods
    #region MonoBehaviour
    private void Awake() {
        messagingCenter = FindObjectOfType<MessagingCenter>();
        gameManager = FindObjectOfType<GameManager>();
    }
    private void Start () {
        ResetValues();

        ResetTexts();

        gameManager.OnGameBegin.Subscribe(_ => RegisterMessages());

        gameManager.OnGameEnd.Subscribe(_ => {
            ResetValues();
            ResetTexts();
            UnregisterMessages();
        });
    }
    
    private void OnDestroy()
    {
        UnregisterMessages();
    }
    #endregion MonoBehaviour
    #region Handlers
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
        UpdateDeathText(id);

        // Penalties on dead player!
        int retr = (int)(playerScoreBonusMultiplier[id] * multLostPercentOnDeath);
        playerScoreBonusMultiplier[id] -= retr;
        UpdatePlayerScoreMult(id);

        if (retr == 0) { return; }
        Vector3 rnd = Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f) * Vector3.right * 10.0f;
        messagingCenter.FireMessage("SpawnMult", new object[] { rnd, retr });
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
        playerPowerUpJauge[id] += val;
        if (playerPowerUpJauge[id] >= powerUpPerPoint)
        {
            playerScoreBonusMultiplier[id]++;
            UpdatePlayerScoreMult(id);
            playerPowerUpJauge[id] -= powerUpPerPoint;
            UpdateDeathText(id);
        }
        UpdateGauge(id);
    }

    private void UpdatePlayerScoreMult(int id)
    {
        string str = "000" + playerScoreBonusMultiplier[id].ToString();
        playerMultText[id].text = str.Substring(str.Length - 3);
    }

    private void UpdateGauge(int id)
    {
        gauges[id].fillAmount = ((float)playerPowerUpJauge[id] / (float)powerUpPerPoint);
    }

    private void UpdateDeathText(int id)
    {
        if (playerDeath == null || playerDeath.Length == 0 || playerDeath.Length < id) { return; }
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
        HandleGainScore(ObjectToValues(obj));
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
        HandleGainScore(ObjectToValues(obj));
    }

    private void HandleGainScore(object obj)
    {
        if (obj is object[] == false)
        {
            if (debug)
            {
                Debug.LogError("Wrong arguments passed.");
            }
            return;
        }
        HandleGainScore(ObjectToValues(obj));
    }

    private void HandleGainScore(Tuple<int, int> values)
    {
        playerCachedScore[values.Item1] += values.Item2 * playerScoreBonusMultiplier[values.Item1];
        UpdateScore(values.Item1);
    }
    #endregion

    private void UpdateScore(int playerId) {
        string str = ("000000" + playerCachedScore[playerId].ToString());
        str = str.Substring(str.Length - 6);
        playerScores[playerId].text = str;
        playerScores[playerId].SendMessage("PlayAnim", SendMessageOptions.DontRequireReceiver);
    }

    private Tuple<int, int> ObjectToValues(object obj)
    {
        var nobj = (object[])obj;
        var id = (int)nobj[0] - 1;
        var score = (int)nobj[1];
        return new Tuple<int, int>(id, score);
    }

    private void ResetValues() {
        playerScoreBonusMultiplier = new[] { 1, 1, 1, 1 };
        playerCachedScore = new[] { 0, 0, 0, 0 };
        playerCachedDeath = new[] { 0, 0, 0, 0 };
        playerPowerUpJauge = new[] { 0, 0, 0, 0 };
    }

    private void RegisterMessages() {
        messagingCenter.RegisterMessage("UnitKilled", HandleUnitKilled);
        messagingCenter.RegisterMessage("UnitTookDamage", HandleUnitTookDamage);
        messagingCenter.RegisterMessage("AddPlayerScoreMultiplier", HandlePlayerScoreMultiplier);
        messagingCenter.RegisterMessage("PlayerGainScore", HandleGainScore);
        messagingCenter.RegisterMessage("PlayerDeath", HandlePlayerDeath);
    }

    private void UnregisterMessages() {
        if (messagingCenter == null) { return; }
        messagingCenter.UnregisterMessage(
            "UnitKilled",
            "UnitTookDamage",
            "AddPlayerScoreMultiplier",
            "PlayerGainScore",
            "PlayerDeath");
    }

    private void ResetTexts() {
        int i, max;
        for (i = 0, max = 4; i < max; ++i) {
            UpdateDeathText(i);
            UpdateGauge(i);
            UpdatePlayerScoreMult(i);
            UpdateScore(i);
        }
    }
    #endregion Methods
}
