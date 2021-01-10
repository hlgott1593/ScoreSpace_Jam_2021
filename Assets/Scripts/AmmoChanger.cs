using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Cannon))]
public class AmmoChanger : MonoBehaviour {
    [SerializeField] private Cannon cannon;

    [SerializeField] public List<GameObject> bombs;
    
    

}