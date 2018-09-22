#define ENABLE_LOCAL_DATA 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskDisplayManager : MonoBehaviour 
{
	public GameObject m_TaskParent;
	public GameObject m_Task3DPrefab ;

	void Awake() 
	{
#if ENABLE_LOCAL_DATA		
		InitalizeLocalTestData();
#endif // ENABLE_LOCAL_DATA
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void InitalizeLocalTestData()
	{
		TaskBundle bundleData = new TaskBundle();
		TaskBundleHelper.TryInitalizeBundleInstance(bundleData);
		int id = 0 ;
		bundleData.Data.TaskID = id++ ;
		m_TaskData.Add(bundleData.Data.TaskID,bundleData);
		CheckAndCreateTaskObj(bundleData);

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

		visual.m_3DObj = GameObject.Instantiate(m_Task3DPrefab,m_TaskParent.transform );
		// init 2d 

		m_TaskVisuals.Add(bundleData.Data.TaskID, visual);
	}


	public class TaskVisualObj
	{
		public GameObject m_3DObj ;
		public GameObject m_2DObj ;
	}

	Dictionary<int,TaskVisualObj> m_TaskVisuals = new Dictionary<int, TaskVisualObj>() ;
	Dictionary<int,TaskBundle> m_TaskData = new Dictionary<int, TaskBundle>();
}

