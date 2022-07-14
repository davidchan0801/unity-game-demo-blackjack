using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePopup : BaseView
{
    public System.Action<MessagePopup> onClick;
    [SerializeField] private Text text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnDestroy()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 1 || Input.GetMouseButtonDown(0)) {
            onClick?.Invoke(this);
            Close();
        }
    }

    void SetMessage(string pMsg) {
        text.text = pMsg;
    }

    static public MessagePopup Open(string pMsg) {
        MessagePopup popup = ViewManager.Instance.ShowView(GameConstants.kModalLayerName, GameConstants.kMessageModalName, GameConstants.kMessageModalPath) as MessagePopup;
        popup.SetMessage(pMsg);
        return popup;
    }

    void Close() {
        gameObject.SetActive(false);
        DestroyImmediate(this);
    }
}
