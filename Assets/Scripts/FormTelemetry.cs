using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class FormTelemetry : MonoBehaviour
{

    public struct FormPart1
    {
        public string answer1_01;
        public string answer1_02;
        public string answer1_03;
        public string answer1_04;
        public string answer1_05;
        public string answer1_06;
        public string answer1_07;
        public string answer1_08;
        public string answer1_09;
        public string answer1_10;
        public string answer1_11;
        public string answer1_12;
        public string answer1_13;
        public string answer1_14;
    }

    public struct FormPart2
    {
        public string answer2_01;
        public string answer2_02;
        public string answer2_03;
        public string answer2_04;
        public string answer2_05;
        public string answer2_06;
        public string answer2_07;
        public string answer2_08;
        public string answer2_09;
        public string answer2_10;
        public string answer2_11;
        public string answer2_12;
        public string answer2_13;
        public string answer2_14;
        public string answer2_15;
        public string answer2_16;
        public string answer2_17;
    }

    public struct FormPart3
    {
        public string answer3;
    }

    private const string FirebaseUrl = "https://gamedev-thesis-default-rtdb.europe-west1.firebasedatabase.app/forms";

    public static IEnumerator DivideAndSubmit(string matchID, FormPart1 formPart1, FormPart2 formPart2, FormPart3 formPart3)
    {

        string json1 = JsonUtility.ToJson(formPart1);
        string json2 = JsonUtility.ToJson(formPart2);
        string json3 = JsonUtility.ToJson(formPart3);
        yield return SubmitToFirebase($"{FirebaseUrl}/{matchID}/part1.json", json1);
        yield return SubmitToFirebase($"{FirebaseUrl}/{matchID}/part2.json", json2);
        yield return SubmitToFirebase($"{FirebaseUrl}/{matchID}/part3.json", json3);
    }

    public static IEnumerator SubmitToFirebase(string url, string dataJson)
    {
        using (UnityWebRequest www = UnityWebRequest.Put(url, dataJson))
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
