using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour {
    private Transform mySprite;
    [SerializeField] private int scoreGranted;
    public GameManager GameManager;
    [SerializeField] private Text healthText;

    public float Health;
    public float Damage;
    [SerializeField] private int dropChance;

    [Header("Movement Settings")] [SerializeField]
    private float orbitRange;

    [SerializeField] private float attackRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float spriteRotateSpeed;
    private bool attacking;
    private float behaviorTimer;

    [Header("Pefabs")] [SerializeField] private GameObject DeathExplosion;

    [Header("Drops")] [SerializeField] private List<Drop> drops;

    public BlackHoleBomb blackHoleBomb;

    [Header("Bump Settings")] [SerializeField]
    private bool pushed;

    [SerializeField] float bumpDuration;
    [SerializeField] float bumpForce;
    [SerializeField] private Bounds shieldBounds;

    void Start() {
        healthText.text = "" + (int) Health;
        mySprite = transform.Find("Sprite");
        shieldBounds = GameObject.Find("ForceField").GetComponent<MeshRenderer>().bounds;
    }

    void Update() {
        if (blackHoleBomb != null) BlackholeMovement();
        else if (!pushed) OrbitalMovement();
        UpdateSpinEffect();
    }

    public void EnterEventHorizon(BlackHoleBomb bomb) => blackHoleBomb = bomb;

    private void BlackholeMovement() {
        var dest = blackHoleBomb.transform.position - transform.position;
        transform.Translate(dest.normalized * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Simply move towards the center of the screen
    /// </summary>
    private void SimpleMovement() {
        Vector3 toCenter = Vector3.zero - transform.position;
        if (toCenter.magnitude > 2f) {
            transform.Translate(toCenter.normalized * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Move into a set range orbiting around, randomly going in and out to damage buildings.
    /// </summary>
    private void OrbitalMovement() {
        // Calculate some info
        Vector3 toCenter = Vector3.zero - transform.position;
        float distance = toCenter.magnitude;
        float range = attacking ? attackRange : orbitRange;

        // Move to the correct range, and orbit when in range
        if (distance > range + 0.1f) {
            transform.Translate(toCenter.normalized * moveSpeed * Time.deltaTime);
        }
        else if (distance < range - 0.1f) {
            transform.Translate(toCenter.normalized * -moveSpeed * Time.deltaTime);
        }
        else {
            Vector3 orbitDirection = Vector2.Perpendicular(toCenter);
            transform.Translate(orbitDirection.normalized * moveSpeed * Time.deltaTime);
        }

        // Every so often, change behavior
        behaviorTimer -= Time.deltaTime;
        if (behaviorTimer <= 0f) {
            behaviorTimer = 1f;
            if (Random.Range(0, 3) == 0) {
                attacking = !attacking;
            }
        }
    }

    /// <summary>
    /// Simple spin effect on the main renderer
    /// </summary>
    private void UpdateSpinEffect() {
        mySprite.transform.Rotate(0f, 0f, spriteRotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Destroys the enemy and increases score.
    /// </summary>
    public void Kill() {
        GameManager.DisplayText($"+{scoreGranted}", transform.position, GameManager.scoreText.color);
        GameManager.IncreaseScore(scoreGranted);
        Instantiate(DeathExplosion, transform.position, Quaternion.identity);
        HandleDrop();
        Destroy(gameObject);
    }

    private void HandleDrop() {
        var roll = Random.Range(0, 100);
        if (roll <= dropChance) return;
        var drop = drops[Random.Range(0, drops.Count)];
        Instantiate(drop, transform.position, Quaternion.identity);
    }


    /// <summary>
    /// Blow up after colliding with a bomb.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Bomb")) {
            if (other.TryGetComponent(out BlackHoleBomb bhBomb)) {
                EnterEventHorizon(bhBomb);
            }
            else if (other.TryGetComponent(out Bomb bomb)) {
                bomb.MyExplodeCallback += Kill;
                bomb.Explode();
            }
        }

        if (other.CompareTag("Explosion")) {
            Explosion explosion = other.GetComponent<Explosion>();
            Health -= explosion.Damage;
            healthText.text = "" + (int) Health;
            if (Health <= 0) {
                Kill();
            }
        }

        if (other.CompareTag("Obstacle")) {
            Obstacle obstacle = other.GetComponent<Obstacle>();
            Health -= obstacle.Damage;
            healthText.text = "" + (int) Health;
            if (Health <= 0) {
                Kill();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Obstacle")) {
            Obstacle obstacle = collision.collider.GetComponent<Obstacle>();
            Health -= obstacle.Damage;
            healthText.text = "" + (int) Health;
            if (Health <= 0) {
                Kill();
            }
            else {

                StartCoroutine(Bump(obstacle.transform.position));
            }
        }
    }

    private IEnumerator Bump(Vector2 source) {
        pushed = true;
        Vector2 start = transform.position;
        float cur = 0;
        Vector2 direction = -(source - start).normalized;
        Vector2 end = start + (direction * bumpForce);
        while (cur < bumpDuration) {
            // Handle not bumping into center
            Vector3 toCenter = Vector3.zero - transform.position;
            float distance = toCenter.magnitude;
            if (distance < (shieldBounds.extents.magnitude + 1f) / 2)
            {
                break;
            }
            // continue bump
            transform.position = Vector3.Lerp(start, end, (cur / bumpDuration));
            cur += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        pushed = false;
    }
}