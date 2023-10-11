using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // 총알 관련 변수
    public Transform bulletImpact;
    ParticleSystem bulletEffect;
    AudioSource bulletAudio;

    // 조준점 관련 변수
    public Transform crosshair;

    // Start is called before the first frame update
    void Start()
    {
        // 총알 파티클 시스템 컴포넌트 가져오기
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        bulletAudio = bulletImpact.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // 크로스헤어 표시
        ARAVRInput.DrawCrosshair(crosshair);

        // 사용자가 IndexTrigger 버튼을 누르면
        if(ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            bulletAudio.Stop();
            bulletAudio.Play();
            // Ray가 카메라의 위치로부터 나가도록 만든다.
            Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
            // Ray의 충돌 정보를 저장하기 위한 변수 지정
            RaycastHit hitInfo;
            // 플레이어 레이어 얻어오기
            int playerLayer = 1 << LayerMask.NameToLayer("Player");
            // 타워 레이어 얻어오기
            int towerLayer = 1 << LayerMask.NameToLayer("Tower");
            int layerMask = playerLayer | towerLayer;
            // Ray를 쏜다. ray가 부딪힌 정보는 hitInfo에 담긴다.
            if(Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                // 총알 파편 효과 처리
                bulletEffect.Stop();
                bulletEffect.Play();
                // 부딪힌 지점 바로 위에서 이펙트가 보이도록 설정
                bulletImpact.position = hitInfo.point;
                bulletImpact.forward = hitInfo.normal;
            }
        }
    }
}
