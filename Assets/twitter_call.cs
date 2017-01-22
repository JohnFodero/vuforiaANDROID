using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Globalization;
using UnityEngine.UI;



//Script was modified/updated by OJ3D - 11/8/2015
//The original script was develolped by Steve Yaus/gambitforhire for twitter API
//https://bitbucket.org/gambitforhire/twitter-search-with-unity/src/bfc5cf8a401f12921c3c4f2355bf3315411d9f13/Assets/Scripts/TwitterAPI.cs?at=master&fileviewer=file-view-default#TwitterAPI.cs-151

public class twitter_call : MonoBehaviour
{

    //
    public GameObject tweet;
    //Yelp API v.2.0 Access Key and Tokens
    private string oauthConsumerKey = "MxLUv2RBzEA2qBJIkymMzmjGj";
    private string oauthConsumerSecret = "X9kHCJTIMtEi1hhVa9DR5r9gGqJ2M4gU673BBZgz7gpjQtBR5S";
    private string oauthToken = "917856360-PofADbCKWObmtgkiRBYivMjdCqd4Y86x3wgF6Bvw";
    private string oauthTokenSecret = "JXJ52saXTGnTR5mHSeOPyDKPhTlC19Dy3tcLPGR1L6s81";
    private string oauthNonce = "";
    private string oauthTimeStamp = "";

    //***********URL Request parameters*******
    // Please refer to http://www.yelp.com/developers/documentation for the API documentation.
    //API SEARCH HOST BASE
    private static string TwitterAPIHost = "https://api.twitter.com";
    private static string StatusPath = "/1.1/statuses/home_timeline.json";
    //SEARCH TERM
    public static string searchterm = "food";//*********I/P your term
                                             //private static string searchtermURL = "term=" + searchterm;
                                             //SEARCH LOCATION
    public static string State = "ca"; //California
    public static string City = "San francisco"; //Sanfrancisco - no spaces
    private static string searchLocation = City + "," + State;//URL friendly Escape//"WWW.EscapeURL ("San Francisoin, CA");*******I/P your location 
                                                              //SEARCH LIMITS
    public static int searchLimit = 3;//*******I/P your search limite here 
                                      //SEARCH DEALS
    public static string searchDeals = "true";

    //***STEP0******
    void Start()
    {
        //*********Restaurants Search Request
        string FullTwitterURL = TwitterAPIHost + StatusPath;
        PrepareOAuthData();
        StartCoroutine(SearchRestaurantsRequest(FullTwitterURL));
    }

