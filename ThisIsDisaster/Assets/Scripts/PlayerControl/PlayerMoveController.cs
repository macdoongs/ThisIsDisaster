using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour {
    public float moveSpeed = 1f;

    void Update() {
        Vector3 currentPos = transform.position;
        if (Input.GetKey(KeyCode.W))
        {
            MoveUp(ref currentPos);
        }

        if (Input.GetKey(KeyCode.S))
        {
            MoveDown(ref currentPos);
        }

        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft(ref currentPos);
        }

        if (Input.GetKey(KeyCode.D))
        {
            MoveRight(ref currentPos);
        }
    }
    //deltaTime : 프레임에 렉이 걸린만큼 값이 커져 프레임렉을 보정
    void MoveUp(ref Vector3 pos)
    {
        transform.Translate(0,moveSpeed * Time.deltaTime, 0);
    }

    void MoveDown(ref Vector3 pos)
    {

        transform.Translate(0, -moveSpeed * Time.deltaTime, 0);
    }

    void MoveLeft(ref Vector3 pos)
    {

        transform.Translate(-moveSpeed * Time.deltaTime, 0, 0);
    }

    void MoveRight(ref Vector3 pos)
    {

        transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
    }
}