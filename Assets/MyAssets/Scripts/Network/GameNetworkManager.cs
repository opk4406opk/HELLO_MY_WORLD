using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// 현재 unity3d 엔진에서 stable .NET 3.5 버전에 맞춘 테스트 네트워크매니저 class.
/// </summary>
public class GameNetworkManager {

	private readonly static string  addr = "http://127.0.0.1:8080";
	
	public static void ConnectLoginServer()
	{
		Dictionary<string, string> dummyData = new Dictionary<string, string>();
		dummyData.Add("ip", "192.168.219.0");
		dummyData.Add("user_name", "JJW");
		JSONObject jsonData = new JSONObject(dummyData);

		HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(addr));
		byte[] byteData = Encoding.ASCII.GetBytes(jsonData.ToString());
		webReq.Method = HTTP_REQUEST_METHOD.POST;
		webReq.ContentType = "application/json";
		webReq.ContentLength = byteData.Length;

		using (Stream dataStream = webReq.GetRequestStream())
		{
			dataStream.Write(byteData, 0, byteData.Length);
		}

		using(WebResponse webResponse = webReq.GetResponse())
		{
			webResponse.Close();
		}
	}
}

public struct HTTP_REQUEST_METHOD
{
	public static string POST = "POST";
}
