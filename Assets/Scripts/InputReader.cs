using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, PlayerControls.IPlayerActions {
    public event UnityAction FireEvent = delegate {  };
    private bool ShouldFire { get; set; } = false;

    private PlayerControls playerControls;

    private void OnEnable() {
        if (playerControls == null) {
            playerControls = new PlayerControls();
            playerControls.Player.SetCallbacks(this);
        }

        EnableGameplayInput();
    }

    public void EnableGameplayInput() {
        playerControls.Player.Enable();
    }
    
    public void OnFire(InputAction.CallbackContext context) {
        if (context.performed) ShouldFire = true;
        else ShouldFire = false;
    }

    public void OnLook(InputAction.CallbackContext context) { }

    private void Update() {
        // if(ShouldFire) FireEvent.Invoke();
    }
}