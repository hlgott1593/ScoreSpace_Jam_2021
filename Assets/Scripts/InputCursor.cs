using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCursor : MonoBehaviour
{
	private Cannon cannon;

	// Start is called before the first frame update
	void Start()
	{
		cannon = transform.parent.GetComponent<Cannon>();
	}

	// Update is called once per frame
	void Update()
	{
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		transform.position = cannon.GetMousePos();
	}
}
