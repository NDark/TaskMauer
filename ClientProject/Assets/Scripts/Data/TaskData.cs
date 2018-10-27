
[System.Serializable]
public class TaskData
{
	public int TaskID;
	public string Title = string.Empty;
	public string Assignee= string.Empty;
	public long TimeStamp;
	public int ProgressInt;
	public float ProgressFloat;
	public string Link= string.Empty;
}

[System.Serializable]
public class TaskVisual
{
	public string PositionStr= string.Empty;
	public bool IsPin;
}

[System.Serializable]
public class TaskRelation
{
	public int ParentID;
	public string RelativesStr ;
	public int NeedFollowID ;

	public TaskRelative [] GetTaskRelative()
	{
		if (null == Relatives)
		{
			this.Relatives = UnityEngine.JsonUtility.FromJson<TaskRelative[]>(RelativesStr);
		}
		return this.Relatives;
	}
	public void SetRelatives( TaskRelative [] set )
	{
		this.Relatives = set;
		this.RelativesStr = UnityEngine.JsonUtility.ToJson(this.Relatives);
	}
	TaskRelative [] Relatives ;
}

[System.Serializable]
public class TaskRelative
{
	public int ID;
	public string Type= string.Empty;
}

[System.Serializable]
public class TaskBundle
{
	public TaskData Data ;
	public TaskVisual Visual ;
	public TaskRelation Relation ;
}
