using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System;
using System.IO;
using System.Diagnostics;

public class Widget {
	public string name;
	public int probabibilty;
	public int duration;

	public Widget(string _name, int _probability, int _duration) {
		name = _name;
		probabibilty = _probability;
		duration = _duration;
	}
}


public class Scheduling {
	public float start;
	public float end;
	public List<Widget> widget;

	public Scheduling(float _start, float _end) {
		start = _start;
		end = _end;
		widget = new List<Widget>();
	}
}

public class SceneScheduler : MonoBehaviour {

	Image img;
	float alpha;
	float shutdownTimeFloat;
	List<Scheduling> scheduling;
	List<Widget> widgetToPickIn;
	public float fadeTime = 3f;
	public int defaultDuration = 20;
    Widget defaultWidget;

	void Start () {
		defaultWidget = new Widget("Clock", 10, defaultDuration);
		LoadConfig();
		StartCoroutine(LoadScene(defaultWidget));
		Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

	IEnumerator LoadScene (Widget scene) {
		DateTime time = DateTime.Now;
		int hour = time.Hour;
		int minute = time.Minute;
		float currentTime = hour + minute/60f;
		if(shutdownTimeFloat<=currentTime) {
			string path = Application.dataPath;
			string fileName="";
			if (Application.platform == RuntimePlatform.OSXPlayer) {
				ProcessStartInfo process = new ProcessStartInfo();
				process.FileName = "osascript";
				process.Arguments = string.Format("-e 'tell application \"System Events\" to shut down'");
				process.UseShellExecute = false;
				Process.Start(process);
			}
			else if (Application.platform == RuntimePlatform.WindowsPlayer) {
				
			}
			else if (Application.platform == RuntimePlatform.OSXEditor) {
				UnityEngine.Debug.Log("Faking Shutting Down ....");
			}
			Application.Quit();
		}
		else {
			SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Additive);
			yield return new WaitForSeconds(fadeTime);
			GameObject status = GameObject.Find("Status");
			if (status.GetComponent<SceneStatus>().readyToOpen == true)	{
				StartCoroutine(PanelFadeOut());
				StartCoroutine(CloseScene(scene.name, scene.duration));
			}
			else {
				// load default scene
	            SceneManager.UnloadSceneAsync(scene.name);
	            SceneManager.LoadSceneAsync(defaultWidget.name, LoadSceneMode.Additive);
	            StartCoroutine(PanelFadeOut());
	            StartCoroutine(CloseScene(defaultWidget.name, defaultWidget.duration));
			}
		}
	}

	IEnumerator CloseScene (string sceneName, int delay) {
		yield return new WaitForSeconds(delay);
		StartCoroutine(PanelFadeIn());
		yield return new WaitForSeconds(fadeTime);
		SceneManager.UnloadSceneAsync(sceneName);
		yield return new WaitForSeconds(1f);
		StartCoroutine(LoadScene(PickAScene(sceneName)));
	}


	IEnumerator PanelFadeOut() {
		img =  GameObject.Find("Panel").GetComponent<Image>();
		alpha = 1f;
		while(alpha>0f) {
			img.color = new Color(0f,0f,0f,alpha);
            alpha = alpha - Time.deltaTime / fadeTime;
			yield return true;
		}
		img.color = new Color(0f,0f,0f,0f);
	}

	IEnumerator PanelFadeIn() {
		img =  GameObject.Find("Panel").GetComponent<Image>();
		alpha = 0f;
		while(alpha<1f) {
			img.color = new Color(0f,0f,0f,alpha);
            alpha = alpha + Time.deltaTime / fadeTime;
			yield return true;
		}
		img.color = new Color(0f,0f,0f,1f);
	}


