using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TaskDisplayManager : MonoBehaviour 
{
	IEnumerator StartRequestFetchTasks( TaskUpdateRequestBase reqObj)
	{
		string url = "http://li1440-68.members.linode.com:3102/FetchTasks";


		var jsonStr = JsonUtility.ToJson(reqObj);

		var jsonRaw = System.Text.Encoding.UTF8.GetBytes(jsonStr);
		var uploader = new UnityEngine.Networking.UploadHandlerRaw(jsonRaw);
		uploader.contentType = "application/json; charset=utf-8";

		var req = UnityEngine.Networking.UnityWebRequest.Post( url , string.Empty ) ;
		req.uploadHandler = uploader ;
		req.SetRequestHeader("Accept", "application/json");

		yield return req.Send() ;
		if (req.isError)
		{
			Debug.LogError("StartRequestFetchTasks req.error" + req.error);
			yield break;
		}
		else
		{
			StandardResponse res = null ;
			Debug.Log("StartRequestFetchTasks completed response=" + req.downloadHandler.text );
			try 
			{
				res = JsonUtility.FromJson<StandardResponse>( req.downloadHandler.text ) ;
			}
			catch
			{
				yield break ;
			}

			if( null == res || res.Success == false )
			{
				if (null != res)
				{
					Debug.LogWarning("StartRequestFetchTasks() res.Success=" + res.Success);
				}
				else
				{
					Debug.LogWarning("StartRequestFetchTasks() null == res" );
				}

				yield break ;
			}
			else
			{
				StartCoroutine(ParseFetchTask(res));
			}
		}

		yield return null;
	}
	IEnumerator StartRequestTaskAdd( TaskAddRequest reqObj )
	{
		string url = "http://li1440-68.members.linode.com:3102/TaskAdd";


		var jsonStr = JsonUtility.ToJson(reqObj);

		var jsonRaw = System.Text.Encoding.UTF8.GetBytes(jsonStr);
		var uploader = new UnityEngine.Networking.UploadHandlerRaw(jsonRaw);
		uploader.contentType = "application/json; charset=utf-8";

		var req = UnityEngine.Networking.UnityWebRequest.Post( url , string.Empty ) ;
		req.uploadHandler = uploader ;
		req.SetRequestHeader("Accept", "application/json");
		/*
		* StartRequestTaskAdd completed response={
		"Success": true,
		"Code": 0,
		"Message": "",
		"Key": "",
		"Content": "{\"UpdateSerial\":0,\"RequestSerial\":0}"}
		*/
		yield return req.Send() ;
		if (req.isError)
		{
			Debug.LogError("StartRequestTaskAdd req.error" + req.error);
			yield break;
		}
		else
		{
			StandardResponse res = null ;
			Debug.Log("StartRequestTaskAdd completed response=" + req.downloadHandler.text );
			try 
			{
				res = JsonUtility.FromJson<StandardResponse>( req.downloadHandler.text ) ;
			}
			catch
			{
				yield break ;
			}

			if (null == res || res.Success == false)
			{
				if (null != res)
				{
					Debug.LogWarning("StartRequestTaskAdd() res.Success=" + res.Success);
				}
				else
				{
					Debug.LogWarning("StartRequestTaskAdd() null == res");
				}

				yield break;
			}
			else
			{
				TaskUpdateRequestBase fetchReq = new TaskUpdateRequestBase() ;
				StartCoroutine(StartRequestFetchTasks(fetchReq));
			}
		}

		yield return null;
	}

	IEnumerator ParseFetchTask( StandardResponse res) 
	{
		if (null == res)
		{
			yield break;
		}

		TasksResponse contentObj = null ;
		string content = res.Content;
		try 
		{
			contentObj = JsonUtility.FromJson<TasksResponse>( content ) ;
		}
		catch
		{
			yield break ;
		}
		yield return null;
		Debug.Log("ParseTaskAddResponse contentObj.UpdateSerial=" + contentObj.UpdateSerial );
		Debug.Log("ParseTaskAddResponse contentObj.RequestSerial=" + contentObj.RequestSerial );
	}

}
