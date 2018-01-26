using UnityEngine;
using UnityEngine.UI;
using System;

public class DisplayClock : MonoBehaviour {

    private string[] monthWord = { "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre" };

	// Use this for initialization
	void Start () {
		draw();
		GameObject status = GameObject.Find("Status");
		status.GetComponent<SceneStatus>().readyToOpen = true;
	}
	
	// Update is called once per frame
	void Update () {
		draw();
	}

	void draw() {
		DateTime time = DateTime.Now;
		String hour = time.Hour.ToString().PadLeft(2,'0');
		String minute = time.Minute.ToString().PadLeft(2,'0');
		String second = time.Second.ToString().PadLeft(2,'0');
		String day = time.Day.ToString().PadLeft(2,'0');
        int month = int.Parse(time.Month.ToString().PadLeft(2,'0'));
		String year = time.Year.ToString().PadLeft(2,'0');
		GameObject date = GameObject.Find("Date");
		date.GetComponent<Text>().text = day + " " + monthWord[month-1] + " " + year;
		GameObject hourObject = GameObject.Find("Hour");
		hourObject.GetComponent<Text>().text = hour+ "  " + minute + "  " + second;
	}
}
