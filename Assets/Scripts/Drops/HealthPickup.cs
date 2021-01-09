using System;
using UnityEngine;

namespace Drops {
    public class HealthPickup : Drop {
        protected override void Pickup() {
            foreach (var building in FindObjectsOfType<Building>()) {
                building.Health += 100;
            }
        }
    }
}