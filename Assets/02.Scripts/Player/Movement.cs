using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Movement : MonoBehaviourPun, IPunObservable
{
    private CharacterController controller;
    private new Transform transform; //기존의 transform 메소드를 숨긴다. 해당 기능으로 사용.
    private Animator animator;
    private new Camera camera;
    private PhotonView pv = null;

    private Plane plane; //가상의 Plane에 레이캐스팅 하기위한 변수
    private Ray ray;
    private Vector3 hitPoint;
    private Vector3 receivePos;
    private Quaternion receiveRot;

    private float moveSpeed = 10.0f;
    private float damping = 10.0f;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        pv.Synchronization = ViewSynchronization.UnreliableOnChange;
        pv.ObservedComponents[0] = this;

        transform = GetComponent<Transform>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        camera = Camera.main;

        plane = new Plane(transform.up, transform.position); //가상의 바닥을 player오브젝트 위치를 기준으로 생성한다. (플레이어 오브젝트의 기준점은 바닥에 있으므로 plane은 위를 보고 바닥에 생성된다.)
    }
    void Update()
    {
        if (pv.IsMine) //로컬 클라이언트 일때 움직임 조정 가능.
        {
            Move();
            Turn();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, receivePos, Time.deltaTime * damping);//선형 보간, 부드럽게 이동
            transform.rotation = Quaternion.Lerp(transform.rotation, receiveRot, Time.deltaTime * damping);//부드럽게 화면을 돌린다.
        }
    }

    float h => Input.GetAxis("Horizontal"); //이후 new Input System으로 바꾼다.
    float v => Input.GetAxis("Vertical"); //이후 new Input System으로 바꾼다.
    void Move()
    {
        Vector3 cameraForward = camera.transform.forward; //메인 카메라가 바라보는 방향을 벡터에 저장한다.
        Vector3 cameraRight = camera.transform.right; //메인 카메라가 바라보는 방향의 오른쪽 방향을 벡터에 저장한다.
        cameraForward.y = 0.0f; //저장한 벡터의 y값을 0으로 바꾼다.
        cameraRight.y = 0.0f; //저장한 벡터의 y값을 0으로 바꾼다.

        Vector3 moveDir = (cameraForward * v) + (cameraRight * h); //앞, 뒤로 이동하는 방향은 카메라가 보고있는 전방을 기준으로, 좌, 우로 이동하는 방향은 카메라가 보고있는 오른쪽을 기준으로 움직인다.
        moveDir.Set(moveDir.x, 0.0f, moveDir.z); //위에서 저장한 값에서 y축값을 다시 없애는 작업을 한다.

        controller.SimpleMove(moveDir * moveSpeed); //플레이어 오브젝트가 moveDir방향으로 moveSpeed속도로 움직인다.

        float forward = Vector3.Dot(moveDir, cameraForward); //애니메이션 처리를 위한 값을 저장
        float strafe = Vector3.Dot(moveDir, cameraRight); //해당 함수는 두 벡터가 같은 방향을 바라볼수록 1에 가까운수를 출력하고, 반대방향을 바라볼수록 -1을 출력한다.

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }

    void Turn()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition); //스크린 화면에 있는 마우스를 포지션을 3D월드 좌표로 바꾸고 해당 좌표로 메인 카메라에서 ray를 발사함.

        float enter = 0.0f; //가상의 바닥에 레이를 발사해 충돌한 지점의 거리를 enter로 저장한다.
        plane.Raycast(ray, out enter);
        hitPoint = ray.GetPoint(enter); //가상의 바닥에 레이가 충돌한 좌표값을 저장한다.

        Vector3 lookDir = hitPoint - transform.position; //회전할 방향의 벡터를 계산
        lookDir.y = 0.0f; //계산된 벡터의 y값을 0으로 초기화

        transform.localRotation = Quaternion.LookRotation(lookDir); //플레이어 오브젝트의 회전값을 지정한다. (회전 방향을 lookDir 벡터로 선언)
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }   
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
