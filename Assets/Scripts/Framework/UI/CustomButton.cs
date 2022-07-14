using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CustomButton : MonoBehaviour
{
    public System.Action<CustomButton> onClick;
    [SerializeField] private string sfxName = "button";
    [SerializeField] private string btnTextKey = "";
    private Button button;
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        text = GetComponentInChildren<Text>();
        if (!btnTextKey.Equals(""))
            text.text = LocalizationManager.Instance.GetString(btnTextKey);
        LocalizationManager.Instance.onReload += OnReload;
    }

    void OnDestroy() {
        LocalizationManager.Instance.onReload -= OnReload;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool interactable {
        get {
            return button.interactable;
        }
        set {
            button.interactable = value;
        }
    }

    private void OnClick()
    {
        SoundManager.Instance.Play(0, sfxName);
        onClick?.Invoke(this);
    }

    void OnReload() {
        if (!btnTextKey.Equals(""))
            text.text = LocalizationManager.Instance.GetString(btnTextKey);
    }
}
