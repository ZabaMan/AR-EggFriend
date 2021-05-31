using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCursor : MonoBehaviour
{
    [SerializeField] private GameObject placeEggUI;
    public GameManager GameManager;
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private ARPlaneManager arPlaneManager;
    public GameObject cursor;
    public GameObject instantiatedObject;
    public ARRaycastManager raycastManager;

    public bool cursorActive = true;

    private bool hostSpawningOthers = false;

    private Dictionary<string, GameManager.Egg> otherEggs = new Dictionary<string, GameManager.Egg>();
    // Start is called before the first frame update
    void Start()
    {
        EnableSpawning(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (cursorActive)
        {
            
            UpdateCursor();
            
            
        }

        
    }

    [ContextMenu("Spawn")]
    public void Spawn()
    {
        EggFriend egg = PhotonNetwork.Instantiate("EggFriend", transform.position + new Vector3(0, 0.025f, 0), transform.rotation).GetComponent<EggFriend>();
        //EggFriend egg = Instantiate(instantiatedObject, transform.position + new Vector3(0, 0.025f, 0), transform.rotation).GetComponent<EggFriend>();
        egg.SetColourRandomly();
        GameManager.CreateEggFriend(egg);
        cursorActive = false;
        cursor.SetActive(cursorActive);
        placeEggUI.SetActive(false);
        arPlaneManager.enabled = false;
        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }


    void UpdateCursor()
    {
        Vector2 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        if (hits.Count > 0)
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
        }
    }

    public void EnableSpawning(bool enable)
    {
        cursorActive = enable;
        cursor.SetActive(cursorActive);
        placeEggUI.SetActive(true);
    }
}
