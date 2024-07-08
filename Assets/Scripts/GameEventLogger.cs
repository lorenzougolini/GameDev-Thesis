using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameEventLogger : MonoBehaviour
{
    public static GameEventLogger Instance { get; private set; }

    private List<string> eventLogs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            eventLogs = new List<string>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LogEvent(string eventMessage)
    {
        string logMessage = $"{Time.time}: {eventMessage}";
        Debug.Log(logMessage); // Optional: Log to console for real-time feedback
        eventLogs.Add(logMessage);
    }

    public List<string> GetEventLogs()
    {
        return new List<string>(eventLogs);
    }

    public void SaveLogsToFile(string filePath)
    {
        File.WriteAllLines(filePath, eventLogs);
    }
}
