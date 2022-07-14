using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public class PlayerTurn {
        public PlayerTurn() {
            playerID = 0;
            handID = 0;
        }
        
        public PlayerTurn(int pPlayerID, int pHandID) {
            playerID = pPlayerID;
            handID = pHandID;
        }

        public int playerID;
        public int handID;
    }

    public class HandWin {
        public HandWin() {
            handID = 0;
            isWin = false;
        }
        
        public HandWin(int pHandID, bool pIsWin) {
            handID = pHandID;
            isWin = pIsWin;
        }

        public int handID;
        public bool isWin;
    }

    public const int kDefaultMoney = 10000;
    public const int kDefaultDeckNum = 4;
    static public readonly string[] kDefaultLanguage = {"ENGLISH", "ESPAÑOLA"};
    static public readonly string[] kDefaultLanguageKey = {"en", "es"};
    static public List<Card>[] playerCards;
    static public int currentBet;

    static private int _PlayerCardNumber(int pPlayerIndex, bool countAceAs11, ref bool hasAce) {
        int number = 0;
        for (int i=0;i<playerCards[pPlayerIndex].Count;i++) {
            int currentCardNumber = playerCards[pPlayerIndex][i].CardNumber;
            if (currentCardNumber == 1 && countAceAs11) {
                currentCardNumber = 11;
                hasAce = true;
            }
            number += currentCardNumber;
        }
        return number;
    }

    static public int PlayerCardNumber(int pPlayerIndex) {
        bool hasAce = false;
        int number = _PlayerCardNumber(pPlayerIndex, true, ref hasAce);
        if (number > 21 && hasAce) {
            number = _PlayerCardNumber(pPlayerIndex, false, ref hasAce);
        }
        return number;
    }

    static public bool IsSplitCardMode(int pPlayerID) {
        return GameData.playerCards[pPlayerID*2+1].Count > 0;
    }

}
