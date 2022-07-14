using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : BaseView
{
    [SerializeField] private CustomButton closeBtn;
    [SerializeField] private Text languageText;
    [SerializeField] private CustomButton nextLanguageBtn;
    [SerializeField] private CustomButton perviousLanguageBtn;
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private CustomButton homeBtn;
    [SerializeField] private CustomButton resetBtn;
    [SerializeField] private GameObject homeBtnRowObj;
    [SerializeField] private GameObject resetBtnRowObj;

    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick += OnClick;
        homeBtn.onClick += OnClick;
        resetBtn.onClick += OnClick;
        perviousLanguageBtn.onClick += OnClick;
        nextLanguageBtn.onClick += OnClick;
        volumeSlider.onValueChanged.AddListener(OnValueChanged);
        bgmToggle.onValueChanged.AddListener(OnBGMValueChanged);
        sfxToggle.onValueChanged.AddListener(OnSFXValueChanged);
        bgmToggle.isOn = !SoundManager.Instance.IsBGMMuted;
        sfxToggle.isOn = !SoundManager.Instance.IsSFXMuted;
        volumeSlider.value = SoundManager.Instance.Volume;
        languageText.text = GameData.kDefaultLanguage[GameProfile.Language];
    }

    void OnDestroy()
    {
        closeBtn.onClick -= OnClick;
        homeBtn.onClick -= OnClick;
        resetBtn.onClick -= OnClick;
        perviousLanguageBtn.onClick -= OnClick;
        nextLanguageBtn.onClick -= OnClick;
        volumeSlider.onValueChanged.RemoveListener(OnValueChanged);
        bgmToggle.onValueChanged.RemoveListener(OnBGMValueChanged);
        sfxToggle.onValueChanged.RemoveListener(OnSFXValueChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public SettingPopup Open(bool pIsMainMenu) {
        SettingPopup popup = ViewManager.Instance.ShowView(GameConstants.kModalLayerName, GameConstants.kSettingModalName, GameConstants.kSettingModalPath) as SettingPopup;
        popup.ShowHomeButton(!pIsMainMenu);
        return popup;
    }

    void Close() {
        gameObject.SetActive(false);
        DestroyImmediate(gameObject);
    }
    
    void ShowHomeButton(bool pShouldShowButton) {
        homeBtnRowObj.SetActive(pShouldShowButton);
        resetBtnRowObj.SetActive(!pShouldShowButton);
    }

    void OnClick(CustomButton pBtn) {
        if (pBtn == closeBtn) {
            Close();
        }
        else if (pBtn == homeBtn) {
            ViewManager.Instance.ShowView(GameConstants.kViewLayerName, GameConstants.kMenuViewName, GameConstants.kMenuViewPath);
            ViewManager.Instance.CloseView(GameConstants.kHUDLayerName, GameConstants.kGameHUDName);
            ViewManager.Instance.CloseView(GameConstants.kViewLayerName, GameConstants.kGameViewName);
            Close();
        }
        else if (pBtn == resetBtn) {
            GameProfile.PlayerTotalMoney = GameData.kDefaultMoney;
        }
        else if (pBtn == perviousLanguageBtn) {
            int languageID = GameProfile.Language;
            languageID -= 1;
            if (languageID < 0)
                languageID = GameData.kDefaultLanguage.Length - 1;
            GameProfile.Language = languageID;
            languageText.text = GameData.kDefaultLanguage[languageID];
            LocalizationManager.Instance.SetLanguage(GameData.kDefaultLanguageKey[GameProfile.Language]);
        }
        else if (pBtn == nextLanguageBtn) {
            int languageID = GameProfile.Language;
            languageID += 1;
            if (languageID == GameData.kDefaultLanguage.Length)
                languageID = 0;
            GameProfile.Language = languageID;
            languageText.text = GameData.kDefaultLanguage[languageID];
            LocalizationManager.Instance.SetLanguage(GameData.kDefaultLanguageKey[GameProfile.Language]);
        }
    }

    void OnBGMValueChanged(bool value) {
        SoundManager.Instance.IsBGMMuted = !value;
        SoundManager.Instance.Play(0, "button");
    }

    void OnSFXValueChanged(bool value) {
        SoundManager.Instance.IsSFXMuted = !value;
        SoundManager.Instance.Play(0, "button");
    }
    
    void OnValueChanged(float value) {
        SoundManager.Instance.Volume = value;
    }
}
