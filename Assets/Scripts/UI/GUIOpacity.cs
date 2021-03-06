﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[RequireComponent(typeof(CanvasGroup))]
public class GUIOpacity : MonoBehaviour
{
	protected const string PPSTR = "GUIOpacity";
	protected const float DEFAULT = 1f;
	protected const float MIN = .001f;
	protected static float alphaMult = 1f;

	void Awake()
	{
		if(!PlayerPrefs.HasKey(PPSTR))
		{
			PlayerPrefs.SetFloat(PPSTR, DEFAULT);
		}
		AlphaMult = PlayerPrefs.GetFloat(PPSTR);
	}

	protected void updateAlphas(float v)
	{
		foreach(CanvasGroup cg in GetComponentsInChildren<CanvasGroup>())
		{
			if(cg.alpha < MIN)
			{
				cg.alpha = MIN;
			}
			cg.alpha *= v;
		}
	}

	public static void updateAll(float v)
	{
		foreach(GUIOpacity go in GameObject.FindObjectsOfType<GUIOpacity>())
		{
			go.updateAlphas(v);
		}
	}

	public static float AlphaMult
	{
		get
		{
			return alphaMult;
		}
		set
		{
			if(value == 0)
			{
				updateAll(0);
				alphaMult = MIN;
			}
			// If the value is being changed, reset alphas and set to new values
			if(value != AlphaMult)
			{
				updateAll(1/AlphaMult);	// Reset alphas
				alphaMult = value;
				updateAll(AlphaMult);	// Set to new value
				PlayerPrefs.SetFloat(PPSTR, AlphaMult);
			}
		}
	}
}

[InitializeOnLoad][ExecuteInEditMode]
[CustomEditor(typeof(GUIOpacity))]
public class GUIOpacityEditor : Editor
{
	float a = GUIOpacity.AlphaMult;
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		a = EditorGUILayout.Slider("Alpha Multiplier", a, 0f, 1);
		GUIOpacity.AlphaMult = a;
	}
}
