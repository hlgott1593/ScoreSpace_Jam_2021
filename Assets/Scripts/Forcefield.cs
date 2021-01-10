using DG.Tweening;
using UnityEngine;

public class Forcefield : MonoBehaviour {
    [SerializeField] private Collider2D col;
    [SerializeField] private Renderer rend;
    private static readonly int EmmisionColor = Shader.PropertyToID("Color_1851709F");
    private static readonly int DissolveTime = Shader.PropertyToID("Vector1_895D7E5B");
    private bool shieldsActive;
    [SerializeField] private float initialEnergy = 1000;
    [field: SerializeField] public float Energy { get; set; }
    [SerializeField] private float shieldAnimationDuration = 2;
    

    private void Awake() {
        shieldsActive = true;
        Energy = initialEnergy;
    }

    private Color ColorFromHealth() {
        if (Energy / initialEnergy > 0.5) return Color.cyan;
        if (Energy / initialEnergy > 0.25) return Color.yellow;
        return Color.red;
    }

    private void ShieldsDown() {
        shieldsActive = false;
        rend.material.SetFloat(DissolveTime, 1);
        var s = DOTween.Sequence();
        s.Append(rend.material.DOFloat(-1, DissolveTime, shieldAnimationDuration));
        s.AppendCallback(() => {
            rend.enabled = false;
            col.enabled = false;
        });
        s.Play();
    }

    private void ShieldsUp() {
        shieldsActive = true;
        rend.material.SetFloat(DissolveTime, -1);
        var s = DOTween.Sequence();
        s.AppendCallback(() => {
            rend.enabled = true;
            col.enabled = true;
        });
        s.Append(rend.material.DOFloat(1, DissolveTime, shieldAnimationDuration));
        s.Play();
    }

    private void Update() {
        if (Energy > 0 && !shieldsActive) ShieldsUp();
        else if (Energy <= 0 && shieldsActive) ShieldsDown();
        rend.material.SetColor(EmmisionColor, ColorFromHealth());
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.TryGetComponent(out Enemy enemy)) {
            Energy -= enemy.Damage * Time.deltaTime;
        }
    }
}