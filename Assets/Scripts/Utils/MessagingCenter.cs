using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagingCenter : Singleton<MessagingCenter>
{
    protected MessagingCenter() { }

    private Dictionary<string, Action<object>> messageDict;

    public bool RegisterMessage(string messageName, Action<object> messageAction)
    {
        CheckDict();
        if (messageDict.ContainsKey(messageName) == false)
        {
            messageDict.Add(messageName, messageAction);
            Debug.LogFormat("Added {0}", messageName);
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
            Debug.LogErrorFormat("Does not contains a message named {0}.", messageName);
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
                Debug.LogErrorFormat("Does not contains a message named {0}.", messageName);
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
