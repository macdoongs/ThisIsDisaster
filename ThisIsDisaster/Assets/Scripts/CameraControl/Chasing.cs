using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chasing : MonoBehaviour {
    public GameObject Player;
    public GameObject Target;
    public float CameraZ = -10; // 카메라 높이 -10으로 고정
	// Update와 다르게 프레임 상관없이 업데이트
	void FixedUpdate () {
        Vector3 TargetPos = new Vector3(Target.transform.position.x, Target.transform.position.y, CameraZ);
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * 2f);
	}
}
