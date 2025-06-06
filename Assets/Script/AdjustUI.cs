using UnityEngine;

public class AdjustUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AdjustCollider();
    }
    void AdjustCollider()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        BoxCollider collider = GetComponent<BoxCollider>();
        if(rectTransform != null&& collider != null)
        {
            collider.size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, collider.size.z);
        }
    }
}
