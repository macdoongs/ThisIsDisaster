using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    public Joystick joystick;   //조이스틱 스크립트
    public float MoveSpeed;     //플레이어 이동속도
    
    private Vector3 _moveVector; //플레이어 이동벡터
    private Transform _transform;

    void Start()
    {
        _transform = transform;      //Transform 캐싱
        _moveVector = Vector3.zero;  //플레이어 이동벡터 초기화
    }

    void Update()
    {
        //터치패드 입력 받기
        HandleInput();
    }

    void FixedUpdate()
    {
        //플레이어 이동
        Move();
    }

    public void HandleInput()
    {
        _moveVector = PoolInput();
    }

    public Vector3 PoolInput()
    {
        float h = joystick.GetHorizontalValue();
        float v = joystick.GetVerticalValue();
        Vector3 moveDir = new Vector3(h, v, 0).normalized;

        return moveDir;
    }

    public void Move()
    {
        _transform.Translate(_moveVector * MoveSpeed * Time.deltaTime);
    }
}
