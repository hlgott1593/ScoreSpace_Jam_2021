using System;
using UnityEngine;
using UnityEngine.UI;


public class BombUISelector : MonoBehaviour {
    private Cannon cannon;
    [SerializeField] private BombAmmoMapping bomb;
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private Color availableColor;
    [SerializeField] private Color unavailableColor;

    private void OnEnable() {
        cannon = FindObjectOfType<Cannon>();
    }

    private void Update() {
        if (bomb.ammoLeft > 0 && !button.interactable) {
            button.interactable = true;
            image.color = availableColor;
        }
        else if (bomb.ammoLeft <= 0 && button.interactable) {
            button.interactable = false;
            image.color = unavailableColor;
        }
    }

    public void Selected() {
        if (bomb.ammoLeft > 0) cannon.bomb = bomb;
    }
}