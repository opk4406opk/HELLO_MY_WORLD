using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public enum LOG_TYPE
{
	NORMAL = 0,
	ERROR = 1
}
public class KojeomLogger : MonoBehaviour {

	public static void DebugLog(string log, LOG_TYPE logType = LOG_TYPE.NORMAL)
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
				default:
					break;
			}
			Debug.Log(changedLog);
		}
	}
}
