using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weather : MonoBehaviour {
    WWW www;
    string url = "http://dataservice.accuweather.com/currentconditions/v1/350540?apikey=WLX0L0CjGGNu7Z54zFZQcT9fCUi4hJhB%20&details=false";
    // Use this for initialization
    void Start() {
        www = new WWW(url);
        while (!www.isDone)
        {
            for (int i = 0; i < 100; i++)
            {
                i++;
            }
        }
       // Debug.Log("pieeee");
        Invoke("getWeather", 1);
      //  Debug.Log("I STILL LIKE PIE");
    }
    void getWeather() { 
        string weather, temp;
        int index, index2, index3, index4;
        if (www != null && www.isDone)
        {
            index = www.text.IndexOf("WeatherText");
            index2 = www.text.IndexOf("\"WeatherIcon\"");
            index3 = www.text.IndexOf("\"Imperial\"");
            index4 = www.text.IndexOf(",");
            weather = www.text.Substring(index + 14, index2 - 97);
            temp = www.text.Substring(index3 + 20, 4);
            weather = weather + " \n" + temp + "F";
           // weather = www.text.Substring(index+14, index2-98);
           // Debug.Log("HELPME");
            Debug.Log(weather);
           GetComponent<TextMesh>().text = weather;

        }
        else
        {
          //  Debug.Log("HELLO");
            Debug.Log("finished too early");
        }


    }

    // Update is called once per frame
    void Update ()
    { 
  


		
	}
}
