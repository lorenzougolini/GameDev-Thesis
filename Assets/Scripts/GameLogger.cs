using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameLogger : MonoBehaviour
{
    public static GameLogger Instance { get; private set; }

    [SerializeField] public bool logToFile;

    private List<string> eventLogs;
    private string logFilePath;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            eventLogs = new List<string>();
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void LogEvent(string eventMessage) {
        string logMessage = $"{Time.time}: {eventMessage}\n";
        eventLogs.Add(logMessage);
    }

    public List<string> GetEventLogs() {
        return new List<string>(eventLogs);
    }

    public void SaveLogsToFile() {
        if (!string.IsNullOrEmpty(logFilePath) && logToFile)
            File.WriteAllLines(logFilePath, eventLogs);
            Debug.Log($"File saved at: {logFilePath}");
    }

    public void SetLogFilePath(string path) {
        logFilePath = path;
    }

    public void ClearLogs() {
        eventLogs.Clear();
    }
}
