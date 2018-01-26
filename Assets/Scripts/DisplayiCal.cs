using System.Collections;
using UnityEngine;
using iCalAPI;
using UnityEngine.UI;
using System;

public class DisplayiCal : MonoBehaviour {


    int tryNumber = 0;
    iCal conferencesCalendar;

    public string url;

    void Start()
    {
        StartCoroutine(getiCal());
    }

    public IEnumerator getiCal()
    {
		WWW www = new WWW(url);
		yield return www;
		iCal conference = new iCal(www.text);
        //Randomize the Event
        int eventNumber = UnityEngine.Random.Range(0, conference.vevent.Count);
        GameObject dateUI = GameObject.Find("Dates");
        conference.vevent[eventNumber].DTEND = conference.vevent[eventNumber].DTEND.Subtract(new System.TimeSpan(1, 0, 0, 0));
        if (conference.vevent[eventNumber].DTSTART==conference.vevent[eventNumber].DTEND){
            dateUI.GetComponent<Text>().text = conference.vevent[eventNumber].DTSTART.Day.ToString().PadLeft(2,'0') + "/" + conference.vevent[eventNumber].DTSTART.Month.ToString().PadLeft(2, '0') + "/" +  conference.vevent[eventNumber].DTEND.Year.ToString();
        }
        else {
            dateUI.GetComponent<Text>().text = conference.vevent[eventNumber].DTSTART.Day.ToString().PadLeft(2, '0') + "/" + conference.vevent[eventNumber].DTSTART.Month.ToString().PadLeft(2, '0') + " - " + conference.vevent[eventNumber].DTEND.Day.ToString().PadLeft(2, '0') + "/" + conference.vevent[eventNumber].DTEND.Month.ToString().PadLeft(2, '0') + "/" + conference.vevent[eventNumber].DTEND.Year.ToString();
        }
        GameObject locationUI = GameObject.Find("Location");
        locationUI.GetComponent<Text>().text = conference.vevent[eventNumber].LOCATION;
        GameObject summaryUI = GameObject.Find("Summary");
        summaryUI.GetComponent<Text>().text = conference.vevent[eventNumber].SUMMARY;
        GameObject status = GameObject.Find("Status");
        status.GetComponent<SceneStatus>().readyToOpen = true;
        www = new WWW(conference.vevent[eventNumber].DESCRIPTION);
        //Debug.Log(conference.vevent[eventNumber].DESCRIPTION);
        yield return www;
        GameObject poster = GameObject.Find("Poster");
        if (www.texture != null)
        {
            poster.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
        }
        yield return null;
    }
}
