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
	public Dropdown m_TypeDropDown;
	public Dropdown m_ProgressIntDropDown;
	public Text m_ProgressText;
	public Slider m_ProgressFloatSlider;
	public InputField m_ParentInput ;
	public Toggle m_IsPin ;

	public System.Action OnPressModifyButton = new System.Action( () => {} );

	public void PressModify()
	{
		OnPressModifyButton();
	}

	public void ProgressOnChanged()
	{
		m_ProgressText.text = ((int)(m_ProgressFloatSlider.value)).ToString() ;
	}

	public void SetProgressFloat( float value)
	{
		m_ProgressFloatSlider.value = value;
		m_ProgressText.text = ((int)(value)).ToString() ;
	}

	public void ResetProgressIntDropDownList()
	{
		m_ProgressIntDropDown.ClearOptions();
		List<string> list = new List<string>();
		for (int i = 0; i <=(int) ProgressIntType.OnHold; ++i)
		{
			list.Add(ProgressIntTypeStringMapHelper.GetString(i));
		}
		m_ProgressIntDropDown.AddOptions(list);
	}

	public void Clear()
	{
		m_TaskID.text = "";
		m_TitleInput.text = "";
		m_AssigneeInput.text = "";
		m_LinkInput.text = "";
		m_TypeDropDown.value = 0;
		m_ProgressIntDropDown.value = 0;
		m_ProgressText.text = "";
		SetProgressFloat(0);
		m_ParentInput.text = "";
		m_IsPin.isOn = false;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
