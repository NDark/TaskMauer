﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TaskDisplayManager : MonoBehaviour 
{
	void AddTaskPositionHelper( TaskBundle bundle )
	{
		if (null != bundle)
		{
			m_TaskCalculator.Add(bundle.Data.TaskID, new TaskPositionHelper
				{
					Bundle = bundle
				});
		}
	}

	void CalculateUnAssignedVisualTask()
	{

		// create helper for all tasks.
		m_TaskCalculator.Clear() ;

		{
			var taskI = m_TaskData.GetEnumerator();
			while (taskI.MoveNext())
			{
				AddTaskPositionHelper(taskI.Current.Value);
			}
		}

		// calculate the parent depth
		CalculateParentDepth(m_TaskCalculator) ;

		// calculate the XSpace for each task
		{
			var taskJ = m_TaskData.GetEnumerator();
			while (taskJ.MoveNext())
			{
				var data = taskJ.Current.Value;

				// calculate space of this task. 
				// based on the children count.
				float childNum = 0 ;

				var taskChild = m_TaskData.GetEnumerator();
				while (taskChild.MoveNext())
				{
					var dataChild = taskChild.Current.Value;

					if (dataChild != data && dataChild.Relation.ParentID == data.Data.TaskID )
					{
						++childNum;
					}
				}

				if (childNum < 2)// child number 0 and 1 is treated as 0 (size 1)
				{
					childNum = 0; 
				}
				m_TaskCalculator[data.Data.TaskID].XSpace = childNum + 1 ;
			}
		}

		{
			var taskK = m_TaskData.GetEnumerator();
			while (taskK.MoveNext())
			{
				var data = taskK.Current.Value;
				m_TaskCalculator[data.Data.TaskID].IsSetPosition = (data.Visual.PositionStr != string.Empty);
			}
		}


		List<int> rowIndiceSet = new List<int>();

		// calculate y
		{
			var taskL = m_TaskData.GetEnumerator();
			while (taskL.MoveNext())
			{
				var data = taskL.Current.Value;
				if(false== m_TaskCalculator[data.Data.TaskID].IsSetPosition)
				{
					int rowIndex = CalculateRowIndex(data.Data.Assignee);		
					m_TaskCalculator[data.Data.TaskID].RowIndex = rowIndex;
					if (-1 == rowIndiceSet.IndexOf(rowIndex))
					{
						rowIndiceSet.Add(rowIndex);
					}
				}


			}
		}

		List<int> parentIDSet = new List<int>();

		// collect parent id
		{
			var taskL = m_TaskData.GetEnumerator();
			while (taskL.MoveNext())
			{
				var data = taskL.Current.Value;
				int taskParenID = m_TaskCalculator[data.Data.TaskID].Bundle.Relation.ParentID;
				if(false== m_TaskCalculator[data.Data.TaskID].IsSetPosition && taskParenID != 0 )
				{
					if (-1 == parentIDSet.IndexOf(m_TaskCalculator[data.Data.TaskID].Bundle.Relation.ParentID))
					{
						
						parentIDSet.Add(taskParenID);
					}
				}


			}
		}

		Dictionary<float,List <int> > sortedForEachParent = new Dictionary<float, List<int>>();


		// for each parent id
		{
			var parentIDEnum = parentIDSet.GetEnumerator();
			while (parentIDEnum.MoveNext())
			{
				List<int> taskInTheSameParentID = new List<int>();
				var taskM = m_TaskData.GetEnumerator();
				while (taskM.MoveNext())
				{
					var data = taskM.Current.Value;
					if (false == m_TaskCalculator[data.Data.TaskID].IsSetPosition)
					{
						if (data.Relation.ParentID == parentIDEnum.Current)
						{
							taskInTheSameParentID.Add(data.Data.TaskID);
						}
					}
				}

				if (taskInTheSameParentID.Count > 0)
				{
					TaskVisualObj visualNode = TryFindTaskVisual(parentIDEnum.Current);
					TaskVidual2DObjectHelper visual = visualNode.m_2DHelper;
					visual.SetupParentPanel(this,parentIDEnum.Current,taskInTheSameParentID );
				}

				SortVisualTaskInARow(taskInTheSameParentID);

				sortedForEachParent.Add(parentIDEnum.Current, taskInTheSameParentID);
			}
		}

		// according to each sorted list, assigned position
		foreach (var row in sortedForEachParent.Values)
		{
			float tempX = 0;
			foreach (var taskID in row )
			{
				// calculate the size of each task 
				TaskBundle task = m_TaskData[taskID];
				TaskPositionHelper calculator = m_TaskCalculator[taskID];

				TaskVisualObj visual = TryFindTaskVisual(taskID);
				TaskVisualObj visualParent = TryFindTaskVisual(task.Relation.ParentID);
				if(null!= visual && null != visualParent )
				{
					visual.m_3DObj.transform.SetParent(visualParent.m_3DObj.transform);
					visual.m_3DObj.transform.localPosition = new Vector3( tempX , 0.7f , 1 ) ;
				}

				tempX += calculator.XSpace;
			}
		}

		Dictionary<float,List <int> > sortedForEachYRow = new Dictionary<float, List<int>>();

		// for each y 
		{
			var rowIndex = rowIndiceSet.GetEnumerator();
			while (rowIndex.MoveNext())
			{
				List<int> taskInTheSameRow = new List<int>();
				var taskM = m_TaskData.GetEnumerator();
				while (taskM.MoveNext())
				{
					var data = taskM.Current.Value;
					if (false == m_TaskCalculator[data.Data.TaskID].IsSetPosition)
					{
						int taskParenID = m_TaskCalculator[data.Data.TaskID].Bundle.Relation.ParentID;
						if (m_TaskCalculator[data.Data.TaskID].RowIndex == rowIndex.Current 
							&& 0 == taskParenID )
						{
							taskInTheSameRow.Add(data.Data.TaskID);
						}
					}
				}

				SortVisualTaskInARow(taskInTheSameRow);

				sortedForEachYRow.Add(rowIndex.Current, taskInTheSameRow);
			}
		}


		float tempY = 0;
		// according to each sorted list, assigned position
		foreach (var row in sortedForEachYRow.Values)
		{
			float maxYSpace = 0;
			float tempX = 0;
			foreach (var taskID in row )
			{
				// calculate the size of each task 
				TaskBundle task = m_TaskData[taskID];
				TaskPositionHelper calculator = m_TaskCalculator[taskID];

				TaskVisualObj visual = TryFindTaskVisual(taskID);

				if(null!= visual)
				{
					visual.m_3DObj.transform.localPosition = new Vector3( tempX , tempY , 0 ) ;
				}

				if (calculator.YSpace > maxYSpace)
				{
					maxYSpace = calculator.YSpace; 
				}

				tempX += calculator.XSpace;
			}

			tempY += maxYSpace;
		}
	}



	void SortVisualTaskInARow( List<int> sortingIndexInARow )
	{
		for (int i = 0; i < sortingIndexInARow.Count; ++i)
		{
			for( int j = i+1 ; j < sortingIndexInARow.Count ; ++j )
			{
				TaskBundle taskI = m_TaskData[sortingIndexInARow[i]];
				TaskBundle taskJ = m_TaskData[sortingIndexInARow[j]];

				// check time 
				if (taskI.Data.TimeStamp > taskJ.Data.TimeStamp)
				{

				}
				else if (taskI.Data.TimeStamp == taskJ.Data.TimeStamp)
				{
					// check relation
				}

			}
		}

	}

	void CalculateAllDepthToRoot( Dictionary<int, TaskPositionHelper > taskCalculaor  ) 
	{
		// find the depth of each task (not consider children node)
		var taskJ = taskCalculaor.GetEnumerator();
		while (taskJ.MoveNext())
		{
			var calculator = taskJ.Current.Value;
			calculator.DepthToRoot = FindParentDepthByThisNode( taskCalculaor, taskJ.Current.Key ) ;
		}
	}

	void CalculateParentDepthTilParent( Dictionary<int, TaskPositionHelper > taskCalculaor
		, TaskPositionHelper helper , int tempDepth ) 
	{
		var tempTask = helper ;

		var bottomDepth = tempDepth+1;

		while (0 != tempTask.Bundle.Relation.ParentID)
		{

			var parent = taskCalculaor[tempTask.Bundle.Relation.ParentID];
			if (tempDepth-1 >= parent.DepthToRoot)
			{
				parent.DepthToRoot = tempDepth -1;
				parent.ParentDepth = bottomDepth - parent.DepthToRoot;
			}

			--tempDepth;

			tempTask = parent;
		}
	}

	void CalculateParentDepthTilParent( Dictionary<int, TaskPositionHelper > taskCalculaor ) 
	{
		var taskK = taskCalculaor.GetEnumerator();
		while (taskK.MoveNext())
		{
			var calculator = taskK.Current.Value;

			// try increment parents' depth
			CalculateParentDepthTilParent( taskCalculaor , calculator , calculator.DepthToRoot ) ;
		}
	}

	void CalculateYSpaceFromParentDepth( TaskPositionHelper helper ) 
	{
		helper.YSpace = helper.ParentDepth + 1;
	}


	void CalculateParentDepth( Dictionary<int, TaskPositionHelper > taskCalculaor ) 
	{
		CalculateAllDepthToRoot(taskCalculaor);

		CalculateParentDepthTilParent(taskCalculaor);

		var taskL = taskCalculaor.GetEnumerator();
		while (taskL.MoveNext())
		{
			var calculator = taskL.Current.Value;
			CalculateYSpaceFromParentDepth(calculator);
		}
	}


	int FindParentDepthByThisNode( Dictionary<int, TaskPositionHelper > taskCalculatorMap , int id )
	{
		int ret = 0 ;

		TaskPositionHelper thisTask = taskCalculatorMap[id];
		while ( 0 != thisTask.Bundle.Relation.ParentID )
		{
			thisTask = taskCalculatorMap[thisTask.Bundle.Relation.ParentID];
			++ret;
		}
		return ret ;
	}

	Dictionary<int, TaskPositionHelper > m_TaskCalculator = new Dictionary<int, TaskPositionHelper>() ;

}

[System.Serializable]
public class TaskPositionHelper
{
	public TaskBundle Bundle ;
	public float XSpace = 1 ;
	public float YSpace = 1 ;
	public int RowIndex = 0;// YIndex
	public int DepthToRoot = 0;
	public int ParentDepth = 0;

	public bool IsSetPosition = false ;
}

