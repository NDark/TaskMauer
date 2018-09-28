#define ENABLE_LOCAL_DATA 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TaskDisplayManager : MonoBehaviour 
{
	
	public GameObject m_Task3DParent;
	public GameObject m_Task2DParent;
	public Camera m_3DCamera ;

	public GameObject m_Task3DPrefab ;
	public GameObject m_Task2DPrefab ;

	void Awake() 
	{
#if ENABLE_LOCAL_DATA		
		InitalizeLocalTestData();
#endif // ENABLE_LOCAL_DATA
	}

	// Use this for initialization
	void Start () 
	{
		CalculateUnAssignedVisualTask();
		CalculateCameraZoomToAll();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	TaskVisualObj TryFindTaskVisual( int id )
	{
		if (m_TaskVisuals.ContainsKey(id))
		{
			return m_TaskVisuals[id];
		}
		return null;
	}

	TaskBundle TryFindTaskData( int id )
	{
		if (m_TaskData.ContainsKey(id))
		{
			return m_TaskData[id];
		}
		return null;
	}

	int CalculateRowIndex( string assignee )
	{
		int indexInTheArray = m_ExistAssignee.IndexOf(assignee);
		int assigneeIndex = (string.Empty != assignee && -1 != indexInTheArray) ? indexInTheArray+1 : 0 ;
		int y =  assigneeIndex ;
		return y;
	}

	void InitalizeLocalTestData()
	{
		int id = 0 ;
		/*
		### Task1 
		### Task2 Member1
		### Task3 parent Task2 
		### Task4 relative Task1
		### Task5 Member2
		### Task6 Member3
		### Task7 PositionStr
		### Task8 IsPin
		### Task9 follow Task4

		*/
		++id; // zero is not a valid taskid
		int task1ID = 0 ;
		int task3ID = 0 ;

		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
			task1ID = bundleData.Data.TaskID;
			bundleData.Data.Title = "Task1";
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}
		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task2";
			bundleData.Data.Assignee = "Member1";
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}
		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			task3ID = bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task3";
			bundleData.Relation.ParentID = task1ID;// task1 
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}

		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task4";
			bundleData.Relation.Relatives = new TaskRelative[1];
			bundleData.Relation.Relatives[0] = new TaskRelative();
			bundleData.Relation.Relatives[0].ID = 0;
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}
		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task5";
			bundleData.Data.Assignee = "Member2";
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}
		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task6";
			bundleData.Data.Assignee = "Member3";
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}


		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task7";
			CheckAndCreateTaskObj(bundleData);
			bundleData.Relation.ParentID = task3ID;// task3ID 
			AddTask(bundleData);
		}
	}

	void CheckAndCreateTaskObj(TaskBundle bundleData)
	{
		if (null == bundleData)
		{
			return; 
		}
		if (!m_TaskVisuals.ContainsKey(bundleData.Data.TaskID))
		{
			CreateTaskObj(bundleData);
		}
	}

	void CreateTaskObj( TaskBundle bundleData )
	{
		TaskVisualObj visual = new TaskVisualObj();

		visual.m_3DObj = GameObject.Instantiate(m_Task3DPrefab , m_Task3DParent.transform );

		// init 2d 
		var obj2d = GameObject.Instantiate( m_Task2DPrefab , m_Task2DParent.transform ) ;

		visual.m_2DHelper = obj2d.AddComponent<TaskVidual2DObjectHelper>();
		visual.m_2DHelper.Setup();
		visual.m_2DHelper.UpdateLinkURL(bundleData.Data.Link);
		visual.m_2DHelper.UpdateTitle(bundleData.Data.Title);

		var task2dupdate = obj2d.AddComponent<Task2DUpdateWith3D>();
		task2dupdate.Setup(visual.m_3DObj, m_3DCamera);

		m_TaskVisuals.Add(bundleData.Data.TaskID, visual);
	}

	void AddTask( TaskBundle bundleData )
	{
		string assignee = bundleData.Data.Assignee;
		if (string.Empty != bundleData.Data.Assignee && -1 == m_ExistAssignee.IndexOf(assignee))
		{
			m_ExistAssignee.Add(assignee);
		}
		m_TaskData.Add(bundleData.Data.TaskID, bundleData);
	}

	void CalculateCameraZoomToAll()
	{
		Vector3 sum = Vector3.zero;
		if (m_TaskVisuals.Count <= 0 )
		{
			return;
		}
		float minX = float.MaxValue ;
		float maxX = float.MinValue ;
		float minY = float.MaxValue ;
		float maxY = float.MinValue ;

		int count = m_TaskVisuals.Count;
		var taskVisual = m_TaskVisuals.GetEnumerator();
		while (taskVisual.MoveNext())
		{
			var obj = taskVisual.Current.Value;
			Vector3 pos = obj.m_3DObj.transform.position;
			if (pos.x > maxX)
			{
				maxX = pos.x;
			}
			if (pos.x < minX)
			{
				minX = pos.x;
			}

			if (pos.y > maxY)
			{
				maxY = pos.y;
			}
			if (pos.y < minY)
			{
				minY = pos.y;
			}

		
		}
		sum.x = (maxX + minX) * 0.5f;
		sum.y = (maxY + minY) * 0.5f;

		float distanceInX = maxX - minX;
		float distanceInY = maxY - minY;

		float maxLength = (distanceInX>distanceInY) ?distanceInX : distanceInY;
		// check zoom
		float lenghHalfBall = 0.5f;
		float suggestedRatio = 1.1f;
		float suggestLength =( maxLength + lenghHalfBall*2) * suggestedRatio ;
		var suggestedFarDistance = suggestLength * 0.5f / Mathf.Tan(m_3DCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

		float nearPlane = m_3DCamera.nearClipPlane * 1.1f;
		if( suggestedFarDistance < nearPlane )
		{
			suggestedFarDistance = nearPlane;
		}

		float farPlane = m_3DCamera.farClipPlane * 0.9f;
		if( suggestedFarDistance > farPlane )
		{
			suggestedFarDistance = farPlane;
		}


		sum.z = -1 * suggestedFarDistance ;

		m_3DCamera.transform.position = sum;
	}

	public class TaskVisualObj
	{
		public GameObject m_3DObj ;
		public TaskVidual2DObjectHelper m_2DHelper ;
	}

	Dictionary<int,TaskVisualObj> m_TaskVisuals = new Dictionary<int, TaskVisualObj>() ;
	Dictionary<int,TaskBundle> m_TaskData = new Dictionary<int, TaskBundle>();
	List<string> m_ExistAssignee = new List<string>();
}

