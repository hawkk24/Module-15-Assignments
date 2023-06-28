using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicScript : MonoBehaviour
{
    public int livesCounter = 0;
    public TextMeshProUGUI livesCounterText;
    public int relicsCounter = 0;
    public TextMeshProUGUI relicsCounterText;

    public void addLife()
    {
        livesCounter += 1;
        livesCounterText.text = livesCounter.ToString();
    }

    public void addRelic()
    {
        relicsCounter += 1;
        relicsCounterText.text = relicsCounter.ToString();
    }
}


