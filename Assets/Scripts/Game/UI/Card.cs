using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Card : MonoBehaviour
{
    public int CardIndex;
    public Image Image;

    public int CardNumber {
        get {
            int number = CardIndex%13;
            number += 1;
            if (number > 10)
                number = 10;
            return number;
        }
    }

    public int CardActualNumber {
        get {
            int number = CardIndex%13;
            number += 1;
            return number;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
