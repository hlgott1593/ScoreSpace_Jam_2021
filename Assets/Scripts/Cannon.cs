using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Cannon : MonoBehaviour {
    private Camera mainCamera;
    [SerializeField] private GameManager gameManager;
    private bool shooting;
    private Vector3 targetLocation;
    [SerializeField] private InputReader inputReader;

    public LineRenderer TargettingLine;
    public float TargettingLineProgress;
    public float TargettingLineProgressAmount;

    [Header("Settings")] private float coolDownTime;
    private float coolDownTimer;
    [SerializeField] private bool sinStyleLine;

    [Header("Prefabs")] [SerializeField] public BombAmmoMapping defaultMapping;
    [HideInInspector] public BombAmmoMapping bomb;
    [SerializeField] private GameObject targetNodePrefab;
    [SerializeField] private GameObject targetConfirmedVFX;


    [Header("SFX")] [SerializeField] private AudioClip[] onClickHasNext;
    [SerializeField] private AudioClip[] onClickLast;
    private AudioSource audioSource;
    private Vector2 mousePos;

    private void OnEnable() {
        bomb = defaultMapping;
        inputReader.FireEvent += PlaceBombs;
        inputReader.PointerEvent += UpdateCursor;
    }

    private void UpdateCursor(InputAction.CallbackContext arg0) {
        mousePos = arg0.ReadValue<Vector2>();
    }

    private void OnDisable() {
        inputReader.FireEvent -= PlaceBombs;
        inputReader.PointerEvent -= UpdateCursor;
    }

    public void PlayOnClick(bool hasNext = true) {
        if (!hasNext) {
            audioSource.PlayOneShot(onClickLast[Random.Range(0, onClickLast.Length)]);
        }

        audioSource.PlayOneShot(onClickHasNext[Random.Range(0, onClickHasNext.Length)]);
    }

    // Start is called before the first frame update
    void Start() {
        defaultMapping.ammoLeft = Int32.MaxValue;
        mainCamera = Camera.main;
        gameManager = transform.root.GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        UpdateCannonRotation();
        UpdateTimers();
    }

    private void UpdateCannonRotation() {
        // Look at the mouse
        Vector3 toMouse = (GetMousePos() - transform.position).normalized;

        float rotZ = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90);
        // transform.rotation = XLookRotation(toMouse, Vector3.up);
    }

    private void PlaceBombs() {
        if (coolDownTimer <= 0f && !shooting) {
            coolDownTimer = coolDownTime;
            shooting = true;
            bomb.ammoLeft--;
            targetLocation = GetMousePos();
            Destroy(Instantiate(targetConfirmedVFX, targetLocation, Quaternion.identity), 2.5f);
            gameManager.SlowDownTime();
            BeginTargetting();
        }
    }

    /// <summary>
    /// Updates the timers.
    /// </summary>
    private void UpdateTimers() {
        if (coolDownTimer > 0f) {
            coolDownTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Spawns the given bomb type at the given position.
    /// </summary>
    private void SpawnBomb(GameObject _bomb, Vector3 _position) {
        Transform bomb = Instantiate(_bomb).transform;
        bomb.position = _position;
    }

    /// <summary>
    /// Spawns and configures a chain of target nodes towards the target position.
    /// </summary>
    public void BeginTargetting() {
        List<Vector3> linePositions = new List<Vector3>();
        Vector3 toTarget = targetLocation - transform.position;
        float distance = toTarget.magnitude;
        float waveDistance = 1f;
        toTarget.Normalize();
        Vector3 toTargetPerp = Vector2.Perpendicular(toTarget).normalized;

        TargetNode firstNode = null;
        TargetNode lastNode = null;
        int numNodes = 10;
        for (int i = 0; i < numNodes; i++) {
            TargetNode node = Instantiate(targetNodePrefab, gameManager.TargettingNodes).GetComponent<TargetNode>();
            node.TimeToSelect = 1f;
            node.MyCannon = this;
            if (lastNode != null) {
                lastNode.NextNode = node;
                node.gameObject.SetActive(false);
            }

            if (firstNode == null) {
                firstNode = node;
            }

            lastNode = node;

            float distanceRatio = (float) i / (numNodes - 1);
            Vector3 position =
                new Vector3(toTarget.x * distance * distanceRatio, toTarget.y * distance * distanceRatio);
            float angle = distanceRatio * 360f * Mathf.Deg2Rad;
            if (bomb.title == "Blackhole")
                position += new Vector3(toTargetPerp.x * Mathf.Sin(angle) * waveDistance,
                    toTargetPerp.y * Mathf.Sin(angle) * waveDistance, 0f);
            node.transform.position = position;
            linePositions.Add(position);
        }

        firstNode.TimeToSelect = 3f;
        firstNode.transform.position = transform.position; // So we always begin by hovering over the cannon
        firstNode.GetComponent<SpriteRenderer>().enabled = true;
        lastNode.transform.position = targetLocation; // So the final one is always over the target

        TargettingLine.positionCount = numNodes;
        TargettingLine.SetPositions(linePositions.ToArray());
        TargettingLine.gameObject.SetActive(true);

        SetTargettingLineProgress(0f);
        TargettingLineProgressAmount = 1f / numNodes;
    }

    public void SetTargettingLineProgress(float _middleTime) {
        float preTime = _middleTime - 0.2f;
        float postTime = _middleTime + 0.2f;
        if (preTime < 0f) {
            preTime = 0f;
        }

        if (postTime > 1f) {
            postTime = 1f;
        }

        Gradient gradient = TargettingLine.colorGradient;
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
        alphaKeys[1].time = preTime;
        alphaKeys[2].time = _middleTime;
        alphaKeys[3].time = postTime;
        gradient.SetKeys(gradient.colorKeys, alphaKeys);
        TargettingLine.colorGradient = gradient;

        TargettingLineProgress = _middleTime;
    }

    /// <summary>
    /// Callback for if the player doesn't get the target node in time
    /// </summary>
    public void FailedTarget() {
        var target = FindObjectsOfType<TargetNode>().First(x => x.gameObject.activeInHierarchy);
        var spawnPos = target == null ? RandomPosition(5f, 5f) : target.transform.position;

        gameManager.NormalizeTime();
        SpawnBomb(bomb.prefab, spawnPos);
        shooting = false;

        foreach (Transform child in gameManager.TargettingNodes) {
            Destroy(child.gameObject);
        }

        TargettingLine.gameObject.SetActive(false);
    }


    /// <summary>
    /// Callback for when the player selects the final node
    /// </summary>
    public void FinalizeTarget() {
        gameManager.NormalizeTime();
        SpawnBomb(bomb.prefab, targetLocation);
        StartCoroutine(EnableFiringAfterDelay());

        foreach (Transform child in gameManager.TargettingNodes) {
            Destroy(child.gameObject);
        }

        TargettingLine.gameObject.SetActive(false);

        if (bomb.ammoLeft <= 0) bomb = defaultMapping;
    }

    private IEnumerator EnableFiringAfterDelay() {
        yield return new WaitForSeconds(0.33f);
        shooting = false;
    }

    /// <summary>
    /// The world space position of the mouse
    /// </summary>
    public Vector3 GetMousePos() {
        Vector3 point = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x,mousePos.y, 0f)); //mainCamera.nearClipPlane));
        point.z = 0f;
        return point;
    }

    
    /// <summary>
    /// Returns a random position within the given bounds.
    /// </summary>
    public Vector3 RandomPosition(float _xDistance, float _yDistance) {
        float x = Random.Range(-_xDistance, _xDistance);
        float y = Random.Range(-_yDistance, _yDistance);
        return new Vector3(x, y, 0f);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!other.gameObject.CompareTag("Enemy")) return;
        gameManager.HandleGameOver();
        enabled = false;
    }
}