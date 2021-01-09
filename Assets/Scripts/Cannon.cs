using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
	private Camera mainCamera;
	[SerializeField] GameManager gameManager;
	private bool shooting;
	private Vector3 targetLocation;

	[Header("Settings")]
	private float coolDownTime;
	private float coolDownTimer;

	[Header("Prefabs")]
	[SerializeField] private GameObject bombPrefab;
	[SerializeField] private GameObject targetNodePrefab;

	// Start is called before the first frame update
	void Start()
	{
		mainCamera = Camera.main;
		gameManager = transform.parent.GetComponent<GameManager>();
	}

	// Update is called once per frame
	void Update()
	{
		UpdateInput();
		UpdateTimers();
	}

	/// <summary>
	/// Checks for mouse input to begin the bomb teleportation process.
	/// </summary>
	private void UpdateInput()
	{
		// Place bombs
		if (Input.GetKeyDown(KeyCode.Mouse0) && coolDownTimer <= 0f && !shooting)
		{
			coolDownTimer = coolDownTime;
			shooting = true;
			targetLocation = GetMousePos();
			gameManager.SlowDownTime();
			BeginTargetting();
		}

		// Look at the mouse
		Vector3 toMouse = (GetMousePos() - transform.position).normalized;
		transform.rotation = XLookRotation(toMouse, Vector3.up);
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
		Vector3 toTarget = targetLocation - transform.position;
		float distance = toTarget.magnitude;
		float waveDistance = 1f;
		toTarget.Normalize();
		Vector3 toTargetPerp = Vector2.Perpendicular(toTarget).normalized;

		TargetNode firstNode = null;
		TargetNode lastNode = null;
		int numNodes = 5;
		for (int i = 0; i < numNodes; i++)
		{
			TargetNode node = Instantiate(targetNodePrefab, gameManager.TargettingNodes).GetComponent<TargetNode>();
			node.TimeToSelect = 2f;
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
		}

		firstNode.TimeToSelect = 3f;
		firstNode.transform.position = transform.position; // So we always begin by hovering over the cannon
		lastNode.transform.position = targetLocation; // So the final one is always over the target
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
	}

	/// <summary>
	/// The world space position of the mouse
	/// </summary>
	public Vector3 GetMousePos()
	{
		Vector2 mouse = Input.mousePosition;
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
