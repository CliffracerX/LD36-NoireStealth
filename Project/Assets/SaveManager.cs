using UnityEngine;
using System.Collections;

public class SaveManager : MonoBehaviour
{
	public bool[] completedLevels;
	public int[] levelRatings;
	public string[] levelNames;
	public static SaveManager instance;
	public string[] levelLoadCommands,levelCodenames;
	public ConsoleScript console;
	public int[] levelSceneIds;
	public int musVol = 1;

	public void LoadLevel(string level)
	{
		int whichLevel = int.Parse(level);
		Application.LoadLevel(levelSceneIds[whichLevel]);
	}

	void Start()
	{
		SaveManager.instance = this;
		for(int i = 0; i<levelNames.Length; i++)
		{
			if(PlayerPrefs.HasKey(levelNames[i]))
			{
				int value = PlayerPrefs.GetInt(levelNames[i]);
				completedLevels[i]=true;
				levelRatings[i]=value;
			}
			if(i>0)
			{
				if(completedLevels[i-1]==true)
				{
					console.programs[levelLoadCommands[i]].unlocked=true;
					console.programs["loadLevel"].effects[0].stringA+="\n"+levelCodenames[i]+" ("+levelNames[i]+")";
				}
			}
			else
			{
				console.programs[levelLoadCommands[i]].unlocked=true;
				console.programs["loadLevel"].effects[0].stringA+="\n"+levelCodenames[i]+" ("+levelNames[i]+")";
			}
		}
		if(PlayerPrefs.HasKey("Music"))
		{
			musVol=PlayerPrefs.GetInt("Music");
		}
	}

	public void SetMusic(string status)
	{
		if(status=="off")
		{
			musVol=0;
			PlayerPrefs.SetInt("Music", 0);
		}
		else if(status=="on")
		{
			musVol=1;
			PlayerPrefs.SetInt("Music", 1);
		}
	}

	public void CompleteLevel(int id, int score)
	{
		completedLevels[id]=true;
		levelRatings[id]=score;
		Cursor.lockState=CursorLockMode.None;
		PlayerPrefs.SetInt(levelNames[id], score);
		Application.LoadLevel(0);
	}
}