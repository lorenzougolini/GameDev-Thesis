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
        public string answer1;
        public string answer2;
        public string answer3;
        public string answer4;
        public string answer5;
        public string answer6;
        public string answer7;
        public string answer8;
        public string answer9;
        public string answer10;
    }

    // Form link
    private const string GoogleFormBaseUrl = "https://docs.google.com/forms/d/e/1FAIpQLScZI1M_f_jnAZvyQhKrlw65qqyrSXDl13lguwkYe_RJgv-fBQ/";

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
        
        WWWForm form = new();

        form.AddField(_gform_match_id, feedbackData.matchID);
        form.AddField(_gform_round_number, feedbackData.roundNumber);
        form.AddField(_gform_question_1, feedbackData.answer1);
        form.AddField(_gform_question_2, feedbackData.answer2);
        form.AddField(_gform_question_3, feedbackData.answer3);
        form.AddField(_gform_question_4, feedbackData.answer4);
        form.AddField(_gform_question_5, feedbackData.answer5);
        form.AddField(_gform_question_6, feedbackData.answer6);
        form.AddField(_gform_question_7, feedbackData.answer7);
        form.AddField(_gform_question_8, feedbackData.answer8);
        form.AddField(_gform_question_9, feedbackData.answer9);
        form.AddField(_gform_question_10, feedbackData.answer10);

        using (UnityWebRequest www = UnityWebRequest.Post(urlGoogleFormResponse, form))
        {
            // send data
            // yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                Debug.LogError(www.responseCode);
            }
            else
                Debug.Log("Form upload complete!");
        }

        yield return null;
    }
}
