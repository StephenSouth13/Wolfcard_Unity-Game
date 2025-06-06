using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(PhotonView))]

public class RoomManager : MonoBehaviourPunCallbacks
{

    public GameObject AvatarPrefab;

    public List<Transform> avatarSpawmPoints;

    private Dictionary<int, GameObject> playerAvatars = new Dictionary<int, GameObject>();
    private List<int> userId = new List<int>();
    private Dictionary<int, int> playerSpawnIndex = new Dictionary<int, int>();

    int FindEmptySlot()
    {
        for(int i = 0;i < avatarSpawmPoints.Count; i++)
        {
            if (!userId.Contains(i)) return i;
        }
        return -1;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int PlayerId = otherPlayer.ActorNumber;
        if (playerAvatars.TryGetValue(PlayerId, out var avatar))
        {
            Destroy(avatar);
            playerAvatars.Remove(PlayerId);
        }

        if (playerSpawnIndex.TryGetValue(PlayerId, out int idx))
        {
            userId.Remove(idx);
            playerSpawnIndex.Remove(PlayerId);
        }
    }
    public override void OnLeftRoom()
    {
        playerAvatars.Clear();
        userId.Clear(); // Xóa các slot đã dùng
        playerSpawnIndex.Clear(); // Xóa thông tin gán slot của từng người chơi
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("[OnJoinedRoom] Bắt đầu vào phòng");

        if (PhotonNetwork.IsMasterClient)
        {
            int idx = FindEmptySlot();
            Debug.Log($"[OnJoinedRoom] MasterClient. Slot trống: {idx}");

            if (idx >= 0)
            {
                Debug.Log($"[OnJoinedRoom] SpawnLocalAvatar bắt đầu");

                // Thêm kiểm tra null
                if (AvatarPrefab == null)
                    Debug.LogError("[OnJoinedRoom] AvatarPrefab đang null");

                if (avatarSpawmPoints == null || avatarSpawmPoints.Count <= idx || avatarSpawmPoints[idx] == null)
                    Debug.LogError("[OnJoinedRoom] Điểm spawn đang null");

                if (PhotonNetwork.LocalPlayer == null)
                    Debug.LogError("[OnJoinedRoom] LocalPlayer đang null");

                SpawnLocalAvatar(PhotonNetwork.LocalPlayer, idx);
                userId.Add(idx);
            }
        }
        else
        {
            if (photonView == null)
            {
                Debug.LogError("[OnJoinedRoom] photonView bị null trên client!");
            }
            else
            {
                Debug.Log("[OnJoinedRoom] Gửi RPC RequestSpawn tới Master");
                photonView.RPC(nameof(RPC_RequestSpawn), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }

    }

    [PunRPC]
    void RPC_RequestSpawn(int actorNumber)
    {
        Debug.Log($"[RPC_RequestSpawn] Gọi bởi actorNumber = {actorNumber}, isMaster: {PhotonNetwork.IsMasterClient}");
        if (playerSpawnIndex.ContainsKey(actorNumber))
        {
            Debug.Log($"[RPC_RequestSpawn] Player {actorNumber} đã spawn rồi.");
            return;
        }

        int idx = FindEmptySlot();
        if (idx >= 0)
        {
            Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            if (targetPlayer != null)
            {
                photonView.RPC(nameof(RPC_SpawnAvatar), targetPlayer, idx);
                userId.Add(idx);
                playerSpawnIndex[actorNumber] = idx;

                Debug.Log($"[RPC_RequestSpawn] Đã yêu cầu spawn cho player {actorNumber} tại slot {idx}.");
            }
            else
            {
                Debug.LogWarning($"[RPC_RequestSpawn] Không tìm thấy player với actorNumber = {actorNumber}");
            }
        }
        else
        {
            Debug.LogWarning("[RPC_RequestSpawn] Không còn slot trống để spawn.");
        }
    }


    [PunRPC]
    void RPC_SpawnAvatar(int idx)
    {
        SpawnLocalAvatar(PhotonNetwork.LocalPlayer, idx); // không dùng info.Sender
    }

    void SpawnLocalAvatar(Player p,  int idx)
    {
        var go = PhotonNetwork.Instantiate(
            AvatarPrefab.name,
            avatarSpawmPoints[idx].position,
            avatarSpawmPoints[idx].rotation,
            0, new object[] { idx }
        );

        if (go == null)
        {
            Debug.LogError("[SpawnLocalAvatar] PhotonNetwork.Instantiate trả về null!");
        }
        else
        {
            Debug.Log("[SpawnLocalAvatar] Instantiate thành công: " + go.name);
        }

        var avatarManager = go.GetComponent<AvatarManager>();
        if (avatarManager == null)
        {
            Debug.LogError("[SpawnLocalAvatar] Prefab không có component AvatarManager!");
            return;
        }
        playerAvatars[p.ActorNumber] = go;


    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (playerAvatars.ContainsKey(newMasterClient.ActorNumber))
        {
            Debug.Log($"{playerAvatars[newMasterClient.ActorNumber]} là chủ phòng mới");
        }
        else
        {
            Debug.Log("Chủ phòng mới chưa có avatar hoặc chưa spawn");
        }
    }


}
