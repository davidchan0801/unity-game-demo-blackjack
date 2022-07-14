using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : BaseView, IListener
{
    [SerializeField] private CustomButton settingBtn;
    [SerializeField] private CanvasGroup chipsPanel;
    [SerializeField] private GameObject betArea;
    [SerializeField] private CustomButton[] chipBtns;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private CustomButton hitBtn;
    [SerializeField] private CustomButton splitBtn;
    [SerializeField] private CustomButton standBtn;
    [SerializeField] private CustomButton dealBtn;
    [SerializeField] private Text totalText;
    private List<CustomButton> bettingChipList = new List<CustomButton>();
    private int playerID;
    private int handIndex;

    // Start is called before the first frame update
    void Start()
    {
        NotificationManager.AddListener(this, Notification.PLAYER_READY, Notification.BETTING, Notification.DISTRIBUTE_CARD, Notification.PLAYER_TURN, Notification.ASK_CARD, Notification.SPLIT_CARD, Notification.HAND_RESULT, Notification.END_TURN, Notification.CLEAN);
        settingBtn.onClick += OnClick;
        hitBtn.onClick += OnClick;
        splitBtn.onClick += OnClick;
        standBtn.onClick += OnClick;
        dealBtn.onClick += OnClick;
        foreach (CustomButton btn in chipBtns) {
            btn.onClick += OnChipClick;
        }
    }

    void OnDestroy()
    {
        NotificationManager.RemoveListener(this);
        settingBtn.onClick -= OnClick;
        hitBtn.onClick -= OnClick;
        splitBtn.onClick -= OnClick;
        standBtn.onClick -= OnClick;
        dealBtn.onClick -= OnClick;
        foreach (CustomButton btn in chipBtns) {
            btn.onClick -= OnChipClick;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    void RefreshPlayerTotalMoney() {
        totalText.text = ""+GameProfile.PlayerTotalMoney;
    }

    void RefreshChips() {
        chipBtns[0].interactable = GameProfile.PlayerTotalMoney >= 100;
        chipBtns[1].interactable = GameProfile.PlayerTotalMoney >= 500;
        chipBtns[2].interactable = GameProfile.PlayerTotalMoney >= 1000;
    }

    void OnClick(CustomButton pBtn) {
        if (pBtn == settingBtn) {
            SettingPopup.Open(false);
        }
        else if (pBtn == hitBtn) {
            if (GameData.IsSplitCardMode(playerID)) {
                NotificationManager.SendNotification(Notification.ASK_CARD, new GameData.PlayerTurn(playerID, handIndex));
            }
            else
                NotificationManager.SendNotification(Notification.ASK_CARD, new GameData.PlayerTurn(playerID, 0));
        }
        else if (pBtn == standBtn) {
            if (GameData.IsSplitCardMode(playerID)) {
                if (handIndex == 0)
                    NotificationManager.SendNotification(Notification.PLAYER_TURN, new GameData.PlayerTurn(playerID, 1));
                else
                    NotificationManager.SendNotification(Notification.PLAYER_TURN, new GameData.PlayerTurn(playerID+1, 0));
            }
            else
                NotificationManager.SendNotification(Notification.PLAYER_TURN, new GameData.PlayerTurn(playerID+1, 0));
        }
        else if (pBtn == splitBtn) {
            NotificationManager.SendNotification(Notification.SPLIT_CARD, playerID);
        }
        else if (pBtn == dealBtn) {
            dealBtn.gameObject.SetActive(false);
            foreach (CustomButton btn in bettingChipList) {
                btn.onClick -= OnRemoveChipClick;
            }
            chipsPanel.interactable = false;
            NotificationManager.SendNotification(Notification.DISTRIBUTE_CARD);
        }
    }

    void OnChipClick(CustomButton pBtn) {
        CustomButton btn = Instantiate(pBtn);
        RectTransform rect = btn.GetComponent(typeof(RectTransform)) as RectTransform;
        rect.sizeDelta = new Vector2(150, 150);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.localPosition = new Vector3(0,5*bettingChipList.Count+0,0);
        btn.transform.SetParent(betArea.transform, false);
        btn.onClick -= OnChipClick;
        btn.onClick += OnRemoveChipClick;
        bettingChipList.Add(btn);
        
        if (pBtn == chipBtns[0]) {
            GameProfile.PlayerTotalMoney -= 100;
            GameData.currentBet += 100;
            btn.name = "100";
        }
        else if (pBtn == chipBtns[1]) {
            GameProfile.PlayerTotalMoney -= 500;
            GameData.currentBet += 500;
            btn.name = "500";
        }
        else if (pBtn == chipBtns[2]) {
            GameProfile.PlayerTotalMoney -= 1000;
            GameData.currentBet += 1000;
            btn.name = "1000";
        }
        RefreshChips();
        RefreshPlayerTotalMoney();
        dealBtn.gameObject.SetActive(true);
    }

    void OnRemoveChipClick(CustomButton pBtn) {
        bettingChipList.Remove(pBtn);
        if (pBtn.name.Equals("100")) {
            GameProfile.PlayerTotalMoney += 100;
            GameData.currentBet -= 100;
        }
        else if (pBtn.name.Equals("500")) {
            GameProfile.PlayerTotalMoney += 500;
            GameData.currentBet -= 500;
        }
        else if (pBtn.name.Equals("1000")) {
            GameProfile.PlayerTotalMoney += 1000;
            GameData.currentBet -= 1000;
        }
        pBtn.onClick -= OnRemoveChipClick;

        RefreshChips();
        RefreshPlayerTotalMoney();
        DestroyImmediate(pBtn.gameObject);
        if (bettingChipList.Count == 0)
            dealBtn.gameObject.SetActive(false);
    }

    void NotifyPlayerReady(int pPlayerID) {
        RefreshPlayerTotalMoney();
        GameData.currentBet = 0;
        playerID = pPlayerID;
    }

    void NotifyBetting() {
        hitBtn.gameObject.SetActive(false);
        splitBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);
        dealBtn.gameObject.SetActive(false);
        chipsPanel.interactable = true;
        RefreshChips();
    }

    void NotifyPlayerTurn(GameData.PlayerTurn pPlayerData) {
        int playerDataID = pPlayerData.playerID;
        int playerDataHandIndex = pPlayerData.handID;
        if (playerDataID == playerID) {
            if (playerDataHandIndex == 0 && GameData.IsSplitCardMode(playerDataID)) {
                Vector3 pos = controlPanel.transform.localPosition;
                pos.x = -270;
                controlPanel.transform.localPosition = pos;
                handIndex = playerDataHandIndex;
            }
            else {
                Vector3 pos = controlPanel.transform.localPosition;
                pos.x = 520;
                controlPanel.transform.localPosition = pos;
                handIndex = playerDataHandIndex;
            }
            hitBtn.gameObject.SetActive(true);
            standBtn.gameObject.SetActive(true);
            if (playerDataID == 0 && GameData.playerCards[playerDataID*2].Count == 2 && GameData.playerCards[playerDataID*2+1].Count == 0) {
                if (GameData.playerCards[playerDataID*2][0].CardActualNumber == GameData.playerCards[playerDataID*2][1].CardActualNumber) {
                    if (GameData.currentBet <= GameProfile.PlayerTotalMoney)
                        splitBtn.gameObject.SetActive(true);
                }
            }
        }
        else {
            hitBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
            splitBtn.gameObject.SetActive(false);
            dealBtn.gameObject.SetActive(false);
        }
    }

    void NotifySplitCard(int pPlayerID) {
        if (pPlayerID == playerID) {
            List<CustomButton> newBettingChipList = new List<CustomButton>();
            foreach (CustomButton chipBtn in bettingChipList) {
                CustomButton btn = Instantiate(chipBtn);
                RectTransform rect = btn.GetComponent(typeof(RectTransform)) as RectTransform;
                rect.sizeDelta = new Vector2(150, 150);
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.localPosition = new Vector3(400,5*newBettingChipList.Count+0,0);
                btn.transform.SetParent(betArea.transform, false);
                btn.onClick -= OnChipClick;
                btn.onClick -= OnRemoveChipClick;
                newBettingChipList.Add(btn);
            }
            bettingChipList.AddRange(newBettingChipList);

            hitBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
            splitBtn.gameObject.SetActive(false);
            dealBtn.gameObject.SetActive(false);

            GameProfile.PlayerTotalMoney -= GameData.currentBet;
            GameData.currentBet *= 2;
            RefreshPlayerTotalMoney();
        }
    }

    void NotifyAskingCard(GameData.PlayerTurn pPlayerData) {
        int playerDataID = pPlayerData.playerID;
        if (playerDataID == playerID) {
            hitBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
            splitBtn.gameObject.SetActive(false);
            dealBtn.gameObject.SetActive(false);
        }
    }

    void NotifyHandResult(GameData.HandWin pHandData) {
        int handID = pHandData.handID;
        bool handWin = pHandData.isWin;
        if (handWin) {
            List<CustomButton> newBettingChipList = new List<CustomButton>();
            for (int i=0;i<bettingChipList.Count;i++) {
                CustomButton chipBtn = bettingChipList[i];
                bool shouldGen = false;
                if (handID == 0) {
                    if (chipBtn.transform.localPosition.x < 500) {
                        shouldGen = true;
                    }
                }
                else {
                    if (chipBtn.transform.localPosition.x > 500) {
                        shouldGen = true;
                    }
                }
                if (shouldGen) {
                    CustomButton btn = Instantiate(chipBtn);
                    RectTransform rect = btn.GetComponent(typeof(RectTransform)) as RectTransform;
                    rect.sizeDelta = new Vector2(150, 150);
                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    btn.transform.SetParent(betArea.transform, false);
                    Vector3 pos = chipBtn.transform.localPosition;
                    pos.x += 100;
                    rect.localPosition = pos;
                    btn.onClick -= OnChipClick;
                    btn.onClick -= OnRemoveChipClick;
                    newBettingChipList.Add(btn);
                }
            }
            bettingChipList.AddRange(newBettingChipList);
        }
        else {
            if (GameData.IsSplitCardMode(0)) {
                List<CustomButton> newBettingChipList = new List<CustomButton>();
                newBettingChipList.AddRange(bettingChipList);
                for (int i=0;i<bettingChipList.Count;i++) {
                    CustomButton chipBtn = bettingChipList[i];
                    bool shouldGen = false;
                    if (handID == 0) {
                        if (chipBtn.transform.localPosition.x < 500)
                            shouldGen = true;
                    }
                    else {
                        if (chipBtn.transform.localPosition.x > 500)
                            shouldGen = true;
                    }
                    if (shouldGen) {
                        newBettingChipList.Remove(chipBtn);
                        DestroyImmediate(chipBtn.gameObject);
                    }
                }
                bettingChipList = newBettingChipList;  
            }
            else {
                List<CustomButton> newBettingChipList = new List<CustomButton>();
                newBettingChipList.AddRange(bettingChipList);
                for (int i=0;i<bettingChipList.Count;i++) {
                    CustomButton chipBtn = bettingChipList[i];
                    newBettingChipList.Remove(chipBtn);
                    DestroyImmediate(chipBtn.gameObject);
                }
                bettingChipList = newBettingChipList;
            }
            // if (GameData.IsSplitCardMode(0) && handID == 0) {
            //     List<CustomButton> newBettingChipList = new List<CustomButton>();
            //     newBettingChipList.AddRange(bettingChipList);
            //     for (int i=0+handID*(bettingChipList.Count/2);i<handID*(bettingChipList.Count/2)+bettingChipList.Count/2;i++) {
            //         CustomButton chipBtn = bettingChipList[i];
            //         newBettingChipList.Remove(chipBtn);
            //         DestroyImmediate(chipBtn.gameObject);
            //     }
            //     bettingChipList = newBettingChipList;
            // }
            // else {
            //     List<CustomButton> newBettingChipList = new List<CustomButton>();
            //     newBettingChipList.AddRange(bettingChipList);
            //     for (int i=0+handID*(bettingChipList.Count/2);i<handID*(bettingChipList.Count/2)+bettingChipList.Count/2;i++) {
            //         CustomButton chipBtn = bettingChipList[i];
            //         newBettingChipList.Remove(chipBtn);
            //         DestroyImmediate(chipBtn.gameObject);
            //     }
            //     bettingChipList = newBettingChipList;
            // }
        }
    }

    void NotifyClean() {
        foreach (CustomButton chipBtn in bettingChipList) {
            DestroyImmediate(chipBtn.gameObject);
        }
        bettingChipList.Clear();
        RefreshPlayerTotalMoney();
    }

    public void NotificationReceived(Notification pNotification, object pData)
    {
        switch (pNotification)
        {
            case Notification.PLAYER_READY:
                NotifyPlayerReady((int)pData);
                break;
            case Notification.BETTING:
                NotifyBetting();
                break;
            case Notification.PLAYER_TURN:
                NotifyPlayerTurn((GameData.PlayerTurn)pData);
                break;
            case Notification.ASK_CARD:
                NotifyAskingCard((GameData.PlayerTurn)pData);
                break;
            case Notification.SPLIT_CARD:
                NotifySplitCard((int)pData);
                break;
            case Notification.HAND_RESULT:
                NotifyHandResult((GameData.HandWin)pData);
                break;
            case Notification.CLEAN:
                NotifyClean();
                break;
        }
    }
}
