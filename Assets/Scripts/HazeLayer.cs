using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazeLayer : MonoBehaviour
{
    [SerializeField] Transform follow;
    public float ParallaxFactor = 0f;
 
	Vector3 theDimension;
 
	Vector3 theStartPosition;
 
	void Start () 
	{
		theStartPosition = transform.position;
 
		theDimension = GetComponent<Renderer>().bounds.size;
	}
 
	void Update ()
	{
        Vector3 newPos = theStartPosition + follow.position * ParallaxFactor;
        transform.position = newPos;

        if (transform.position.y - theDimension.y / 2 > Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).y)
        {
            var offset = Vector3.up * -theDimension.y * 2;
            theStartPosition += offset;
            transform.position += offset;
        }
	}
}
