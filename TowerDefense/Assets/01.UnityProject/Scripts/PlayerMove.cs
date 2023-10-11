using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 캐릭터 컨트롤러 컴포넌트
    private CharacterController characterController;

    // 이동속도
    public float speed = 5f;
    // 점프 크기
    public float jumpPower = 5f;
    // 중력 관련 변수
    public float gravity = -20f;
    float yVelocity = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.zero;
        // 업데이트에 중력을 적용하는 로직
        yVelocity += gravity * Time.deltaTime;

        if(characterController.isGrounded)
        {
            yVelocity = 0f;
        }

        // 사용자가 점프 버튼을 누르면 속도에 점프 크기를 할당한다.
        if(ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
        {
            yVelocity = jumpPower;
        }

        direction.y = yVelocity;
        characterController.Move(direction * speed * Time.deltaTime);
    }
}
