using System;
using UnityEngine;

namespace Drops {
    public class HealthPickup : Drop {
        protected override void Pickup() {
            var ff = FindObjectOfType<Forcefield>();
            if (ff == null) return;
            ff.Energy += amount;
        }
    }
}