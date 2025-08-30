

    //[JsonConverter(typeof(StringEnumConverter))]
    //var settings = new JsonSerializerSettings
//{
  //  Formatting = Formatting.Indented,
    //Converters = { new StringEnumConverter() }
//};

//string json = JsonConvert.SerializeObject(tile, settings);
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class SaveGame //unsafe for untrusted json
{
    private static string savePath = Path.Combine(Application.persistentDataPath, "Saves");

    private static JsonSerializerSettings GetSettings()
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All
        };
        settings.Converters.Add(new Vector2Converter());
        return settings;
    }

    public static void SaveCurrentGame(SaveData state, string path)
    {
        Debug.Log("Saving game to " + savePath + "...");
        string json = JsonConvert.SerializeObject(state, GetSettings());
        File.WriteAllText(Path.Combine(savePath, path), json);
    }

    public static SaveData LoadGame(string path)
    {
        string fullPath = Path.Combine(savePath, path);
        if (!File.Exists(fullPath))
        {
            Debug.Log("No save file found at " + fullPath);
            return null;
        }

        string json = File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<SaveData>(json, GetSettings());
    }
}