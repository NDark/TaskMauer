using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task2DUpdateWith3D : MonoBehaviour 
{
	public GameObject m_Target = null;
	public Camera m_Camera = null ;
	public RectTransform m_Self ;

	public void Setup(GameObject target, Camera camera)
	{
		m_Target = target;
		m_Camera = camera;
	}

	// Use this for initialization
	void Start () 
	{
		m_Self = this.GetComponent<RectTransform>();
		if (null == m_Self)
		{
			this.enabled = false;
		}

		if (null == m_Target)
		{
			this.enabled = false;
		}

		if (null == m_Camera)
		{
			this.enabled = false;
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (null != m_Camera && null!= m_Target && null!= m_Self )
		{
			CoordinateTools.UpdateRectFrom3DWorldPos( m_Camera
				, m_Target.transform.position 
				, m_Self
			);
		}

	}
}
