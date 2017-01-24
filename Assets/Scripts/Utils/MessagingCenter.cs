using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagingCenter : MonoBehaviour
{
    protected MessagingCenter() { }

    [SerializeField]
    private bool debug = false;

    private Dictionary<string, Action<object>> messageDict;
    
    public bool RegisterMessage(string messageName, Action<object> messageAction)
    {
        CheckDict();
        if (messageDict.ContainsKey(messageName) == false)
        {
            messageDict.Add(messageName, messageAction);
            if (debug)
            {
                Debug.LogFormat("Added {0}", messageName);
            }
        }
        else
        {
            messageDict[messageName] += messageAction;
        }
        return true;
    }

    public bool FireMessage(string messageName, object arg)
    {
        CheckDict();
        if (messageDict.ContainsKey(messageName) == false)
        {
            if (debug)
            {
                Debug.LogErrorFormat("Does not contains a message named {0}.", messageName);
            }
            return false;
        }

        messageDict[messageName](arg);
        return true;
    }

    public bool UnregisterMessage(params string[] messageNames)
    {
        CheckDict();
        foreach (var messageName in messageNames)
        {
            if (messageDict.ContainsKey(messageName) == false)
            {
                if (debug)
                {
                    Debug.LogErrorFormat("Does not contains a message named {0}.", messageName);
                }
                return false;
            }
            messageDict.Remove(messageName);
        }
        return true;
    }

    private void CheckDict()
    {
        if (messageDict == null)
        {
            messageDict = new Dictionary<string, Action<object>>();
        }
    }
}
