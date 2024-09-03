using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Movement : MonoBehaviourPun, IPunObservable
{
    private CharacterController controller;
    private new Transform transform; //������ transform �޼ҵ带 �����. �ش� ������� ���.
    private Animator animator;
    private new Camera camera;
    private PhotonView pv = null;

    private Plane plane; //������ Plane�� ����ĳ���� �ϱ����� ����
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

        plane = new Plane(transform.up, transform.position); //������ �ٴ��� player������Ʈ ��ġ�� �������� �����Ѵ�. (�÷��̾� ������Ʈ�� �������� �ٴڿ� �����Ƿ� plane�� ���� ���� �ٴڿ� �����ȴ�.)
    }
    void Update()
    {
        if (pv.IsMine) //���� Ŭ���̾�Ʈ �϶� ������ ���� ����.
        {
            Move();
            Turn();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, receivePos, Time.deltaTime * damping);//���� ����, �ε巴�� �̵�
            transform.rotation = Quaternion.Lerp(transform.rotation, receiveRot, Time.deltaTime * damping);//�ε巴�� ȭ���� ������.
        }
    }

    float h => Input.GetAxis("Horizontal"); //���� new Input System���� �ٲ۴�.
    float v => Input.GetAxis("Vertical"); //���� new Input System���� �ٲ۴�.
    void Move()
    {
        Vector3 cameraForward = camera.transform.forward; //���� ī�޶� �ٶ󺸴� ������ ���Ϳ� �����Ѵ�.
        Vector3 cameraRight = camera.transform.right; //���� ī�޶� �ٶ󺸴� ������ ������ ������ ���Ϳ� �����Ѵ�.
        cameraForward.y = 0.0f; //������ ������ y���� 0���� �ٲ۴�.
        cameraRight.y = 0.0f; //������ ������ y���� 0���� �ٲ۴�.

        Vector3 moveDir = (cameraForward * v) + (cameraRight * h); //��, �ڷ� �̵��ϴ� ������ ī�޶� �����ִ� ������ ��������, ��, ��� �̵��ϴ� ������ ī�޶� �����ִ� �������� �������� �����δ�.
        moveDir.Set(moveDir.x, 0.0f, moveDir.z); //������ ������ ������ y�ప�� �ٽ� ���ִ� �۾��� �Ѵ�.

        controller.SimpleMove(moveDir * moveSpeed); //�÷��̾� ������Ʈ�� moveDir�������� moveSpeed�ӵ��� �����δ�.

        float forward = Vector3.Dot(moveDir, cameraForward); //�ִϸ��̼� ó���� ���� ���� ����
        float strafe = Vector3.Dot(moveDir, cameraRight); //�ش� �Լ��� �� ���Ͱ� ���� ������ �ٶ󺼼��� 1�� �������� ����ϰ�, �ݴ������ �ٶ󺼼��� -1�� ����Ѵ�.

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }

    void Turn()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition); //��ũ�� ȭ�鿡 �ִ� ���콺�� �������� 3D���� ��ǥ�� �ٲٰ� �ش� ��ǥ�� ���� ī�޶󿡼� ray�� �߻���.

        float enter = 0.0f; //������ �ٴڿ� ���̸� �߻��� �浹�� ������ �Ÿ��� enter�� �����Ѵ�.
        plane.Raycast(ray, out enter);
        hitPoint = ray.GetPoint(enter); //������ �ٴڿ� ���̰� �浹�� ��ǥ���� �����Ѵ�.

        Vector3 lookDir = hitPoint - transform.position; //ȸ���� ������ ���͸� ���
        lookDir.y = 0.0f; //���� ������ y���� 0���� �ʱ�ȭ

        transform.localRotation = Quaternion.LookRotation(lookDir); //�÷��̾� ������Ʈ�� ȸ������ �����Ѵ�. (ȸ�� ������ lookDir ���ͷ� ����)
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
