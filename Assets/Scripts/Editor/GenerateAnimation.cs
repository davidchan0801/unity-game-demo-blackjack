using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
public class GenerateAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [MenuItem("Generate/Start Animation")]
    static void GenerateStartAnimation() {
        Vector2 spacing = new Vector2(20, 20);

        GameObject baseObj = GameObject.Find("Canvas/ViewManager/ViewLayers/MenuView");
        GameObject animation = new GameObject();
        CardSequenceAnimator animator = animation.AddComponent(typeof(CardSequenceAnimator)) as CardSequenceAnimator;
        animation.name = "StartAnimation";
        animation.transform.SetParent(baseObj.transform);
        RectTransform rect = animation.AddComponent(typeof(RectTransform)) as RectTransform;
        rect.sizeDelta = new Vector2(1080,1920);
        rect.localPosition = Vector3.zero;

        GameObject gridCollection = new GameObject();
        gridCollection.name = "GridCollection";
        gridCollection.transform.SetParent(baseObj.transform);
        rect = gridCollection.AddComponent(typeof(RectTransform)) as RectTransform;
        rect.sizeDelta = new Vector2(1080,1920);
        rect.localPosition = Vector3.zero;
        Object[] sprites = Resources.LoadAll("Game/poker");

        for (int i=0;i<14;i++) {
            for (int j=0;j<13;j++) {
                GameObject obj = new GameObject();
                obj.name = string.Format("{0}-{1}",j,i);
                Image img = obj.AddComponent(typeof(Image)) as Image;
                obj.transform.SetParent(gridCollection.transform);
                rect = img.GetComponent(typeof(RectTransform)) as RectTransform;
                rect.sizeDelta = new Vector2(129*0.5f,182*0.5f);
                rect.localPosition = new Vector3(-1080/2+rect.sizeDelta.x/2+j*rect.sizeDelta.x+spacing.x*j, -1920/2+rect.sizeDelta.y/2+i*rect.sizeDelta.y+spacing.y*i);
                Card card = obj.AddComponent(typeof(Card)) as Card;
                card.Image = img;
                int index = (i*5+j)%51+1;
                if (index == 42)
                    index = 43;
                card.frontCardSprite = sprites[index] as Sprite;
                card.backCardSprite = sprites[42] as Sprite;
                card.Image.sprite = card.frontCardSprite;
            }
        }

        for (int i=0;i<13;i++) {
            bool found = false;
            int line = 0;
            GameObject frameObj = new GameObject();
            frameObj.name = string.Format("Frame-{0}",(i+1));
            frameObj.transform.SetParent(animator.transform, false);
            CardFrame frame = frameObj.AddComponent(typeof(CardFrame)) as CardFrame;
            for (int j=i;j>=0;j--) {
                GameObject obj = GameObject.Find(string.Format("Canvas/ViewManager/ViewLayers/MenuView/GridCollection/{0}-{1}",j,line));
                if (obj != null) {
                    frame.Add(obj.GetComponent(typeof(Card)) as Card);
                    obj.transform.SetParent(frame.transform);
                    found = true;
                }
                line++;
            }
            if (found)
                animator.AddFrame(frame);
        }
        DestroyImmediate(gridCollection);
    }
    
}
#endif