using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkMinigame : MonoBehaviourPun
{
    private bool gameActive = false;
    
    private void OnEnable()
    {
        if (!photonView.IsMine && !gameActive)
        {
            ARCentralManager.Project.MinigameManager.JoinGame();
            gameActive = true;
        }
    }

    private void OnDestroy()
    {
        if (!photonView.IsMine && gameActive)
        {
            ARCentralManager.Project.MinigameManager.EndGame();
            gameActive = false;
        }
    }
}
