using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public enum LOG_TYPE
{
	NORMAL = 0,
	ERROR = 1,
    INFO = 2,
    NETWORK_SERVER_INFO = 3,
    NETWORK_CLIENT_INFO = 4,
    SYSTEM = 5
}
public class KojeomLogger : MonoBehaviour {

	public static void DebugLog(string log, LOG_TYPE logType = LOG_TYPE.INFO)
	{
		if(Application.platform == RuntimePlatform.WindowsEditor)
		{
			StringBuilder changedLog = new StringBuilder();
			switch (logType)
			{
				case LOG_TYPE.NORMAL:
					changedLog.AppendFormat("<color=blue><b>[NORMAL]</b></color> {0}", log);
					break;
				case LOG_TYPE.ERROR:
					changedLog.AppendFormat("<color=red><b>[ERROR]</b></color> {0}", log);
					break;
                case LOG_TYPE.INFO:
                    changedLog.AppendFormat("<color=green><b>[INFO]</b></color> {0}", log);
                    break;
                case LOG_TYPE.NETWORK_SERVER_INFO:
                    changedLog.AppendFormat("<color=#99CCFF><b>[NETWORK_SERVER_INFO]</b></color> {0}", log);
                    break;
                case LOG_TYPE.NETWORK_CLIENT_INFO:
                    changedLog.AppendFormat("<color=#6633FF><b>[NETWORK_CLIENT_INFO]</b></color> {0}", log);
                    break;
                case LOG_TYPE.SYSTEM:
                    changedLog.AppendFormat("<color=white><b>[SYSTEM]</b></color> {0}", log);
                    break;
                default:
					break;
			}
			Debug.Log(changedLog);
		}
	}
}
