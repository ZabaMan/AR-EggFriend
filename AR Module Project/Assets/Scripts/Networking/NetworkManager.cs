using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Transform AR_Origin;
    
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private TMP_InputField nicknameText;
    private string nickname;
    [SerializeField] private TextMeshProUGUI hostName, memberNames;
    [SerializeField] private GameObject onlinePanel;
    [SerializeField] private ARCursor _arCursor;

    public void SetNickname()
    {
        nickname = nicknameText.text;
        PhotonNetwork.NickName = nickname;

        if (!PlayerPrefs.HasKey("Nickname"))
        {
            PlayerPrefs.SetString("Nickname", nickname);
            PlayerPrefs.Save();
        }
    }

    public void SetNicknameAuto()
    {
        nicknameText.text = PlayerPrefs.GetString("Nickname");
        SetNickname();
    }
    
    public override void OnPlayerEnteredRoom(Player other)
    {
        ARCentralManager.Project.Log("Player Entered Room"); // not seen if you're the player connecting

        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            SetupOnlinePanel();
        }
    }

    public void SetupOnlinePanel()
    {
        onlinePanel.SetActive(true);
        
        hostName.text = "";
        memberNames.text = "";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.IsMasterClient)
            {
                hostName.text = player.NickName;
            }
            else
            {
                memberNames.text += player.NickName + "\n";
            }
        }
    }


    public override void OnPlayerLeftRoom(Player other)
    {
        ARCentralManager.Project.Log("Player Left Room"); // seen when other disconnects
    }

    public void SetupEggProperties(GameManager.Egg egg)
    {
        ExitGames.Client.Photon.Hashtable eggData = new ExitGames.Client.Photon.Hashtable();
        eggData.Add("colour", egg.colour);
        eggData.Add("offset", AR_Origin.InverseTransformPoint(egg.EggFriend.transform.position));
        PhotonNetwork.SetPlayerCustomProperties(eggData);
    }

    public void WaitBeforeLoadEggProperties(EggFriend eggFriend, PhotonView pv)
    {
        StartCoroutine(LoadEggProperties(eggFriend, pv));
    }
    
    private IEnumerator LoadEggProperties(EggFriend eggFriend, PhotonView pv)
    {
        yield return new WaitForSeconds(1.0f);
        eggFriend.SetColourManually((int) pv.Owner.CustomProperties["colour"]);
        eggFriend.transform.position = AR_Origin.TransformPoint((Vector3)pv.Owner.CustomProperties["offset"]);
    }


    public void SetSyncPoint()
    {
        AR_Origin.position = _arCursor.transform.position;//Camera.main.transform.position;
        AR_Origin.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);

        ARCentralManager.Project.Log("World Center Set To: " + AR_Origin.position);


    }
    
}
