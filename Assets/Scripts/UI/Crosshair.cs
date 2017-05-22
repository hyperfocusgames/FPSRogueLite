using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
	public static Crosshair instance;

	public float gap = 10f;
	public float width = 3f;
	public float height = 6f;
	public float scale = 2f;
	public Color color;
	public Image style;

	private GameObject crosshairs;

	void Start()
	{
		if(instance != null)
		{
			Destroy(this);
			return;
		}

		instance = this;

		createContainer();
	}

	private void createContainer()
	{
		crosshairs = new GameObject();
		crosshairs.transform.parent = transform;
		crosshairs.transform.localPosition = Vector3.zero;
		crosshairs.name = "Crosshairs Container";
	}

	public void generate()
	{
		Image ch;

		ch = Instantiate(style, Vector3.zero, Quaternion.identity, crosshairs.transform);
		ch.transform.localPosition = new Vector3(0, gap, 0);
		ch.color = color;
		ch.transform.localScale = new Vector3(scale, scale, 1);
		ch.rectTransform.sizeDelta = new Vector2(width, height);

		ch = Instantiate(style, Vector3.zero, Quaternion.identity, crosshairs.transform);
		ch.transform.localPosition = new Vector3(0, -gap, 0);
		ch.color = color;
		ch.transform.localScale = new Vector3(scale, scale, 1);
		ch.rectTransform.sizeDelta = new Vector2(width, height);

		ch = Instantiate(style, Vector3.zero, Quaternion.identity, crosshairs.transform);
		ch.transform.localPosition = new Vector3(gap, 0, 0);
		ch.color = color;
		ch.transform.localScale = new Vector3(scale, scale, 1);
		ch.rectTransform.sizeDelta = new Vector2(height, width);

		ch = Instantiate(style, Vector3.zero, Quaternion.identity, crosshairs.transform);
		ch.transform.localPosition = new Vector3(-gap, 0, 0);
		ch.color = color;
		ch.transform.localScale = new Vector3(scale, scale, 1);
		ch.rectTransform.sizeDelta = new Vector2(height, width);
	}

	public void degenerate()
	{
		Destroy(crosshairs);
		createContainer();
	}
}
