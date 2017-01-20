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

    public bool UnregisterMessage(string messageName)
    {
        CheckDict();
        if (messageDict.ContainsKey(messageName) == false)
        {
            Debug.LogErrorFormat("Does not contains a message named {0}.", messageName);
            return false;
        }
        messageDict.Remove(messageName);
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
