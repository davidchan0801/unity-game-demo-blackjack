using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance;
    [SerializeField] private GameObject layerParent;
    protected Dictionary<string, ViewLayer> layers;
    
    // Start is called before the first frame update
    void Awake()
    {
        layers = new Dictionary<string, ViewLayer>();
        ViewManager.Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddLayer(string pLayerName)
    {
        if (layers.ContainsKey(pLayerName))
        {
            Debug.LogError(string.Format("Layer '{0}' already exists.", pLayerName));
            return;
        }

        ViewLayer layer = new GameObject().AddComponent(typeof(ViewLayer)) as ViewLayer;

        layer.name = pLayerName;
        layer.transform.SetParent(layerParent.transform, false);

        layers[pLayerName] = layer;
    }

    public BaseView ShowView(string pLayerName, string pViewName, string pViewPath) {
        ViewLayer layer = layers[pLayerName];
        GameObject viewObj = Instantiate(Resources.Load(pViewPath)) as GameObject;
        BaseView view = viewObj.GetComponent(typeof(BaseView)) as BaseView;
        if (view != null)
            layer.AddView(pViewName, view);
        else
            Debug.LogError(string.Format("View '{0}' does not exist.", pViewPath));
        return view;
    }

    public void CloseView(string pLayerName, string pViewName) {
        ViewLayer layer = layers[pLayerName];
        layer.RemoveView(pViewName);
    }
}
