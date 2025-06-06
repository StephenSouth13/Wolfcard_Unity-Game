using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;



public class NetWorkingManager1 : MonoBehaviourPunCallbacks
{

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public void ConnectToPhoton()
    {
        Debug.Log("Đang Kết nối với Photon master server ... ");

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Cloud!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon Cloud: " + cause);
    }
    void OnApplicationQuit()
    {
        if (PhotonNetwork.InLobby)
        {
            Debug.Log("Đang rời khỏi Lobby...");
            PhotonNetwork.LeaveLobby();
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            Debug.Log("Đã ngắt kết nối khỏi Photon.");
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("Ứng dụng bị tạm dừng,không Ngắt kết nối khỏi Photon!!");
        }
    }
    public override void OnLeftLobby()
    {
        Debug.Log("Đã thoát khỏi Lobby!!");
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Đã vào lobby!!");
    }

}
