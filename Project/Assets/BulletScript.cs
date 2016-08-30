using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{
	public bool fired = false;

	void OnTriggerEnter(Collider other)
	{
		if(!fired)
		{
			other.gameObject.SendMessage("Die");
			fired=true;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		fired=true;
	}
}