using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [HideInInspector] public bool booted;

    [SerializeField] TMP_InputField roomNameInput;
    [SerializeField] TMP_Text createRoomErrorText;
    [SerializeField] TMP_Text ErrorText;
    [SerializeField] TMP_Text RoomName;
    [SerializeField] Transform RoomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startButton;

    [SerializeField] TMP_InputField UserNameInput;
    [SerializeField] AudioSource song;
    [SerializeField] Slider volumeSlide;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (PlayerPrefs.HasKey("username"))
            LoadName(); 

        if (PlayerPrefs.HasKey("soundVol"))
            LoadVol();
        else
        {
            PlayerPrefs.SetFloat("soundVol", 1);
            LoadVol();
        }
        if (PhotonNetwork.IsConnected)
        {
            MenuManager.instance.OpenMenu("TitleScreen");
            return;
        }
        PhotonNetwork.ConnectUsingSettings();
    }

    public void SetVolume()
    {
        AudioListener.volume = volumeSlide.value;
        SaveVolume();
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("soundVol", volumeSlide.value);
    }

    public void LoadVol()
    {
        volumeSlide.value = PlayerPrefs.GetFloat("soundVol");
    }

    public void LoadName()
    {
        PhotonNetwork.NickName = PlayerPrefs.GetString("username");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        if (!booted) {
            MenuManager.instance.OpenMenu("BootScreen");
            if (PlayerPrefs.HasKey("username"))
            {
                UserNameInput.text = PlayerPrefs.GetString("username");
                PhotonNetwork.NickName = PlayerPrefs.GetString("username");
            }
            else
            {
                UserNameInput.text = "doomingston" + Random.Range(0, 20).ToString("00");
                OnUsernameInputValueChanged();
            }
        }
        UserNameInput.text = PhotonNetwork.NickName;
        Debug.Log("connected lobby");
        if (booted) {
            MenuManager.instance.OpenMenu("TitleScreen");
        }
    }

    public void OnUsernameInputValueChanged()
    {
        PhotonNetwork.NickName = UserNameInput.text;
        PlayerPrefs.SetString("username", UserNameInput.text);
    }

    public void Boot()
    {
        MenuManager.instance.OpenMenu("TitleScreen");
        booted = true;
        
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            createRoomErrorText.gameObject.SetActive(true);
            createRoomErrorText.text = "Must enter a valid room name.";
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInput.text);
        MenuManager.instance.OpenMenu("loading");
        createRoomErrorText.gameObject.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        MenuManager.instance.OpenMenu("RoomScreen");
        RoomName.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ErrorText.text = message;
        MenuManager.instance.OpenMenu("ErrorScreen");
    }

    public void ChangeName()
    {
        PhotonNetwork.NickName = UserNameInput.text;
    }

    public void StartGame()
    {
        song.volume = Mathf.Lerp(.25f, 0, .05f);
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("TitleScreen");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("Loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("Loading");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in RoomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList) { continue; }
            Instantiate(roomListItemPrefab, RoomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!booted)
                return;           
            MenuManager.instance.OpenMenu("TitleScreen");
        }

        ChangeName();
    }
}