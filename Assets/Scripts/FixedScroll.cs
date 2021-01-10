using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedScroll : MonoBehaviour
{
    public float theScrollSpeed = 0.025f;
	Transform theCamera;

	void Start () 
	{
	}
	
	void Update ()
	{
		transform.position = new Vector3 ( transform.position.x, transform.position.y + theScrollSpeed, 0);
	}
}
