using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;

public class AddTaskInterfaceHelper : MonoBehaviour 
{
	public InputField m_TitleInput ;
	public InputField m_AssigneeInput ;
	public InputField m_LinkInput ;
	public Dropdown m_TypeDropDown;
	public Slider m_ProgressFloatSlider;
	public InputField m_ParentInput ;


	public System.Action OnPressAddButton = new System.Action( () => {} );

	public void PressAddTask()
	{
		OnPressAddButton();
	}

	public void Clear()
	{
		m_TitleInput.text = "";
		m_AssigneeInput.text = "";
		m_LinkInput.text = "";
		m_TypeDropDown.value = 0;
		m_ProgressFloatSlider.value = 0;
		m_ParentInput.text = "";
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
