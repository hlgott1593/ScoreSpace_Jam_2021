using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Drop : MonoBehaviour {
    public abstract void Pickup();

    protected virtual void Cleanup() {
        Destroy(gameObject);
    }

    protected virtual void OnTriggerStay(Collider _other)
    {
        CursorCollision(_other);
    }

    private void CursorCollision(Collider _other)
    {
        if (_other.CompareTag("Cursor") && Input.GetKey(KeyCode.Mouse0))
        {
            Pickup();
        }
    }
}
