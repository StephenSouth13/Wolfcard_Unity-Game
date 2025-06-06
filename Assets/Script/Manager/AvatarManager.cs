

using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AvatarManager : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public PhotonView view;
    public Button MyButtonPrefab; // đổi lại với knop bên dưới 
                                  // lệnh clcik này sẽ gọi case của ActionOnclickAvatar

    public Image knopImage; // Mark HÌnh tròn;
    public Image Knop;
    public Image avatarImage;
    public Image frameImage;
    public Image board;
    public Image pointed;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI voteText;

    private GamePlayManager AVTClick;
    public void SetData(Player photonPlayer, int index)
    {
        nameText.text = $"#{index + 1}. {photonPlayer.NickName}";
        voteText.text = "";
        knopImage.sprite = Knop.sprite;
        board.sprite = Resources.Load<Sprite>("1");
        pointed.sprite = Resources.Load<Sprite>("2");
        if (photonPlayer.CustomProperties.TryGetValue("AvatarPath", out object avtPathObject))
        {
            string avatarPath = avtPathObject as string;
            Sprite avatarSprite = Resources.Load<Sprite>(avatarPath);
            avatarImage.sprite = avatarSprite;
        }    
        if(photonPlayer.CustomProperties.TryGetValue("FramePath", out object framePathObject))
        {
            string framePath = framePathObject as string;
            Sprite frameSprite = Resources.Load<Sprite>(framePath);
            frameImage.sprite = frameSprite;
        }
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        int index = (int)photonView.InstantiationData[0];
        Transform point = FindFirstObjectByType<RoomManager>().avatarSpawmPoints[index];

        transform.SetParent(point, false);
        transform.localScale = Vector3.one;

        if (photonView.IsMine)
        {
            SetData(PhotonNetwork.LocalPlayer, index);
        }

        SetData(photonView.Owner, index);

        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 100);
        rt.anchoredPosition = Vector2.zero;
    }
    void Start()
    {
        AVTClick = FindFirstObjectByType<GamePlayManager>();
        MyButtonPrefab.onClick.AddListener(() => AVTClick.OnActionButtonClicked(this.photonView.Owner.ActorNumber));
    }
    [PunRPC]
    public void OnAvatarClicked(int attackerActor, int victimActor)
    {
        Debug.Log("Người chơi " + attackerActor + " đã nhấn vào avatar của người chơi " + victimActor);
    }
}
