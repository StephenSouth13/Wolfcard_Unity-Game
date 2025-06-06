using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PhotonRoom : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI RoomNumber;
    public Transform content;
    public GameObject roomPrefab;
    public GameObject PlayPanel;
    public GameObject MainPanel;
    public GameObject MatchPanel;

    private Dictionary<string, GameObject> roomPanels = new Dictionary<string, GameObject>();

    private int index = 1;
    private string roomNamePending;
    public void CreateRoom()
    {

        roomNamePending = $"Phòng: {index}";
        
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 15,
            IsVisible = true,
            IsOpen = true
        };
        PhotonNetwork.CreateRoom(roomNamePending, roomOptions);
        Debug.Log($"đang tạo {roomNamePending}");

    }
    public override void OnCreatedRoom()
    {
        Debug.Log($"Đã tạo {roomNamePending}");
        PlayPanel.SetActive( true );
        CreateRoomUI(roomNamePending);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"{roomNamePending} đã tồn tại");
        index++;
        CreateRoom();
    }
    void CreateRoomUI(string RoomName)
    {
        GameObject newRoomButton = Instantiate(roomPrefab, content);
        TMP_Text NameText = newRoomButton.transform.Find("NameRoom")?.GetComponent<TMP_Text>();
        TMP_Text CountText = newRoomButton.transform.Find("CountPlayers")?.GetComponent<TMP_Text>();
        if (NameText != null)
        {
            NameText.text = RoomName;
        }
        if(CountText != null)
        {
            CountText.text = "0/15";
        }
        newRoomButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => JoinRoom(RoomName));
        roomPanels.Add(RoomName, newRoomButton);
        if (roomPrefab == null)
        {
            Debug.LogError("🚨 `roomPrefab` chưa được gán! Hãy kiểm tra Inspector.");
            return;
        }
    }
    public void JoinRoom(string roomName)
    {
        
        PhotonNetwork.JoinRoom(roomName);
        PlayPanel.SetActive( true );
        Debug.Log($"Đã vào {roomName}");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        Debug.Log($"Đã rời khỏi phòng `{PhotonNetwork.CurrentRoom?.Name}`");

        PlayPanel.SetActive(false);
        MainPanel.SetActive(true);
        MatchPanel.SetActive(false);

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomNumber.text = $"Có {roomList.Count} phòng";
        Debug.Log($"[RoomListUpdate] Tổng số phòng: {roomList.Count}");

        // ======= GIAI ĐOẠN 1: XÓA UI PHÒNG KHÔNG CÒN TRÊN SERVER =======
        HashSet<string> currentRoomNames = new HashSet<string>();
        foreach (var room in roomList)
        {
            currentRoomNames.Add(room.Name);
        }

        List<string> roomsToRemove = new List<string>();
        foreach (var roomName in new List<string>(roomPanels.Keys))
        {
            if (!currentRoomNames.Contains(roomName))
            {
                Destroy(roomPanels[roomName]);
                roomsToRemove.Add(roomName);
                Debug.Log($"[RoomListUpdate] Đã xóa UI phòng: {roomName} (không còn trên server)");
            }
        }
        foreach (var roomName in roomsToRemove)
        {
            roomPanels.Remove(roomName);
        }

        // ======= GIAI ĐOẠN 2: CẬP NHẬT UI CÁC PHÒNG HIỆN CÓ =======
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                continue; // đã xử lý rồi ở trên
            }

            if (!roomPanels.ContainsKey(room.Name))
            {
                CreateRoomUI(room.Name);
            }

            if (roomPanels.TryGetValue(room.Name, out GameObject roomPanel))
            {
                TMP_Text countText = roomPanel.transform.Find("CountPlayers")?.GetComponent<TMP_Text>();
                if (countText != null)
                {
                    countText.text = $"{room.PlayerCount}/{room.MaxPlayers}";
                }
            }

            Debug.Log($"-> Cập nhật: {room.Name} ({room.PlayerCount}/{room.MaxPlayers})");
        }
    }


}

