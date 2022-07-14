using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPopup : BaseView
{
    public System.Action<QuestionPopup, bool> onClick;
    [SerializeField] private CustomButton noBtn;
    [SerializeField] private CustomButton yesBtn;
    [SerializeField] private Text msgText;

    // Start is called before the first frame update
    void Start()
    {
        yesBtn.onClick += OnClick;
        noBtn.onClick += OnClick;
    }

    void OnDestroy()
    {
        yesBtn.onClick -= OnClick;
        noBtn.onClick -= OnClick;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetMessage(string pMsg) {
        msgText.text = pMsg;
    }

    static public QuestionPopup Open(string pMsg) {
        QuestionPopup popup = ViewManager.Instance.ShowView(GameConstants.kModalLayerName, GameConstants.kQuestionModalName, GameConstants.kQuestionModalPath) as QuestionPopup;
        popup.SetMessage(pMsg);
        return popup;
    }

    void Close() {
        gameObject.SetActive(false);
        DestroyImmediate(this);
    }

    void OnClick(CustomButton pBtn) {
        if (pBtn == yesBtn) {
            onClick?.Invoke(this, true);
        }
        else if (pBtn == noBtn) {
            onClick?.Invoke(this, false);
        }
        Close();
    }
}
