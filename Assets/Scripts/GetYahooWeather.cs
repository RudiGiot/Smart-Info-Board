using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YahooAPI;

public class GetYahooWeather : MonoBehaviour {

	int tryNumber = 0;

	public string url;

	void Start () {
		StartCoroutine(Weather());
	}

	public IEnumerator Weather()
	{
		YahooWeather cityWeather;
		do {
			cityWeather = new YahooWeather(url);
			yield return cityWeather;
			tryNumber++;
			//Debug.Log(tryNumber);
		} while ((tryNumber<5)&&(cityWeather.query.count==0));

		if (cityWeather.query.count!=0) {
			GameObject cityUI = GameObject.Find("City");
			cityUI.GetComponent<Text>().text = cityWeather.query.results.channel.location.city + ", " + cityWeather.query.results.channel.location.country;
			GameObject temperatureUI = GameObject.Find("Temperature");
			int temperature = ((cityWeather.query.results.channel.item.condition.temp - 32)*5)/9;
			temperatureUI.GetComponent<Text>().text = temperature.ToString() + "°C";
			GameObject status = GameObject.Find("Status");
			status.GetComponent<SceneStatus>().readyToOpen = true;
		}
	}

	// Attention gérer l'exception au cas où le serveur renvoit un code autre que 200 (404, 500, 303, ...)
}
