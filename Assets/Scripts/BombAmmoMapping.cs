using UnityEngine;


[CreateAssetMenu(fileName ="Bomb Mapping", menuName = "Game/Bomb Mapping")]
public class BombAmmoMapping : ScriptableObject {
    public GameObject prefab;
    public string title;
    public int ammoLeft;
}