using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TaskDisplayManager : MonoBehaviour 
{
	void CalculateUnAssignedVisualTask()
	{

		// create helper for all tasks.
		m_TaskCalculator.Clear() ;
		{
			var taskI = m_TaskData.GetEnumerator();
			while (taskI.MoveNext())
			{
				var data = taskI.Current.Value;
				m_TaskCalculator.Add(data.Data.TaskID, new TaskPositionHelper
					{
						Bundle = data
					});
			}
		}

		// calculate the XSpace for each task
		{
			var taskJ = m_TaskData.GetEnumerator();
			while (taskJ.MoveNext())
			{
				var data = taskJ.Current.Value;

				// calculate space of this task. 
				// based on the children count.
				float x = 1 ;

				m_TaskCalculator[data.Data.TaskID].XSpace = x ;
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
					visual.m_3DObj.transform.localPosition = new Vector3( tempX , -0.5f , 1 ) ;
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
						if (m_TaskCalculator[data.Data.TaskID].RowIndex == rowIndex.Current && 0 == taskParenID )
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


	Dictionary<int, TaskPositionHelper > m_TaskCalculator = new Dictionary<int, TaskPositionHelper>() ;

}

[System.Serializable]
public class TaskPositionHelper
{
	public TaskBundle Bundle ;
	public float XSpace = 1 ;
	public float YSpace = 1 ;
	public int RowIndex = 0;// YIndex
	public int ParentDepth = 0;

	public bool IsSetPosition = false ;
}

