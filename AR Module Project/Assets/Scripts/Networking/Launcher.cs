using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Launcher : MonoBehaviourPunCallbacks
{
    string gameVersion = "1";
    public string playerRoomCode;
    
    [SerializeField] private TextMeshProUGUI roomCodeUI;
    [SerializeField] private TextMeshProUGUI joinedRoomCodeUI;
    [SerializeField] private TMP_InputField roomInput;

    [SerializeField] private UnityEvent OnJoinedRoomEvent;
    private bool joiningOther = false;

    [SerializeField] private NetworkManager _networkManager;

    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        playerRoomCode = RandomString(5);
        roomCodeUI.text = playerRoomCode;
        Connect();
    }
    
    private string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string output = "";
        for (int i = 0; i < length; i++)
        {
            output += chars[Random.Range(0, chars.Length)];
        }

        return output;
    }

    private void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.CreateRoom(playerRoomCode, new RoomOptions());
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }
    
    public override void OnConnectedToMaster()
    {
        ARCentralManager.Project.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        if(!joiningOther)
            PhotonNetwork.CreateRoom(playerRoomCode, new RoomOptions());
        else
        {
            PhotonNetwork.JoinRoom(roomInput.text.ToUpper());
        }

        if (PlayerPrefs.HasKey("Nickname"))
        {
            _networkManager.SetNicknameAuto();
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        ARCentralManager.Project.Log("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason " + cause);
    }

    public override void OnJoinedRoom()
    {
        ARCentralManager.Project.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        if (!joiningOther)
            return;
        OnJoinedRoomEvent.Invoke();
        joinedRoomCodeUI.text = roomInput.text;
        _networkManager.SetupOnlinePanel();
    }

    public void ConnectToOther()
    {
        joiningOther = true;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ARCentralManager.Project.Log("Join Room Failed: " + message);
    }
    
}
