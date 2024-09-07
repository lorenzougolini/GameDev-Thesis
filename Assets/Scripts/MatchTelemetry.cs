using System;
using System.Collections;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class MatchTelemetry : MonoBehaviour
{

    public struct MatchTelemetryStruct {
        public string matchID;
        public Vector2 playerPosition;
        public Vector2 opponentPosition;
        public Vector2 ballPosition;
        public string playerAction;
        public string opponentAction;
        public int playerScore;
        public int opponentScore;
    }

    // Form link
    private const string GoogleFormBaseUrl = "https://docs.google.com/forms/d/e/1FAIpQLSe9jmT-D4S8khV2qWGWd8tkLUBQ3BTaDKCMHx_RLz6iLCNtFg/";

    // Form fields
    private const string _gform_match_id = "entry.445216140";

    private const string _gform_player_positionX = "entry.475489404";
    private const string _gform_player_positionY = "entry.438288379";
    private const string _gform_opponent_positionX = "entry.1426651271";
    private const string _gform_opponent_positionY = "entry.997466543";
    private const string _gform_ball_positionX = "entry.2031955151";
    private const string _gform_ball_positionY = "entry.1664050216";

    private const string _gform_player_action = "entry.169847279";
    private const string _gform_opponent_action = "entry.1838019289";

    private const string _gform_player_score = "entry.324271870";
    private const string _gform_opponent_score = "entry.836300563";

    // private static Guid matchId;
    private static string matchId;

    public static IEnumerator SubmitGoogleForm(MatchTelemetryStruct lvlData)
    {
        CultureInfo ci = CultureInfo.GetCultureInfo("en-GB");
        Thread.CurrentThread.CurrentCulture = ci;

        string urlGoogleFormResponse = GoogleFormBaseUrl + "formResponse";
        Debug.Log("Submitting form to " + urlGoogleFormResponse);
        
        WWWForm form = new();

        form.AddField(_gform_match_id, matchId);
        form.AddField(_gform_player_positionX, lvlData.playerPosition.x.ToString());
        form.AddField(_gform_player_positionY, lvlData.playerPosition.y.ToString());
        form.AddField(_gform_opponent_positionX, lvlData.opponentPosition.x.ToString());
        form.AddField(_gform_opponent_positionY, lvlData.opponentPosition.y.ToString());
        form.AddField(_gform_ball_positionX, lvlData.ballPosition.x.ToString());
        form.AddField(_gform_ball_positionY, lvlData.ballPosition.y.ToString());
        form.AddField(_gform_player_action, lvlData.playerAction);
        form.AddField(_gform_opponent_action, lvlData.opponentAction);
        form.AddField(_gform_player_score, lvlData.playerScore);
        form.AddField(_gform_opponent_score, lvlData.opponentScore);

        // using (UnityWebRequest www = UnityWebRequest.Post(urlGoogleFormResponse, form))
        // {
        //     // send data
        //     // yield return www.SendWebRequest();

        //     if (www.result != UnityWebRequest.Result.Success)
        //     {
        //         Debug.LogError(www.error);
        //         Debug.LogError(www.responseCode);
        //     }
        //     else
        //         Debug.Log("Form upload complete!");
        // }

        yield return null;
    }

    public static void GenerateNewMatchID()
    {
        // matchId = Guid.NewGuid();
        matchId = GameIdController.gameId;
    }

    public static string GUIDToShortString(Guid guid)
    {
        var base64Guid = Convert.ToBase64String(guid.ToByteArray());

        // Replace URL unfriendly characters with better ones
        base64Guid = base64Guid.Replace('+', '-').Replace('/', '_');

        // Remove the trailing ==
        return base64Guid.Substring(0, base64Guid.Length - 2);
    }

    
}
