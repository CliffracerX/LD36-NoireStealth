using UnityEngine;
using System.Collections;

public class DecalScript : MonoBehaviour
{
	public Texture2D decalTex;

	void Start()
	{
		this.GetComponent<MeshRenderer>().material.mainTexture=decalTex;
	}
}