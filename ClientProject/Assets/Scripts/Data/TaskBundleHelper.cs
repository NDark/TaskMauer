using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBundleHelper 
{
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
}
