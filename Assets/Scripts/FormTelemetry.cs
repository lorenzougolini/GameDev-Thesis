using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class FormTelemetry : MonoBehaviour
{
    public struct FormTelemetryStruct {
        public string matchID;
        public string roundNumber;
        public string answer0;
        public string answer1;
        public string answer2;
        public string answer3;
        public string answer4;
        public string answer5;
        public string answer6;
        public string answer7;
        public string answer8;
        public string answer9;
    }

    // Form link
    private const string GoogleFormBaseUrl = "https://docs.google.com/forms/d/e/1FAIpQLScZI1M_f_jnAZvyQhKrlw65qqyrSXDl13lguwkYe_RJgv-fBQ/";

    private const string FormSpreeUrl = "https://formspree.io/f/mdknjvzw";

    private const string FirebaseUrl = "https://gamedev-thesis-default-rtdb.europe-west1.firebasedatabase.app/";

    // Form fields
    private const string _gform_match_id = "entry.299700096";
    private const string _gform_round_number = "entry.406080789";
    private const string _gform_question_1 = "entry.1482541386";
    private const string _gform_question_2= "entry.74165312";
    private const string _gform_question_3 = "entry.1324886053";
    private const string _gform_question_4 = "entry.1309519868";
    private const string _gform_question_5 = "entry.910651363";
    private const string _gform_question_6 = "entry.102596790";
    private const string _gform_question_7 = "entry.1515409854";
    private const string _gform_question_8 = "entry.802400386";
    private const string _gform_question_9 = "entry.1610878678";
    private const string _gform_question_10 = "entry.917199916";

    public static IEnumerator SubmitFeedbackForm(FormTelemetryStruct feedbackData)
    {
        CultureInfo cultureInfo = CultureInfo.GetCultureInfo("en-GB");
        Thread.CurrentThread.CurrentCulture = cultureInfo;

        string urlGoogleFormResponse = GoogleFormBaseUrl + "formResponse";
        Debug.Log("Submitting form to " + urlGoogleFormResponse);
        Debug.Log("Submitting formspree to " + FormSpreeUrl);
        
        WWWForm googleForm = new();
        WWWForm formspreeForm = new();

        googleForm.AddField(_gform_match_id, feedbackData.matchID);
        googleForm.AddField(_gform_round_number, feedbackData.roundNumber);
        googleForm.AddField(_gform_question_1, feedbackData.answer0);
        googleForm.AddField(_gform_question_2, feedbackData.answer1);
        googleForm.AddField(_gform_question_3, feedbackData.answer2);
        googleForm.AddField(_gform_question_4, feedbackData.answer3);
        googleForm.AddField(_gform_question_5, feedbackData.answer4);
        googleForm.AddField(_gform_question_6, feedbackData.answer5);
        googleForm.AddField(_gform_question_7, feedbackData.answer6);
        googleForm.AddField(_gform_question_8, feedbackData.answer7);
        googleForm.AddField(_gform_question_9, feedbackData.answer8);
        googleForm.AddField(_gform_question_10, feedbackData.answer9);

        formspreeForm.AddField("matchID", feedbackData.matchID);
        formspreeForm.AddField("roundNumber", feedbackData.roundNumber);
        formspreeForm.AddField("answer1", feedbackData.answer0);
        formspreeForm.AddField("answer2", feedbackData.answer1);
        formspreeForm.AddField("answer3", feedbackData.answer2);
        formspreeForm.AddField("answer4", feedbackData.answer3);
        formspreeForm.AddField("answer5", feedbackData.answer4);
        formspreeForm.AddField("answer6", feedbackData.answer5);
        formspreeForm.AddField("answer7", feedbackData.answer6);
        formspreeForm.AddField("answer8", feedbackData.answer7);
        formspreeForm.AddField("answer9", feedbackData.answer8);
        formspreeForm.AddField("answer10", feedbackData.answer9);

        // google form connection
        // using (UnityWebRequest www = UnityWebRequest.Post(urlGoogleFormResponse, googleForm))
        // {
        //     // send data
        //     yield return www.SendWebRequest();

        //     if (www.result != UnityWebRequest.Result.Success)
        //     {
        //         Debug.LogError($"Request error with code {www.error}: {www.responseCode}");
        //     }
        //     else
        //         Debug.Log("Form upload complete!");
        // }
        
        // formspree connection
        // using (UnityWebRequest www = UnityWebRequest.Post(FormSpreeUrl, formspreeForm))
        // {
        //     // send data
        //     yield return www.SendWebRequest();

        //     if (www.result != UnityWebRequest.Result.Success)
        //     {
        //         Debug.LogError($"Request error with code {www.error}: {www.responseCode}");
        //     }
        //     else
        //         Debug.Log("Form upload complete!");
        // }


        // firebase database connection
        string jsonFeedbackData = JsonUtility.ToJson(feedbackData);
        using (UnityWebRequest www = UnityWebRequest.Put($"{FirebaseUrl}/{feedbackData.matchID}/round{feedbackData.roundNumber}.json", jsonFeedbackData))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            // send data
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Firebase request error with code {www.error}: {www.responseCode}");
            }
            else
                Debug.Log("Firebase upload complete!");
        }

        yield return null;
    }
}
