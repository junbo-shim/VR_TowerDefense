using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCurve : MonoBehaviour
{
    // 텔레포트를 표시할 UI
    public Transform teleportCircleUI = default;
    // 선을 그릴 라인 렌더러
    private LineRenderer lineRenderer = default;
    // 최초 텔레포트 UI 크기
    private Vector3 originScale = Vector3.one * 0.02f;
    // 커브의 부드러운 정도
    public int lineSmooth = 40;
    // 커브의 길이
    public float curveLength = 50f;
    // 커브의 중력
    public float gravity = -60f;
    // 곡선 시뮬레이션의 간격 및 시간
    public float simulateTime = 0.02f;
    // 곡선을 이루는 점들을 기억할 리스트
    List<Vector3> lines = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        // 시작할 때 비활성화 한다.
        teleportCircleUI.gameObject.SetActive(false);
        // 라인 렌더러 컴포넌트 가져오기
        lineRenderer = GetComponent<LineRenderer>();
        // 라인 렌더러의 선 너비 지정하기
        lineRenderer.startWidth = 0.0f;
        lineRenderer.endWidth = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        // 왼쪽 컨트롤러의 One 버튼을 누르면
        if(ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 라인 렌더러 컴포넌트 활성화
            lineRenderer.enabled = true;
        }
        else if (ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 라인 렌더러 컴포넌트 비활성화
            lineRenderer.enabled = false;
            // 탤레포트 UI가 활성화 돼 있을 때
            if(teleportCircleUI.gameObject.activeSelf)
            {
                GetComponent<CharacterController>().enabled = false;
                // 텔레포트 UI 위치로 순간이동
                transform.position = teleportCircleUI.position + Vector3.up;
                GetComponent<CharacterController>().enabled = true;
            }
            // 텔레포트 UI 비활성화
            teleportCircleUI.gameObject.SetActive(false);
        }
        else if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 주어진 길이 크기의 커브를 만든다.
            MakeLines();
        }
    }

    private void MakeLines()
    {
        // 리스트에 담긴 위치 정보들을 비워준다.
        lines.RemoveRange(0, lines.Count);
        // 선이 진행될 방향을 정한다.
        Vector3 direction = ARAVRInput.LHandDirection * curveLength;
        // 선이 그려질 위치의 초깃값을 설정한다.
        Vector3 position = ARAVRInput.LHandPosition;
        // 최초 위치를 리스트에 담는다.
        lines.Add(position);

        // lineSmooth 개수만큼 반복한다.
        for(int i = 0; i < lineSmooth;  i++)
        {
            // 현재 위치 기억
            Vector3 lastPos = position;
            // 중력을 적용한 속도 계산
            // v = v0 + at
            direction.y += gravity * simulateTime;
            // 등속 운동으로 다음 위치 계산
            //P = P0 + vt
            position += direction * simulateTime;
            //Ray 충돌 체크가 일어났으면
            if(CheckHitRay(lastPos, ref position))
            {
                // 충돌 지점을 등록하고 종료
                lines.Add(position);
                break;
            }
            else
            {
                // 텔레포트 UI 비활성화
                teleportCircleUI.gameObject.SetActive(false);
            }
            // 구한 위치를 등록
            lines.Add(position);
        }
        // 라인 렌더러가 표현할 점의 개수를 등록된 개수의 크기로 할당
        lineRenderer.positionCount = lines.Count;
        // 라인 렌더러에 구해진 점의 정보를 지정
        lineRenderer.SetPositions(lines.ToArray());
    }

    //! 앞 점의 위치와 다음 점의 위치를 받아 레이의 충돌을 체크하는 함수
    private bool CheckHitRay(Vector3 lastPos, ref Vector3 position)
    {
        // 앞 점 lastPos에서 다음 점 posiiton으로 향하는 벡터 계산하는 로직
        Vector3 rayDirection = position - lastPos;
        Ray ray = new Ray(lastPos, rayDirection);
        RaycastHit hitInfo = default;

        // 레이캐스트할 때 레이의 크기를 앞 점과 다음 점 사이의 거리로 한정한다.
        if(Physics.Raycast(ray, out hitInfo, rayDirection.magnitude))
        {
            // 다음 점의 위치를 충돌한 지점으로 설정한다.
            position = hitInfo.point;

            int layer = LayerMask.NameToLayer("Terrain");
            // Terrain 레이어와 충돌했을 경우에만 텔레포트 UI가 표시되도록 한다.
            if(hitInfo.transform.gameObject.layer == layer)
            {
                // 텔레포트 UI 활성화
                teleportCircleUI.gameObject.SetActive(true);
                // 텔레포트 UI의 위치 지정
                teleportCircleUI.position = position;
                // 텔레포트 UI의 위치 지정
                teleportCircleUI.forward = hitInfo.normal;
                float distance = (position - ARAVRInput.LHandPosition).magnitude;
                // 텔레포트 UI가 보일 크기를 설정
                teleportCircleUI.localScale = originScale * Mathf.Max(1, distance);
            }
            return true;
        }

        return false;
    }
}
