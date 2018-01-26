using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class DisplayAnnouncement : MonoBehaviour {

	void Start () {
		string fileToLoad = ProcessDirectory("./Announcement/");
		if(fileToLoad!="") 
		{
			Texture2D tex = LoadPNG(fileToLoad);
			Sprite imageToDisplay = Sprite.Create(tex, new Rect(0, 0, 1920, 1080), new Vector2(0.5f,0.5f), 100);
			GameObject backGroundImage = GameObject.Find("AnnouncementPanel");
			backGroundImage.GetComponent<UnityEngine.UI.Image>().sprite = imageToDisplay;
			GameObject status = GameObject.Find("Status");
			status.GetComponent<SceneStatus>().readyToOpen = true;
		}
	}

	public Texture2D LoadPNG(string filePath) {
		Texture2D tex = null;
		byte[] fileData;
		if (File.Exists(filePath))     {
			fileData = File.ReadAllBytes(filePath);
			tex = new Texture2D(2, 2);
			tex.LoadImage(fileData);
		}
		return tex;
	}

	public static string ProcessDirectory(string targetDirectory) 
	{
		string fileNameToReturn="";
		List<string> availableFile = new List<string>();
		string [] fileEntries = Directory.GetFiles(targetDirectory, "*.jpg");
		foreach(string fileName in fileEntries)
		{
			//Debug.Log(fileName);
			DateTime startDate = new DateTime(Int32.Parse(fileName.Substring(15, 4)), Int32.Parse(fileName.Substring(20, 2)), Int32.Parse(fileName.Substring(23, 2)));
			DateTime endDate = new DateTime(Int32.Parse(fileName.Substring(26, 4)), Int32.Parse(fileName.Substring(31, 2)), Int32.Parse(fileName.Substring(34, 2)));
			if ((DateTime.Now>startDate)&&(DateTime.Now<endDate)) availableFile.Add(fileName);
		}
		System.Random randomGenerator = new System.Random();
		if (availableFile.Count!=0) fileNameToReturn = availableFile[randomGenerator.Next(0, availableFile.Count)];
		return fileNameToReturn;
	}

}
