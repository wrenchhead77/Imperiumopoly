using System;
using System.IO;
using UnityEngine;

public class ErrorLogger : MonoBehaviour
{
    // Log file path (will be set during runtime)
    private string logFilePath;

    // Singleton instance
    public static ErrorLogger Instance;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }

        // Set the log file path relative to the script's directory
        string logDirectory = Path.Combine(Application.dataPath, "Logs"); // "Assets/Logs"

        // Ensure the directory exists
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory); // Create the directory if it doesn't exist
        }

        logFilePath = Path.Combine(logDirectory, "game_log.txt");

        // Initialize the log file
        InitializeLog();

        // Subscribe to Unity's log message system
        Application.logMessageReceived += HandleLog;
    }

    // Initialize the log file (called at the start of the game)
    public void InitializeLog()
    {
        if (!File.Exists(logFilePath))
        {
            // Create the log file if it doesn't exist
            File.WriteAllText(logFilePath, "Game Log Started: " + DateTime.Now + Environment.NewLine);
        }
    }

    // Log informational messages
    public void LogInfo(string message)
    {
        string logMessage = $"INFO [{DateTime.Now}]: {message}";
        WriteLog(logMessage);
        Debug.Log(message); // Also log to Unity Console
    }

    // Log warnings
    public void LogWarning(string message)
    {
        string logMessage = $"WARNING [{DateTime.Now}]: {message}";
        WriteLog(logMessage);
        Debug.LogWarning(message); // Also log to Unity Console
    }

    // Log errors
    public void LogError(string message)
    {
        string logMessage = $"ERROR [{DateTime.Now}]: {message}";
        WriteLog(logMessage);
        Debug.LogError(message); // Also log to Unity Console
    }

    // Write log to the file
    private void WriteLog(string message)
    {
        try
        {
            // Append message to the log file
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(message);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to log file: {e.Message}");
        }
    }

    // Optionally, clear the log file (for debugging purposes)
    public void ClearLog()
    {
        try
        {
            File.WriteAllText(logFilePath, "Game Log Cleared: " + DateTime.Now + Environment.NewLine);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to clear the log file: {e.Message}");
        }
    }

    // Get the log file path (for external access if needed)
    public string GetLogFilePath()
    {
        return logFilePath;
    }

    // Unity's log message received handler
    private void HandleLog(string logString, string stackTrace, LogType logType)
    {
        if (logType == LogType.Error || logType == LogType.Exception)
        {
            // Log error messages and exceptions to the log file
            string logMessage = $"ERROR [{DateTime.Now}]: {logString}\nStack Trace: {stackTrace}";
            WriteLog(logMessage);
        }
    }

    // Unsubscribe from the log message system when not needed
    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
}
