using UnityEngine;
using System.Collections;

public class ExitArea : MonoBehaviour
{
	public int bonusPapersToGrab;
	public int levelID;

	void OnTriggerEnter(Collider other)
	{
		if(other.name=="FPSController")
		{
			int score = 10;
			//int scoreLoss = (int)((CameraScript.instance.bonusPapersLeft)/bonusPapersToGrab);
			//score-=scoreLoss;
			SaveManager.instance.CompleteLevel(levelID, score);
		}
	}
}