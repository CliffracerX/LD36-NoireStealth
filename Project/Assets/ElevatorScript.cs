using UnityEngine;
using System.Collections;

public class ElevatorScript : MonoBehaviour
{
	public float lerpTime,lerpSpeed;
	public Vector3 startPos,goalPos;
	public float minX,minY,minZ,maxX,maxY,maxZ;

	void Start()
	{
		startPos=goalPos=transform.localPosition;
	}

	void FixedUpdate()
	{
		lerpTime+=lerpSpeed*Time.deltaTime;
		Vector3 pos = Vector3.Lerp(startPos, goalPos, lerpTime);
		pos.x=Mathf.Clamp(pos.x, minX, maxX);
		pos.y=Mathf.Clamp(pos.y, minY, maxY);
		pos.z=Mathf.Clamp(pos.z, minZ, maxZ);
		transform.localPosition=pos;
	}

	public void Move(string input)
	{
		lerpTime=0;
		startPos=transform.localPosition;
		string[] split = input.Split('|');
		Vector3 parsedPos = new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
		print("X1: "+parsedPos.x);
		print("Y1: "+parsedPos.y);
		print("Z1: "+parsedPos.z);
		parsedPos+=startPos;
		parsedPos.x=Mathf.Clamp(parsedPos.x, minX, maxX);
		parsedPos.y=Mathf.Clamp(parsedPos.y, minY, maxY);
		parsedPos.z=Mathf.Clamp(parsedPos.z, minZ, maxZ);
		print("X2: "+parsedPos.x);
		print("Y2: "+parsedPos.y);
		print("Z2: "+parsedPos.z);
		goalPos=parsedPos;
	}
}