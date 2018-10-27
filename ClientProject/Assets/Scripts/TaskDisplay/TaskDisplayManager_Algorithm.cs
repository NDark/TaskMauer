using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TaskDisplayManager : MonoBehaviour 
{
	void AddTaskCalculatorFromTaskBundle(TaskBundle bundle)
	{
		TaskPositionHelper helper = InsertTaskPositionHelper(bundle);
		if (null != helper)
		{
			helper.DepthToRoot = FindDepthToRootByThisNode( m_TaskCalculator, bundle.Data.TaskID ) ;

			CalculateParentDepthTilParent( m_TaskCalculator , helper , helper.DepthToRoot ) ;

			CalculateYSpaceFromParentDepth(helper);

			CalculateXSpace(m_TaskData, helper);

			helper.IsSetPosition = (helper.Bundle.Visual.PositionStr != string.Empty);

			CheckAndAssignRowIndex(m_TaskCalculator, helper);

			List<int> sortingArray = new List<int>();
			if (0 != bundle.Relation.ParentID)
			{
				// sor
				sortingArray = CreateSortingArrayForParentIDSet(m_TaskData, bundle.Relation.ParentID);
				SetParentAndLocalPositionInArray(m_TaskData, m_TaskCalculator, sortingArray);
			}
			else
			{
				bool addRowMaxY = false;
				sortingArray = CreateSortingArrayForEachRowIndex(m_TaskData , m_TaskCalculator , helper.RowIndex );
				if (!m_RowToPosY.ContainsKey(helper.RowIndex))
				{
					addRowMaxY = true;
					m_RowToPosY.Add(helper.RowIndex, m_RowMaxYNow);
				}

				float maxYSpace = 0;

				SetLocalPositionInArray( sortingArray , m_RowToPosY[helper.RowIndex] , out maxYSpace);
				if (addRowMaxY)
				{
					m_RowMaxYNow += maxYSpace;
				}

			}

		}

	}

	TaskPositionHelper InsertTaskPositionHelper( TaskBundle bundle )
	{
		if (null != bundle)
		{
			m_TaskCalculator.Add(bundle.Data.TaskID, new TaskPositionHelper
				{
					Bundle = bundle
				});
			return m_TaskCalculator[bundle.Data.TaskID] ;
		}
		return null ;
	}

	void CalculateXSpace( Dictionary<int,TaskBundle> taskBundle , TaskPositionHelper taskHelper )
	{
		var data = taskHelper.Bundle;

		// calculate space of this task. 
		// based on the children count.
		float childNum = 0 ;

		var taskChild = taskBundle.GetEnumerator();
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
		taskHelper.XSpace = childNum + 1 ;
	}

	void CalculateXSpaceForAllTasks( Dictionary<int,TaskBundle> taskBundle , Dictionary<int, TaskPositionHelper > taskHelpers )
	{

		// calculate the XSpace for each task
		var taskJ = taskBundle.GetEnumerator();
		while (taskJ.MoveNext())
		{
			var data = taskJ.Current.Value;
			CalculateXSpace(m_TaskData, taskHelpers[data.Data.TaskID]);
		}
	}

	List<int> CreateSortingArrayForParentIDSet( Dictionary<int,TaskBundle> tasks , int parentID )
	{
	
		List<int> taskInTheSameParentID = new List<int>();

		var taskM = tasks.GetEnumerator();
		while (taskM.MoveNext())
		{
			var data = taskM.Current.Value;
			if (data.Relation.ParentID == parentID)
			{
				taskInTheSameParentID.Add(data.Data.TaskID);
			}
		}

		if (taskInTheSameParentID.Count > 0)
		{
			TaskVisualObj visualNode = TryFindTaskVisual(parentID);
			if (null != visualNode)
			{
				TaskVidual2DObjectHelper visual = visualNode.m_2DHelper;
				visual.SetupParentPanel(this,parentID,taskInTheSameParentID );
			}
		}

		SortVisualTaskInARow(taskInTheSameParentID);

		return taskInTheSameParentID; 

	}

	void SetParentAndLocalPositionInArray( Dictionary<int,TaskBundle> tasks , Dictionary<int, TaskPositionHelper > helpers , List<int> row )
	{
		float tempX = 0;
		foreach (var taskID in row )
		{
			// calculate the size of each task 
			TaskBundle task = tasks[taskID];
			TaskPositionHelper calculator = helpers[taskID];

			TaskVisualObj visual = TryFindTaskVisual(taskID);
			TaskVisualObj visualParent = TryFindTaskVisual(task.Relation.ParentID);
			if(null!= visual && null != visualParent )
			{
				visual.m_3DObj.transform.SetParent(visualParent.m_3DObj.transform);
				if (!calculator.IsSetPosition)
				{
					visual.m_3DObj.transform.localPosition = new Vector3( tempX , 0.7f , 1 ) ;
				}

				// Debug.LogWarning("visual.m_3DObj.transform.localPosition" + visual.m_3DObj.transform.localPosition );
			}

			tempX += calculator.XSpace;
		}
	}

	void SetLocalPositionInArray(List <int> row , float yPos , out float maxYSpace )
	{
		maxYSpace = 0.0f;
		float tempX = 0;

		foreach (var taskID in row )
		{
			// calculate the size of each task 
			TaskBundle task = m_TaskData[taskID];
			TaskPositionHelper calculator = m_TaskCalculator[taskID];
			TaskVisualObj visual = TryFindTaskVisual(taskID);

			if(null!= visual && !calculator.IsSetPosition )
			{
				visual.m_3DObj.transform.localPosition = new Vector3( tempX , yPos , 0 ) ;
			}

			if (calculator.YSpace > maxYSpace)
			{
				maxYSpace = calculator.YSpace; 
			}

			tempX += calculator.XSpace;
		}

	}

	List<int> CreateSortingArrayForEachRowIndex( Dictionary<int,TaskBundle> tasks , Dictionary<int, TaskPositionHelper > helpers , int rowIndex )
	{
		List<int> taskInTheSameRow = new List<int>();

		var taskM = tasks.GetEnumerator();
		while (taskM.MoveNext())
		{
			var data = taskM.Current.Value;
			if (false == helpers[data.Data.TaskID].IsSetPosition)
			{
				int taskParenID = helpers[data.Data.TaskID].Bundle.Relation.ParentID;
				if (m_TaskCalculator[data.Data.TaskID].RowIndex == rowIndex 
					&& 0 == taskParenID )
				{
					taskInTheSameRow.Add(data.Data.TaskID);
				}
			}
		}

		SortVisualTaskInARow(taskInTheSameRow);

		return taskInTheSameRow;
	}

	void CalculateUnAssignedVisualTask()
	{

		// create helper for all tasks.
		m_TaskCalculator.Clear() ;

		{
			var taskI = m_TaskData.GetEnumerator();
			while (taskI.MoveNext())
			{
				InsertTaskPositionHelper(taskI.Current.Value);
			}
		}

		// calculate the parent depth
		CalculateParentDepth(m_TaskCalculator) ;

		CalculateXSpaceForAllTasks(m_TaskData, m_TaskCalculator);

		{
			var taskK = m_TaskData.GetEnumerator();
			while (taskK.MoveNext())
			{
				var data = taskK.Current.Value;
				m_TaskCalculator[data.Data.TaskID].IsSetPosition = (data.Visual.PositionStr != string.Empty);
				var visual = TryFindTaskVisual(data.Data.TaskID);
				visual.m_3DObj.transform.localPosition = TaskBundleHelper.ParsePositionStr(data.Visual.PositionStr);
			}
		}

		CheckAllRowIndexArray();

		List<int> parentIDSet = new List<int>();

		// collect parent id
		{
			var taskL = m_TaskData.GetEnumerator();
			while (taskL.MoveNext())
			{
				var data = taskL.Current.Value;
				int taskParenID = m_TaskCalculator[data.Data.TaskID].Bundle.Relation.ParentID;
				if( taskParenID != 0 )
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
				List<int> taskInTheSameParentID = CreateSortingArrayForParentIDSet(m_TaskData , parentIDEnum.Current );
				sortedForEachParent.Add(parentIDEnum.Current, taskInTheSameParentID);
			}
		}

		// according to each sorted list, assigned position
		foreach (var row in sortedForEachParent.Values)
		{
			SetParentAndLocalPositionInArray(m_TaskData , m_TaskCalculator , row );
		}

		Dictionary<int,List <int> > sortedForEachYRow = new Dictionary<int, List<int>>();

		// for each row index  
		{
			var rowIndex = m_RowIndiceSet.GetEnumerator();
			while (rowIndex.MoveNext())
			{
				List<int>  taskInTheSameRow = CreateSortingArrayForEachRowIndex(m_TaskData , m_TaskCalculator , rowIndex.Current );
				sortedForEachYRow.Add(rowIndex.Current, taskInTheSameRow);
			}
		}

		float tempY = 0;
		// according to each sorted list, assigned position
		foreach (var row in sortedForEachYRow)
		{
			m_RowToPosY.Add(row.Key, tempY);

			float maxYSpace = 0;
			SetLocalPositionInArray( row.Value , tempY , out maxYSpace);
			tempY += maxYSpace;
		}
		m_RowMaxYNow = tempY;

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
			calculator.DepthToRoot = FindDepthToRootByThisNode( taskCalculaor, taskJ.Current.Key ) ;
		}
	}

	void CalculateParentDepthTilParent( Dictionary<int, TaskPositionHelper > taskCalculaor
		, TaskPositionHelper helper , int tempDepth ) 
	{
		var tempTask = helper ;

		var bottomDepth = tempDepth+1;

		while (0 != tempTask.Bundle.Relation.ParentID)
		{

			if (taskCalculaor.ContainsKey(tempTask.Bundle.Relation.ParentID))
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
			else 
			{
				break;
			}
		
		}
	}

	void CalculateAllParentDepthTilParent( Dictionary<int, TaskPositionHelper > taskCalculaor ) 
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

		CalculateAllParentDepthTilParent(taskCalculaor);

		var taskL = taskCalculaor.GetEnumerator();
		while (taskL.MoveNext())
		{
			var calculator = taskL.Current.Value;
			CalculateYSpaceFromParentDepth(calculator);
		}
	}


	int FindDepthToRootByThisNode( Dictionary<int, TaskPositionHelper > taskCalculatorMap , int id )
	{
		int ret = 0 ;

		TaskPositionHelper thisTask = taskCalculatorMap[id];
		while ( 0 != thisTask.Bundle.Relation.ParentID && taskCalculatorMap.ContainsKey(thisTask.Bundle.Relation.ParentID) )
		{
			thisTask = taskCalculatorMap[thisTask.Bundle.Relation.ParentID];
			++ret;
		}
		return ret ;
	}

	void CheckAndAssignRowIndex( Dictionary<int, TaskPositionHelper > taskHelperVec , TaskPositionHelper helper )
	{
		
		var data = helper.Bundle;
		{
			int rowIndex = CalculateRowIndex(data.Data.Assignee);		
			// Debug.LogWarning("data.Data.Assignee="+data.Data.Assignee+" : rowIndex=" +rowIndex);
			helper.RowIndex = rowIndex;
			if (-1 == m_RowIndiceSet.IndexOf(rowIndex))
			{
				m_RowIndiceSet.Add(rowIndex);
			}
		}
	}

	void CheckAllRowIndexArray()
	{

		var taskL = m_TaskData.GetEnumerator();
		while (taskL.MoveNext())
		{
			var data = taskL.Current.Value;
			CheckAndAssignRowIndex(m_TaskCalculator, m_TaskCalculator[data.Data.TaskID]);
		}

	}

	Dictionary<int, TaskPositionHelper > m_TaskCalculator = new Dictionary<int, TaskPositionHelper>() ;

	List<int> m_RowIndiceSet = new List<int>();
	Dictionary<int,float> m_RowToPosY = new Dictionary<int, float>() ;
	float m_RowMaxYNow = 0.0f ;
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

