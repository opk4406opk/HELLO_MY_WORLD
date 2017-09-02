using System;
using System.IO;
using System.Net;
using System.Text;

/// <summary>
/// 현재 unity3d 엔진에서 stable .NET 3.5 버전에 맞춘 테스트 네트워크매니저 class.
/// </summary>
public class GameNetworkManager {

	private readonly static string  addr = "http://127.0.0.1:8080";
	
	public static void ConnectLoginServer()
	{
		HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(addr));
		webReq.Method = HTTP_REQUEST_METHOD.POST;
		byte[] testData = Encoding.ASCII.GetBytes("test-data-here");
		webReq.ContentType = "text/plain";
		webReq.ContentLength = testData.Length;

		using (Stream dataStream = webReq.GetRequestStream())
		{
			dataStream.Write(testData, 0, testData.Length);
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
