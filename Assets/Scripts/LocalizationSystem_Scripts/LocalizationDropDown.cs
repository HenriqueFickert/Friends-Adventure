using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationDropDown : MonoBehaviour
{
    private Dropdown dropDown;
    public List<string> options;

    public string[] languageKeys;

    private int storedValue;

    private void Start()
    {
        dropDown = GetComponent<Dropdown>();
        storedValue = SettingsManager.languegeDropdownValue;

        LoadOption();
    }

    public void LoadOption()
    {
        for (int i = 0; i < languageKeys.Length; i++)
        {
            options.Add(LocalizationManager.instance.GetLocalizedValue(languageKeys[i]));
        }

        dropDown.AddOptions(options);
        dropDown.value = storedValue;
    }

   public void RechardOptions()
    {
        SettingsManager.languegeFileName = languageKeys[dropDown.value];

        PlayerPrefs.SetString("Languege", SettingsManager.languegeFileName);
        string path = Path.Combine(Application.streamingAssetsPath,"LocalizationJson/" + SettingsManager.languegeFileName + ".json");

        LocalizationManager.instance.LoadLocalizedText(path);

        for (int i = 0; i < dropDown.options.Count; i++)
        {
            Dropdown.OptionData newItem = new Dropdown.OptionData(LocalizationManager.instance.GetLocalizedValue(languageKeys[i]));
            dropDown.options.RemoveAt(i);
            dropDown.options.Insert(i, newItem);
        }

        dropDown.captionText.text = LocalizationManager.instance.GetLocalizedValue(languageKeys[dropDown.value]);
        storedValue = dropDown.value;

        PlayerPrefs.SetInt("DropdownValue", storedValue);

        LocalizedText[] localizedText = FindObjectsOfType<LocalizedText>();

        foreach (var item in localizedText)
        {
            item.LoadText();
        }

        dropDown.RefreshShownValue();
    }
}
