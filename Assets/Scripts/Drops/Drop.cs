using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Drop : MonoBehaviour {
    [SerializeField] private float timeToPickup = 2f;
    [SerializeField] public int amount;
    [SerializeField] public Color fontColor;

    private GameManager gameManager;

    private bool hasCollected;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update() {
        timeToPickup -= Time.deltaTime;
        if (timeToPickup > 0 && !hasCollected) return;
        
        TryPickup();
    }

    private void TryPickup() {
        Pickup();

        gameManager.DisplayText($"+{amount}", transform.position, fontColor);
        hasCollected = true;
        Cleanup();
    }
    
    protected abstract void Pickup();

    protected virtual void Cleanup() {
        Destroy(gameObject);
    }
}
