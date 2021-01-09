using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
public class InputReader : ScriptableObject, PlayerControls.IGameplayActions {
    public event UnityAction FireEvent = delegate {  };

    private PlayerControls playerControls;

    private void OnEnable() {
        if (playerControls == null) {
            playerControls = new PlayerControls();
            playerControls.Gameplay.SetCallbacks(this);
        }

        EnableGameplayInput();
    }

    public void EnableGameplayInput() {
        playerControls.Gameplay.Enable();
    }

    public void OnFire(InputAction.CallbackContext context) {
        if (!context.performed) return;

        FireEvent.Invoke();
    }
}