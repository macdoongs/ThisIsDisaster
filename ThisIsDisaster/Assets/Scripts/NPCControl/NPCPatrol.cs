using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour {

	public float speed;
	private float waitTime;
	public float startWaitTime;

	public Transform[] moveSpots;
	private int randomSpot;

	void Start () {
		waitTime = startWaitTime;
		randomSpot = Random.Range (0, moveSpots.Length);
	}

	void Awake(){

	}

	void Update () {
		transform.position = Vector2.MoveTowards (transform.position, moveSpots [randomSpot].position, speed * Time.deltaTime);

		if (Vector2.Distance (transform.position, moveSpots [randomSpot].position) < 0.2f) {
			if (waitTime <= 0) {
				randomSpot = Random.Range (0, moveSpots.Length);
				waitTime = startWaitTime;
			} else {
				waitTime -= Time.deltaTime;
			}
		}

		Transform tf = PlayerMoveController.Player.transform;

		float px = tf.position.x;
		float py = tf.position.y;
		float pz = tf.position.z;

		float nx = this.transform.position.x;
		float ny = this.transform.position.y;
		float nz = this.transform.position.z;

		/*
		if (nx - 1 <= px && px <= nx + 1 && ny - 1 <= py && py <= ny + 1 && nz - 1 <= pz && pz <= nz + 1) {
			Destroy (this.gameObject);
		}*/
	}

	private void OnTriggerEnter2D(Collider2D collision){
		Debug.Log (collision.gameObject.name);
		//Destroy (gameObject);

	}
}