	Widget PickAScene(string previousSceneName){
		//Build the Widget List available at the moment
		widgetToPickIn = new List<Widget>();
		DateTime time = DateTime.Now;
		int hour = time.Hour;
		int minute = time.Minute;
		float hourMinute = hour + minute/60f;
		foreach (Scheduling schedulingElement in scheduling)
		{
			//Debug.Log(schedulingElement.start.ToString() + " - " + schedulingElement.end.ToString() + " : ");
			if ((hourMinute>=schedulingElement.start)&&(hourMinute<=schedulingElement.end)) {
				foreach (Widget widgetElement in schedulingElement.widget)
				{
					// Add th widget in the list if it's not the last loaded (previousSceneName)
					if (widgetElement.name!=previousSceneName) {
						for(int i=0; i<widgetElement.probabibilty; i++) widgetToPickIn.Add(widgetElement);
						//Debug.Log(widgetElement.name + " : " + widgetElement.duration.ToString() + ", " + widgetElement.probabibilty.ToString());
					}
				}
			}
		}
		// Pick a widget in the list
		//Debug.Log("Number to pick : " + widgetToPickIn.Count.ToString());
		if (widgetToPickIn.Count==0) {
			return defaultWidget;
		}
		else {
			System.Random randomGenerator = new System.Random();
			int randomWidget = randomGenerator.Next(0, widgetToPickIn.Count);
			return widgetToPickIn[randomWidget];
		}
	}

	// Update is called once per frame
	void LoadConfig () {
		bool fileLoaded = true;
		scheduling = new List<Scheduling>();
		XmlDocument schedulingXml = new XmlDocument ();
		//TextAsset textAsset = (TextAsset)Resources.Load("Scripts/scheduling.xml", typeof(TextAsset));
		//Debug.Log(textAsset.text);
		//schedulingXml.LoadXml(textAsset.text);
		try
		{
			schedulingXml.Load("./Config/scheduling.xml");
			}
		catch(Exception e)
		{
			fileLoaded = false;
			//Debug.Log(e.ToString());
		}
		//Debug.Log(schedulingXml);
		if (fileLoaded) {
			XmlNode schedulingRoot = schedulingXml.DocumentElement;
			string shutdownTimeString = ((XmlElement)schedulingRoot).GetAttribute("end");
			shutdownTimeFloat = float.Parse(shutdownTimeString.Substring(0,2)) + (float.Parse(shutdownTimeString.Substring(3,2)) / 60f);
			UnityEngine.Debug.Log(shutdownTimeFloat);

			string defaultWidgetName = ((XmlElement)schedulingRoot.SelectSingleNode("default").FirstChild).GetAttribute("name");
			defaultWidget = new Widget(defaultWidgetName, 5, defaultDuration);

			XmlNodeList periodList = schedulingRoot.SelectNodes ("period");
			foreach (XmlElement periodElement in periodList)
			{
				string periodStartString = periodElement.GetAttribute("start");
				float periodStartFloat = float.Parse(periodStartString.Substring(0,2)) + (float.Parse(periodStartString.Substring(3,2)) / 60f);
				string periodEndString = periodElement.GetAttribute("end");
				float periodEndFloat = float.Parse(periodEndString.Substring(0,2)) + (float.Parse(periodEndString.Substring(3,2)) / 60f);
				scheduling.Add(new Scheduling(periodStartFloat, periodEndFloat));
				XmlNodeList widgetList = periodElement.SelectNodes ("widget");
				foreach (XmlElement widgetElement in widgetList)
				{
					string widgetName = widgetElement.GetAttribute("name");
					int widgetProbability = int.Parse(widgetElement.GetAttribute("probability"));			
					int widgetDuration = int.Parse(widgetElement.GetAttribute("duration"));
					scheduling[scheduling.Count-1].widget.Add(new Widget(widgetName, widgetProbability, widgetDuration));
				}
			}
		}
	}

	public static string getShutdownScriptFileNameFromDirectory(string targetDirectory) 
	{
		string fileNameToReturn="";
		UnityEngine.Debug.Log(targetDirectory);
		string [] fileEntries = Directory.GetFiles(targetDirectory, "*.scpt");
		if (fileEntries.Length!=0) fileNameToReturn = fileEntries[0];
		return fileNameToReturn;
	}
}
