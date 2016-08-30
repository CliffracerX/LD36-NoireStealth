using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

[System.Serializable]
public class ProgramEffect
{
	public string stringA,stringB,stringC,stringD;
	public GameObject objA,objB;
	public bool boolA;
	public enum Effect {Print=0, ObjToggle=1, UnlockProgram=2, UnlockAll=3, RunBehaviour=4, AlterPath=5}
	public Effect e;
}

[System.Serializable]
public class Program
{
	public string command,lockMessage;
	public ProgramEffect[] effects;
	public bool unlocked = true;
	public bool freePlayer = false;
}

public class ConsoleScript : MonoBehaviour
{
	public RenderTexture RT;
	public GUIStyle textStyle;
	public Texture2D screenBase;
	int framesTillRTReset = 1;
	public int currentSlot;
	public string currentInput,log,path;
	public float ticksUntilSwitch;
	public bool consoleThingy,currentlyFocused;
	public FirstPersonController fpc;
	public int displayableLines;
	public static int numComputers,currentComputer;
	public int thisComputer;
	public Program[] progArray;
	public Dictionary<string, Program> programs;

	void Start()
	{
		RT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
		tempTex = new Texture2D(400, 300);
		this.thisComputer=ConsoleScript.numComputers;
		ConsoleScript.numComputers+=1;
		path = " > ";
		programs = new Dictionary<string, Program>();
		foreach(Program prog in progArray)
		{
			programs.Add(prog.command, prog);
		}
		this.GetComponent<MeshRenderer>().materials[1].mainTexture=tempTex;
		this.GetComponent<MeshRenderer>().materials[1].SetTexture("_EmissionMap", tempTex);
	}

	void RunProgram(Program p, string input)
	{
		if(p.freePlayer)
		{
			this.currentlyFocused=false;
			fpc.enabled=true;
		}
		if(p.unlocked)
		{
			foreach(ProgramEffect pe in p.effects)
			{
				if(pe.e==ProgramEffect.Effect.Print)
				{
					log+=pe.stringA.Replace("\\n", "\n")+"\n";
				}
				if(pe.e==ProgramEffect.Effect.ObjToggle)
				{
					pe.objA.SetActive(pe.boolA);
				}
				if(pe.e==ProgramEffect.Effect.UnlockProgram)
				{
					if(input.Split(' ')[1]==pe.stringA)
					{
						programs[pe.stringB].unlocked=pe.boolA;
						log+=pe.stringD.Replace("\\n", "\n")+"\n";
					}
					else
					{
						log+=pe.stringC.Replace("\\n", "\n")+"\n";
					}
				}
				if(pe.e==ProgramEffect.Effect.UnlockAll)
				{
					if(input.Split(' ')[1]==pe.stringA)
					{
						foreach(Program prog in programs.Values)
						{
							prog.unlocked=pe.boolA;
						}
						log+=pe.stringC.Replace("\\n", "\n")+"\n";
					}
					else
					{
						log+=pe.stringB.Replace("\\n", "\n")+"\n";
					}
				}
				if(pe.e==ProgramEffect.Effect.RunBehaviour)
				{
					pe.objA.SendMessage(pe.stringA, pe.stringB);
				}
				if(pe.e==ProgramEffect.Effect.AlterPath)
				{
					path = pe.stringA;
				}
			}
		}
		else
		{
			log+=p.lockMessage.Replace("\\n", "\n")+"\n";
		}
	}

	void RunFunction(string input)
	{
		log+=path+input+"\n";
		//print(input.Trim());
		if(programs.ContainsKey(input))
		{
			Program p = programs[input];
			RunProgram(p, input);
		}
		else if(programs.ContainsKey(input.Split(' ')[0]))
		{
			Program p = programs[input.Split(' ')[0]];
			RunProgram(p, input);
		}
	}

	public Texture2D tempTex;

	public void OnPostRender()
	{

	}

