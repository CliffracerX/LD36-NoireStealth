using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerVisibilityHandler : MonoBehaviour
{
	public float visibilityPercent;
	public Texture2D texture;
	public RenderTexture rendText;
	public Color[] visibilityWarningColors;
	public string[] visibilityWarningNames;
	public FirstPersonController fpc;
	public GUIStyle style;
	public RenderTexture rt;

	void Start()
	{
		texture = new Texture2D(1, 1);
	}

	void Update()
	{
		RenderTexture tempTex = RenderTexture.active;
		RenderTexture.active = rendText;
		texture.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
		RenderTexture.active = tempTex;
		Color col = texture.GetPixel(0, 0);
		visibilityPercent = 0;
		visibilityPercent += col.r;
		visibilityPercent += col.g;
		visibilityPercent += col.b;
		visibilityPercent /=  3;
		fpc.visibility=visibilityPercent;
	}

	void OnGUI()
	{
		RenderTexture tempRT = RenderTexture.active;
		RenderTexture.active=rt;
		int warningMode = 4;
		if(visibilityPercent>=0.8f)
			warningMode = 4;
		else if(visibilityPercent<=0.8f && visibilityPercent>0.6f)
			warningMode = 3;
		else if(visibilityPercent<=0.6f && visibilityPercent>0.4f)
			warningMode = 2;
		else if(visibilityPercent<=0.4f && visibilityPercent>0.2f)
			warningMode = 1;
		else if(visibilityPercent<=0.2f)
			warningMode = 0;
		GUI.color = visibilityWarningColors[warningMode];
		GUI.Label(new Rect(15, 15, 240, 24), visibilityWarningNames[warningMode], style);
		GUI.color = Color.white;
		RenderTexture.active=tempRT;
	}
}