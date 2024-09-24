using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class MatchTelemetry : MonoBehaviour
{

    public struct MatchTelemetryStruct {
        public string matchID;
        public List<PlayerTelemetry> playerTelemetry;
        public List<OpponentTelemetry> opponentTelemetry;
        public List<BallTelemetry> ballTelemetry;
        public List<ScoreTelemetry> scoreTelemetry;
        public int player1Goals;
        public int player2Goals;
    }

    public struct PlayerTelemetry
    {
        public float time;
        public Vector2 position;
        public string action;
    }

    public struct OpponentTelemetry
    {
        public float time;
        public Vector2 position;
        public string action;
    }

    public struct BallTelemetry
    {
        public float time;
        public Vector2 position;
    }

    public struct ScoreTelemetry
    {
        public float time;
        public string scoringPlayer;
    }

    public static float telemetryTimeInterval = 0.1f;
    
    // Form link
    private const string FirebaseUrl = "https://gamedev-thesis-default-rtdb.europe-west1.firebasedatabase.app/matches/";

    public static IEnumerator SubmitMatchTelemetry(MatchTelemetryStruct matchData)
    {
        string url = $"{FirebaseUrl}/{GameIdController.gameId}/{GameIdController.RoundNumber}.json";

        // string dataJson = JsonUtility.ToJson(matchData);

        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.Converters.Add(new Vector2Converter());
        string dataJson = JsonConvert.SerializeObject(matchData, settings);

        using (UnityWebRequest www = UnityWebRequest.Put(url, dataJson))
        {
            // send data
            yield return www.SendWebRequest();

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
