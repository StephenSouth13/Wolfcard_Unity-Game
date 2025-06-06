using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class Change_panel : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel_login;
    public GameObject panel_Register;
    public GameObject Main;
    public GameObject Menu;
    public GameObject Rules;
    public GameObject CardVillager_panel;
    public GameObject CardWolf_panel;
    public GameObject CardSolo_panel;
    public GameObject GroupChooseTraitorPanel;
    public GameObject MatchPanel;


    public void LoginSwitch()
    {
        panel1.SetActive(false);
        panel_login.SetActive(true);
    }
    public void LogOutSwith()
    {
        panel_login.SetActive(false);
        panel1.SetActive(true);
    }
    public void LogInRegisterSwitch()
    {
        panel1.SetActive(false);
        panel_Register.SetActive(true);
    }
    public void LogOutRegisterSwitch()
    {
        panel_Register.SetActive(false);
        panel1.SetActive(true);
    }
    public void LogInMain()
    {
        panel_login.SetActive(false);
        Main.SetActive(true);
    }
    public void LogOutMain()
    {
        Menu.SetActive(false);
        Main.SetActive(false);
        panel_login.SetActive(true);
        JsonLogin Back = FindFirstObjectByType<JsonLogin>();
        if (Back != null)
        {
            Back.ResetLoginState();
            Back.LogOut();
        }
    }
    public void LogInMenu()
    {
        Menu.SetActive(true);
    }
    public void LogOutMenu()
    {
        Menu.SetActive(false);
    }
    public void OpenRules()
    {
        Rules.SetActive(true);
    }
    public void CloseRules()
    {
        Rules.SetActive(false);
    }
    public void ViewVillagerCard()
    {
        CardVillager_panel.SetActive(true);
        CardWolf_panel.SetActive(false);
    }
    public void ViewWolfCard()
    {
        ScrollHandler reset = FindFirstObjectByType<ScrollHandler>();
        CardVillager_panel.SetActive(false);
        CardWolf_panel.SetActive(true);
        CardSolo_panel.SetActive(false);
        if (reset != null)
        {
            reset.ResetPosition();
        }
    }
    public void ViewSoloCard()
    {
        ScrollHandler reset = FindFirstObjectByType<ScrollHandler>();
        CardWolf_panel.SetActive(false);
        CardSolo_panel.SetActive(true);
        if (reset != null)
        {
            reset.ResetPosition();
        }
    }
    public void OffViewCard()
    {
        ScrollHandler reset = FindFirstObjectByType<ScrollHandler>();
        CardVillager_panel.SetActive(false);
        CardWolf_panel.SetActive(false);
        CardSolo_panel.SetActive(false);
        if(reset != null)
        {
            reset.ResetPosition();
        }
    }
    public void OpenChooseTraitorPanel()
    {
       GroupChooseTraitorPanel.SetActive(true);
    }
    public void OpenMatchPanelANDJoinLobby()
    {
        Main.SetActive(false);
        MatchPanel.SetActive(true);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            Debug.Log("Chưa vào Phonton server!!");
        }
    }
    public void ExitMatchPanelANDLogoutLobby()
    {
        Main.SetActive(true);
        MatchPanel.SetActive(false);
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            Debug.Log("Đã Rời Lobby!!");
        }
        else
        {
            Debug.Log("Chưa gia nhập lobby !!");
        }
    }
}
