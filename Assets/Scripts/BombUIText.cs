using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BombUIText : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private BombAmmoMapping mapping;

    private string Infinity = "∞";
    
    void Update() {
        textMeshProUGUI.text = $"{mapping.title}: {(mapping.ammoLeft > 999 ?  Infinity : mapping.ammoLeft.ToString())}";
}
}
