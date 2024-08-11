using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the Init scene and destroying all instances");

                        return null;
                    }
                }
                return _instance;
            }

        }
    }

    public static bool applicationIsQuitting = false;

    public override void OnDestroy()
    {
        base.OnDestroy();
        applicationIsQuitting = true;
    }
}
