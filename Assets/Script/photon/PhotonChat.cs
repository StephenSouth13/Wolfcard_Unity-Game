using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PhotonChat : MonoBehaviourPunCallbacks, IOnEventCallback
{
    
    public TMP_InputField inputField;
    public Button send;
    public Transform ChatDisplay;
    public GameObject MessagePrefab;
    private const byte ChatEventCode = 1;
    private List<string> chatHistory = new List<string>();
    private GamePlayManager editGame;

    public bool ifAlive = true; // dữ liệu kiểm soát Chat
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        editGame = FindFirstObjectByType<GamePlayManager>();
        
        inputField.onEndEdit.AddListener(HandleSubmit);
        send.onClick.AddListener(SendMessage);
        PhotonNetwork.AddCallbackTarget(this);
    }
    #region messageSend
    public void HandleSubmit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMessage();
        }
    }
    [PunRPC]
    void ReceiveMessageFromClient(string message, int senderActorNumber)
    {
        if (string.IsNullOrEmpty(JsonLogin.Instance.NameUser))
        {
            Debug.Log("Name User chưa được gán!!");
            return;
        }
        var OrderedPlayer = PhotonNetwork.PlayerList.OrderBy(Player => Player.ActorNumber).ToArray();
        int JoinOrder = System.Array.IndexOf(OrderedPlayer, PhotonNetwork.CurrentRoom.GetPlayer(senderActorNumber)) + 1;
        string FullMessage = $"{JsonLogin.Instance.NameUser}({JoinOrder}): {message}";

        chatHistory.Add(FullMessage);

        // Chỉ thêm vào chatHistory nếu người gửi không phải là chính MasterClient
        if (PhotonNetwork.LocalPlayer.ActorNumber != senderActorNumber)
        {
            chatHistory.Add(FullMessage);
            if (chatHistory.Count > 20)
            {
                chatHistory.RemoveAt(0);
            }
        }


        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["ChatHistory"] = chatHistory.ToArray();
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        
        Debug.Log(FullMessage);
        // Chỉ MasterClient gửi lại tin nhắn tới tất cả người chơi

    }
    public void SendMessage()
    {
        if (!ifAlive)
        {
            Debug.Log("bạn đã bị tắt chat");
            inputField.text = "";
            return;
        }
        string message = inputField.text;
        if (!string.IsNullOrEmpty(message))
        {

            photonView.RPC("ReceiveMessageFromClient", RpcTarget.All, message, PhotonNetwork.LocalPlayer.ActorNumber);

            string fullMessage = $"{JsonLogin.Instance.NameUser}({PhotonNetwork.LocalPlayer.ActorNumber}): {message}";
            PhotonNetwork.RaiseEvent(ChatEventCode, fullMessage, new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            }, SendOptions.SendReliable);
           
            inputField.text = "";
        }
    }
    public override void OnMasterClientSwitched(Player newMasterPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("ChatHistory", out object historyData))
            {
                string[] retored = (string[])historyData;
                chatHistory = new List<string>(retored);
                Debug.Log("Đã nhận lại lịch sử chat: " + chatHistory.Count + " dòng");
            }
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && chatHistory.Count > 0)
        {
            foreach (var msg in chatHistory)
            {
                PhotonNetwork.RaiseEvent(ChatEventCode, msg, new RaiseEventOptions { TargetActors = new int[] { newPlayer.ActorNumber } }, SendOptions.SendReliable);
            }
           
        }
    }
    public void OnEvent(EventData PhotonEvent)
    {
        if(PhotonEvent.Code == ChatEventCode)
        {
            string message = (string)PhotonEvent.CustomData;

            GameObject newMessage = Instantiate(MessagePrefab, ChatDisplay);
            newMessage.GetComponentInChildren<TMP_Text>().text = message;
        }
    }
    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    private void ClearChatDisplay()
    {
        foreach(Transform child in ChatDisplay)
        {
            Destroy(child.gameObject);
        }
    }
    public override void OnLeftRoom()
    {
        ClearChatDisplay();        // Xóa clone trong ChatDisplay
        chatHistory.Clear();       // Nếu là master thì xóa luôn bộ nhớ
    }
    #endregion
    #region GameMessage
    public void StartCountdown() // nút button startgame gọi đầu tiên
    {
        StartCoroutine(CountdownCoroutine());
    }
    IEnumerator CountdownCoroutine()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; // Khóa phòng, không cho người chơi mới tham gia
            PhotonNetwork.CurrentRoom.IsVisible = false; // Ẩn phòng khỏi danh sách phòng
            string[] countdownMessages =
            {
            "<color=blue><b>Trò chơi bắt đầu sau: </b></color>",
            "<color=red>5</color>",
            "<color=red>4</color>",
            "<color=red>3</color>",
            "<color=red>2</color>",
            "<color=red>1</color>",
            "<color=green><b>Bắt đầu!</b></color>"
            };

            foreach (var msg in countdownMessages)
            {
                PhotonNetwork.RaiseEvent(ChatEventCode, msg, new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.All
                }, SendOptions.SendReliable);
                yield return new WaitForSeconds(1f);
            }
            editGame.OnStartGame();
        }
    }
    #endregion
}
