using UnityEngine;
using UnityEngine.EventSystems;

public class Rolling : MonoBehaviour
{
    private bool IsDargging;
    private Vector3 offset;
    public float minY = -8f;
    public float maxY = 6f;

    // Update is called once per frame
    void Update()
    {
        Vector3 inputPos = Vector3.zero;
        bool hasInput = false;
        // sử lý chạm
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPos = touch.position;
            if(touch.phase  == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                hasInput = true;
            }
        }
        // sử lý chuột
        else if(Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            inputPos = Input.mousePosition;
            hasInput = true;
        }

        if (hasInput)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, Camera.main.transform.position.z));
            if(!IsDargging && hasInput) // khi nhấn chuột
            {
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
                if(hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    IsDargging = true;
                    offset = transform.position - worldPos;
                }
            }
            if (IsDargging)
            {
                float newY = Mathf.Clamp(worldPos.y + offset.y, minY, maxY);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }else
            {
                offset.y = 0;
            }
        }
        if(Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase  == TouchPhase.Ended))
        {
            IsDargging = false;
        }
    }
}
