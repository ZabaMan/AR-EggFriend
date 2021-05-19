using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCursor : MonoBehaviour
{
    public GameManager GameManager;
    public GameObject cursor;
    public GameObject instantiatedObject;
    public ARRaycastManager raycastManager;

    public bool cursorActive = true;
    // Start is called before the first frame update
    void Start()
    {
        cursor.SetActive(cursorActive);
    }

    // Update is called once per frame
    void Update()
    {
        if (cursorActive)
        {
            
            UpdateCursor();
            
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Spawn();
            }
        }

        
    }

    [ContextMenu("Spawn")]
    private void Spawn()
    {
        GameObject egg = Instantiate(instantiatedObject, transform.position + new Vector3(0, 0.025f, 0), transform.rotation);
        GameManager.CreateEggFriend(egg.GetComponent<EggFriend>());
        cursorActive = false;
        cursor.SetActive(cursorActive);
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
}
