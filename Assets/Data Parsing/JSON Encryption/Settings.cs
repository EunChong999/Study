using UnityEngine;
using System.IO;
using TMPro;
using System.Security.Cryptography;
using System.Text;

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

    public PlayerDatas playerData = new();

    private string path;
    private const string fileName = "/save";
    private const string encryptionKey = "1234567890123456"; // 16자리 키

    void Start()
    {
        path = Application.persistentDataPath + fileName;
        Debug.Log($"Save file path: {path}");

        if (File.Exists(path))
            LoadData();
        else
            SaveData();

        UpdateUIText();
    }

    private void Update()
    {
        UpdateUIText();
    }

    private void UpdateUIText()
    {
        string jsonString = JsonUtility.ToJson(playerData);
        jsonString = jsonString.Replace("\n", "").Replace("\r", "").Replace(" ", "");

        curFileText.text = jsonString;
        curEncryptedFileText.text = Encrypt(jsonString, encryptionKey);
    }

    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(playerData);
        string encryptedData = Encrypt(jsonData, encryptionKey);
        File.WriteAllText(path, encryptedData);
    }

    public void LoadData()
    {
        try
        {
            string encryptedData = File.ReadAllText(path);
            string decryptedData = Decrypt(encryptedData, encryptionKey);
            PlayerDatas loadedData = JsonUtility.FromJson<PlayerDatas>(decryptedData);

            if (loadedData != null)
                playerData = loadedData;
            else
            {
                Debug.LogWarning("Failed to decode save data. Resetting to default.");
                ResetData();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error loading save data: {ex.Message}");
            ResetData();
        }
    }

    public void AddData()
    {
        playerData.level += 1;
        playerData.itemLevel[0] += 1;
        playerData.itemLevel[1] += 2;
        playerData.itemLevel[2] += 3;
    }

    public void RemoveData()
    {
        playerData.level = Mathf.Max(0, playerData.level - 1);
        playerData.itemLevel[0] = Mathf.Max(0, playerData.itemLevel[0] - 1);
        playerData.itemLevel[1] = Mathf.Max(0, playerData.itemLevel[1] - 2);
        playerData.itemLevel[2] = Mathf.Max(0, playerData.itemLevel[2] - 3);
    }

    public void ResetData()
    {
        playerData = new PlayerDatas();
        SaveData();
    }

    private string Encrypt(string plainText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16]; // IV를 0으로 초기화

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return System.Convert.ToBase64String(encryptedBytes);
        }
    }

    private string Decrypt(string encryptedText, string key)
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[16]; // IV를 0으로 초기화

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] encryptedBytes = System.Convert.FromBase64String(encryptedText);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to decrypt data: {ex.Message}");
            return null;
        }
    }
}
