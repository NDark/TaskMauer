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
	public Dropdown m_ProgressIntDropDown;
	public Text m_ProgressText;
	public Slider m_ProgressFloatSlider;
	public InputField m_ParentInput ;


	public System.Action OnPressAddButton = new System.Action( () => {} );

	public void ProgressOnChanged()
	{
		m_ProgressText.text = ((int)(m_ProgressFloatSlider.value)).ToString() ;
	}

	public void PressAddTask()
	{
		OnPressAddButton();
	}

	public void SetProgressFloat( float value)
	{
		m_ProgressFloatSlider.value = value;
		m_ProgressText.text = ((int)(value)).ToString() ;
	}

	public void Clear()
	{
		m_TitleInput.text = "";
		m_AssigneeInput.text = "";
		m_LinkInput.text = "";
		m_TypeDropDown.value = 0;
		m_ProgressIntDropDown.value = 0;
		SetProgressFloat(0);
		m_ParentInput.text = "";
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
