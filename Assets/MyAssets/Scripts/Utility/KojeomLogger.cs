using System.Collections;
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
    EDITOR_TOOL = 6
}
public class KojeomLogger {

    private static List<string> logs = new List<string>();
	public static void DebugLog(string log, LOG_TYPE logType = LOG_TYPE.INFO)
	{
        string savedLog = string.Format("[Time-stamp]{0}::{1}\n", TimeStamp(), log);
        logs.Add(savedLog);
        GameNetworkManager.LogPushToLoggerServer(savedLog);
        //
        if (Application.platform == RuntimePlatform.WindowsEditor)
		{
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
                default:
					break;
			}
            Debug.Log(consoleLog);
		}
    }
    private static string TimeStamp()
    {
        return DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
    }
}
