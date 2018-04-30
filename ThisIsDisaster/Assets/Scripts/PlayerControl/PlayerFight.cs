using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFight : MonoBehaviour {
	
	void OnCollisionEnter (Collision collision)
	{
		Debug.Log ("Col", collision.collider.gameObject);

		if(collision.collider.gameObject.tag == "Enemy")
		{
			Destroy(collision.collider.gameObject);
		}
	}

	void OnTriggerEnter(Collider collider){
		Debug.Log ("Col", collider.gameObject);

		if(collider.transform.tag == "Enemy"){ 
			Destroy(collider.gameObject);
		}
	}
}
