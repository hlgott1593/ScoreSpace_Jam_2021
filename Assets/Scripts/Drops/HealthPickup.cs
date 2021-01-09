using System;
using UnityEngine;

namespace Drops {
    public class HealthPickup : Drop {
        
        public override void Pickup() {
            Debug.Log("pickup");
            foreach (var building in FindObjectsOfType<Building>()) {
                building.Health += 100;
            }

            Cleanup();
        }
    }
}