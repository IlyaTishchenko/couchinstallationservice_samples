using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveGame(GameData gameData)
    {
        var formatter = new BinaryFormatter();
        var path = Application.persistentDataPath + "/save.bin";
        var stream = new FileStream(path, FileMode.Create);

        var data = new GameData(gameData);

        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Data saved successfuly");
    }

    public static GameData LoadGame()
    {
        var path = Application.persistentDataPath + "/save.bin";
        if (File.Exists(path))
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Open);

            var data = formatter.Deserialize(stream) as GameData;

            stream.Close();

            Debug.Log("Data loaded successfuly");
            return data;
        }
        else
        {
            Debug.LogWarning("Save does't exists");
            return null;
        }
    }

    public static void DeleteSave()
    {
        var path = Application.persistentDataPath + "/save.bin";

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Data deleted successfuly");
        }
        else
        {
            Debug.LogWarning("Save does't exists");
        }
    }
}
