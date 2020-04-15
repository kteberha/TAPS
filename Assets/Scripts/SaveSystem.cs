using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    public static void CreateNewGameData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/gameData.taps";
        FileStream stream = new FileStream(path, FileMode.Create);
        GameData gData = new GameData();

        //set default values
        gData.startingWorkday = 1;
        for (int i = 0; i < gData.orderHighScores.Length; i++)
            gData.orderHighScores[i] = 0;
        for (int i = 0; i < gData.packageHighScores.Length; i++)
            gData.packageHighScores[i] = 0;
        for (int i = 0; i < gData.refundsHighScores.Length; i++)
            gData.refundsHighScores[i] = 99;

        bf.Serialize(stream, gData);
        stream.Close();
    }

    public static void CreateNewSettingsData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/optionsData.taps";
        FileStream stream = new FileStream(path, FileMode.Create);
        OptionsData oData = new OptionsData();

        //set default values

        bf.Serialize(stream, oData);
        stream.Close();
    }

    public static void SaveGameData(GameData gData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/gameData.taps";
        FileStream stream = new FileStream(path, FileMode.Create);

        bf.Serialize(stream, gData);
        stream.Close();
    }

    public static GameData LoadGameData()
    {
        string path = Application.persistentDataPath + "/gameData.taps";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData gData = (GameData)bf.Deserialize(stream);
            /*for(int i = 0; i < gData.orderHighScores.Length; i++)
            {
                Debug.Log("Order highscores: " + gData.orderHighScores[i]);
            }
            */
            stream.Close();
            return gData;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void SaveOptionsData(OptionsData _oData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/optionsData.taps";
        FileStream stream = new FileStream(path, FileMode.Create);

        bf.Serialize(stream, _oData);
        stream.Close();

    }

    public static OptionsData LoadOptionsData()
    {
        string path = Application.persistentDataPath + "/optionsData.taps";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            OptionsData oData = (OptionsData)bf.Deserialize(stream);
            return oData;
        }
        else
        {
            Debug.LogError("Options file not found in " + path);
            return null;
        }
    }

    public static void DeleteAllFiles()
    {
        string gamePath = Application.persistentDataPath + "/gameData.taps";
        string optionPath = Application.persistentDataPath + "/optionsData.taps";
        if(File.Exists(gamePath))
        {
            Debug.Log("deleted game data");
            File.Delete(gamePath);
        }
        if(File.Exists(optionPath))
        {
            Debug.Log("deleted options data");
            File.Delete(optionPath);
        }
    }
}
