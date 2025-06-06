using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonStatus : MonoBehaviour
{
    private string photonStatus;
    public TextMeshProUGUI textStatus;

    // Update is called once per frame
    void Update()
    {
        this.photonStatus = PhotonNetwork.NetworkClientState.ToString();
        this.textStatus.text = this.photonStatus;
    }
}
