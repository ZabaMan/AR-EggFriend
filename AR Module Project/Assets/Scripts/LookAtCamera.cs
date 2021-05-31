using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LookAtCamera : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    private Transform target;
    
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        transform.parent = ARCentralManager.Project.NetworkManager.AR_Origin;
    }
    
    private Transform camera;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        camera = Camera.main.transform;
        ARCentralManager.Project.GameManager.SetFood(transform);
    }

    public void SetImageToFollow(Transform toFollow)
    {
        target = toFollow;
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
            transform.position = target.position;
        transform.LookAt(camera);
    }
}
