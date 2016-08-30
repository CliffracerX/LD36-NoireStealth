using UnityEngine;
using System.Collections;

public class PrinterScript : MonoBehaviour
{
	public GameObject paperPrefab;
	public float printingTimeLeft;
	public bool isPrinting;
	public string printType;
	public Transform outTrans;
	public AudioSource printingSound;

	void Update()
	{
		if(isPrinting)
		{
			printingTimeLeft-=Time.deltaTime;
		}
		printingSound.enabled=isPrinting;
		if(printingTimeLeft<=0)
		{
			isPrinting=false;
			printingTimeLeft=99999;
			GameObject go = (GameObject)Instantiate(paperPrefab, outTrans.position, outTrans.rotation);
			go.name=printType;
		}
	}

	public void StartPrinting(string input)
	{
		print("Printing from: "+input);
		isPrinting=true;
	}
}