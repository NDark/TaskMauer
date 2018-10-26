using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBundleHelper 
{
	public static TaskBundle CreateABundleInstance()
	{
		TaskBundle ret = new TaskBundle();
		TaskBundleHelper.TryInitalizeBundleInstance(ret);
		return ret;
	}

	public static TaskBundle CopyFromAddTaskInterfaceHelper( AddTaskInterfaceHelper ui )
	{

		TaskBundle bundleData = new TaskBundle();
		bundleData.Data = new TaskData();
		bundleData.Relation = new TaskRelation();

		bundleData.Data.Title = ui.m_TitleInput.text;
		bundleData.Data.Assignee = ui.m_AssigneeInput.text;
		bundleData.Data.Link = ui.m_LinkInput.text;

		int parentID = 0;
		int.TryParse(ui.m_ParentInput.text, out parentID);
		bundleData.Relation.ParentID = parentID;

		return bundleData;
	}
	public static void CopyBundle( TaskBundle input , TaskBundle dest )
	{
		dest.Data.TaskID = input.Data.TaskID ;
		dest.Data.Title = input.Data.Title; 
		dest.Data.Assignee = input.Data.Assignee;
		dest.Data.Link = input.Data.Link;
		dest.Relation.ParentID = input.Relation.ParentID;

	}
	public static void TryInitalizeBundleInstance( TaskBundle budnle )
	{
		if (null == budnle.Data)
		{
			budnle.Data = new TaskData();
		}
		if (null == budnle.Relation)
		{
			budnle.Relation = new TaskRelation();
		}
		if (null == budnle.Visual)
		{
			budnle.Visual = new TaskVisual();
		}

	}

	public static Vector3 ParsePositionStr( string input )
	{
		Vector3 ret = Vector3.zero;
		char[] splitor = { ',' };
		string[] strVec = input.Split(splitor, System.StringSplitOptions.RemoveEmptyEntries);
		if (strVec.Length>= 3 )
		{
			float.TryParse(strVec[0], out ret.x);
			float.TryParse(strVec[1], out ret.y);
			float.TryParse(strVec[2], out ret.z);
		}
		return ret;
	}
}
