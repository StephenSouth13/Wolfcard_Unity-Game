using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class ClickRaycast2 : MonoBehaviour
{
    public GameObject CardVillager_panel;
    public Image MidPaper1;
    public Image Right_hand1;
    public Image Left_hand1;

    public GameObject CardWolf_panel;
    public Image MidPaper2;
    public Image Right_hand2;
    public Image Left_hand2;

    public GameObject CardSolo_panel;
    public Image MidPaper3;
    public Image Right_hand3;
    public Image Left_hand3;

    public GraphicRaycaster raycaster;
    private PointerEventData pointerEvenData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointerEvenData = new PointerEventData(EventSystem.current);
    }

    // Update is called once per frame
    void Update()
    {
        bool isClicked = false;
        Vector2 clickPosition = Vector2.zero;
        if (Input.GetMouseButtonDown(0))
        {
            isClicked = true;
            clickPosition = Input.mousePosition;
        }
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            isClicked = true;
            clickPosition = Input.GetTouch(0).position;
        }
        if (CardVillager_panel.activeSelf)
        {
            if (isClicked)
            {
                pointerEvenData.position = clickPosition;
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(pointerEvenData, results);
                bool ClickOnImage = false;

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject == MidPaper1.gameObject || result.gameObject == Left_hand1.gameObject || result.gameObject == Right_hand1.gameObject)
                    {
                        ClickOnImage = true;
                        break;
                    }
                }
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(clickPosition);
                if (!ClickOnImage)
                {
                    CardVillager_panel.SetActive(false);
                    CardWolf_panel.SetActive(false);
                    CardSolo_panel.SetActive(false);
                }
            }
        }
        if (CardWolf_panel.activeSelf)
        {
            if (isClicked)
            {
                pointerEvenData.position = clickPosition;
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(pointerEvenData, results);
                bool ClickOnImage = false;

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject == MidPaper2.gameObject || result.gameObject == Left_hand2.gameObject || result.gameObject == Right_hand2.gameObject)
                    {
                        ClickOnImage = true;
                        break;
                    }
                }
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(clickPosition);
                if (!ClickOnImage)
                {
                    CardVillager_panel.SetActive(false);
                    CardWolf_panel.SetActive(false);
                    CardSolo_panel.SetActive(false);
                }
            }
        }
        if (CardSolo_panel.activeSelf)
        {
            if (isClicked)
            {
                pointerEvenData.position = clickPosition;
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(pointerEvenData, results);
                bool ClickOnImage = false;

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject == MidPaper3.gameObject || result.gameObject == Left_hand3.gameObject || result.gameObject == Right_hand3.gameObject)
                    {
                        ClickOnImage = true;
                        break;
                    }
                }
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(clickPosition);
                if (!ClickOnImage)
                {
                    CardVillager_panel.SetActive(false);
                    CardWolf_panel.SetActive(false);
                    CardSolo_panel.SetActive(false);
                }
            }
        }

    }
}