    //***STEP1******
    private void PrepareOAuthData()
    {
        oauthNonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture)));
        TimeSpan _timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
        oauthTimeStamp = Convert.ToInt64(_timeSpan.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        // Override the nounce and timestamp here if troubleshooting with Twitter's OAuth Tool
        //oauthNonce = "69db07d069ac50cd673f52ee08678596";
        //oauthTimeStamp = "1442419142";
    }

    //***STEP2****** &***FINAL STEP8 JSON READBACK****** 
    //Restaurants Search Run
    private IEnumerator SearchRestaurantsRequest(string YelpURL2)
    {

        // Fix up hashes to be webfriendly
        searchLocation = Uri.EscapeDataString(searchLocation);

        SortedDictionary<string, string> TwitterParamsDictionary = new SortedDictionary<string, string>()
        {
            {"term", searchterm},
            {"location",searchLocation} //maybe limit and deals
        	  //Add {"limit",searchLimit},
            	  //Add {"deals_filter",searchDeals}
		};

        WWW query = CreateYelpAPIQuery(YelpURL2, TwitterParamsDictionary);
        yield return query;

        //Write request response to Unity Console
        //Debug.Log(query.text);
        int a, b, top;
        if (query.text.Length > 15)
        {

            int start_index = query.text.IndexOf(",\"text\":\"") + 9;
            int end_index = query.text.IndexOf("truncated") - 3;
            string output = query.text.Substring(start_index, end_index - start_index);
            
            Debug.Log(output);
            String tweetFun = output;
            Debug.Log(tweetFun.Length);
            String line1, line2, line3, finalString;
            line1 = "";
            line2 = "";
            line3 = "";
            finalString = null;
            if (tweetFun.Length < 47)
            {
                line1 = tweetFun.Substring(0, tweetFun.Length);
                
            }
            if (tweetFun.Length > 47 && tweetFun.Length < 95)
            {
                line1 = tweetFun.Substring(0, 47);
                line2 = tweetFun.Substring(48, tweetFun.Length - 47-1);
            }
            if (tweetFun.Length >= 95)
            {
                line1 = tweetFun.Substring(0, 47);
                line2 = tweetFun.Substring(48, 47);
                line3 = tweetFun.Substring(95, tweetFun.Length - 94-1);
            }
            
           /* String finalString = "";
            String newLine1;
            int end = tweetFun.Length;
            a = 0;
            top = 0;
           */
           /* Debug.Log(end);
            b = 47;
            
            while (b < end)
            {
             for (int i = b; i > a; i--)
             {
                    Debug.Log(tweetFun);
                    if (tweetFun[i].Equals(" "))
                    {
                        newLine1 = tweetFun.Substring(top, a);
                        Debug.Log("Start: " + finalString);
                        a = i;
                        b = 2*a;
                        finalString = finalString + newLine1 + "\n";
                        Debug.Log("End: " + finalString);
                       
                    }
                    if (b >= end)
                        break;
                    else
                    {
                        a++;
                        b++;
                    }
             }
         }
            finalString = finalString + tweetFun.Substring(a, end - a);
           // finalString = "hellooooo";
           */

            GameObject tempTextBox = (GameObject)Instantiate(tweet);
            TextMesh theText = tempTextBox.transform.GetComponent<TextMesh>();
            theText.text = line1 + "\n" + line2 + "\n" + line3;

        }
        Debug.Log(query.error);

        //Convert String into JSON node
        //JSONNode JSONText = JSONNode.Parse(query.text);//Send JSONnode
    }

    //***STEP3******
    private WWW CreateYelpAPIQuery(string YelpURL, SortedDictionary<string, string> TwitterParamsDictionary)
    {
        //Create Signature
        string signature = CreateSignature(YelpURL, TwitterParamsDictionary);
        //Debug.Log("OAuth Signature: " + signature);

        //Create Authorization Header
        string authHeaderParam = CreateAuthorizationHeaderParameter(signature, this.oauthTimeStamp);
        //Debug.Log("Auth Header: " + authHeaderParam);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        //headers["Authorization"] = "Basic " + System.Convert.ToBase64String (System.Text.Encoding.ASCII.GetBytes (authHeaderParam));
        headers["Authorization"] = authHeaderParam;

        string YelpParams = ParamDictionaryToString(TwitterParamsDictionary);
        WWW query = new WWW(YelpURL + "?" + YelpParams, null, headers);
        return query;

    }

    //***STEP4******
    // Taken from http://www.i-avington.com/Posts/Post/making-a-twitter-oauth-api-call-using-c
    private string CreateSignature(string url, SortedDictionary<string, string> searchParamsDictionary)
    {
        //string builder will be used to append all the key value pairs
        StringBuilder signatureBaseStringBuilder = new StringBuilder();
        signatureBaseStringBuilder.Append("GET&");
        signatureBaseStringBuilder.Append(Uri.EscapeDataString(url));
        signatureBaseStringBuilder.Append("&");

        //the key value pairs have to be sorted by encoded key
        SortedDictionary<string, string> urlParamsDictionary = new SortedDictionary<string, string>()
        {
			//******Alphabetically Organize Signature Parameters -Indicated in Section 3 - Create Signature Base String -https://blog.nraboy.com/2014/11/understanding-request-signing-oauth-1-0a-providers/
			{"oauth_consumer_key", this.oauthConsumerKey},
            {"oauth_nonce", this.oauthNonce},
            {"oauth_signature_method", "HMAC-SHA1"},
            {"oauth_timestamp", this.oauthTimeStamp},
            {"oauth_token", this.oauthToken},
            {"oauth_version", "1.0"}
        };

        foreach (KeyValuePair<string, string> keyValuePair in searchParamsDictionary)
        {
            urlParamsDictionary.Add(keyValuePair.Key, keyValuePair.Value);
        }

        signatureBaseStringBuilder.Append(Uri.EscapeDataString(ParamDictionaryToString(urlParamsDictionary)));
        string signatureBaseString = signatureBaseStringBuilder.ToString();

        //Debug.Log("Signature Base String: " + signatureBaseString);

        //generation the signature key the hash will use
        string signatureKey = Uri.EscapeDataString(this.oauthConsumerSecret) + "&" + Uri.EscapeDataString(this.oauthTokenSecret);

        HMACSHA1 hmacsha1 = new HMACSHA1(new ASCIIEncoding().GetBytes(signatureKey));

        //hash the values
        string signatureString = Convert.ToBase64String(hmacsha1.ComputeHash(new ASCIIEncoding().GetBytes(signatureBaseString)));

        return signatureString;
    }

    //***STEP5******
    private string CreateAuthorizationHeaderParameter(string signature, string timeStamp)
    {   //******Organize Paramters like how Oauth.net has it ordered in section 7-Accessing Protected Resources //http://oauth.net/core/1.0a/#anchor12
        string authorizationHeaderParams = String.Empty;
        authorizationHeaderParams += "OAuth ";

        authorizationHeaderParams += "oauth_consumer_key="
            + "\"" + Uri.EscapeDataString(this.oauthConsumerKey) + "\", ";

        authorizationHeaderParams += "oauth_token=" + "\"" +
            Uri.EscapeDataString(this.oauthToken) + "\", ";

        authorizationHeaderParams += "oauth_signature_method=" + "\"" +
            Uri.EscapeDataString("HMAC-SHA1") + "\", ";

        authorizationHeaderParams += "oauth_signature=" + "\""
            + Uri.EscapeDataString(signature) + "\", ";

        authorizationHeaderParams += "oauth_timestamp=" + "\"" +
            Uri.EscapeDataString(timeStamp) + "\", ";

        authorizationHeaderParams += "oauth_nonce=" + "\"" +
            Uri.EscapeDataString(this.oauthNonce) + "\", ";

        authorizationHeaderParams += "oauth_version=" + "\"" +
            Uri.EscapeDataString("1.0") + "\"";

        return authorizationHeaderParams;
    }

    //***STEP6******
    private string ParamDictionaryToString(IDictionary<string, string> paramsDictionary)
    {
        StringBuilder dictionaryStringBuilder = new StringBuilder();
        foreach (KeyValuePair<string, string> keyValuePair in paramsDictionary)
        {
            //append a = between the key and the value and a & after the value
            dictionaryStringBuilder.Append(string.Format("{0}={1}&", keyValuePair.Key, keyValuePair.Value));
        }
        // Get rid of the extra & at the end of the string
        string paramString = dictionaryStringBuilder.ToString().Substring(0, dictionaryStringBuilder.Length - 1);
        return paramString;
    }

}