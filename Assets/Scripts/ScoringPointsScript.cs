using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScoringPointsScript : MonoBehaviour
{
    private int game = 1;
    public BallScript ball;
    private int points = 0;
    public List<ObjectFallScript> pins1 = new List<ObjectFallScript>();
    public List<ObjectFallScript> pins2 = new List<ObjectFallScript>();
    public GameObject button;

    void Start()
    {
        
    }

    void Update()
    {
        if (pins1[0].gameObject.activeInHierarchy && Scoring(pins1) == 10)
        {
            points = 10;
            pins1[0].gameObject.transform.parent.gameObject.SetActive(false);

            if (button.activeInHierarchy)
            {
                pins2[0].gameObject.transform.parent.gameObject.SetActive(true);
                button.SetActive(false);
            }
                ball.ComeBack();
            
        }
        else
        {
            points = Scoring(pins1);
        }
        if (pins2[0].gameObject.activeInHierarchy)
        {
            points += Scoring(pins2);
        }

        gameObject.GetComponent<TextMeshProUGUI>().text = points.ToString();
    }

    int Scoring(List<ObjectFallScript> pins)
    {
        int X = 0;
        foreach (var pin in pins)
        {
            if (pin.fall)
            {
                X++;
            }
        }
        return X;
    }
}
