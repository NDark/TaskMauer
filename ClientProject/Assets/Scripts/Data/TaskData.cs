
[System.Serializable]
public class TaskData
{
	public int TaskID;
	public string Title = string.Empty;
	public string Assignee= string.Empty;
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
	public TaskRelative [] Relatives ;
	public int NeedFollowID ;
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
	public TaskRelative Relative ;
}

