using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Egg
    {
        public EggFriend EggFriend { get; private set;  }
        public int colour { get; }

        public Egg(EggFriend egg, int c)
        {
            EggFriend = egg;
            colour = c;
        }

        public Egg(int c)
        {
            colour = c;
        }

        public void AddFriend(EggFriend eggFriend)
        {
            EggFriend = eggFriend;
        }
    }

    private Egg _eggFriend;
    public Egg GetEggFriend() => _eggFriend;
    public Dictionary<string, EggFriend> _eggFriendsOnline = new Dictionary<string, EggFriend>();
    [SerializeField] private GameObject cupcake, cupcakeButton;
    [SerializeField] private ARTrackedImageManager arTrackedImageManager;
    [SerializeField] private ARAnchorManager arAnchorManager;
    [SerializeField] private MinigameManager _minigameManager;
    [SerializeField] private NetworkManager _networkManager;
    
    [SerializeField] private RectTransform textMask, barMask;
    void OnEnable() => arTrackedImageManager.trackedImagesChanged += OnChanged;
    void OnDisable() => arTrackedImageManager.trackedImagesChanged -= OnChanged;
    
    private void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            var cupcakeGO = PhotonNetwork.Instantiate("Cupcake", newImage.transform.position, newImage.transform.rotation);
            cupcakeGO.GetComponent<LookAtCamera>().SetImageToFollow(newImage.transform);
            ARCentralManager.Project.Log("Cupcake Made");
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event
        }

        foreach (var removedImage in eventArgs.removed)
        {
            _eggFriend.EggFriend.RemoveFood();
        }
    }

    public void SetFood(Transform cupcake)
    {
        _eggFriend.EggFriend.SetFood(cupcake);
        foreach (var onlineEggFriend in _eggFriendsOnline)
        {
            onlineEggFriend.Value.SetFood(cupcake);
        }
    }

    [ContextMenu("Spawn Food")]
    public void MakeFood()
    {
        var cupcakeGO = PhotonNetwork.Instantiate("Cupcake", transform.position, transform.rotation);
        _eggFriend.EggFriend.SetFood(cupcakeGO.transform);
        ARCentralManager.Project.Log("Cupcake Made");
    }

    public void ToggleFood()
    {
        _eggFriend.EggFriend.foodAvailable = !_eggFriend.EggFriend.foodAvailable;
    }
    
    public void CreateEggFriend(EggFriend egg)
    {
        _eggFriend = new Egg(egg, egg.eggColour);
        egg.SetUI(textMask, barMask);
        egg.gameObject.AddComponent<ARAnchor>();
        
        _networkManager.SetupEggProperties(_eggFriend);
    }
    
    [ContextMenu("Start Minigame")]
    public void StartMinigame()
    {
        _minigameManager.StartGame(_eggFriend.EggFriend, _eggFriendsOnline);
    }

    public void AddEggFriendsToOnlineList(string nickname, EggFriend eggFriend)
    {
        _eggFriendsOnline.Add(nickname, eggFriend);
    }
}
