using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
    private Transform mySprite;
    public GameManager GameManager;

    public float Health;
    public float Damage;

    [Header("Movement Settings")] [SerializeField]
    public float moveSpeed;

    [SerializeField] public float spriteRotateSpeed;

    [Header("Pefabs")] [SerializeField] private GameObject DeathExplosion;

    private float behaviorTimer;

    // Start is called before the first frame update
    void Start() {
        mySprite = transform.Find("Sprite");
    }

    // Update is called once per frame
    void Update() {
        UpdateSpinEffect();
    }

    /// <summary>
    /// Simple spin effect on the main renderer
    /// </summary>
    private void UpdateSpinEffect() {
        mySprite.transform.Rotate(0f, 0f, spriteRotateSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Enemy")) {
            Enemy enemy = collision.collider.GetComponent<Enemy>();
            Health -= enemy.Damage;
            if (Health <= 0) {
                Kill();
            }
        }

        if (collision.gameObject.CompareTag("Building")) {
            Kill();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Bomb")) {
            if (TryGetComponent(out BlackHoleBomb blackHoleBomb)) {
                blackHoleBomb.MyExplodeCallback += Kill;
                return;
            }

            if (TryGetComponent(out Bomb bomb)) {
                if (bomb.EarlyCheck) {
                    bomb.MyExplodeCallback += Kill;
                    bomb.Explode();
                }
            }
        }

        if (other.CompareTag("Explosion")) {
            Explosion explosion = other.GetComponent<Explosion>();
            Health -= explosion.Damage;
            if (Health <= 0) {
                Kill();
            }
        }
    }

    public void Kill() {
        Instantiate(DeathExplosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}