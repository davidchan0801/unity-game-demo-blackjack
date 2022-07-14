using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashView : BaseView
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeView());
    }

    IEnumerator ChangeView() {
        yield return new WaitForSeconds(2.0f);
        ViewManager.Instance.ShowView(GameConstants.kViewLayerName, GameConstants.kMenuViewName, GameConstants.kMenuViewPath);
        ViewManager.Instance.CloseView(GameConstants.kViewLayerName, GameConstants.kSplashViewName);
    }
}
