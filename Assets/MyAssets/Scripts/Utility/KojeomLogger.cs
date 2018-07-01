using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public enum LOG_TYPE
{
	NORMAL = 0,
	ERROR = 1,
    INFO = 2,
    NETWORK_SERVER_INFO = 3,
    NETWORK_CLIENT_INFO = 4,
    SYSTEM = 5,
    EDITOR_TOOL = 6,
    USER_INPUT = 7,
    DATABASE = 8
}
public class KojeomLogger {
    public static string GetGUIDebugLogs()
    {
        StringBuilder totalLog = new StringBuilder();
        for(int idx = guiDebugLogs.Count-1; idx > 0; idx--)
        {
            totalLog.Append(guiDebugLogs[idx]);
        }
        return totalLog.ToString();
    }
    private static List<string> logFileBuffer = new List<string>();
    private static List<string> guiDebugLogs = new List<string>();
	public static void DebugLog(string log, LOG_TYPE logType = LOG_TYPE.INFO)
	{
        string savedLog = string.Format("[Time-stamp]{0}::[{1}]{2}\n", TimeStamp(), logType, log);
        if (logType == LOG_TYPE.ERROR ||
            logType == LOG_TYPE.EDITOR_TOOL ||
            logType == LOG_TYPE.NETWORK_CLIENT_INFO ||
            logType == LOG_TYPE.NETWORK_SERVER_INFO ||
            logType == LOG_TYPE.SYSTEM ||
            logType == LOG_TYPE.INFO ||
            logType == LOG_TYPE.DATABASE)
        {
            logFileBuffer.Add(savedLog);
        }
        if(logFileBuffer.Count > 256)
        {
            FlushToFile();
        }
        //
        StringBuilder consoleLog = new StringBuilder();
        switch (logType)
        {
            case LOG_TYPE.NORMAL:
                consoleLog.AppendFormat("<color=blue><b>[NORMAL]</b></color> {0}", log);
                break;
            case LOG_TYPE.ERROR:
                consoleLog.AppendFormat("<color=red><b>[ERROR]</b></color> {0}", log);
                break;
            case LOG_TYPE.INFO:
                consoleLog.AppendFormat("<color=green><b>[INFO]</b></color> {0}", log);
                break;
            case LOG_TYPE.NETWORK_SERVER_INFO:
                consoleLog.AppendFormat("<color=#99CCFF><b>[NETWORK_SERVER_INFO]</b></color> {0}", log);
                break;
            case LOG_TYPE.NETWORK_CLIENT_INFO:
                consoleLog.AppendFormat("<color=#6633FF><b>[NETWORK_CLIENT_INFO]</b></color> {0}", log);
                break;
            case LOG_TYPE.SYSTEM:
                consoleLog.AppendFormat("<color=white><b>[SYSTEM]</b></color> {0}", log);
                break;
            case LOG_TYPE.EDITOR_TOOL:
                consoleLog.AppendFormat("<color=#9900FF><b>[EDITOR_TOOL]</b></color> {0}", log);
                break;
            case LOG_TYPE.USER_INPUT:
                consoleLog.AppendFormat("<color=#FF6600><b>[USER_INPUT]</b></color> {0}", log);
                break;
            case LOG_TYPE.DATABASE:
                consoleLog.AppendFormat("<color=#FFCC00><b>[DATABASE]</b></color> {0}", log);
                break;
            default:
                break;
        }
        string guiDebugLog = string.Format("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
        guiDebugLogs.Add(guiDebugLog);
        if(guiDebugLogs.Count > 1000)
        {
            guiDebugLogs.Clear();
        }
        //
        if (Application.platform == RuntimePlatform.WindowsEditor)
		{
            Debug.Log(consoleLog);
            GameNetworkManager.LogPushToLoggerServer(savedLog);
        }
    }
    private static string TimeStamp(bool isUseFileName = false)
    {
        if (isUseFileName)
        {
            return DateTime.Now.ToString("[연도]yy년MM월dd일[시간]hh(시)mm(분)s(초)(fff) tt");
        }
        return DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff(tt)");
    }

    private static string SimpleTimeStamp()
    {
        return DateTime.Now.ToString("hh:mm:ss.fff");
    }

    public static void FlushToFile()
    {
        StringBuilder logs = new StringBuilder();
        foreach (var log in logFileBuffer)
        {
            logs.Append(log);
        }
        var logbytes = Encoding.UTF8.GetBytes(logs.ToString());
        //
        string logFilePath = null;
        var curPlatform = Application.platform;
        if (curPlatform == RuntimePlatform.WindowsEditor || curPlatform == RuntimePlatform.WindowsPlayer)
        {
            Directory.CreateDirectory(ConstFilePath.LOG_FILE_ROOT_WIN_PATH);
            logFilePath = string.Format("{0}[GAME-LOG]{1}.txt", ConstFilePath.LOG_FILE_ROOT_WIN_PATH, TimeStamp(true));
        }
        else if(curPlatform == RuntimePlatform.Android)
        {
            //
        }
        File.WriteAllBytes(logFilePath, logbytes);
        logFileBuffer.Clear();
    }
}
