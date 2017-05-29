using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatDisplay : MonoBehaviour
{
	protected Text t;
	protected float value;
	public string display = "Bits";
	public string buffer = " ";
	public bool ltr = true;

	void Awake()
	{
		t = GetComponentInChildren<Text>();
	}

	public float Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
			setString();
		}
	}

	public bool LeftToRight
	{
		get
		{
			return ltr;
		}
		set
		{
			ltr = value;
			if(ltr)
			{
				t.alignment = TextAnchor.MiddleLeft;
			}
			else
			{
				t.alignment = TextAnchor.MiddleRight;
			}
		}
	}

	public bool RightToLeft
	{
		get
		{
			return !ltr;
		}
		set
		{
			LeftToRight = !value;
		}
	}

	protected void setString()
	{
		LeftToRight = ltr;
		if(LeftToRight)
		{
			t.text = display + buffer + value;
		}
		else
		{
			t.text = value + buffer + display;
		}
	}
}
