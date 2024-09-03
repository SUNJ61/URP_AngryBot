using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;
public class Fire : MonoBehaviourPun
{
    private Transform firePos;
    private GameObject bulletPrefab;
    private ParticleSystem muzzleFlash;
    private PhotonView pv = null;

    private readonly string BulletName = "Bullet_EX";
    private bool isMouseClick => Input.GetMouseButtonDown(0); //���콺 ���� ��ư�� ������ true, �ƴϸ� �ڵ����� false�� ������Ʈ
    void Start()
    {
        pv = GetComponent<PhotonView>();
        firePos = transform.GetChild(2).GetChild(0).GetComponent<Transform>();
        muzzleFlash = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
        bulletPrefab = Resources.Load<GameObject>(BulletName);
    }
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; //��ư ������ �߻� �̺�Ʈ ����

        if(pv.IsMine && isMouseClick) //���ǹ��� ������ � �÷��̾ ���� ����� ��� �÷��̾ ���� �߻��Ѵ�.
        {
            FireBullet(pv.Owner.ActorNumber); //���� Ŭ���̾�Ʈ�� �Լ� ȣ��. �Լ��� ���� Ŭ���̾�Ʈ�� ���� ��ȣ�� �ִ´�.
            pv.RPC("FireBullet", RpcTarget.Others, pv.Owner.ActorNumber); //����Ʈ Ŭ���̾�Ʈ���� �Լ��� ȣ���ϵ��� �����. �̶� ����Ʈ Ŭ���̾�Ʈ�� �Լ��� ȣ�� �� �� ���� Ŭ���̾�Ʈ�� ������ȣ�� �ִ´�.
        }
    }

    [PunRPC]
    private void FireBullet(int actorNum)
    {
        if (!muzzleFlash.isPlaying) muzzleFlash.Play(true); //�����÷����� �÷������� �ƴ϶�� �÷���.

        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);

        bullet.GetComponent<Bullet>().actorNumber = actorNum; //�Ѿ˽�ũ��Ʈ�� �ִ� int���� ���� ��ȣ�� �ִ´�.
    }
}
