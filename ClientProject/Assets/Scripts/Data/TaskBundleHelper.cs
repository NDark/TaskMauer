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


	public static TaskBundle CopyFromModifyTaskInterfaceHelper( ModifyTaskInterfaceHelper ui , TaskBundle srcBundle )
	{
		TaskBundle modifyBundle = new TaskBundle();
		modifyBundle.Data = new TaskData();
		modifyBundle.Relation = new TaskRelation();
		modifyBundle.Visual = new TaskVisual();

		CopyBundle(srcBundle, modifyBundle);

		modifyBundle.Data.Title = ui.m_TitleInput.text;
		modifyBundle.Data.Assignee = ui.m_AssigneeInput.text;
		modifyBundle.Data.Link = ui.m_LinkInput.text;
		modifyBundle.Data.Type = ui.m_TypeDropDown.value;
		modifyBundle.Data.ProgressInt = ui.m_ProgressIntDropDown.value;
		modifyBundle.Data.ProgressFloat = ui.m_ProgressFloatSlider.value;


		int parentID = 0;
		int.TryParse(ui.m_ParentInput.text, out parentID);
		modifyBundle.Relation.ParentID = parentID;

		modifyBundle.Visual.IsPin = ui.m_IsPin.isOn;

		return modifyBundle;
	}

	public static TaskBundle CopyFromAddTaskInterfaceHelper( AddTaskInterfaceHelper ui )
	{

		TaskBundle bundleData = new TaskBundle();
		bundleData.Data = new TaskData();
		bundleData.Relation = new TaskRelation();

		bundleData.Data.Title = ui.m_TitleInput.text;
		bundleData.Data.Assignee = ui.m_AssigneeInput.text;
		bundleData.Data.Link = ui.m_LinkInput.text;
		bundleData.Data.Type = ui.m_TypeDropDown.value;
		bundleData.Data.ProgressInt = ui.m_ProgressIntDropDown.value;
		bundleData.Data.ProgressFloat = ui.m_ProgressFloatSlider.value;


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
		dest.Data.Type = input.Data.Type;
		dest.Data.ProgressFloat = input.Data.ProgressFloat;
		dest.Data.ProgressInt = input.Data.ProgressInt;
		dest.Relation.ParentID = input.Relation.ParentID;
		dest.Relation.SetRelatives( input.Relation.GetTaskRelative() ) ;
		dest.Relation.NeedFollowID = input.Relation.NeedFollowID;

		dest.Visual.PositionStr = input.Visual.PositionStr;
		dest.Visual.IsPin = input.Visual.IsPin;

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
