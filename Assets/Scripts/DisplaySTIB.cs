using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

public class WaitingLine  : IComparable
{
	public int lineNumber;
	public double waitingTime;

	public WaitingLine(int _lineNumber, double _watingTime)
	{
		this.lineNumber = _lineNumber;
		this.waitingTime = _watingTime;
	}

	public int CompareTo(object obj) 
	{
		WaitingLine waitingLineToCompare = obj as WaitingLine;
		if (waitingLineToCompare.waitingTime < waitingTime)
		{
			return 1;
		}
		if (waitingLineToCompare.waitingTime > waitingTime)
		{
			return -1;
		}
		return 0;
	}
}

public class DisplaySTIB : MonoBehaviour {

	public Sprite line92;
	public Sprite line93;
	bool requestFinished = false;
    bool requestErrorOccurred = false;
	List<WaitingLine> waitingLines = new List<WaitingLine>();

	private string requestedURL;

	// Use this for initialization
	void Start()
	{
		//crée l'url de requête qui contient le consumerid/key
		//RequestUrl();
		//démarre la demande d'accès à la stib
		StartCoroutine(GetRequest("https://opendata-api.stib-mivb.be/OperationMonitoring/1.0/PassingTimeByPoint/6308"));
	}

	IEnumerator GetRequest(string uri)
	{
		//store l'url de requête dans une unitywebrequest
		UnityWebRequest request = UnityWebRequest.Get(uri);

		//ajoute les headers d'application + token à la requete
		request.SetRequestHeader("Accept", "application/json");
		request.SetRequestHeader("Authorization", "Bearer 01dc5ca7d2c53c40771fcce562bb0377");

		//envoi la requête et attend la réponse
		yield return request.Send();
        GameObject status = GameObject.Find("Status");
        status.GetComponent<SceneStatus>().readyToOpen = true;
		// Show results as text        
		Debug.Log(request.downloadHandler.text);
		StopInfo stibData = new StopInfo();
		stibData = JsonUtility.FromJson<StopInfo>(request.downloadHandler.text);
		for(int i=0; i<stibData.points[0].passingTimes.Length; i++) {
			waitingLines.Add(new WaitingLine(int.Parse(stibData.points[0].passingTimes[i].lineId), (transformDate(stibData.points[0].passingTimes[i].expectedArrivalTime) - DateTime.Now).TotalMinutes));
		}
		waitingLines.Sort();
		for(int i=0; i<waitingLines.Count; i++) {
			GameObject lineUI = GameObject.Find("Line"+i);
			if (waitingLines[i].lineNumber==92) lineUI.GetComponent<Image>().sprite = line92;
			if (waitingLines[i].lineNumber==93) lineUI.GetComponent<Image>().sprite = line93;
			GameObject timeToWaitUI = GameObject.Find("TimeToWait"+i);
			if(Math.Round(waitingLines[i].waitingTime)<=0) timeToWaitUI.GetComponent<Text>().text = char.ConvertFromUtf32(0x2193)+char.ConvertFromUtf32(0x2193);
			else timeToWaitUI.GetComponent<Text>().text = Math.Round(waitingLines[i].waitingTime).ToString().PadLeft(2,'0')+"'";
		}
		Debug.Log((transformDate(stibData.points[0].passingTimes[0].expectedArrivalTime) - DateTime.Now).Minutes);
		Debug.Log(stibData.points[0].passingTimes[0].lineId);
	}


	private DateTime transformDate(string dateToTransform)
	{
		int expectedArrivalYear = int.Parse(dateToTransform.Substring(0, 4));
		int expectedArrivalMonth = int.Parse(dateToTransform.Substring(5, 2));
		int expectedArrivalDay = int.Parse(dateToTransform.Substring(8, 2));
		int expectedArrivalHour = int.Parse(dateToTransform.Substring(11, 2));
		int expectedArrivalMinute = int.Parse(dateToTransform.Substring(14, 2));
		int expectedArrivalSecond = int.Parse(dateToTransform.Substring(17, 2));
		DateTime expectedArrivalDateTime = new DateTime(expectedArrivalYear, expectedArrivalMonth, expectedArrivalDay, expectedArrivalHour, expectedArrivalMinute, expectedArrivalSecond);
		return expectedArrivalDateTime;
	}
	//request URL avec la librairie Oauth et les logs du compte dev stib
	private void RequestUrl()
	{
		OAuth_CSharp oauth = new OAuth_CSharp("dH8vA2I1PREqcTOJqfO2kUgDWq4a", "4UBJu7NZYHCLLhIzJ5zA6FbCa48a");
		string requestURL = oauth.GenerateRequestURL("https://opendata-api.stib-mivb.be/OperationMonitoring/1.0/VehiclePositionByLine/93%2C92", "GET");
		requestedURL = requestURL;

	}
}
