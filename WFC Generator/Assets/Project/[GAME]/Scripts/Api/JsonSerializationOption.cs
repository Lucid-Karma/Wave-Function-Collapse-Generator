using Newtonsoft.Json;
using System;
using UnityEngine;

public class JsonSerializationOption : ISerializationOption
{
    public string ContentType => "application/json";
    public T Deserialize<T>(string text, int id)
    {
        try
        {
            T[] result = JsonConvert.DeserializeObject<T[]>(text);
            Debug.Log($"Success: {text}");
            return result[id];
        }
        catch (Exception ex)
        {
            Debug.LogError($"Could not parse response {text}. {ex.Message}");
            return default;
        }
    }
}
