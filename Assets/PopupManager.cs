using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public bool isPopupOpen = false;
    private GameObject popup = null;
    [SerializeField] private GameObject relicPopup;
    [SerializeField] private GameObject relic2Popup;
    Dictionary<string, GameObject> relicDict;

    private void Awake()
    {
        relicDict = new Dictionary<string, GameObject>();
        relicDict.Add("Relic", relicPopup);
        relicDict.Add("Relic2", relic2Popup);
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

    public void hidePopup()
    {
        if (popup is not null)
        {
            popup.SetActive(false);
            isPopupOpen = false;
            popup = null;
        }
    }
}
