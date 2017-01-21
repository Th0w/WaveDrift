using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        var nobj = (object[])obj;
        int pid = (int)nobj[0] - 1;
        int score = (int)nobj[1];
        playerCachedScore[pid] += score * playerScoreBonusMultiplier[pid];
        string str = ("000000" + playerCachedScore[pid].ToString());
        str = str.Substring(str.Length - 6);
        playerScores[pid].text = str;
    }

    private void HandleUnitKilled(object obj)
    {
        if (obj is object[] == false)
        {
            Debug.LogWarning("Wrong arguments passed.");
        }
        var nobj = (object[])obj;
        int pid = (int)nobj[0] - 1;
        int score = (int)nobj[1];
        playerCachedScore[pid] += score * playerScoreBonusMultiplier[pid];
        string str = ("000000" + playerCachedScore[pid].ToString());
        str = str.Substring(str.Length - 6);
        playerScores[pid].text = str;

    }
}
