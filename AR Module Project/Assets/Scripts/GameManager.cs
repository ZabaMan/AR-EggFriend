using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GameManager : MonoBehaviour
{
    public class Egg
    {
        public EggFriend EggFriend { get; }

        public Egg(EggFriend egg)
        {
            EggFriend = egg;
        }
    }

    private Egg _eggFriend;
    [SerializeField] private GameObject cupcake;
    [SerializeField] private ARTrackedImageManager arTrackedImageManager;
    [SerializeField] private ARAnchorManager arAnchorManager;
    [SerializeField] private MinigameManager _minigameManager;
    
    [SerializeField] private RectTransform textMask, barMask;
    void OnEnable() => arTrackedImageManager.trackedImagesChanged += OnChanged;
    void OnDisable() => arTrackedImageManager.trackedImagesChanged -= OnChanged;
    
    private void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            var cupcakeGO = Instantiate(cupcake, newImage.transform.position, newImage.transform.rotation);
            cupcakeGO.transform.parent = newImage.transform;
            _eggFriend.EggFriend.SetFood(cupcakeGO.transform);
            ARDebugger.Debug.Log("Cupcake Made");
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
    
    public void CreateEggFriend(EggFriend egg)
    {
        _eggFriend = new Egg(egg);
        egg.SetUI(textMask, barMask);
        egg.gameObject.AddComponent<ARAnchor>();
    }
    
    [ContextMenu("Start Minigame")]
    public void StartMinigame()
    {
        _minigameManager.StartGame(_eggFriend.EggFriend);
    }
}
