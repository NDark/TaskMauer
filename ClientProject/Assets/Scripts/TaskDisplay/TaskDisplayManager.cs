#define ENABLE_LOCAL_DATA 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskDisplayManager : MonoBehaviour 
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

	Vector3 CalculatePosition( string assignee )
	{
		Vector2 ret = new Vector2();
		const float rowHeight = 1.0f ;

		int indexInTheArray = m_ExistAssignee.IndexOf(assignee);
		int assigneeIndex = (string.Empty != assignee && -1 != indexInTheArray) ? indexInTheArray+1 : 0 ;
		float y = rowHeight * assigneeIndex ;


		ret.y = y;
		return ret;
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
		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
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
			bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task3";
			bundleData.Relation.ParentID = 0;
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

	void CalculateUnAssignedVisualTask()
	{
		var taskData = m_TaskData.GetEnumerator();
		while (taskData.MoveNext())
		{
			var data = taskData.Current.Value;
			if (data.Visual.PositionStr == string.Empty )
			{
				Vector3 pos = CalculatePosition(data.Data.Assignee);
				TaskVisualObj visual = TryFindTaskVisual(data.Data.TaskID);
				if (null != visual)
				{
					visual.m_3DObj.transform.position = pos;
				}
			}
		}
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

			sum += pos;
		}
		sum /= count;

		// check zoom
		sum.z = m_3DCamera.transform.position.z;


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

