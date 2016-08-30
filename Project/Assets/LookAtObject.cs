using UnityEngine;
using System.Collections;

public class LookAtObject : MonoBehaviour
{
	public Transform transToWatch;
	public float range = 25f;

	void FixedUpdate()
	{
		if(Vector3.Distance(transform.position, transToWatch.position)<=range)
		{
			transform.LookAt(transToWatch.position);
		}
	}
}