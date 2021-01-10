using System;
using UnityEngine;


public class BombUISelector : MonoBehaviour {
    private Cannon cannon;
    [SerializeField] private BombAmmoMapping bomb;
    private void OnEnable() {
        cannon = FindObjectOfType<Cannon>();
    }

    public void Selected() {
        if(bomb.ammoLeft > 0) cannon.bomb = bomb;
    }
}