using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task2DParentRegion : MonoBehaviour 
{
	public TaskDisplayManager m_System = null ;
	public int m_ParentTaskID = 0 ;
	public List<int> m_ChildrenTaskIDs = new List<int>();

	void Awake() 
	{
		m_UpdateTimer.IntervalSec = 1;

	}
	public void SetupParentPanel(TaskDisplayManager manager , int parentID , List<int> childrenIDList )
	{
		m_System = manager;
		m_ParentTaskID = parentID;
		m_ChildrenTaskIDs = childrenIDList ;
		CollectTasks();
	}

	// Use this for initialization
	void Start () 
	{
		m_ParentRect = this.transform.parent.GetComponent<RectTransform>();
		m_Self = this.GetComponent<RectTransform>();
		if (null == m_Self || null == m_ParentRect )
		{
			this.enabled = false;
		}

		if (null == m_System)
		{
			this.enabled = false;
		}

		if (0 == m_ParentTaskID)
		{
			this.enabled = false;
		}

		if (0 == m_ChildrenTaskIDs.Count )
		{
			this.enabled = false;
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_UpdateTimer.IsReady(Time.time))
		{
			bool valid = (null != m_System && 0 != m_ParentTaskID && 0 != m_ChildrenTaskIDs.Count);
			if (!valid)
			{
				this.enabled = false;
				return ;
			}

			CalculateRegion();

			m_UpdateTimer.Rewind(Time.time);
		}

	}

	void CollectTasks()
	{
		List<RectTransform> childrenRegions = new List<RectTransform>();
		if ( null != m_System && 0 != m_ParentTaskID && 0 != m_ChildrenTaskIDs.Count )
		{
			TaskDisplayManager.TaskVisualObj parent = m_System.TryFindTaskVisual(m_ParentTaskID);
			if (null == parent || parent.m_2DHelper == null )
			{
				return;
			}
			childrenRegions.Add(parent.m_2DHelper.SelfRect);

			foreach (var i in m_ChildrenTaskIDs)
			{
				TaskDisplayManager.TaskVisualObj child = m_System.TryFindTaskVisual(i);

				if ( null != child && null !=  child.m_2DHelper)
				{
					childrenRegions.Add(child.m_2DHelper.SelfRect);
				}
			}
		}


		m_ChildrenRegions = childrenRegions;
	}

	void CalculateRegion()
	{
		
		Vector3 min=new Vector3(float.MaxValue,float.MaxValue,0);
		Vector3 max=new Vector3(float.MinValue,float.MinValue,0);

		foreach (var i in m_ChildrenRegions)
		{
			Vector3[] worldCorners= new Vector3[4];
			i.GetWorldCorners(worldCorners);
			foreach (var worldCorner in worldCorners)
			{
				Vector3 local = m_ParentRect.InverseTransformPoint( worldCorner );

				if (local.x < min.x)
				{
					min.x = local.x;
				}
				if (local.x > max.x)
				{
					max.x = local.x;
				}

				if (local.y < min.y)
				{
					min.y = local.y;
				}
				if (local.y > max.y)
				{
					max.y = local.y;
				}
			}


		}

		m_Self.anchoredPosition = new Vector2((min.x+max.x)*0.5f, (min.y+max.y)*0.5f);
		m_Self.sizeDelta = new Vector2(max.x - min.x, max.y - min.y );
	}

	RectTransform m_ParentRect ;
	RectTransform m_Self ;
	List<RectTransform> m_ChildrenRegions = new List<RectTransform>();
	CountDownTimer m_UpdateTimer = new CountDownTimer() ;
}
