using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ViewManager.Instance.AddLayer(GameConstants.kViewLayerName);
        ViewManager.Instance.AddLayer(GameConstants.kHUDLayerName);
        ViewManager.Instance.AddLayer(GameConstants.kModalLayerName);

        ViewManager.Instance.ShowView(GameConstants.kViewLayerName, GameConstants.kSplashViewName, GameConstants.kSplashViewPath);


        // https://docs.google.com/spreadsheets/d/1EgdbqTs184X3QZZKnumI9PFqGhiTl9pdkidFKSUsLAE/edit?usp=sharing
        // Copy text directly from the above link
        LocalizationManager.Instance.Load("en", new Dictionary<string,string>{{"menu_title","MENU"},{"menu_blackjack","BLACKJACK"},{"button_hit","HIT"},{"button_split","SPLIT"},{"button_stand","STAND"},{"button_deal","DEAL"},{"button_no","NO"},{"button_yes","YES"},{"setting_title","SETTING"},{"setting_bgm","BGM"},{"setting_sfx","SFX"},{"setting_volume","VOLUME"},{"setting_reset","RESET MONEY"},{"setting_home","HOME"},{"btn_reset","RESET"},{"btn_go","GO"},{"reset_title","Do you want to reset the game?"},{"player_win","PLAYER WIN"},{"dealer_win","DEALER WIN"},{"player_draw","DRAW"},{"hand1_win","HAND 1 WIN"},{"hand1_lose","HAND 1 LOST"},{"hand1_draw","HAND 1 DRAW"},{"hand2_win","HAND 2 WIN"},{"hand2_lose","HAND 2 LOST"},{"hand2_draw","HAND 2 DRAW"},{"setting_language","LANGUAGE"}});
        LocalizationManager.Instance.Load("es", new Dictionary<string,string>{{"menu_title","MENÚ"},{"menu_blackjack","VEINTIUNA"},{"button_hit","PEGAR"},{"button_split","SEPARAR"},{"button_stand","PARARSE"},{"button_deal","ACUERDO"},{"button_no","NO"},{"button_yes","SÍ"},{"setting_title","AJUSTE"},{"setting_bgm","música de fondo"},{"setting_sfx","SFX"},{"setting_volume","VOLUMEN"},{"setting_reset","RESTABLECER DINERO"},{"setting_home","HOGAR"},{"btn_reset","REINICIAR"},{"btn_go","VAMOS"},{"reset_title","¿Quieres reiniciar el juego?"},{"player_win","JUGADOR GANADO"},{"dealer_win","CONCESIONARIO GANADO"},{"player_draw","JUEGO DE DIBUJO"},{"hand1_win","MANO 1 GANA"},{"hand1_lose","MANO 1 PERDIDA"},{"hand1_draw","MANO 1 JUEGO DE DIBUJO"},{"hand2_win","MANO 2 GANA"},{"hand2_lose","MANO 2 PERDIDA"},{"hand2_draw","MANO 2 JUEGO DE DIBUJO"},{"setting_language","IDIOMA"}});
        LocalizationManager.Instance.SetLanguage(GameData.kDefaultLanguageKey[GameProfile.Language]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
