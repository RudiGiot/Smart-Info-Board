using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
using System.IO;
using UnityEngine.UI;

public class DisplayVideo : MonoBehaviour {

	void Start()
	{
		StartCoroutine(PlayVideo());
	}

	IEnumerator PlayVideo()
	{
		GameObject screen = GameObject.Find("Screen");
		VideoPlayer screenVideoPlayer = screen.GetComponent<VideoPlayer>();

		string fileName="";
		//GameObject titleText = GameObject.Find("Title");
		//titleText.GetComponent<Text>().text = Application.dataPath.Substring(0, Application.dataPath.Length-22);
		string path = Application.dataPath;
		if (Application.platform == RuntimePlatform.OSXPlayer) {
			fileName = getVideoFileNameFromDirectory(Application.dataPath.Substring(0, Application.dataPath.Length-27) + "Video/");
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer) {
			//fileName = getVideoFileNameFromDirectory(Application.dataPath.Substring(0, Application.dataPath.Length-8) + "Video/");
		}
		else if (Application.platform == RuntimePlatform.OSXEditor) {
			fileName = getVideoFileNameFromDirectory(Application.dataPath.Substring(0, Application.dataPath.Length-6) + "Video/");
		}
		if(fileName!="")
		{
			screenVideoPlayer.url = fileName;
			//screenVideoPlayer.frame = 100;
			screenVideoPlayer.isLooping = false;
			while (!screenVideoPlayer.isPrepared)
			{
				//Preparing Video
				yield return null;
			}
			//Texture2D tex = new Texture2D(screenVideoPlayer.texture.width, screenVideoPlayer.texture.height);
			//screen.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, screenVideoPlayer.texture.width, screenVideoPlayer.texture.height), new Vector2(0.5f, 0.5f), 100.0f);
			screenVideoPlayer.Play();
			GameObject status = GameObject.Find("Status");
			status.GetComponent<SceneStatus>().readyToOpen = true;
		}
	}

	public static string getVideoFileNameFromDirectory(string targetDirectory) 
	{
		string fileNameToReturn="";
		Debug.Log(targetDirectory);
		string [] fileEntries = Directory.GetFiles(targetDirectory, "*.mp4");
		System.Random randomGenerator = new System.Random();
		if (fileEntries.Length!=0) fileNameToReturn = fileEntries[randomGenerator.Next(0, fileEntries.Length)];
		return fileNameToReturn;
	}

}
