using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public enum LOG_TYPE
{
	NORMAL,
	ERROR,
    INFO,
    SYSTEM,
    EDITOR_TOOL,
    USER_INPUT,
    DATABASE,
    DEBUG_TEST,
    //
    P2P_NETWORK_SERVER_INFO,
    P2P_NETWORK_CLIENT_INFO,
    P2P_NETWORK_MANAGER_INFO,
    P2P_NETWORK_SERVER_ERROR,
    P2P_NETWORK_SERVER_WARNNING,
    //
    SOCKET_NETWORK_INFO
}
public class KojeomLogger {
    public static string GetGUIDebugLogs()
    {
        StringBuilder totalLog = new StringBuilder();
        for(int idx = guiDebugLogBuffer.Count-1; idx > 0; idx--)
        {
            totalLog.Append(guiDebugLogBuffer[idx]);
        }
        return totalLog.ToString();
    }
    private static List<string> logFileBuffer = new List<string>();
    private static List<string> guiDebugLogBuffer = new List<string>();
	public static void DebugLog(string log, LOG_TYPE logType = LOG_TYPE.INFO)
	{
        string savedLog = string.Format("[Time-stamp]{0}::[{1}]{2}\n", TimeStamp(), logType, log);
        if (logType == LOG_TYPE.ERROR ||
            logType == LOG_TYPE.EDITOR_TOOL ||
            logType == LOG_TYPE.P2P_NETWORK_CLIENT_INFO ||
            logType == LOG_TYPE.P2P_NETWORK_SERVER_INFO ||
            logType == LOG_TYPE.P2P_NETWORK_SERVER_ERROR ||
            logType == LOG_TYPE.P2P_NETWORK_SERVER_WARNNING ||
            logType == LOG_TYPE.P2P_NETWORK_MANAGER_INFO ||
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
        StringBuilder guiDebugLog = new StringBuilder();
        switch (logType)
        {
            case LOG_TYPE.NORMAL:
                consoleLog.AppendFormat("<color=blue><b>[NORMAL]</b></color> {0}", log);
                guiDebugLog.AppendFormat("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
                break;
            case LOG_TYPE.ERROR:
                consoleLog.AppendFormat("<color=red><b>[ERROR]</b></color> {0}", log);
                guiDebugLog.AppendFormat("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
                break;
            case LOG_TYPE.INFO:
                consoleLog.AppendFormat("<color=#3366FF><b>[INFO]</b></color> {0}", log);
                guiDebugLog.AppendFormat("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
                break;
            case LOG_TYPE.P2P_NETWORK_SERVER_INFO:
                consoleLog.AppendFormat("<color=#99CCFF><b>[P2P_NETWORK_SERVER_INFO]</b></color> {0}", log);
                guiDebugLog.AppendFormat("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
                break;
            case LOG_TYPE.P2P_NETWORK_CLIENT_INFO:
                consoleLog.AppendFormat("<color=#6633FF><b>[P2P_NETWORK_CLIENT_INFO]</b></color> {0}", log);
                guiDebugLog.AppendFormat("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
                break;
            case LOG_TYPE.SYSTEM:
                consoleLog.AppendFormat("<color=white><b>[SYSTEM]</b></color> {0}", log);
                guiDebugLog.AppendFormat("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
                break;
            case LOG_TYPE.EDITOR_TOOL:
                consoleLog.AppendFormat("<color=#9900FF><b>[EDITOR_TOOL]</b></color> {0}", log);
                break;
            case LOG_TYPE.USER_INPUT:
                consoleLog.AppendFormat("<color=#FF6600><b>[USER_INPUT]</b></color> {0}", log);
                break;
            case LOG_TYPE.DATABASE:
                consoleLog.AppendFormat("<color=#FFCC00><b>[DATABASE]</b></color> {0}", log);
                guiDebugLog.AppendFormat("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
                break;
            case LOG_TYPE.P2P_NETWORK_MANAGER_INFO:
                consoleLog.AppendFormat("<color=#6699CC><b>[P2P_NETWORK_MANAGER_INFO]</b></color> {0}", log);
                guiDebugLog.AppendFormat("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
                break;
            case LOG_TYPE.P2P_NETWORK_SERVER_ERROR:
                consoleLog.AppendFormat("<color=red><b>[P2P_NETWORK_SERVER_ERROR]</b></color> {0}", log);
                guiDebugLog.AppendFormat("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
                break;
            case LOG_TYPE.P2P_NETWORK_SERVER_WARNNING:
                consoleLog.AppendFormat("<color=yellow><b>[P2P_NETWORK_SERVER_WARNNING]</b></color> {0}", log);
                guiDebugLog.AppendFormat("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
                break;
            case LOG_TYPE.DEBUG_TEST:
                consoleLog.AppendFormat("<color=#9cfab3><b>[DEBUG_TEST]</b></color> {0}", log);
                guiDebugLog.AppendFormat("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
                break;
            case LOG_TYPE.SOCKET_NETWORK_INFO:
                consoleLog.AppendFormat("<color=#99FF33><b>[SOCKET_NETWORK_INFO]</b></color> {0}", log);
                guiDebugLog.AppendFormat("<color=white>[Time]:{0}, [log]:{1}</color>\n", SimpleTimeStamp(), consoleLog.ToString());
                break;
            default:
                break;
        }
        
        if(guiDebugLog.ToString().Equals("") == false)
        {
            guiDebugLogBuffer.Add(guiDebugLog.ToString());
        }

        if (guiDebugLogBuffer.Count > 1000)
        {
            guiDebugLogBuffer.Clear();
        }
        //
        if (Application.platform == RuntimePlatform.WindowsEditor)
		{
            Debug.Log(consoleLog);
            P2PNetworkManager.LogPushToLoggerServer(savedLog);
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
        else if(curPlatform == RuntimePlatform.Android || curPlatform == RuntimePlatform.IPhonePlayer)
        {
            //
        }
        File.WriteAllBytes(logFilePath, logbytes);
        logFileBuffer.Clear();
    }
}
