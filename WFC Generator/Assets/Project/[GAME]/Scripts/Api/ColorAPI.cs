using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

public class ColorApi : MonoBehaviour
{
    [ContextMenu("StartTest")]
    public async void StartTest()
    {
        var url = "https://www.thecolorapi.com";

        var httpClient = new HttpClient(new JsonSerializationOption());

        var result = await httpClient.Get<JSON_DATA>(url, 1);
        Debug.Log(result.Hex);
    }
}

