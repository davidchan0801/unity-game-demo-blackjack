using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardFrame : MonoBehaviour
{
    [SerializeField] private List<Card> cards = new List<Card>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Add(Card card) {
        cards.Add(card);
    }

    public void ShowBackSprite() {
        foreach(Card card in cards) {
            card.Image.sprite = card.backCardSprite;
        }
    }

    public void ShowFrontSprite() {
        foreach(Card card in cards) {
            card.Image.sprite = card.frontCardSprite;
        }
    }

    public void Flip() {
        foreach(Card card in cards) {
            card.Flip();
        }
    }
}
