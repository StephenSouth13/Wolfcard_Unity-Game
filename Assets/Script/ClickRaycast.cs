using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickRaycast : MonoBehaviour
{
    public GameObject Rule_panel;
    public GameObject Change_Rule;

    public GameObject Menu_panel;
    public Image Menu_Image;
    
    public PolygonCollider2D polygonCollider;

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
        if (Rule_panel.activeSelf)
        {
            if (Change_Rule.activeSelf)
            {
                if (isClicked)
                {
                    pointerEvenData.position = clickPosition;
                    List<RaycastResult> results = new List<RaycastResult>();
                    raycaster.Raycast(pointerEvenData, results);

                    bool ClickOnImage = false;
                    foreach (RaycastResult result in results)
                    {
                        if (result.gameObject == Change_Rule)
                        {
                            ClickOnImage = true;
                            break;
                        }
                    }

                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(clickPosition);
                    if (!ClickOnImage && !polygonCollider.OverlapPoint(mousePosition))
                    {
                        Rule_panel.SetActive(false);
                    }
                        
                }
            }
        }
         if (Menu_panel.activeSelf)
        {
            if (isClicked)
            {
                pointerEvenData.position = clickPosition;
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(pointerEvenData, results);
                bool ClickOnImage = false;
                foreach(RaycastResult result in results)
                {
                    if (result.gameObject == Menu_Image.gameObject)
                    {
                        ClickOnImage = true;
                        break;
                    }
                }
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(clickPosition);
                if (!ClickOnImage)
                {
                    Menu_panel.SetActive(false);
                }
            }

        }
    }
}
