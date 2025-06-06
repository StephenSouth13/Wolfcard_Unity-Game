using UnityEngine;

public class ScrollHandler : MonoBehaviour
{
    public GameObject Right_hand;
    public GameObject Left_hand;
    private bool isScrolling = false;
    private bool RightHandMoving = true;
    private Vector3 RightPos;
    private Vector3 LeftPos;
    private Vector3 MidPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MidPos = transform.position;
        RightPos = Right_hand.transform.position;
        LeftPos = Left_hand.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0)  || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            isScrolling = true;
        }
        else if(Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            isScrolling = false;
        }
        if(isScrolling )
        {
            Vector3 midPos = transform.position;
            if (RightHandMoving)
            {
                Right_hand.transform.position = new Vector3(RightPos.x, midPos.y - 0.5f, RightPos.z);
            }
            else
            {
                Left_hand.transform.position = new Vector3(LeftPos.x, midPos.y - 0.5f, LeftPos.z);
                Right_hand.transform.position = new Vector3(RightPos.x, RightPos.y - 1f, RightPos.z);
            }
        }
        if (!isScrolling)
        {
            if (RightHandMoving)
            {
                RightHandMoving = false;
            }else
            {
                RightHandMoving= true;
            }
        }
    }
    public void ResetPosition()
    {
        transform.position = MidPos;
        Right_hand.transform.position = RightPos;
        Left_hand.transform.position = LeftPos;
    }
}
