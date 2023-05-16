using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using fNbt;
using Level_Editor_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class ButtonFunctions : MonoBehaviour
{
	private Level level;

	private Timers timers;

	private CanvasManager canvasManager;

	private PlayerAudioManager audio;

	public AudioClip buttonHover;

	public AudioClip buttonClick;

	private void Start()
	{
		audio = Game.OnStartResolve<PlayerAudioManager>();
		canvasManager = Game.OnStartResolve<CanvasManager>();
		level = Game.OnStartResolve<Level>();
		timers = Game.OnStartResolve<Timers>();
		Debug.Log(timers);
	}

	public void RestartLevel()
	{
		level.RestartLevel();
	}

	public void NextLevel()
	{
		level.NextLevel();
	}

	public void Startlevel(string name)
	{
		Game.StartLevel(name);
	}

	public void MainMenu()
	{
		SceneManager.LoadScene(0);
		timers.ResetFullGameRun();
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void OpenOptions()
	{
		canvasManager.OpenMenu(canvasManager.Options);
	}

	public void OpenChapter1Select()
	{
		canvasManager.OpenMenu(canvasManager.Chapter1Select);
	}

	public void OpenReplays()
	{
		canvasManager.OpenMenu(canvasManager.Replays);
	}
	
	public void OpenMaps()
	{
		canvasManager.OpenMenu(canvasManager.CustomMap);
	}

	public void OpenReplaysFolder()
	{
		string text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Lightstrafe\\replays";
		Directory.CreateDirectory(text);
		Process.Start("explorer.exe", text);
	}

	public void OpenMapsFolder()
	{
		string text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Lightstrafe\\maps";
		Directory.CreateDirectory(text);
		Process.Start("explorer.exe", text);
	}

	public void DeleteAllReplays()
	{
		string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Lightstrafe\\replays";
		for (int i = 0; i < Directory.GetFiles(path).Length; i++)
		{
			File.Delete(Directory.GetFiles(path)[i]);
		}
	}

	public void ResetTimes()
	{
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
			if (PlayerPrefs.HasKey("BestTime" + fileNameWithoutExtension))
			{
				PlayerPrefs.DeleteKey("BestTime" + fileNameWithoutExtension);
			}
		}
		
	}

	public void ResetModdedTimes()
	{
		string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Lightstrafe\\maps";
		Directory.CreateDirectory(path);
		string[] files = Directory.GetFiles(path);
		foreach (string text in files)
		{
			NbtFile file = new NbtFile(text);
			string name = file.RootTag.Get<NbtString>("MapName").Value;
			Debug.Log(name);
			if (PlayerPrefs.HasKey("BestTime" + name))
			{
				PlayerPrefs.DeleteKey("BestTime" + name);
			}
		}	}

	public void ResetBinds()
	{
		PlayerInput.ResetBindsToDefault();
	}

	public void PlayButtonHover()
	{
		audio.PlayAudioDeferred(buttonHover, looping: false, 0.4f, ignoreTimescale: true);
		SessionManager.LockEditor = true;
	}

	public void StopButtonHover()
	{
		SessionManager.LockEditor = false;
	}
	
	public void PlayButtonClick()
	{
		audio.PlayOneShotDeferred(buttonClick, looping: false, 1f, ignoreTimescale: true);
	}
	
	public void OpenMapEditor()
	{
		SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
	}
}
