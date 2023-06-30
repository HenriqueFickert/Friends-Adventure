using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    public string key;
    public bool isTextMeshPro;
    private Text text;
    public TextMeshProUGUI textMesh;
    private void OnEnable()
    {
        if (isTextMeshPro)
            textMesh = GetComponent<TextMeshProUGUI>();
        else
            text = GetComponent<Text>();

        if (LocalizationManager.instance.GetIsReady())
            LoadText();
    }

    private IEnumerator Start()
    {
        while (!LocalizationManager.instance.GetIsReady())
        {
            yield return null;
        }
        LoadText();
    }

    public void LoadText()
    {
        if (isTextMeshPro)
            textMesh.text = LocalizationManager.instance.GetLocalizedValue(key);
        else
            text.text = LocalizationManager.instance.GetLocalizedValue(key);
    }
}
