using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;

public class AddTaskInterfaceHelper : MonoBehaviour 
{
	public InputField m_TitleInput ;
	public InputField m_AssigneeInput ;
	public InputField m_LinkInput ;
	public InputField m_ParentInput ;


	public System.Action OnPressAddButton = new System.Action( () => {} );

	public void PressAddTask()
	{
		OnPressAddButton();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
