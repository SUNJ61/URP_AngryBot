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
    private bool isMouseClick => Input.GetMouseButtonDown(0); //마우스 왼쪽 버튼을 누르면 true, 아니면 자동으로 false로 업데이트
    void Start()
    {
        pv = GetComponent<PhotonView>();
        firePos = transform.GetChild(2).GetChild(0).GetComponent<Transform>();
        muzzleFlash = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
        bulletPrefab = Resources.Load<GameObject>(BulletName);
    }
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; //버튼 누를때 발사 이벤트 멈춤

        if(pv.IsMine && isMouseClick) //조건문이 없으면 어떤 플레이어가 총을 쏘더라도 모든 플레이어가 총을 발사한다.
        {
            FireBullet(pv.Owner.ActorNumber); //로컬 클라이언트가 함수 호출. 함수에 로컬 클라이언트의 고유 번호를 넣는다.
            pv.RPC("FireBullet", RpcTarget.Others, pv.Owner.ActorNumber); //리모트 클라이언트들이 함수를 호출하도록 만든다. 이때 리모트 클라이언트가 함수를 호출 할 때 로컬 클라이언트의 고유번호를 넣는다.
        }
    }

    [PunRPC]
    private void FireBullet(int actorNum)
    {
        if (!muzzleFlash.isPlaying) muzzleFlash.Play(true); //머즐플래쉬가 플레이중이 아니라면 플레이.

        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);

        bullet.GetComponent<Bullet>().actorNumber = actorNum; //총알스크립트에 있는 int값에 고유 번호를 넣는다.
    }
}
