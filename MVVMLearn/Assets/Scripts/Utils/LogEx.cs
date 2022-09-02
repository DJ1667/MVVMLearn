using UnityEngine;

public class LogEx
{
    public static bool IsShowLog = true;

    public static void Log(object message)
    {
        Debug.Log(message);
    }

    public static void LogError(object message)
    {
        Debug.LogError(message);
    }

    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }
}