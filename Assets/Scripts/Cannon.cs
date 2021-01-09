using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Cannon : MonoBehaviour
{
	private Camera mainCamera;
	[SerializeField] private GameManager gameManager;
	private bool shooting;
	private Vector3 targetLocation;
	[SerializeField] private InputReader inputReader;

	public LineRenderer TargettingLine;
	public float TargettingLineProgress;
	public float TargettingLineProgressAmount;

	[Header("Settings")]
	private float coolDownTime;
	private float coolDownTimer;

	[Header("Prefabs")]
	[SerializeField] private GameObject bombPrefab;
	[SerializeField] private GameObject targetNodePrefab;


	[Header("SFX")] 
	[SerializeField] private AudioClip[] onClickHasNext;
	[SerializeField] private AudioClip[] onClickLast;
	private AudioSource audioSource;

	private void OnEnable() {
		inputReader.FireEvent += PlaceBombs;
	}

	private void OnDisable() {
		inputReader.FireEvent -= PlaceBombs;
	}

	public void PlayOnClick(bool hasNext = true)
	{
		if (!hasNext)
		{
			audioSource.PlayOneShot(onClickLast[Random.Range(0,onClickLast.Length)]);
		}
		
		audioSource.PlayOneShot(onClickHasNext[Random.Range(0,onClickHasNext.Length)]);
		
	}
	
	// Start is called before the first frame update
	void Start()
	{
		mainCamera = Camera.main;
		gameManager = transform.parent.GetComponent<GameManager>();
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		UpdateCannonRotation();
		UpdateTimers();
	}

	private void UpdateCannonRotation()
	{
		// Look at the mouse
		Vector3 toMouse = (GetMousePos() - transform.position).normalized;
		transform.rotation = XLookRotation(toMouse, Vector3.up);
	}

	private void PlaceBombs() {
		if (coolDownTimer <= 0f && !shooting) {
			coolDownTimer = coolDownTime;
			shooting = true;
			targetLocation = GetMousePos();
			gameManager.SlowDownTime();
			BeginTargetting();
		}
	}

	/// <summary>
	/// Updates the timers.
	/// </summary>
	private void UpdateTimers()
	{
		if (coolDownTimer > 0f)
		{
			coolDownTimer -= Time.deltaTime;
		}
	}

	/// <summary>
	/// Spawns the given bomb type at the given position.
	/// </summary>
	private void SpawnBomb(GameObject _bomb, Vector3 _position)
	{
		Transform bomb = Instantiate(_bomb).transform;
		bomb.position = _position;
	}

	/// <summary>
	/// Spawns and configures a chain of target nodes towards the target position.
	/// </summary>
	public void BeginTargetting()
	{
		List<Vector3> linePositions = new List<Vector3>();
		Vector3 toTarget = targetLocation - transform.position;
		float distance = toTarget.magnitude;
		float waveDistance = 1f;
		toTarget.Normalize();
		Vector3 toTargetPerp = Vector2.Perpendicular(toTarget).normalized;

		TargetNode firstNode = null;
		TargetNode lastNode = null;
		int numNodes = 10;
		for (int i = 0; i < numNodes; i++)
		{
			TargetNode node = Instantiate(targetNodePrefab, gameManager.TargettingNodes).GetComponent<TargetNode>();
			node.TimeToSelect = 1f;
			node.MyCannon = this;
			if (lastNode != null)
			{
				lastNode.NextNode = node;
				node.gameObject.SetActive(false);
			}
			if (firstNode == null)
			{
				firstNode = node;
			}
			lastNode = node;

			float distanceRatio = (float)i / (numNodes - 1);
			Vector3 position = new Vector3(toTarget.x * distance * distanceRatio, toTarget.y * distance * distanceRatio);
			float angle = distanceRatio * 360f * Mathf.Deg2Rad;
			position += new Vector3(toTargetPerp.x * Mathf.Sin(angle) * waveDistance, toTargetPerp.y * Mathf.Sin(angle) * waveDistance, 0f);
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

	public void SetTargettingLineProgress(float _middleTime)
	{
		float preTime = _middleTime - 0.2f;
		float postTime = _middleTime + 0.2f;
		if (preTime < 0f)
		{
			preTime = 0f;
		}
		if (postTime > 1f)
		{
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
	public void FailedTarget()
	{
		gameManager.NormalizeTime();
		SpawnBomb(bombPrefab, RandomPosition(5f, 5f));
		shooting = false;

		foreach (Transform child in gameManager.TargettingNodes)
		{
			Destroy(child.gameObject);
		}

		TargettingLine.gameObject.SetActive(false);
	}

	/// <summary>
	/// Callback for when the player selects the final node
	/// </summary>
	public void FinalizeTarget()
	{
		gameManager.NormalizeTime();
		SpawnBomb(bombPrefab, targetLocation);
		shooting = false;

		foreach (Transform child in gameManager.TargettingNodes)
		{
			Destroy(child.gameObject);
		}

		TargettingLine.gameObject.SetActive(false);
	}

	/// <summary>
	/// The world space position of the mouse
	/// </summary>
	public Vector3 GetMousePos()
	{
		Vector2 mouse = Mouse.current.position.ReadValue();
		Vector3 point = mainCamera.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 0f)); //mainCamera.nearClipPlane));
		point.z = 0f;
		return point;
	}

	/// <summary>
	/// Returns a random position within the given bounds.
	/// </summary>
	public Vector3 RandomPosition(float _xDistance, float _yDistance)
	{
		float x = Random.Range(-_xDistance, _xDistance);
		float y = Random.Range(-_yDistance, _yDistance);
		return new Vector3(x, y, 0f);
	}

	/// <summary>
	/// Thanks! https://gamedev.stackexchange.com/questions/139515/lookrotation-make-x-axis-face-the-target-instead-of-z
	/// </summary>
	Quaternion XLookRotation(Vector3 right, Vector3 up)
	{
		Quaternion rightToForward = Quaternion.Euler(0f, -90f, 0f);
		Quaternion forwardToTarget = Quaternion.LookRotation(right, up);

		return forwardToTarget * rightToForward;
	}
}
