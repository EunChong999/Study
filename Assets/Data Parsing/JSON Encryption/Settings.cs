using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static UnityEditor.Progress;
using TMPro;

[System.Serializable]
public class PlayerDatas
{
    public int level = 0;
    public int[] itemLevel = { 0, 0, 0 };
}

public class Settings : MonoBehaviour
{
    public TextMeshProUGUI curFileText;
    public TextMeshProUGUI curEncryptedFileText;
    public string curFileName;
    public string curEncryptedFileName;
    public static Settings instance;

    public PlayerDatas playerData = new();

    private string path;
    private string fileName = "/save";

    void Start()
    {
        instance = this;

        path = Application.persistentDataPath + fileName;
        Debug.Log(path);

        curFileName = JsonUtility.ToJson(playerData);

        string jdata = JsonUtility.ToJson(playerData);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jdata);
        string code = System.Convert.ToBase64String(bytes);

        curEncryptedFileName = code;
    }

    private void Update()
    {
        curFileText.text = curFileName;
        curEncryptedFileText.text = curEncryptedFileName;
    }

    public void SaveData()
    {
        curFileName = JsonUtility.ToJson(playerData);

        string jdata = JsonUtility.ToJson(playerData);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jdata);
        string code = System.Convert.ToBase64String(bytes);

        curEncryptedFileName = code;

        File.WriteAllText(path, code);
    }

    public void LoadData()
    {
        if (!File.Exists(path)) 
        {
            SaveData();
        }

        string code = File.ReadAllText(path);

        curEncryptedFileName = code;

        byte[] bytes = System.Convert.FromBase64String(code);
        string jdata = System.Text.Encoding.UTF8.GetString(bytes);
        playerData = JsonUtility.FromJson<PlayerDatas>(jdata);

        curFileName = JsonUtility.ToJson(playerData);
    }

    public void AddData()
    {
        playerData.level += 1;
        playerData.itemLevel[0] += 1;
        playerData.itemLevel[1] += 2;
        playerData.itemLevel[2] += 3;
    }

    public void ResetData()
    {
        playerData.level = 0;
        playerData.itemLevel[0] = 0;
        playerData.itemLevel[1] = 0;
        playerData.itemLevel[2] = 0;

        SaveData();
    }
}
