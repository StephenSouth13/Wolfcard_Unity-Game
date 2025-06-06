using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GamePlayManager : MonoBehaviourPunCallbacks
{
    private AvatarManager avtManager; // lấy các trường dữ liệu của script
    private PhotonChat ptChat; // lấy các trường dữ liệu của script


    #region SetUp Characters
    private Dictionary<Sprite, string> spriteCharacter = new Dictionary<Sprite, string>();
    private Dictionary<string, Action> actionCharacter = new Dictionary<string, Action>();
    private List<string> normarlName = new List<string> { "Dân Làng"};

    private List<string> nameAllVilagers = new List<string>
    {
        "Phù Thủy", "Trưởng Làng", "Cupid", "Thợ Săn", "Đạo Tặc","Kẻ Thế Thân", "Người Bệnh","Tiên Tri", "Bảo Vệ","kẻ Phản Bội(Dân)"
    };
    private List<string> nameWolf = new List<string>
    {
        "Sói Săn"
    };
    private List<string> nameAllWolf = new List<string>
    {
        "Sói Đầu Đàn", "Sói Bệnh", "Sói con"
    };
    private List<string> nameAllSolo = new List<string>
    {
        "Người Thổi Sáo", "Kẻ Chán Đời", "Sói Xám", "Gương Nhân Bản"
    };
    void SetUpNameToSprite(Sprite[] sprite, List<string> name)
    {
        
        for(int i = 0; i < sprite.Length; i++)
        {
            spriteCharacter[sprite[i]] = name[i];
        }
    }
    void SetUpAllActionToAllName()
    {
        actionCharacter["Dân Làng"] = villagerFT;
        actionCharacter["Tiên Tri"] = ProphetFT;
        actionCharacter["Bảo Vệ"] = GuardianFT;
        actionCharacter["Sói Săn"] = WolfFT;
    }
    private void Awake()
    {
        avtManager = FindFirstObjectByType<AvatarManager>();
        ptChat = FindFirstObjectByType<PhotonChat>();
        SetUpAllActionToAllName();

        SetUpNameToSprite(NormalVilager, normarlName);
        SetUpNameToSprite(Mode3Villagers, nameAllVilagers);
        SetUpNameToSprite(Wolf, nameWolf);
        SetUpNameToSprite(Mode2Wolf, nameAllWolf);
        SetUpNameToSprite(Mode2Solo, nameAllSolo);
    }
    public void ExecuteAction(string CharacterName)
    {
        if (actionCharacter.TryGetValue(CharacterName, out Action action))
        {
            action.Invoke();
        }
        else
        {
            Debug.Log($"Không tìm thấy hành động cho nhân vật : {CharacterName}");
        }
    }
    public string GetName(Sprite sprite)
    {
        return spriteCharacter.TryGetValue(sprite, out string characterName) ? characterName : "Không xác định";
    }

    #endregion


    public Button StartGame;
    public Button Card;

    // list ban đầu cho 5 người chơi
    public Sprite[] NormalVilager; // chỉ 1 dân thường (tính chất lặp lại)        --- (gán tên)
    public Sprite[] Villagers; // 1 bảo vệ, 1 tiên tri 
    public Sprite[] Mode2Villagers; // (mode 8 người trở lên)full dân trừ kẻ phản bội và dân thường (tính chất độc nhất)
                                    
    public Sprite[] Mode3Villagers; // Thêm kẻ phản bội => full dân trừ dân thường--- (gán tên)
                                    // (gán sprite thứ tự theo tên của list) 

    public Sprite[] Wolf; // 1 sói săn (tính chất lặp lại)                        --- (gán tên)
    public Sprite[] Mode2Wolf; // (mode 8 người trở lên) add full sói trừ sói săn --- (gán tên)
                               // (gán sprite thứ tự theo tên của list) 

    // list khi 6 người chơi: 0 add;
    // list khi 7 người chơi: 
    // list Khi 8 người chơi: Add full sói dân trừ kẻ phản bội(dân) và gương(solo)
    public Sprite[] Solo; // 1 sói xám, 1 Kẻ chán đời, 1 Người thổi sáo 
    public Sprite[] Mode2Solo; // thêm gương => full solo (sau 10 người chơi)     --- (gán tên)
                               // (gán sprite thứ tự theo tên của list) 

    private List<Action> gameModes = new List<Action>();
    // Sau 10 người Add kẻ phản bội(dân), gương

    // CHẾ ĐỘ: 1: nhiều dân, nhiều solo
    // dân 90(4), 70(5,5,5,6,7), 60(7,8,9,8,9)
    // sói 10(1), 20(1,2,2,2,2), 30(3,3,3,4,4)
    // solo       10(0,0,1,1,1), 10(1,1,1,2,2)

    // CHẾ ĐỘ 2: Nhiều sói, ít solo;
    // dân 90(4), 70(5,5,6,7,7), 60(8,8,9,9,10)
    // sói 10(1), 20(1,2,2,2,3), 30(3,4,4,4,4)
    // solo        0(0,0,0,0,0), 10(0,0,0,1,1)

    // CHẾ ĐỘ 3: sói nhiều,tăng nhẹ solo, dân nhiều vai mạnh giảm dân thường(hoặc bằng hết)
    // dân 90(4), 70(4,4,5,6,6), 60(7,7,7,8,9)
    // sói 10(1), 20(2,2,2,2,3), 30(3,4,4,4,4)
    // solo        0(0,1,1,1,1), 10(1,1,2,2,2)

    /* NOTE****
        - cần khóa phòng khi bắt đầu -- (ok xong)
        - CountDown trước khi khóa (tránh việc vừa khóa lại có người vào trước 1s) -- (ok xong)
        - tính lặp lại của dân thường và sói thường , tính độc nhất các thẻ đặt biệt; -- (OK xong)
        - tạo class Nhân vật lưu (tên , sprite() , Action()) (không cần thiết) (cần khi có firebase)
        - Cần countDown để chuyển bối cảnh chơi (tối , sáng , vote) 
        - cần chế độ vote trên Prefab (?) 
        - khi nhấn button từ avatarManager.cs thì sẽ kiểm tra có phải chính mình hay không và  gọi các acTion bên GamePlayManager(tất cả)
            + hiện tại action đang dc gọi ngay khi gán Sprite, Giờ cần set điều kiện để thực hiện trong thời gian quy định
        - sẽ dùng case when để thêm điều kiện để kiểm soát thời gian thực hiện action
        - bên trong CountDown sẽ thiết lập 3 trường dữ liệu và isPlaying làm kiểm soát vòng lặp While(true) -- (ok xong)

        - test lại onclick bằng lệnh đơn giản với avatar -- (ok xong đã thay đổi 1 chút là để object thêm component buton còn button thì xóa component thành markImage)
        - thực hiện vote người chơi khi có Phase "vote" sẽ đổi numberOfAction = 0; -- (ok xong)
        - numberOfActioc sẽ được gán vào khi bắt đầu chia bài  -- (ok xong)
            được để bên trong code Action() của từng thẻ bài khi gọi invoke sẽ tự dộng gán-- (ok xong)
        - cần có bộ tính = list để đếm xem có bao nhiêu sói bao nhiêu dân và bao nhiêu solo để tiến hành kết thúc ván game
        - Wolf cần có chức năng cắn vào phase "nighht" và 2 khung chat ngày là đêm riêng biệt
            + cần có thông báo ai bị giết vào "Day" tiếp theo;
            + cơ chế xử lý chặn người chơi == ifAlive(); 
            + bên PhotonChat sẽ có bool ifAlive(true) để kiểm soát việc chat nếu người 
                chơi bị loại điều này sẽ thực hiện trong action của GamePlayManager == false
        - Button của avatar người chơi sẽ thực hiện các chức năng : 
            + chức năng nhận diện người chơi onClick trên avatar -- (ok xong)
            + xem thông tin người chơi khi chưa bắt đầu game <để xem xét có thời gian thì thêm vào>
            + vote người chơi -- (ok xong)
            + chọn người giết cứu và bảo vệ các chức năng nhờ listener(ActionOnclickAvatar);

        - các giá trị cần reset khi game kết thúc :
            + MasterVoteCount, votecount , BackupNumberAction, OutPlayer
    */
    
    #region CountDownTime
    public Image DayImage;
    public Image NightImage;
    public TextMeshProUGUI CountDownTimer;
    public Button StopGame; // Default trường hợp vòng lặp vô tận
    private int CountPlayers;

    private bool isPlaying = true;
    private Coroutine phaseCoroutine;

    private List<string> villagerCount = new List<string>();
    private List<string> wolfCount = new List<string>();
    private List<string> soloCount = new List<string>();

    private string gamePhase = "Night";

    public Image day;
    public Image vote;
    public Image night;
    public float dayTime = 30;
    public float voteTime = 15;
    public float nightTime = 20;
    private float timeRemaining;

    public IEnumerator PhaseLoop()
    {
        while (isPlaying)
        {
            setPhase("Night", nightTime, Color.blue);
            Debug.Log($"NumberOfAction Đã đổi sang : {NumberOfAction}");
            yield return StartCoroutine(Countdown());
            if (!isPlaying) yield break;

            setPhase("Day", dayTime, Color.green);
            Debug.Log($"NumberOfAction Đã đổi sang : {NumberOfAction}");
            yield return StartCoroutine(Countdown());
            if(!isPlaying) yield break;

            setPhase("Vote", voteTime, Color.red);
            Debug.Log($"NumberOfAction Đã đổi sang : {NumberOfAction}");
            yield return StartCoroutine(Countdown());
            foreach (var votes in MasterVoteCount)
            {
                Debug.Log($"Voter : {votes.Key} => votefor{votes.Value}");
            }
            EndVoting();
            PointedOff();

        }
    }
    private IEnumerator Countdown()
    {
        CountDownTimer.gameObject.SetActive(true);
        while(timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            CountDownTimer.text = $"{Mathf.Ceil(timeRemaining)}s";
            yield return null;  
        }
    }
    private void setPhase(string phase, float duration, Color color)
    {
        gamePhase = phase;
        timeRemaining = duration;
        CountDownTimer.color = color;

        switch (gamePhase)
        {
            case "Day":
                DayImage.gameObject.SetActive(true);
                NightImage.gameObject.SetActive(false);
                day.gameObject.SetActive(true);
                night.gameObject.SetActive(false);
                vote.gameObject.SetActive(false);
                break;
            case "Night":
                if (NumberOfAction != 0)
                {
                    BackupNumberAction = NumberOfAction;
                }
                DayImage.gameObject.SetActive(false);
                NightImage.gameObject.SetActive(true);
                if (BackupNumberAction != 0 && NumberOfAction == 0)
                {
                    NumberOfAction = BackupNumberAction;
                    
                }
                
                day.gameObject.SetActive(false);
                night.gameObject.SetActive(true);
                vote.gameObject.SetActive(false);
                break;
            case "Vote":
                NumberOfAction = 0;
                day.gameObject.SetActive(false);
                night.gameObject.SetActive(false);
                vote.gameObject.SetActive(true);
                break;
            default:
                day.gameObject.SetActive(false);
                night.gameObject.SetActive(false);
                vote.gameObject.SetActive(false);
                CountDownTimer.gameObject.SetActive(false );
                break;
        }
        Debug.Log($"Bắt đầu pha: {phase}");
    }
    public void stopGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Đã dừng vòng lặp cho tất cả người chơi ");

            photonView.RPC("StopGameForAll", RpcTarget.All);

        }
    }
    [PunRPC]
    private void StartCountDownForAll()
    {
        isPlaying = true;
        Debug.Log("Game bắt đầu đếm ngược");
        phaseCoroutine = StartCoroutine(PhaseLoop());
    }
    [PunRPC]
    private void StopGameForAll()
    {
        
        isPlaying = false;
        timeRemaining = 0;
        if (phaseCoroutine != null) // Kiểm tra nếu Coroutine tồn tại
        {
            StopCoroutine(phaseCoroutine);
            Debug.Log("Đã dừng vòng lặp PhaseLoop!");
            phaseCoroutine = null; // Reset giá trị
        }

    }
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CountPlayers = PhotonNetwork.PlayerList.Length;
        OnOffButtonStart();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CountPlayers = PhotonNetwork.PlayerList.Length;
        Debug.Log($"Người chơi :{newPlayer.NickName} đã vào phòng.");
        OnOffButtonStart();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CountPlayers = PhotonNetwork.PlayerList.Length;
        Debug.Log($"Người chơi :{otherPlayer.NickName} đã rời phòng, số người hiện tại : {CountPlayers}.");
        OnOffButtonStart();
    }
    private void OnOffButtonStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartGame.gameObject.SetActive(CountPlayers >= 5);
        }
    }
    //public void CheckPlayersCount(int CPlayers)
    //{
    //    if (CPlayers == 5)
    //    {

    //    }
    //}
    public void OnStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int randomMode = UnityEngine.Random.Range(0, 2);
            switch(randomMode)
            {
                case 0:
                    Mode1();
                    break;
                case 1:
                    Mode2();
                    break;
                case 2:
                    Mode3();
                    break;
                default:
                    Debug.Log("Lỗi mode chơi");
                    break;
            }
            photonView.RPC("StartCountDownForAll", RpcTarget.All);

        }
    }
    public void Mode1()
    {
        List<Sprite> roles = new List<Sprite>();
        HashSet<Sprite> SelectedCharacter = new HashSet<Sprite>();
        if (CountPlayers < 5)
        {
            Debug.Log("[OnStartGame] Lỗi thiếu người chơi !!! Lỗi hiển thị nút Start");
            return;
        }
        else
        {
            switch (CountPlayers)
            {
                case 5:
                    for (int i = CountPlayers; i > CountPlayers - 2; i--)
                    {
                        roles.Add(GetWeightedRandomSprite(NormalVilager, new int[] { 1 }));
                        
                    }
                    for (int i = CountPlayers; i > CountPlayers - 2; i--)
                    {
                        Sprite chosenSprite;
                        do
                        {
                            chosenSprite = GetWeightedRandomSprite(Villagers, new int[] { 1, 1 });
                        }while( SelectedCharacter.Contains(chosenSprite) );
                        SelectedCharacter.Add(chosenSprite);
                        roles.Add(chosenSprite);
                        
                    }
                    roles.Add(GetWeightedRandomSprite(Wolf, new int[] { 1 }));
                    

                    Shuffle(roles);
                    photonView.RPC("AssignRoles", RpcTarget.All, roles.ConvertAll(sprite => sprite.name).ToArray());

                    break;
                default:
                    if (CountPlayers > 15)
                    {
                        Debug.Log("[OnStartGame] Lỗi số đém người chơi !!! ");
                    }
                    break;
            }
        }
    }
    public void Mode2()
    {
        List<Sprite> roles = new List<Sprite>();
        HashSet<Sprite> SelectedCharacter = new HashSet<Sprite>();
        if (CountPlayers < 5)
        {
            Debug.Log("[OnStartGame] Lỗi thiếu người chơi !!! Lỗi hiển thị nút Start");
            return;
        }
        else
        {
            switch (CountPlayers)
            {
                case 5:
                    for (int i = CountPlayers; i > CountPlayers - 2; i--)
                    {
                        roles.Add(GetWeightedRandomSprite(NormalVilager, new int[] { 1 }));
                    }
                    for (int i = CountPlayers; i > CountPlayers - 2; i--)
                    {
                        Sprite chosenSprite;
                        do
                        {
                            chosenSprite = GetWeightedRandomSprite(Villagers, new int[] { 1, 1 });
                        } while (SelectedCharacter.Contains(chosenSprite));
                        SelectedCharacter.Add(chosenSprite);
                        roles.Add(chosenSprite);
                    }
                    roles.Add(GetWeightedRandomSprite(Wolf, new int[] { 1 }));

                    Shuffle(roles);
                    photonView.RPC("AssignRoles", RpcTarget.All, roles.ConvertAll(sprite => sprite.name).ToArray());

                    break;
                default:
                    if (CountPlayers > 15)
                    {
                        Debug.Log("[OnStartGame] Lỗi số đém người chơi !!! ");
                    }
                    break;
            }
        }
    }
    public void Mode3()
    {
        List<Sprite> roles = new List<Sprite>();
        HashSet<Sprite> SelectedCharacter = new HashSet<Sprite>();
        if (CountPlayers < 5)
        {
            Debug.Log("[OnStartGame] Lỗi thiếu người chơi !!! Lỗi hiển thị nút Start");
            return;
        }
        else
        {
            switch (CountPlayers)
            {
                case 5:
                    for (int i = CountPlayers; i > CountPlayers - 2; i--)
                    {
                        roles.Add(GetWeightedRandomSprite(NormalVilager, new int[] { 1 }));
                    }
                    for (int i = CountPlayers; i > CountPlayers - 2; i--)
                    {
                        Sprite chosenSprite;
                        do
                        {
                            chosenSprite = GetWeightedRandomSprite(Villagers, new int[] { 1, 1 });
                        } while (SelectedCharacter.Contains(chosenSprite));
                        SelectedCharacter.Add(chosenSprite);
                        roles.Add(chosenSprite);
                    }
                    roles.Add(GetWeightedRandomSprite(Wolf, new int[] { 1 }));

                    Shuffle(roles);
                    photonView.RPC("AssignRoles", RpcTarget.All, roles.ConvertAll(sprite => sprite.name).ToArray());

                    break;
                default:
                    if (CountPlayers > 15)
                    {
                        Debug.Log("[OnStartGame] Lỗi số đém người chơi !!! ");
                    }
                    break;
            }
        }
    }
    #region Actions
    private List<int> VoteCount = new List<int>();
    private Dictionary<int, int> MasterVoteCount = new Dictionary<int, int>();

    int currentvote = -1;
    private int NumberOfAction;
    private int BackupNumberAction; // lưu tạm giá trị NumberOfAction khi hết time thì gán lại cho numberOfAction
                                    // set các logic hành động cho Button avatar
    private Dictionary<int, int> lastPointedAvt = new Dictionary<int, int>();
    private Dictionary<int, int> voteMap = new Dictionary<int, int>(); // dùng để lưu nhưng gì ShowBoardTextVote làm, dùng lại để xóa ở các client khác
    private int OutPlayer = 0;

    public void OnActionButtonClicked(int victimActorNumber) // avtar sẽ gọi hàm này đầu tiên khi nhấn nút
    {
        int attacker = PhotonNetwork.LocalPlayer.ActorNumber;

        // Gửi RPC tới avatar bị nhấn, cần tìm avatar tương ứng
        AvatarManager victim = FindAvatarByActorNumber(victimActorNumber);
        if (victim != null)
        {
            victim.photonView.RPC("OnAvatarClicked", RpcTarget.All, attacker, victimActorNumber);
            ActionOnclickAvatar(victimActorNumber);
        }
    }
    public AvatarManager FindAvatarByActorNumber(int actorNumber)
    {
        foreach (AvatarManager avm in FindObjectsByType<AvatarManager>(FindObjectsSortMode.None))
        {
            if (avm.photonView.Owner.ActorNumber == actorNumber)
                return avm;
        }
        return null;
    }
    public void PointedPublic(int actorNumber)
    {
        photonView.RPC("ShowVotePointed", RpcTarget.All,  actorNumber);
        int localActor = PhotonNetwork.LocalPlayer.ActorNumber;
        photonView.RPC("ShowBoardTextVote", RpcTarget.All, localActor, actorNumber);
    }
    [PunRPC]
    void ShowVotePointed(int actorNumber)
    {
        int localPlayer = PhotonNetwork.LocalPlayer.ActorNumber;
        if (lastPointedAvt.ContainsKey(localPlayer))
        {
            int oldActor = lastPointedAvt[localPlayer];
            AvatarManager oldTarget = FindAvatarByActorNumber(oldActor);
            if (oldTarget != null)
            {
                oldTarget.pointed.gameObject.SetActive(false);
            }
        }

        AvatarManager newTarget = FindAvatarByActorNumber(actorNumber);
        if (newTarget != null)
        {
            newTarget.pointed.gameObject.SetActive(true);
            lastPointedAvt[localPlayer] = actorNumber;
        }
    }
    [PunRPC]
    void ShowBoardTextVote(int localActor, int actorNumber)
    {
        AvatarManager selfAvt = FindAvatarByActorNumber(localActor);
        if (selfAvt != null)
        {
            selfAvt.voteText.gameObject.SetActive(true);
            selfAvt.voteText.text = actorNumber.ToString(); // Ai mình vote
            selfAvt.board.gameObject.SetActive(true);
        }
        voteMap[localActor] = actorNumber;
    }
    [PunRPC]
    void TurnOffBoardTextVote()
    {
        foreach(var pair in voteMap)
        {
            int actor = pair.Key;
            AvatarManager selfAvt = FindAvatarByActorNumber(actor);
            if (selfAvt != null)
            {
                selfAvt.voteText.gameObject.SetActive(false);
                selfAvt.voteText.text = ""; // Ai mình vote
                selfAvt.board.gameObject.SetActive(false);
            }
        }
        
        voteMap.Clear();
    }
    public void PointedOff()
    {
        photonView.RPC("TurnOffPointed", RpcTarget.All);
        photonView.RPC("TurnOffBoardTextVote", RpcTarget.All);
    }
    [PunRPC]
    void TurnOffPointed()
    {
        foreach(var pair in lastPointedAvt)
        {

            AvatarManager target = FindAvatarByActorNumber(pair.Value);
            if (target != null)
            {
                target.pointed.gameObject.SetActive(false);
            }
            AvatarManager selfAvt = FindAvatarByActorNumber(pair.Key);
            if (selfAvt != null)
            {
                selfAvt.voteText.gameObject.SetActive(false);
                selfAvt.voteText.text = "";
                selfAvt.board.gameObject.SetActive(false);
            }
        }
        lastPointedAvt.Clear();
    }

    public void ActionOnclickAvatar(int victimActor)
    {
        Debug.Log($"[ActionOnclickAvatar] Đã được gọi, victimActor: {victimActor}");
        Debug.Log($"[ActionOnclickAvatar] Local: {PhotonNetwork.LocalPlayer.ActorNumber}, Owner: {photonView.Owner.ActorNumber}");
        

        switch (NumberOfAction)
        {
            case 0 when gamePhase == "Vote": // vote sẽ gán trực tiếp numberAction cho tất cả người chơi khi đến thời gian
                PointedPublic(victimActor);
                Vote(victimActor);
                Debug.Log("[ActionOnclickAvatar] Đã vào case 0");
                break;
            // các case khác sẽ là giá trị được gán ngay khi nhận sprite(thẻ bài)
            case 1: // Dân làng 
                Debug.Log("[ActionOnclickAvatar] Đã vào case 1");
                break;
            case 2 when gamePhase == "Night": // Tiên tri
                Debug.Log("[ActionOnclickAvatar] Đã vào case 2");
                break;
            case 3 when gamePhase == "Night": // Bảo vệ
                Debug.Log("[ActionOnclickAvatar] Đã vào case 3");
                break;
            case 4 when gamePhase == "Night": // Sói
                Debug.Log("[ActionOnclickAvatar] Đã vào case 4");
                break;
            default:
                Debug.Log("Không có action này !!!");
                break;
        }

    }
    // Set time cho các action bên dưới 
    public void villagerFT()
    {
        NumberOfAction = 1;
        Debug.Log("Dân làng vô dụng");
    }
    public void ProphetFT()
    {
        NumberOfAction = 2;
        Debug.Log("Hãy tiên đoán 1 người chơi");
    }
    public void GuardianFT()
    {
        NumberOfAction = 3;
        Debug.Log("Bảo vệ 1 người chơi");
    }
    public void WolfFT()
    {
        NumberOfAction = 4;
        Debug.Log("Thức dậy cắn người!!");
    }
    #region vote

    public void Vote(int newTargetPlayerActorNumber)
    {
        if( currentvote == newTargetPlayerActorNumber)
        {
            VoteCount.Remove(newTargetPlayerActorNumber);
            currentvote = -1;


            Debug.Log($"Đã hủy vote cho :{newTargetPlayerActorNumber}");
        }else
        {
            if(currentvote != -1) // nếu đã vote ai khác trước đó xóa đi khỏi danh sách
            {
                VoteCount.Remove(currentvote);
                
            }
            VoteCount.Add(newTargetPlayerActorNumber);
            currentvote = newTargetPlayerActorNumber;
            
            Debug.Log($"Đã chuyển vote sang cho: {newTargetPlayerActorNumber}");
            // gửi vote lên master client
            photonView.RPC("SubmitVoteToMaster", RpcTarget.MasterClient, currentvote, PhotonNetwork.LocalPlayer.ActorNumber);
            Debug.Log($"Người chơi {newTargetPlayerActorNumber} đã vote cho {currentvote}");
        }
    }
    [PunRPC]
    void SubmitVoteToMaster(int TargetPlayerActorNumber, int voterActorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) { return; } // chỉ master xử lý
        MasterVoteCount[voterActorNumber] = TargetPlayerActorNumber;
        

    }
    public int GetMostVote(Dictionary<int,int> votes)
    {
        var voteResult = votes.Values
                              .GroupBy(v => v)
                              .OrderByDescending(g => g.Count())
                              .FirstOrDefault();
        return voteResult?.Key ?? -1;
        
    }
    void EndVoting()
    {
        if(!PhotonNetwork.IsMasterClient) { return; } // chỉ master xử lý

        int mostVotePlayer = GetMostVote(MasterVoteCount);
        int VotesForPlayer = MasterVoteCount.Values.Count(v => v == mostVotePlayer);
        int totalPlayer = PhotonNetwork.PlayerList.Length - OutPlayer;

        Debug.Log($"[EndVoting] mostVotePlayer : {mostVotePlayer}");
        if (mostVotePlayer == -1)
        {
            Debug.Log("Không có phiếu bầu !!");
            return;
        }
        else 
        {
            Debug.Log($"[EndVoting] VotesForPlayer: {VotesForPlayer}, totalPlayer: {totalPlayer}");
            if ((float)VotesForPlayer / totalPlayer >= 0.6f)
            {
                photonView.RPC("VotingResult", RpcTarget.All, mostVotePlayer);
            }
            else
            {
                Debug.Log($"Tổng số phiếu không đủ 60% người chơi");
            }
        }
    }
    [PunRPC]
    public void VotingResult(int mostVotedPlayer)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == mostVotedPlayer)
        {
            ptChat.ifAlive = false;
            OutPlayer += 1;
            Debug.Log($"Đã tắt chat của người chơi: {mostVotedPlayer}");
        }
    }
    #endregion
    [PunRPC]
    public void AliveAll() // phát lại ifAlive cho tất cả người chơi Khi kết thúc game
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ptChat.ifAlive = true;
            Debug.Log("Đã phát lại Alive == true");
        }
    }
    [PunRPC]
    public void SetAliveFalse()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ptChat.ifAlive = false;
            Debug.Log("Người chơi bị loại tạm thời cấm chat, voice, Action()");
        }
    }
    #endregion
    #region Random RPC
    [PunRPC]
    void AssignRoles(string[] assignedRoles)
    {
        int myIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        if(myIndex < assignedRoles.Length)
        {
            Sprite assignedSprite = System.Array.Find(NormalVilager, s => s.name == assignedRoles[myIndex]) ??
                                    System.Array.Find(Villagers, s => s.name == assignedRoles[myIndex]) ??

                                    System.Array.Find(Wolf, s => s.name == assignedRoles[myIndex]) ??
                                    System.Array.Find(Solo, s => s.name == assignedRoles[myIndex]);
            this.Card.image.sprite = assignedSprite;
            string name = GetName(assignedSprite);
            Debug.Log($"Người chơi {PhotonNetwork.LocalPlayer.NickName} nhận nhân vật: {name}");
            ExecuteAction(name);
          
        }
    }
    void Shuffle(List<Sprite> list)
    {
        for(int i = list.Count - 1; i > 0; i--)
        {
            int randIndex = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[randIndex]) = (list[randIndex], list[i]);
        }
    }
    Sprite GetWeightedRandomSprite(Sprite[] options, int[] weight)
    {
        int totalWeight = 0;
        foreach(int w in weight) totalWeight+= w;

        int randomValue = UnityEngine.Random.Range(1, totalWeight + 1);

        for(int i = 0; i < options.Length; i++)
        {
            if(randomValue <= weight[i])
                return options[i];
            randomValue -= weight[i];
        }
        return options[options.Length - 1];
    }
    #endregion
}
