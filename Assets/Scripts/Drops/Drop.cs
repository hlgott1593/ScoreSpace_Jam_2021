using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Drop : MonoBehaviour {
    [SerializeField] private float timeToPickup = 2f;
    private bool hasCollected;
    private void Update() {
        timeToPickup -= Time.deltaTime;
        if (timeToPickup > 0 && !hasCollected) return;
        
        TryPickup();
    }

    private void TryPickup() {
        Pickup();
        hasCollected = true;
        Cleanup();
    }
    
    protected abstract void Pickup();

    protected virtual void Cleanup() {
        Destroy(gameObject);
    }
}
