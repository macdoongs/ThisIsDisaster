using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloodEffect : MonoBehaviour
{
	GameObject tsunamiObject = null;
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
		tsunamiObject = EventManager.Manager.MakeWorldTsunami(this.transform);
		tsunamiObject.SetActive (true);
	}

	// Update is called once per frame
	void Update()
	{
		float speed = 1.0f;
		float step = speed * Time.deltaTime;
		tsunamiObject.transform.position = Vector3.MoveTowards(tsunamiObject.transform.position, Vector3.one, step);
	}
}

