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
		if (null == budnle.Relative)
		{
			budnle.Relative = new TaskRelative();
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
