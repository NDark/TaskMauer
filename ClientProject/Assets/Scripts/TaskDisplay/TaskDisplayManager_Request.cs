using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TaskDisplayManager : MonoBehaviour 
{
	IEnumerator StartRequestModifyTask( TaskAddRequest reqObj)
	{
		string url = "http://li1440-68.members.linode.com:3102/TaskModify";

		var jsonStr = JsonUtility.ToJson(reqObj);
		Debug.Log("jsonStr" + jsonStr);
		var jsonRaw = System.Text.Encoding.UTF8.GetBytes(jsonStr);
		var uploader = new UnityEngine.Networking.UploadHandlerRaw(jsonRaw);
		uploader.contentType = "application/json; charset=utf-8";

		var req = UnityEngine.Networking.UnityWebRequest.Post( url , string.Empty ) ;
		req.uploadHandler = uploader ;
		req.SetRequestHeader("Accept", "application/json");

		yield return req.Send() ;
		if (req.isError)
		{
			Debug.LogError("StartRequestModifyTask req.error" + req.error);
			yield break;
		}
		else
		{
			StandardResponse res = null ;
			Debug.Log("StartRequestModifyTask completed response=" + req.downloadHandler.text );
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
					Debug.LogWarning("StartRequestModifyTask() res.Success=" + res.Success);
				}
				else
				{
					Debug.LogWarning("StartRequestModifyTask() null == res" );
				}

				yield break ;
			}
			else
			{
				Debug.Log("StartRequestModifyTask completed" );
				m_ModifyTaskInterfaceHelper.Clear();
				FetchTask();
			}
		}

		yield return null;
	}

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
		Debug.LogWarning("jsonStr" + jsonStr);
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
			Debug.LogError("StartRequestTaskAdd req.error=" + req.error);
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
				m_AddTaskInterfaceHelper.Clear();
				FetchTask();
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

		var updateSerial = TaskMauerStaticData.GetUpdateSerial();
		if (contentObj.UpdateSerial > updateSerial)
		{
			
			TaskMauerStaticData.SetUpdateSerial(contentObj.UpdateSerial);
		}

		if (null == contentObj.TaskVec)
		{
			Debug.LogWarning("null == contentObj.TaskVec");
			yield break;
		}
		Debug.Log("ParseTaskAddResponse contentObj.TaskVec.Length=" + contentObj.TaskVec.Length );

		for (int i = 0; i < contentObj.TaskVec.Length; ++i)
		{
			UpdateTaskBundle(contentObj.TaskVec[i]);
		}

	}


	void UpdateTaskBundle( TaskBundle inputBundle )
	{
		if (null == inputBundle)
		{
			return;
		}

		int taskID = inputBundle.Data.TaskID;
		TaskBundle previousBundle = TryFindTaskData(taskID);
		TaskVisualObj previousVisual = TryFindTaskVisual(taskID);
		TaskBundle targetBundleData = null;

		if (null == previousVisual || null == previousBundle)
		{
			if( null != previousVisual || null != previousBundle )
			{
				// fatal error.
				return;
			}
		}

		if (null != previousBundle )
		{
			Debug.LogWarning("null != previousBundle");
			targetBundleData = previousBundle;

			TaskBundleHelper.CopyBundle(inputBundle,targetBundleData);

			// update visual data
			SetTaskVisualDataFromBundle(previousVisual.m_2DHelper, targetBundleData ) ;

			// update position
			SetTaskVisual3DFromBundle(previousVisual.m_3DObj, targetBundleData);

		}
		else 
		{
			Debug.LogWarning("new bundle data");

			targetBundleData = TaskBundleHelper.CreateABundleInstance();

			TaskBundleHelper.CopyBundle(inputBundle,targetBundleData);

			CheckAndCreateTaskObj(targetBundleData);
			AddTask(targetBundleData);
			AddTaskCalculatorFromTaskBundle(targetBundleData);
		}


	}



}
