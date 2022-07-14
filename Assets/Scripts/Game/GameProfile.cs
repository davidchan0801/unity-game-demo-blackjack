using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProfile {
    static public int PlayerTotalMoney {
        get {
            return PlayerPrefs.GetInt("total", GameData.kDefaultMoney);
        }
        set {
            PlayerPrefs.SetInt("total", value);
        }
    }

    static public int Language {
        get {
            return PlayerPrefs.GetInt("language", 0);
        }
        set {
            PlayerPrefs.SetInt("language", value);
        }
    }
}
