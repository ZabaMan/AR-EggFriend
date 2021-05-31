using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Enemy : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    private Transform egg;
    private MinigameManager _minigameManager;
    [SerializeField] private float speed;
    private bool dead = false;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        transform.parent = ARCentralManager.Project.NetworkManager.AR_Origin;
        
        _minigameManager = ARCentralManager.Project.MinigameManager;
        
        if (!photonView.IsMine)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            Dictionary<string, EggFriend> _eggFriendsOnline = ARCentralManager.Project.GameManager._eggFriendsOnline;
            if (_eggFriendsOnline.ContainsKey((string) instantiationData[0]))
            {
                egg = _eggFriendsOnline[(string) instantiationData[0]].transform;
            }
            else
            {
                egg = ARCentralManager.Project.GameManager.GetEggFriend().EggFriend.transform;
            }
            
            transform.position = ARCentralManager.Project.NetworkManager.AR_Origin.TransformPoint((Vector3) instantiationData[1]);
        }

    }

    public void Setup(Transform playersEgg)
    {
        egg = playersEgg;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            transform.LookAt(egg);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, egg.transform.position) < 0.05f)
            {
                _minigameManager.EndGame();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet") && !dead)
        {
            _minigameManager.KillDrill(this);
            dead = true;
        }
    }

    private void OnDestroy()
    {
        _minigameManager.AddScore();
    }
}
