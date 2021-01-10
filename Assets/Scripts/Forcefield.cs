using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forcefield : MonoBehaviour {
    [SerializeField] private Collider2D collider2D;
    [SerializeField] private Renderer rend;
    private static readonly int EmmisionColor = Shader.PropertyToID("Color_1851709F");

    [field: SerializeField] public float Health { get; set; } = 1000;

    private Color ColorFromHealth() {
        if (Health > 500) return Color.cyan;
        if (Health > 250) return Color.yellow;
        if (Health > 0) return Color.red;
        return Color.clear;
    }

    private void ShieldsDown() {
        rend.enabled = false;
        collider2D.enabled = false;
    }

    private void ShieldsUp() {
        rend.enabled = true;
        collider2D.enabled = true;
    }
    
    private void Update() {
        if (Health > 0) ShieldsUp();
        else ShieldsDown();
        
        rend.material.SetColor(EmmisionColor, ColorFromHealth());
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.TryGetComponent(out Enemy enemy)) {
            Health -= enemy.Damage * Time.deltaTime;
        }
    }
}