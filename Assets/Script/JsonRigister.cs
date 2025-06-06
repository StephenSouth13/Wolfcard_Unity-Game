using System.Collections;
using System.Collections.Generic;

using System.IO;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


[System.Serializable]
public class PlayerListWrapper
{
    public List<Players> allPlayers;
}

public class JsonRigister : MonoBehaviour
{
    public TMP_InputField TK;
    public TMP_InputField MK;
    public TMP_InputField NLMK;
    public TMP_InputField Name;
    public TMP_Text messageError;
    public TMP_Text messageError2;

    public void Register()
    {
        Players data = new Players();
        data.Account = TK.text;
        data.Password = MK.text;

        // Kiểm tra thông tin nhập vào
        if (string.IsNullOrEmpty(TK.text) || string.IsNullOrEmpty(MK.text) || string.IsNullOrEmpty(NLMK.text))
        {
            if (string.IsNullOrEmpty(Name.text))
            {
                ShowError2();
            }
            ShowError1();
            return;
        }
        if (NLMK.text != MK.text)
        {
            ShowError();
            return;
        }
        if (string.IsNullOrEmpty(Name.text))
        {
            messageError.gameObject.SetActive(false);
            ShowError2();
            return;
        }
        data.Name = Name.text;
        PhotonNetwork.NickName = Name.text;

        // Xử lý local: Lưu file JSON
        string Check = Application.dataPath + "/TaiKhoanNguoiChoi";
        PlayerListWrapper wrapper = new PlayerListWrapper();
        wrapper.allPlayers = new List<Players>();
        if (File.Exists(Check))
        {
            string oldJson = File.ReadAllText(Check);
            Debug.Log("String json :" + oldJson);
            wrapper = JsonUtility.FromJson<PlayerListWrapper>(oldJson);
            if (wrapper != null && wrapper.allPlayers != null)
            {
                foreach (var acc in wrapper.allPlayers)
                {
                    if (acc.Account == data.Account)
                    {
                        messageError.gameObject.SetActive(true);
                        messageError.text = "Tài khoản đã tồn tại!!!";
                        messageError.color = Color.red;
                        return;
                    }
                    else
                    {
                        messageError.gameObject.SetActive(false);
                    }
                    if (acc.Name == data.Name)
                    {
                        messageError2.gameObject.SetActive(true);
                        messageError2.text = "Tên account đã tồn tại!!!";
                        messageError2.color = Color.red;
                        return;
                    }
                    else
                    {
                        messageError2.gameObject.SetActive(false);
                    }
                }
            }
        }
        int newId = wrapper.allPlayers.Count > 0 ? wrapper.allPlayers[^1].Id + 1 : 1;
        data.Id = newId;
        data.AvatarPath = "Avatar/1";
        data.FramePath = "Frame/1";
        wrapper.allPlayers.Add(data);
        string newJson = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(Check, newJson);
        ShowSuccess();
    }

    public void ShowError()
    {
        messageError.gameObject.SetActive(true);
        messageError2.gameObject.SetActive(false);
        messageError.text = "Mật khẩu không trùng khớp, vui lòng nhập lại!";
        messageError.color = Color.red;
    }
    public void ShowError1()
    {
        messageError.gameObject.SetActive(true);
        messageError.text = "Hãy nhập đầy đủ thông tin tài khoản!";
        messageError.color = Color.red;
    }
    public void ShowError2()
    {
        messageError2.gameObject.SetActive(true);
        messageError2.text = "Bạn chưa đặt tên ingame!";
        messageError2.color = Color.red;
    }
    public void ShowSuccess()
    {
        messageError.gameObject.SetActive(false);
        messageError2.gameObject.SetActive(true);
        messageError2.text = "Chúc mừng, tài khoản đã được tạo thành công!";
        messageError2.color = Color.blue;
    }

}
