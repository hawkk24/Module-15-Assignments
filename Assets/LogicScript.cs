using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    public int livesCounter = 0;
    public int relicsCounter = 0;

    public void addLife()
    {
        livesCounter += 1;
        // add logic to update UI
    }

    public void addRelic()
    {
        relicsCounter += 1;
        // add logic to update UI
    }
}