	void Update()
	{
		ticksUntilSwitch -= Time.deltaTime;
		if(ticksUntilSwitch<=0)
		{
			ticksUntilSwitch=0.5f;
			consoleThingy=!consoleThingy;
		}
		if(currentlyFocused && Input.GetButtonUp("Pause"))
		{
			currentlyFocused=false;
			fpc.enabled=true;
		}
		if(currentlyFocused)
		{
			if(Input.inputString=="\b")
			{
				currentInput=currentInput.Remove(currentInput.Length-1);
				fpc.clickSource.clip=fpc.keyClickBackspace;
				fpc.clickSource.Play();
			}
			else if(Input.inputString=="\n")
			{
				//print("WARNING: THREAT DETECTED");
				fpc.clickSource.clip=fpc.keyClickEnter;
				fpc.clickSource.Play();
			}
			else if(Input.GetKeyUp(KeyCode.Return))
			{
				RunFunction(currentInput.Trim());
				currentInput="";
				fpc.clickSource.clip=fpc.keyClickEnter;
				fpc.clickSource.Play();
			}
			else
			{
				currentInput+=Input.inputString;
				if(Input.inputString!="")
				{
					fpc.clickSource.clip=fpc.keyClickDefault;
					fpc.clickSource.Play();
				}
			}
		}
	}

	//These three functions courtesy of AngryAnt
	//http://angryant.com/2013/07/17/OnRenderTextureGUI/
	//I'm using these in the hopes of convincing Unity to stop rescaling the GUI

	RenderTexture m_PreviousActiveTexture = null;
	
	
	protected void BeginRenderTextureGUI (RenderTexture targetTexture)
	{
		if (Event.current.type == EventType.Repaint)
		{
			m_PreviousActiveTexture = RenderTexture.active;
			if (targetTexture != null)
			{
				RenderTexture.active = targetTexture;
				GL.Clear (false, true, new Color (0.0f, 0.0f, 0.0f, 0.0f));
			}
		}
	}

	protected void EndRenderTextureGUI ()
	{
		if (Event.current.type == EventType.Repaint)
		{
			if(RT.width!=Screen.width || RT.height!=Screen.height)
			{
				RT.DiscardContents();
				RT.Release();
				RT.width=Screen.width;
				RT.height=Screen.height;
				RT.Create();
			}
			else
			{
				m_PreviousActiveTexture=RenderTexture.active;
				RenderTexture.active=RT;
				//Destroy(tempTex);
				//tempTex = new Texture2D(800, 600);
				tempTex.ReadPixels(new Rect(0, 0, 400, 300), 0, 0);
				tempTex.Apply();
				RenderTexture.active=m_PreviousActiveTexture;
				//print("DONE?!");
			}
			RenderTexture.active = m_PreviousActiveTexture;
		}
	}

	void OnGUI()
	{
		BeginRenderTextureGUI(RT);
		GUI.DrawTexture(new Rect(0, 0, 400, 300), screenBase);
		string thingy = "_";
		if(consoleThingy)
		{
			thingy="█";
		}
		string curIn = currentInput;
		string tempP = path;
		string l = log;
		/*if(path!="")
		{
			currentSlot=l.Length;
			thingy=tempP+curIn+thingy;
		}
		else
		{
			thingy="|";
		}*/
		string l2 = l;
		//l2=log.Insert(currentSlot, thingy);
		string[] lines = l2.Split("\n"[0]);
		int minLine, maxLine, currentLine;
		currentLine=lines.Length;
		//print(currentLine);
		minLine=currentLine-(displayableLines);
		if(minLine<0)
			minLine=0;
		maxLine=currentLine+(displayableLines/2);
		if(maxLine>lines.Length)
			maxLine=lines.Length;
		l2="";
		for(int i = minLine; i<maxLine; i++)
		{
			if(i==maxLine-1)
			{
				l2+=tempP;
				l2+=curIn;
				l2+=thingy;
			}
			l2+=lines[i]+"\n";
		}
		l=l2;
		thingy="";
		tempP="";
		curIn="";
		GUI.Label(new Rect(30, 15, 370, 285), l+tempP+curIn+thingy, textStyle);
		EndRenderTextureGUI();
	}
}