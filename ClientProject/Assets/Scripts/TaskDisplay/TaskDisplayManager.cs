﻿// #define ENABLE_LOCAL_DATA 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TaskDisplayManager : MonoBehaviour 
{
	const float CONST_CAMERA_NEAREST_Z = -1;
	const float CONST_ZOOM_SPEED = 1.0f;
	const float CONST_CLICK_SEC = 0.3f;
	const float CONST_PAN_SPEED = 0.5f ;
	const float CONST_DRAG_OBJECT_SPEED = 1.5f ;
	public GameObject m_Task3DParent;
	public GameObject m_Task2DParent;
	public Camera m_3DCamera ;

	public GameObject m_Task3DPrefab ;
	public GameObject m_Task2DPrefab ;
	public Task2DUpdateWith3D m_SelectionHelper ;
	public AddTaskInterfaceHelper m_AddTaskInterfaceHelper;
	public ModifyTaskInterfaceHelper m_ModifyTaskInterfaceHelper;

	void Awake() 
	{
#if ENABLE_LOCAL_DATA		
		InitalizeLocalTestData();
#endif // ENABLE_LOCAL_DATA

		m_MouseIsDownTimer.IntervalSec = CONST_CLICK_SEC;
	}

	public void ConfirmModifyTask()
	{
		if (null == m_ModifyTaskInterfaceHelper)
		{
			return;
		}

		TaskVisualObj visual = TryFindTaskVisual(m_SelectedObjTaskID);
		TaskBundle bundle = TryFindTaskData(m_SelectedObjTaskID);
		if (null != bundle && null != visual )
		{
			// uploading apply change to task

			UpdateyDataToVisual(bundle , visual);
		}

		m_ModifyTaskInterfaceHelper.gameObject.SetActive(false);
	}

	public void AddTask()
	{
		Debug.LogWarning("AddTask");

		if (null == m_AddTaskInterfaceHelper)
		{
			return;
		}

		{
			TaskBundle bundle = TaskBundleHelper.CopyFromAddTaskInterfaceHelper(m_AddTaskInterfaceHelper);
			// prepare bundle to upload string
			TaskAddRequest req = new TaskAddRequest() ;
			req.RequestSerial = m_RequestSerial++;
			req.UpdateSerial = m_UpdateSerial; 
			req.Task = bundle;

			StartCoroutine(StartRequestTaskAdd(req));
		}

		m_AddTaskInterfaceHelper.gameObject.SetActive(false);
	}

	// Use this for initialization
	void Start () 
	{
		SetupStructrue();

		CalculateUnAssignedVisualTask();
		CalculateCameraZoomToAll();

	}

	public TaskVisualObj TryFindTaskVisual( int id )
	{
		if (m_TaskVisuals.ContainsKey(id))
		{
			return m_TaskVisuals[id];
		}
		return null;
	}

	// Update is called once per frame
	void Update () 
	{
		CheckInput();
		
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
		### Task7 parent Task3
		### Task8 PositionStr
		### Task9 parent Task3 PositionStr
		### Task10 follow Task4

		*/
		++id; // zero is not a valid taskid
		int task1ID = 0 ;
		int task2ID = 0 ;
		int task3ID = 0 ;

		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
			task1ID = bundleData.Data.TaskID;
			bundleData.Data.Title = "Task1";
			bundleData.Data.Link = "www.google.com.tw";
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}
		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			task2ID = bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task2";
			bundleData.Data.Assignee = "Member1";
			bundleData.Data.Link = "www.google.com.tw";
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}
		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			task3ID = bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task3";
			bundleData.Relation.ParentID = task1ID;// task1 
			bundleData.Data.Link = "www.google.com.tw";
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
			bundleData.Data.Link = "www.google.com.tw";
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}
		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task5";
			bundleData.Data.Assignee = "Member2";
			bundleData.Data.Link = "www.google.com.tw";
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}
		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task6";
			bundleData.Data.Assignee = "Member3";
			bundleData.Data.Link = "www.google.com.tw";
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}


		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task7";
			bundleData.Data.Link = "www.google.com.tw";
			bundleData.Relation.ParentID = task3ID;// task3ID 
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}

		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task8";
			bundleData.Visual.PositionStr = "2.0,0.5,0";
			CheckAndCreateTaskObj(bundleData);
			AddTask(bundleData);
		}

		{
			TaskBundle bundleData = TaskBundleHelper.CreateABundleInstance();
			bundleData.Data.TaskID = id++;
			bundleData.Data.Title = "Task9";
			bundleData.Visual.PositionStr = "3.0,0.5,0";
			bundleData.Relation.ParentID = task2ID;// task2ID 
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
		visual.m_3DObj.name = bundleData.Data.TaskID.ToString();

		// init 2d 
		var obj2d = GameObject.Instantiate( m_Task2DPrefab , m_Task2DParent.transform ) ;
		obj2d.name = visual.m_3DObj.name;

		visual.m_2DHelper = obj2d.AddComponent<TaskVidual2DObjectHelper>();
		visual.m_2DHelper.Setup();
		visual.m_2DHelper.UpdateLinkURL(bundleData.Data.Link);
		visual.m_2DHelper.UpdateTitle(bundleData.Data.Title);
		visual.m_2DHelper.UpdateAssignee(bundleData.Data.Assignee);

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

	void CheckInput()
	{
		bool mouseGoesUp = Input.GetMouseButtonUp(0);
		if (m_MouseIsDownTimer.IsActive 
			&& mouseGoesUp
		)
		{
			if (!m_MouseIsDownTimer.IsReady(Time.time))
			{
				MouseClick(Input.mousePosition);
			}
			else
			{
				// mosue release
				MouseRelease() ;
			}
		}


		if (m_MouseIsDownTimer.IsActive
			&& m_MouseIsDownTimer.IsReady(Time.time ))
		{
			if (IsSelected())
			{
				DragSelectObject(Input.mousePosition);
			}
			else
			{
				DragScene(Input.mousePosition);
			}
		}

		if( Input.GetMouseButtonDown(0))
		{
			m_MouseIsDownTimer.Rewind(Time.time);

		}

		if (Input.mouseScrollDelta.y != 0)
		{
			CameraZoomIn(Input.mousePosition, Input.mouseScrollDelta.y );
		}

		bool mouseButtonIsDown = Input.GetMouseButton(0);
		m_MouseIsDownTimer.Active(mouseButtonIsDown);

		if (mouseButtonIsDown)
		{
			m_InputMousePositionPrevious = Input.mousePosition;
		}
	}

	void DragSelectObject(Vector3 inputMousePosition)
	{
		if (IsSelected())
		{
			TaskVisualObj visual = TryFindTaskVisual(m_SelectedObjTaskID);
			if (null != visual)
			{
				m_SelectedObjDraged = true;
				Vector3 delta = inputMousePosition - m_InputMousePositionPrevious;
				visual.m_3DObj.transform.Translate( CONST_DRAG_OBJECT_SPEED * delta* Time.deltaTime );	
			}
		}

	}

	void DragScene(Vector3 inputMousePosition)
	{
		Vector3 delta = inputMousePosition - m_InputMousePositionPrevious;
		m_3DCamera.transform.Translate(-1 * CONST_PAN_SPEED * delta* Time.deltaTime );
	}

	bool IsUIEmpty()
	{
		bool ret = !m_AddTaskInterfaceHelper.gameObject.activeSelf 
			&& !m_ModifyTaskInterfaceHelper.gameObject.activeSelf 
			;
		return ret;
	}
	void MouseClick( Vector3 inputMousePosition )
	{
		if (!IsUIEmpty())
		{
			return;
		}

		if (this.IsSelected() )
		{
			m_SelectedObjTaskID = 0;
			ShowSelectionGUI( false );
		}
		else
		{
			// check click
			var taskID = CheckClickVisual( Input.mousePosition ) ;
			SelectATask(taskID);
		}
	}

	void UpdateyDataToVisual( TaskBundle bundle , TaskVisualObj visual )
	{
		if (null == bundle)
		{
			return;
		}
		if (null == visual)
		{
			return;
		}

		bundle.Data.Title = m_ModifyTaskInterfaceHelper.m_TitleInput.text;
		bundle.Data.Assignee = m_ModifyTaskInterfaceHelper.m_AssigneeInput.text;
		bundle.Data.Link = m_ModifyTaskInterfaceHelper.m_LinkInput.text;
		int.TryParse(m_ModifyTaskInterfaceHelper.m_ParentInput.text, out bundle.Relation.ParentID);

		visual.m_2DHelper.UpdateTitle( bundle.Data.Title ) ;
		visual.m_2DHelper.UpdateAssignee( bundle.Data.Assignee ) ;
		visual.m_2DHelper.UpdateLinkURL( bundle.Data.Link ) ;

	}

	void FetchDataToEditor( TaskBundle bundle )
	{
		if (null == bundle)
		{
			return;
		}
		if (null == m_ModifyTaskInterfaceHelper)
		{
			return;
		}

		m_ModifyTaskInterfaceHelper.m_TaskID.text = bundle.Data.TaskID.ToString();
		m_ModifyTaskInterfaceHelper.m_TitleInput.text = bundle.Data.Title ;
		m_ModifyTaskInterfaceHelper.m_AssigneeInput.text = bundle.Data.Assignee ;
		m_ModifyTaskInterfaceHelper.m_LinkInput.text = bundle.Data.Link;
		m_ModifyTaskInterfaceHelper.m_ParentInput.text = bundle.Relation.ParentID.ToString();

		m_ModifyTaskInterfaceHelper.gameObject.SetActive(true);
	}

	void SelectATask(int selectTaskID )
	{
		m_SelectedObjTaskID = selectTaskID;
		TaskBundle bundle = TryFindTaskData(m_SelectedObjTaskID);
		TaskVisualObj visual = TryFindTaskVisual(m_SelectedObjTaskID);

		if (null != visual)
		{
			FetchDataToEditor(bundle);
			ShowSelectionGUI(true);
			UpdateSelectionGUI(visual);
		}
	}
	int CheckClickVisual( Vector3 inputMousePos )
	{
		int ret = 0;
		var ray = m_3DCamera.ScreenPointToRay(inputMousePos);
		var hits = Physics.RaycastAll(ray);
		if (hits.Length > 0)
		{
			for (int i = 0; i < hits.Length; ++i)
			{
				
				int id = 0;
				if (int.TryParse(hits[i].collider.gameObject.name, out id))
				{
					

					if (m_TaskVisuals.ContainsKey(id))
					{
						ret = id;	
						return ret;
					}

				}
			}
		}
		return ret ;
	}

	void ShowSelectionGUI( bool show )
	{
		if (m_SelectionHelper)
		{
			m_SelectionHelper.gameObject.SetActive(show);
			m_SelectionHelper.enabled = show;
		}

		if ( false == show )
		{
			UpdateSelectionGUI(null);
		}
	}

	void UpdateSelectionGUI( TaskVisualObj visual )
	{
		if (null == visual)
		{
			m_SelectionHelper.m_Target = null ;
			return;
		}
		m_SelectionHelper.m_Camera = this.m_3DCamera;
		m_SelectionHelper.m_Target = visual.m_3DObj;

	}

	void CameraZoomIn(Vector3 inputMousePosition , float zoomInValue )
	{
		Ray ray = m_3DCamera.ScreenPointToRay(inputMousePosition);

		Vector3 zoomVec = ray.direction * zoomInValue * CONST_ZOOM_SPEED;
		Vector3 destination = m_3DCamera.transform.position + zoomVec;
		if (destination.z > CONST_CAMERA_NEAREST_Z)
		{
			return;
		}
		m_3DCamera.transform.Translate(zoomVec);

	}

	void SetupStructrue()
	{

		m_AddTaskInterfaceHelper.OnPressAddButton += AddTask;
		m_ModifyTaskInterfaceHelper.OnPressModifyButton += ConfirmModifyTask;

		ShowSelectionGUI(false);
	}

	void MouseRelease() 
	{
		if (m_SelectedObjDraged  )
		{
			if (IsSelected())
			{
				// update position to data
				TaskBundle bundle = TryFindTaskData( m_SelectedObjTaskID ) ;
				TaskVisualObj visual = TryFindTaskVisual(m_SelectedObjTaskID);
				if ( null != visual && null != bundle)
				{
					Vector3 pos = visual.m_3DObj.transform.localPosition;
					// update packet

					bundle.Visual.PositionStr = string.Format("{0}{1}{2}",pos.x , pos.y, pos.z  );
					bundle.Visual.IsPin = true;

				}
			}
			m_SelectedObjDraged = false;
		}

	}
	bool IsSelected()
	{
		return (0 != m_SelectedObjTaskID);
	}
	bool m_SelectedObjDraged = false ;
	int m_SelectedObjTaskID = 0 ;
	CountDownTimer m_MouseIsDownTimer = new CountDownTimer();
	Vector3 m_InputMousePositionPrevious = Vector3.zero ;

	public class TaskVisualObj
	{
		public GameObject m_3DObj ;
		public TaskVidual2DObjectHelper m_2DHelper ;
	}

	Dictionary<int,TaskVisualObj> m_TaskVisuals = new Dictionary<int, TaskVisualObj>() ;
	Dictionary<int,TaskBundle> m_TaskData = new Dictionary<int, TaskBundle>();
	List<string> m_ExistAssignee = new List<string>();

	int m_RequestSerial = 0 ;
	int m_UpdateSerial = 0 ;
}

