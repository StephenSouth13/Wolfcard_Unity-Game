using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject PanelToClose;
    public HashSet<GameObject> ignoreGameObject = new HashSet<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(Transform child in PanelToClose.transform)
        {
            ignoreGameObject.Add(child.gameObject);
        }
    }

    public void OnPointerClick(PointerEventData evenData)
    {
        foreach(GameObject ojt in ignoreGameObject)
        {
            if (evenData.pointerPress == ojt) return;
        }
        PanelToClose.SetActive(false);
    }
}
