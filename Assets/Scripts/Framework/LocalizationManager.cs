using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    static public LocalizationManager Instance = null;
    public System.Action onReload;
    private Dictionary<string, Dictionary<string,string>> localizableDict = new Dictionary<string, Dictionary<string,string>>();
    private string currentLanguageKey = "";

    void Awake() {
        Instance = this;
    }

    public void Load(string pLanguageKey, Dictionary<string,string> pDictionary) {
        localizableDict[pLanguageKey] = pDictionary;
    }

    public void SetLanguage(string pLanguageKey) {
        if (localizableDict.ContainsKey(pLanguageKey))
            currentLanguageKey = pLanguageKey;
        onReload?.Invoke();
    }

    public string GetString(string pKey) {
        if (!currentLanguageKey.Equals("") && localizableDict[currentLanguageKey].ContainsKey(pKey))
            return localizableDict[currentLanguageKey][pKey];
        Debug.LogError(string.Format("Key: {0} is missing", pKey));
        return "";
    }
}
