using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameView : BaseView, IListener
{
    [SerializeField] private Sprite[] cardSprite;
    [SerializeField] private Image cardDeckImg;
    [SerializeField] private Image[] playerPanel;
    [SerializeField] private GameObject cardCollectionObj;
    private List<Card> cardDeck = new List<Card>();

    static bool isTestingMode = false;
    static int testingCardIndex = 0;
    int[] testingCardSequences = {0,1,13,3,4,5,6,7,10,11,19,20,21,22};
    
    // Start is called before the first frame update
    void Start()
    {
        NotificationManager.AddListener(this, Notification.PLAYER_READY, Notification.DISTRIBUTE_CARD, Notification.PLAYER_TURN, Notification.ASK_CARD, Notification.SPLIT_CARD, Notification.END_TURN, Notification.CLEAN);
        StartCoroutine(Setup());
    }

    void OnDestroy()
    {
        NotificationManager.RemoveListener(this);
    }

    IEnumerator Setup() {
        GameData.playerCards = new List<Card>[4];
        for (int i=0;i<4;i++)
            GameData.playerCards[i] = new List<Card>();
        ShuffleCard();
        yield return new WaitForSeconds(1.0f);
        if (GameProfile.PlayerTotalMoney <= 0) {
            QuestionPopup popup = QuestionPopup.Open(LocalizationManager.Instance.GetString("reset_title"));
            popup.onClick += OnResetGameClick;
        }
        else
            NotificationManager.SendNotification(Notification.PLAYER_READY, 0);
    }

    void ShuffleCard() {
        foreach (Card card in cardDeck) {
            DestroyImmediate(card.gameObject);
        }
        cardDeck.Clear();
        for (int i=0;i<GameData.kDefaultDeckNum;i++){
            for (int j=0;j<cardSprite.Length-1;j++) {
                GameObject obj = new GameObject();
                obj.name = string.Format("{0}-{1}",j/13+1,j%13+1);
                Image cardImg = obj.AddComponent(typeof(Image)) as Image;
                Card card = obj.AddComponent(typeof(Card)) as Card;
                card.CardIndex = j;
                card.Image = cardImg;
                cardDeck.Add(card);
                obj.transform.SetParent(cardCollectionObj.transform, false);
            }
        }
    }

    void DistrubteCard(int pPlayerID, bool pFlipped, TweenCallback pCallback, int pPlayerIndex = 0) {
        SoundManager.Instance.Play(0, "card-sounds-35956");
        Vector3 pos = playerPanel[pPlayerID].transform.localPosition;
        if (GameData.IsSplitCardMode(pPlayerID)) {
            if (pPlayerIndex == 0)
                pos.x -= 200;
            else
                pos.x += 150;
            pos.x += 50*GameData.playerCards[pPlayerID*2+pPlayerIndex].Count;
        }
        else {
            pos.x += 50*GameData.playerCards[pPlayerID*2+pPlayerIndex].Count;
        }
        pos.y = 0;
            pos.z = 0;

        int cardIndex = 0;
        if (isTestingMode)
            cardIndex = testingCardSequences[testingCardIndex++];
        else
            cardIndex = Random.Range(0,cardDeck.Count);
        Card card = cardDeck[cardIndex];
        if (!isTestingMode)
            cardDeck.Remove(card);
        GameData.playerCards[pPlayerID*2+pPlayerIndex].Add(card);

        RectTransform rect = card.Image.GetComponent(typeof(RectTransform)) as RectTransform;
        rect.sizeDelta = new Vector2(129*1.5f,182*1.5f);
        card.Image.transform.SetParent(playerPanel[pPlayerID].transform, false);
        if (pFlipped)
            card.Image.sprite = cardSprite[cardSprite.Length-1];
        else
            card.Image.sprite = cardSprite[card.CardIndex];
        card.Image.transform.position = cardDeckImg.transform.position;
        card.Image.transform.localScale = Vector3.zero;
        card.Image.transform.DOScale(Vector3.one, 0.5f);
        card.Image.transform.DOLocalMove(pos, 0.5f).OnComplete(() => {
            pCallback();
        });
    }

    void OnResetGameClick(QuestionPopup popup, bool pShouldReset) {
        popup.onClick -= OnResetGameClick;
        if (pShouldReset) {
            GameProfile.PlayerTotalMoney = GameData.kDefaultMoney;
            NotificationManager.SendNotification(Notification.PLAYER_READY, 0);
        }
        else {
        }
    }

    void NotifyPlayerReady(int pPlayerID) {
        if (pPlayerID != 1){
            if (cardDeck.Count <= GameData.kDefaultDeckNum*52/2)
                ShuffleCard();
            NotificationManager.SendNotification(Notification.BETTING);
        }
    }

    void NotifyDistributeCard() {
        DistrubteCard(0, false, ()=> {
            DistrubteCard(1, true, ()=> {
                DistrubteCard(0, false, ()=> {
                    DistrubteCard(1, false, ()=> {
                        int playerNumber = GameData.PlayerCardNumber(0);
                        int dealerNumber = GameData.PlayerCardNumber(2);
                        if (dealerNumber == 21) {
                            GameData.playerCards[2][0].Image.sprite = cardSprite[GameData.playerCards[2][0].CardIndex];
                            NotificationManager.SendNotification(Notification.END_TURN);
                        }
                        else if (playerNumber == 21) {
                            NotificationManager.SendNotification(Notification.PLAYER_TURN, new GameData.PlayerTurn(1, 0));
                        }
                        else {
                            NotificationManager.SendNotification(Notification.PLAYER_TURN, new GameData.PlayerTurn(0, 0));
                        }
                    });
                });
            });
        });
    }

    void NotifyAskingCard(GameData.PlayerTurn pPlayerData) {
        int playerDataPlayerID = pPlayerData.playerID;
        int playerDataHandIndex = pPlayerData.handID;
        DistrubteCard(playerDataPlayerID, false, ()=> {
            int currentPlayerNumber = GameData.PlayerCardNumber(playerDataPlayerID*2+playerDataHandIndex);
            if (currentPlayerNumber > 21) {
                if (playerDataPlayerID == 0) {
                    if (GameData.playerCards[1].Count == 0)
                        NotificationManager.SendNotification(Notification.END_TURN, 1);
                    else  {
                        if (playerDataHandIndex == 0)
                            NotificationManager.SendNotification(Notification.PLAYER_TURN, new GameData.PlayerTurn(playerDataPlayerID, playerDataHandIndex+1));
                        else if (playerDataHandIndex == 1) {
                            int playerNumber1 = GameData.PlayerCardNumber(playerDataPlayerID*2);
                            if (playerNumber1 > 21)
                                NotificationManager.SendNotification(Notification.END_TURN, 1);
                            else
                                NotificationManager.SendNotification(Notification.PLAYER_TURN, new GameData.PlayerTurn(1, 0));
                        }
                    }
                }
                else if (playerDataPlayerID == 1)
                    NotificationManager.SendNotification(Notification.END_TURN, 1);
            }
            else if (currentPlayerNumber == 21) {
                if (playerDataPlayerID == 0) {
                    if (GameData.playerCards[1].Count == 0)
                        NotificationManager.SendNotification(Notification.PLAYER_TURN, new GameData.PlayerTurn(1, 0));
                    else  {// Split Card Mode
                        if (playerDataHandIndex == 0)
                            NotificationManager.SendNotification(Notification.PLAYER_TURN, new GameData.PlayerTurn(playerDataPlayerID, playerDataHandIndex+1));
                        else if (playerDataHandIndex == 1)
                            NotificationManager.SendNotification(Notification.PLAYER_TURN, new GameData.PlayerTurn(1, 0));
                    }
                }
                else if (playerDataPlayerID == 1)
                    NotificationManager.SendNotification(Notification.END_TURN);
            }
            else
                NotificationManager.SendNotification(Notification.PLAYER_TURN, new GameData.PlayerTurn(playerDataPlayerID, playerDataHandIndex));
        }, playerDataHandIndex);
    }

    void NotifySplitCard(int pPlayerID) {
        GameData.playerCards[1].Add(GameData.playerCards[0][1]);
        GameData.playerCards[0].Remove(GameData.playerCards[0][1]);

        Vector3 pos = playerPanel[pPlayerID].transform.position;
        pos.x -= 200;
        GameData.playerCards[0][0].Image.transform.position = pos;
        pos = playerPanel[pPlayerID].transform.position;
        pos.x += 150;
        GameData.playerCards[1][0].Image.transform.position = pos;
        
        DistrubteCard(0, false, () => {
            DistrubteCard(0, false, () => {
                NotificationManager.SendNotification(Notification.PLAYER_TURN, new GameData.PlayerTurn(0, 0));
            }, 1);
        });
    }

    void NotifyPlayerTurn(GameData.PlayerTurn pPlayerData) {
        int playerDataID = pPlayerData.playerID;
        int playerDataHandIndex = pPlayerData.handID;
        if (playerDataID == 1) {
            int dealerNumber = GameData.PlayerCardNumber(playerDataID*2+playerDataHandIndex);
            if (dealerNumber < 17) {
                NotificationManager.SendNotification(Notification.ASK_CARD, new GameData.PlayerTurn(playerDataID, playerDataHandIndex));
            }
            else {
                NotificationManager.SendNotification(Notification.END_TURN);
            }
        }
    }

    void OnMessageClick(MessagePopup popup) {
        popup.onClick -= OnMessageClick;
        NotificationManager.SendNotification(Notification.CLEAN, 0);
    }

    void NotifyEndTurn() {
        GameData.playerCards[2][0].Image.sprite = cardSprite[GameData.playerCards[2][0].CardIndex];
        int playerNumber1 = GameData.PlayerCardNumber(0);
        int playerNumber2 = 0;
        bool isSplitMode = GameData.IsSplitCardMode(0);
        if (isSplitMode)
            playerNumber2 = GameData.PlayerCardNumber(1);
        int dealerNumber = GameData.PlayerCardNumber(2);
        
        string handMessage = "";
        if (playerNumber1 == 21 && dealerNumber != 21) {
            if (!isSplitMode) {
                MessagePopup popup = MessagePopup.Open(LocalizationManager.Instance.GetString("player_win"));
                popup.onClick += OnMessageClick;
                GameProfile.PlayerTotalMoney += GameData.currentBet*2;
            }
            else {
                handMessage = LocalizationManager.Instance.GetString("hand1_win");
                GameProfile.PlayerTotalMoney += GameData.currentBet;
            }
            SoundManager.Instance.Play(0,"success");
            NotificationManager.SendNotification(Notification.HAND_RESULT, new GameData.HandWin(0, true));
        }
        else if (dealerNumber == 21) {
            if (!isSplitMode) {
                MessagePopup popup = MessagePopup.Open(LocalizationManager.Instance.GetString("dealer_win"));
                popup.onClick += OnMessageClick;
            }
            else{
                handMessage = LocalizationManager.Instance.GetString("hand1_lose");
            }
            NotificationManager.SendNotification(Notification.HAND_RESULT, new GameData.HandWin(0, false));
        }
        else if (playerNumber1 > 21) {
            if (!isSplitMode) {
                MessagePopup popup = MessagePopup.Open(LocalizationManager.Instance.GetString("dealer_win"));
                popup.onClick += OnMessageClick;
            }
            else{
                handMessage = LocalizationManager.Instance.GetString("hand1_lose");
            }
            NotificationManager.SendNotification(Notification.HAND_RESULT, new GameData.HandWin(0, false));
        }
        else if (dealerNumber > 21) {
            if (!isSplitMode) {
                MessagePopup popup = MessagePopup.Open(LocalizationManager.Instance.GetString("player_win"));
                popup.onClick += OnMessageClick;
                GameProfile.PlayerTotalMoney += GameData.currentBet*2;
            }
            else{
                handMessage = LocalizationManager.Instance.GetString("hand1_win");
                GameProfile.PlayerTotalMoney += GameData.currentBet;
            }
            SoundManager.Instance.Play(0,"success");
            NotificationManager.SendNotification(Notification.HAND_RESULT, new GameData.HandWin(0, true));
        }
        else if (dealerNumber > playerNumber1) {
            if (!isSplitMode) {
                MessagePopup popup = MessagePopup.Open(LocalizationManager.Instance.GetString("dealer_win"));
                popup.onClick += OnMessageClick;
            }
            else{
                handMessage = LocalizationManager.Instance.GetString("hand1_lose");
            }
            NotificationManager.SendNotification(Notification.HAND_RESULT, new GameData.HandWin(0, false));
        }
        else if (playerNumber1 > dealerNumber) {
            if (!isSplitMode) {
                MessagePopup popup = MessagePopup.Open(LocalizationManager.Instance.GetString("player_win"));
                
                popup.onClick += OnMessageClick;
                GameProfile.PlayerTotalMoney += GameData.currentBet*2;
            }
            else{
                handMessage = LocalizationManager.Instance.GetString("hand1_win");
                GameProfile.PlayerTotalMoney += GameData.currentBet;
            }
            SoundManager.Instance.Play(0,"success");
            NotificationManager.SendNotification(Notification.HAND_RESULT, new GameData.HandWin(0, true));
        }
        else if (playerNumber1 == dealerNumber) {
            if (!isSplitMode) {
                MessagePopup popup = MessagePopup.Open(LocalizationManager.Instance.GetString("player_draw"));
                popup.onClick += OnMessageClick;
                GameProfile.PlayerTotalMoney += GameData.currentBet;
            }
            else{
                handMessage = LocalizationManager.Instance.GetString("hand1_draw");
                GameProfile.PlayerTotalMoney += GameData.currentBet/2;
            }
            GameProfile.PlayerTotalMoney += GameData.currentBet;
        }
        else {
            Debug.LogError(string.Format("PlayerNumber1: {0} DealerNumber: {1}", playerNumber1, dealerNumber));
        }

        if (isSplitMode) {
            if (playerNumber2 == 21 && dealerNumber != 21) {
                handMessage += "\n"+LocalizationManager.Instance.GetString("hand2_win");
                MessagePopup popup = MessagePopup.Open(handMessage);
                SoundManager.Instance.Play(0,"success");
                popup.onClick += OnMessageClick;
                GameProfile.PlayerTotalMoney += GameData.currentBet;
                NotificationManager.SendNotification(Notification.HAND_RESULT, new GameData.HandWin(1, true));
            }
            else if (dealerNumber == 21) {
                handMessage += "\n"+LocalizationManager.Instance.GetString("hand2_lose");
                MessagePopup popup = MessagePopup.Open(handMessage);
                popup.onClick += OnMessageClick;
                NotificationManager.SendNotification(Notification.HAND_RESULT, new GameData.HandWin(1, false));
            }
            else if (playerNumber2 > 21) {
                handMessage += "\n"+LocalizationManager.Instance.GetString("hand2_lose");
                MessagePopup popup = MessagePopup.Open(handMessage);
                popup.onClick += OnMessageClick;
                NotificationManager.SendNotification(Notification.HAND_RESULT, new GameData.HandWin(1, false));
            }
            else if (dealerNumber > 21) {
                handMessage += "\n"+LocalizationManager.Instance.GetString("hand2_win");
                MessagePopup popup = MessagePopup.Open(handMessage);
                SoundManager.Instance.Play(0,"success");
                popup.onClick += OnMessageClick;
                GameProfile.PlayerTotalMoney += GameData.currentBet;
                NotificationManager.SendNotification(Notification.HAND_RESULT, new GameData.HandWin(1, true));
            }
            else if (dealerNumber > playerNumber2) {
                handMessage += "\n"+LocalizationManager.Instance.GetString("hand2_lose");
                MessagePopup popup = MessagePopup.Open(handMessage);
                popup.onClick += OnMessageClick;
                NotificationManager.SendNotification(Notification.HAND_RESULT, new GameData.HandWin(1, false));
            }
            else if (playerNumber2 > dealerNumber) {
                handMessage += "\n"+LocalizationManager.Instance.GetString("hand2_win");
                MessagePopup popup = MessagePopup.Open(handMessage);
                SoundManager.Instance.Play(0,"success");
                popup.onClick += OnMessageClick;
                GameProfile.PlayerTotalMoney += GameData.currentBet;
                NotificationManager.SendNotification(Notification.HAND_RESULT, new GameData.HandWin(1, true));
            }
            else if (playerNumber2 == dealerNumber) {
                handMessage += "\n"+LocalizationManager.Instance.GetString("hand2_draw");
                MessagePopup popup = MessagePopup.Open(handMessage);
                popup.onClick += OnMessageClick;
                GameProfile.PlayerTotalMoney += GameData.currentBet/2;
            }
            else {
                Debug.LogError(string.Format("PlayerNumber2: {0} DealerNumber: {1}", playerNumber2, dealerNumber));
            }
        }
    }

    void NotifyClean() {
        for (int i=0;i<GameData.playerCards.Length;i++) {
            foreach(Card card in GameData.playerCards[i]) {
                DestroyImmediate(card.gameObject);
            }
            GameData.playerCards[i].Clear();
        }
        GameData.currentBet = 0;
        if (GameProfile.PlayerTotalMoney <= 0) {
            QuestionPopup popup = QuestionPopup.Open(LocalizationManager.Instance.GetString("reset_title"));
            popup.onClick += OnResetGameClick;
        }
        else {
            NotificationManager.SendNotification(Notification.PLAYER_READY, 0);
        }
    }

    public void NotificationReceived(Notification pNotification, object pData)
    {
        switch (pNotification)
        {
            case Notification.PLAYER_READY:
                NotifyPlayerReady((int)pData);
                break;
            case Notification.DISTRIBUTE_CARD:
                NotifyDistributeCard();
                break;
            case Notification.ASK_CARD:
                NotifyAskingCard((GameData.PlayerTurn)pData);
                break;
            case Notification.SPLIT_CARD:
                NotifySplitCard((int)pData);
                break;
            case Notification.PLAYER_TURN:
                NotifyPlayerTurn((GameData.PlayerTurn)pData);
                break;
            case Notification.END_TURN:
                NotifyEndTurn();
                break;
            case Notification.CLEAN:
                NotifyClean();
                break;
        }
    }
}
