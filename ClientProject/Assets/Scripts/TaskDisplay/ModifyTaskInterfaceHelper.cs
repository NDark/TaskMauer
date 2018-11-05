using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;

public class ModifyTaskInterfaceHelper : MonoBehaviour 
{
	public Text m_TaskID ;
	public InputField m_TitleInput ;
	public InputField m_AssigneeInput ;
	public InputField m_LinkInput ;
	public InputField m_ParentInput ;
	public Toggle m_IsPin ;

	public System.Action OnPressModifyButton = new System.Action( () => {} );

	public void PressModify()
	{
		OnPressModifyButton();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
