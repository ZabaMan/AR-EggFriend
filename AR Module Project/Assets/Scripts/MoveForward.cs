using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MoveForward :  MonoBehaviourPun, IPunInstantiateMagicCallback
{

    [SerializeField] private float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        transform.parent = ARCentralManager.Project.NetworkManager.AR_Origin;
        
        if (!photonView.IsMine)
        {
            object[] instantiationData = info.photonView.InstantiationData;

            transform.position = ARCentralManager.Project.NetworkManager.AR_Origin.TransformPoint((Vector3) instantiationData[0]);
        }
    }
}
