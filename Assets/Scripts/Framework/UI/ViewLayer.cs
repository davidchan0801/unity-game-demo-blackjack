using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewLayer : MonoBehaviour
{
    protected Dictionary<string, BaseView> views;

    void Awake()
    {
        views = new Dictionary<string, BaseView>();
        RectTransform trans = gameObject.AddComponent<RectTransform>();
        trans.anchorMax = Vector2.one;
        trans.anchorMin = Vector2.zero;
        trans.sizeDelta = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddView(string pViewName, BaseView pView) {
        views[pViewName] = pView;
        pView.transform.SetParent(transform, false);
    }

    public void RemoveView(string pViewName) {
        BaseView view = views[pViewName];
        views.Remove(pViewName);
        Destroy(view.gameObject);
    }
}