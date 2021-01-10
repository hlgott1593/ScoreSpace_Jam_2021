using System;
using UnityEngine;

namespace Drops {
    public class HealthPickup : Drop {
        [SerializeField] private int amount = 100;
        protected override void Pickup() {
            var ff = FindObjectOfType<Forcefield>();
            if (ff == null) return;
            ff.Health += amount;
        }
    }
}