using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PopupManager : MonoBehaviour
{
    public bool isPopupOpen = true;

    [SerializeField] private GameObject relicPopup;
    [SerializeField] private GameObject relic2Popup;
    [SerializeField] private GameObject startingPopup;
    [SerializeField] private GameObject finishingPopup;

    private GameObject popup = null;
    private LogicScript logicScript;
    private Dictionary<string, GameObject> relicDict;

    private void Awake()
    {
        relicDict = new Dictionary<string, GameObject>();
        relicDict.Add("Relic", relicPopup);
        relicDict.Add("Relic2", relic2Popup);
        popup = startingPopup;
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    public void showRelicPopup(string relicObjName)
    {
        try
        {
            popup = relicDict[relicObjName];
            isPopupOpen = true;
            popup.SetActive(true);
        }
        catch
        {
            Debug.LogWarning(string.Format("Unknown Relic Object Name '{0}' was sent.", relicObjName));
        }
    }

    public void showFinishingPopup()
    {
        popup = finishingPopup;
        isPopupOpen = true;
        TextMeshProUGUI congratsObj = finishingPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        string congratsText = congratsObj.text;
        congratsText = congratsText.Replace("{livesCounter}", logicScript.livesCounter.ToString());
        congratsText = congratsText.Replace("{relicsCounter}", logicScript.relicsCounter.ToString());
        congratsObj.text = congratsText;
        popup.SetActive(true);
    }

    public void hidePopup()
    {
        if (popup is not null)
        {
            popup.SetActive(false);
            isPopupOpen = false;
            if (popup.Equals(finishingPopup))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            popup = null;
        }
    }
}
