using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string key = "";
    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent(typeof(Text)) as Text;
        if (!key.Equals(""))
            text.text = LocalizationManager.Instance.GetString(key);
        LocalizationManager.Instance.onReload += OnReload;
    }

    void OnDestroy() {
        LocalizationManager.Instance.onReload -= OnReload;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnReload() {
        if (!key.Equals(""))
            text.text = LocalizationManager.Instance.GetString(key);
    }
}
