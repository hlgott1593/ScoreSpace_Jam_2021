using System;
using UnityEngine;

namespace Drops {
    public class BlackholeAmmoPickup : Drop {
        [SerializeField] private BombAmmoMapping bhBomb;
        protected override void Pickup() {
            bhBomb.ammoLeft += amount;
        }
    }
}