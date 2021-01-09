using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Drop : MonoBehaviour {
    [SerializeField] private InputReader inputReader;
    [SerializeField] private bool cursorColliding;
    private void OnEnable() {
        inputReader.FireEvent += TryPickup;
    }

    private void OnDisable() {
        inputReader.FireEvent -= TryPickup;
    }

    private void TryPickup() {
        if (!cursorColliding) return;
        Pickup();
        Cleanup();
    }
    
    protected abstract void Pickup();

    protected virtual void Cleanup() {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Cursor")) return;
        cursorColliding = true;
    }

    private void OnTriggerExit(Collider other) {
        if(!other.CompareTag("Cursor")) return;
        cursorColliding = false;
    }
}
