using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LandslideEffect : MonoBehaviour
{
	GameObject landslideObject = null;
	public int level = 0;
	public void SetLevel(int level)
	{
		this.level = level;
	}

	public void SetActive(bool state)
	{
		gameObject.SetActive(state);
	}

	// Use this for initialization
	void Start()
	{
		landslideObject = EventManager.Manager.MakeWorldLandslide(this.transform);
		landslideObject.SetActive (true);
	}

	// Update is called once per frame
	void Update()
	{
		float speed = 1.0f;
		float step = speed * Time.deltaTime;
		landslideObject.transform.position = Vector3.MoveTowards(landslideObject.transform.position, Vector3.one, step);
	}
}

