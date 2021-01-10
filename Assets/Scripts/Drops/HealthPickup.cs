using System;
using UnityEngine;

namespace Drops {
    public class HealthPickup : Drop {
        [SerializeField] private int amount = 100;
        protected override void Pickup() {
            foreach (var building in FindObjectsOfType<Building>()) {
                building.Health += amount;
            }
        }
    }
}