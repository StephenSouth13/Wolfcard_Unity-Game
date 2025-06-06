using UnityEngine;
using UnityEngine.UI;
using UnityEngine.IO;
using System.IO;
using TMPro;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;



public class JsonLogin : MonoBehaviour
{
    public static JsonLogin Instance;
    public TMP_InputField TK;
    public TMP_InputField MK;

    public TMP_Text MessageError;
    public Change_panel PanelController;
    bool IsLoggedIn = false;

    public string NameUser;
    private string CurrenUser = "";
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void Login()
    {

        PlayerManager accounts = new PlayerManager();
        string Check = Application.dataPath + "/TaiKhoanNguoiChoi";
        if (!File.Exists(Check))
        {
            Debug.Log("Tệp không tồn tại!! " + Check);
            return;
        }else
        {

            string OldJson = File.ReadAllText(Check);
            PlayerListWrapper wrapper = JsonUtility.FromJson<PlayerListWrapper>(OldJson);
            if (wrapper != null && wrapper.allPlayers != null)
            {

                foreach (var ac in wrapper.allPlayers)
                {
                    if (ac.Account == TK.text && ac.Password == MK.text)
                    {
                        
                        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("IsOnline") && (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsOnline"])
                        {
                            
                            MessageError.gameObject.SetActive(true);
                            MessageError.text = "Tài Khoản đang Online trên thiết bị khác!!";
                            MessageError.color = Color.red;
                            return;
                        }

                        CurrenUser = ac.Account;


                        ac.AvatarPath = "Avatar/1";//sẽ thay đôi sau !!
                        ac.FramePath = "Frame/1";


                        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
                        playerProperties["IsOnline"] = true;
                        playerProperties["AvatarPath"] = ac.AvatarPath;
                        playerProperties["FramePath"] = ac.FramePath;
                        playerProperties["Id"] = ac.Id;
                        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
                        IsLoggedIn = true;
                        MessageError.gameObject.SetActive(false);
                        NameUser = ac.Name;
                        break;
                    }

                }
            }

            if (!IsLoggedIn)
            {
                MessageError.gameObject.SetActive(true);
                MessageError.text = "Sai tài khoản hoặc mật khẩu!!";
                MessageError.color = Color.red;
                return;
            }
        }
        if((IsLoggedIn && PanelController != null))
        {
            PhotonNetwork.NickName = NameUser;
            PanelController.LogInMain();
        }
    }
    public void ResetLoginState()
    {
        IsLoggedIn = false;
        TK.text = "";
        MK.text = "";
        MessageError.gameObject.SetActive(false);
    }
    public void LogOut()
    {
        if (!string.IsNullOrEmpty(CurrenUser))
        {
            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
            playerProperties["IsOnline"] = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties( playerProperties);
        }
        CurrenUser = "";
        Debug.Log("Đã Đăng xuất account");
    }
    public void OnApplicationQuit()
    {
        LogOut();
        
    }
}