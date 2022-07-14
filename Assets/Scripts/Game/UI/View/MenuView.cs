using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuView : BaseView
{
    [SerializeField] private CustomButton settingBtn;
    [SerializeField] private CustomButton pokerBtn;

    // Start is called before the first frame update
    void Start()
    {
        settingBtn.onClick += OnClick;
        pokerBtn.onClick += OnClick;
        SoundManager.Instance.PlayBGMMusic("chill-abstract-intention-12099");
    }

    void OnDestroy()
    {
        settingBtn.onClick -= OnClick;
        pokerBtn.onClick -= OnClick;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick(CustomButton pBtn) {
        if (pBtn == settingBtn) {
            SettingPopup.Open(true);
        }
        else if (pBtn == pokerBtn) {
            ViewManager.Instance.ShowView(GameConstants.kViewLayerName, GameConstants.kGameViewName, GameConstants.kGameViewPath);
            ViewManager.Instance.ShowView(GameConstants.kHUDLayerName, GameConstants.kGameHUDName, GameConstants.kGameHUDPath);
            ViewManager.Instance.CloseView(GameConstants.kViewLayerName, GameConstants.kMenuViewName);
        }
    }
}
