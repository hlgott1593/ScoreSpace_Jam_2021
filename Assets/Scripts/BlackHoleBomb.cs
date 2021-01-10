using System;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleBomb : Bomb {
    private List<Enemy> interactors = new List<Enemy>();

    private void OnEnable() {
        MyExplodeCallback += ApplyBlackhole;
    }
    private void OnDisable() {
        MyExplodeCallback -= ApplyBlackhole;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out Enemy enemy)) interactors.Add(enemy);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!other.TryGetComponent(out Enemy enemy) || !interactors.Contains(enemy)) return;
        interactors.Remove(enemy);
    }

    public void ApplyBlackhole() {
        for (int i = 0; i < interactors.Count; i++) {
            interactors[i].EnterEventHorizon(this);
        }
    }
}