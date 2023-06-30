using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    private Dictionary<string, string> localizedText = new Dictionary<string, string>();

    public static LocalizationManager instance;

    private bool isReady = false;

    private string missingTextString = "Localized key not found!";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadLocalizedText("LocalizationJson/" + SettingsManager.languegeFileName + ".json");
    }

    public void LoadLocalizedText(string fileName)
    {
        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.Items.Length; i++)
            {
                localizedText.Add(loadedData.Items[i].key, loadedData.Items[i].value);
            }

            Debug.Log("Data Loaded, dictonary contains: " + localizedText.Count + " entries");
        }else
        {
            Debug.Log("Cannot find file");
        }

        isReady = true;
    }

    public string GetLocalizedValue(string key) {

        string result = missingTextString;

        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;
    }

    public bool GetIsReady()
    {
        return isReady;
    }
}
