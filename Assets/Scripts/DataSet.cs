using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataSet : MonoBehaviour
{

    private string requestedURL;

    void Start()
    {
        //crée l'url de requête qui contient le consumerid/key
        RequestUrl();
        //démarre la demande d'accès à la stib
        StartCoroutine(GetRequest(requestedURL));
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

        // Show results as text        
        Debug.Log(request.downloadHandler.text);
        Rootobject stibData = new Rootobject();
        //stibData = JsonUtility.FromJson<Rootobject>(request.downloadHandler.text.Substring(1, request.downloadHandler.text.Length-2));
        stibData = JsonUtility.FromJson<Rootobject>(request.downloadHandler.text);
        Debug.Log(stibData.lines[0].vehiclePositions[0].pointId);
        //Debug.Log(stibData.lines.Length);

    }

    //request URL avec la librairie Oauth et les logs du compte dev stib
    private void RequestUrl()
    {
        OAuth_CSharp oauth = new OAuth_CSharp("dH8vA2I1PREqcTOJqfO2kUgDWq4a", "4UBJu7NZYHCLLhIzJ5zA6FbCa48a");
        string requestURL = oauth.GenerateRequestURL("https://opendata-api.stib-mivb.be/OperationMonitoring/1.0/VehiclePositionByLine/93%2C92", "GET");
        requestedURL = requestURL;

    }

}
