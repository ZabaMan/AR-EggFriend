using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform egg;
    private MinigameManager _minigameManager;
    [SerializeField] private float speed;

    public void Setup(Transform playersEgg, MinigameManager minigameManager)
    {
        egg = playersEgg;
        _minigameManager = minigameManager;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(egg);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, egg.transform.position) < 0.1f)
        {
            _minigameManager.EndGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet"))
        {
            _minigameManager.KillDrill(this);
        }
    }
}
