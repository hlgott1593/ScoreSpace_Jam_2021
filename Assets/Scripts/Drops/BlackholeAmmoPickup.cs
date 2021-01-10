using System;
using UnityEngine;

namespace Drops {
    public class BlackholeAmmoPickup : Drop {
        [SerializeField] private BombAmmoMapping bhBomb;
        [SerializeField] private int amount = 5;
        protected override void Pickup() {
            bhBomb.ammoLeft += amount;
        }
    }
}