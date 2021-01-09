﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetNode : MonoBehaviour
{
	private float timeLeft;
	private float startingScale;
	public Cannon MyCannon;
	public float TimeToSelect;
	public TargetNode NextNode;

	// Start is called before the first frame update
	void Start()
	{
		startingScale = transform.localScale.x;
		timeLeft = TimeToSelect;
	}

	// Update is called once per frame
	void Update()
	{
		timeLeft -= Time.unscaledDeltaTime;
		if (timeLeft <= 0f)
		{
			MyCannon.FailedTarget();
		}
		//UpdateSize();
	}

	/// <summary>
	/// Keep the size of the node accurate with how much time is left.
	/// </summary>
	private void UpdateSize()
	{
		float ratio = timeLeft / TimeToSelect;
		if (ratio < 0f)
		{
			ratio = 0f;
		}
		float newScale = Mathf.Lerp(0f, startingScale, ratio);
		transform.localScale = new Vector3(newScale, newScale, newScale);
	}

	//private void OnTriggerEnter(Collider _other)
	//{
	//	CursorCollision(_other);
	//}

	private void OnTriggerStay(Collider _other)
	{
		CursorCollision(_other);
	}

	/// <summary>
	/// If no next node was configured, this is the last one, finalize it with the cannon so it spawns the bomb.
	/// If not, activate the next node for the player to target.
	/// </summary>
	private void CursorCollision(Collider _other)
	{
		if (_other.CompareTag("Cursor") && Input.GetKey(KeyCode.Mouse0))
		{
			if (NextNode == null)
			{
				MyCannon.FinalizeTarget();
			}
			else
			{
				NextNode.SetActive(true);
				Destroy(gameObject);
				MyCannon.SetTargettingLineProgress(MyCannon.TargettingLineProgress + MyCannon.TargettingLineProgressAmount);
			}
		}
	}

    private void SetActive(bool v)
    {
        gameObject.SetActive(true);
		GetComponent<SpriteRenderer>().enabled = true;
    }
}
