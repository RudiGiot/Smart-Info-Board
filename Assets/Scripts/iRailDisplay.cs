using System.Collections;
using UnityEngine;
using iRailResponse;
using UnityEngine.UI;
using System.Globalization;
using System;

public class iRailDisplay : MonoBehaviour {

	int tryNumber = 0;
    public int maxLine = 10;

	public string url;

	void Start () {
		StartCoroutine(iRail());
	}

	public IEnumerator iRail()
	{
		WWW www = new WWW(url);
		yield return www;
		//Debug.Log(www.text);
		iRailFeed XMLFeed = new iRailFeed(www.text);
		if (XMLFeed.liveBoard.Departures.Departure.Count!=0) {
            int i = 0;
            while (i < maxLine && i<XMLFeed.liveBoard.Departures.Departure.Count)
            {
                GameObject timeUI = GameObject.Find("Time"+i.ToString());
                DateTime departureTime = DateTime.ParseExact(XMLFeed.liveBoard.Departures.Departure[i].Time.Formatted, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture);
                timeUI.GetComponent<Text>().text = departureTime.Hour.ToString().PadLeft(2, '0') + ":" + departureTime.Minute.ToString().PadLeft(2, '0');
                Debug.Log(XMLFeed.liveBoard.Departures.Departure[i].Time.Formatted);
                GameObject destinationUI = GameObject.Find("Destination"+ i.ToString());
                destinationUI.GetComponent<Text>().text = XMLFeed.liveBoard.Departures.Departure[i].Station.Standardname;
                GameObject trackUI = GameObject.Find("Track"+ i.ToString());
                trackUI.GetComponent<Text>().text = XMLFeed.liveBoard.Departures.Departure[i].Platform.Text.PadLeft(2, '0');
                GameObject remarkUI = GameObject.Find("Remark"+ i.ToString());
                remarkUI.GetComponent<Text>().text = "";
                if (XMLFeed.liveBoard.Departures.Departure[i].Canceled == "1")
                {
                    remarkUI.GetComponent<Text>().text = "Canceled";
                }
                else if (XMLFeed.liveBoard.Departures.Departure[i].Delay != "0")
                {
                    remarkUI.GetComponent<Text>().text = "Delayed " + (int.Parse(XMLFeed.liveBoard.Departures.Departure[i].Delay) / 60).ToString() + "'";
                }
                GameObject status = GameObject.Find("Status");
                status.GetComponent<SceneStatus>().readyToOpen = true;
                i++;
            }
		}
	}
}
