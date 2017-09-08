using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;

// ref : https://msdn.microsoft.com/ko-kr/library/system.net.httpwebrequest.begingetrequeststream(v=vs.110).aspx

/// <summary>
/// 현재 unity3d 엔진에서 stable .NET 3.5 버전에 맞춘 테스트 네트워크매니저 class.
/// </summary>
public class GameNetworkManager {

	public delegate void del_HttpRequest(bool isSuccessLogin);
	public static event del_HttpRequest PostHttpRequest;
	private readonly static string  addr = "http://127.0.0.1:8080";

	private static void GetRequestStreamCallBack(IAsyncResult asynchronousResult)
	{
		// init dummyData
		Dictionary<string, string> dummyData = new Dictionary<string, string>();
		dummyData.Add("ip", "192.168.219.0");
		dummyData.Add("user_name", "JJW");
		JSONObject jsonData = new JSONObject(dummyData);
		byte[] byteData = Encoding.ASCII.GetBytes(jsonData.ToString());
		// set Request.
		HttpWebRequest req = (HttpWebRequest)asynchronousResult.AsyncState;
		// End the operation
		Stream postStream = req.EndGetRequestStream(asynchronousResult);
		postStream.Write(byteData, 0, byteData.Length);
		postStream.Close();
		// Start the asynchronous operation to get the response
		req.ContentType = "application/json";
		req.BeginGetResponse(new AsyncCallback(GetResponseCallBack), req);
	}
	private static void GetResponseCallBack(IAsyncResult asynchronousResult)
	{
		HttpWebRequest req = (HttpWebRequest)asynchronousResult.AsyncState;
		// End the operation
		HttpWebResponse response = (HttpWebResponse)req.EndGetResponse(asynchronousResult);
		Stream streamResponse = response.GetResponseStream();
		StreamReader streamRead = new StreamReader(streamResponse);
		string responseData = streamRead.ReadToEnd();
		
		// Close the stream object
		streamResponse.Close();
		streamRead.Close();

		// Release the HttpWebResponse
		response.Close();

		if (responseData.Equals("true")) PostHttpRequest(true);
		else PostHttpRequest(false);
	}
	public static void ConnectLoginServer()
	{
		HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(addr));
		webReq.Method = HTTP_REQUEST_METHOD.POST;
		webReq.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallBack), webReq);
	}
}



public struct HTTP_REQUEST_METHOD
{
	public static string POST = "POST";
}
