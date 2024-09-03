using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Player = Photon.Realtime.Player; //게임중에 실시간으로 접속한 플레이어를 찾는다.
public class Damage : MonoBehaviourPun
{
    private Renderer[] renderers;
    private Animator animator;
    private CharacterController cc;
    private Gamemanager gamemanager; //게임매니저에 접근하기 위해 선언. 로그를 추가하기위해

    private int initHp = 100;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashRespawn = Animator.StringToHash("Respawn");

    private readonly string bulletTag = "BULLET";
    private readonly string SpawnPiontTag = "SpawnPointGroup";

    public int curHp = 100;
    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        gamemanager = GameObject.Find("GameManager").GetComponent<Gamemanager>();
    }
    private void OnEnable()
    {
        curHp = initHp;
    }
    private void OnCollisionEnter(Collision col)
    {
        if(curHp > 0 && col.collider.CompareTag(bulletTag))
        {
            curHp -= 20;
            if(curHp <= 0)
            {
                if(photonView.IsMine)
                {
                    var actorNum = col.collider.GetComponent<Bullet>().actorNumber;

                    Player lastShooterPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNum); //총알에서 얻어낸 고유번호로 현재 룸에 있는 플레이어를 찾는다.

                    string msg = string.Format //맞은 사람의 닉네임을 띄우고 마지막에 총알을 맞춘 플레이어의 닉네임을 띄운다.
                        ("\n<color=#00ff00>{0}</color> is killed by <color=#ff0000>{1}</color>",photonView.Owner.NickName, lastShooterPlayer.NickName);

                    photonView.RPC("KillMessge", RpcTarget.AllBufferedViaServer, msg); //버퍼에 해당메세지 보내는 것을 저장
                }

                StartCoroutine(PlayerDie());
            }
        }
    }

    [PunRPC]
    void KillMessge(string msg)
    {
        gamemanager.msgList.text += msg;
    }
    IEnumerator PlayerDie()
    {
        cc.enabled = false; //움직이지 못하도록 컨트롤러 비활성화
        animator.SetTrigger(hashDie);
        animator.SetBool(hashRespawn, false);

        yield return new WaitForSeconds(3.0f); //죽는 모션을 보고 비활성화

        SetPlayerVisable(false); //캐릭터 투명 처리
        animator.SetBool(hashRespawn, true);

        yield return new WaitForSeconds(2.0f); //총 리스폰 시간 5초

        Transform[] points = GameObject.Find(SpawnPiontTag).GetComponentsInChildren<Transform>(); //스폰 위치 랜덤으로 찾기
        int idx = Random.Range(1, points.Length);
        transform.position = points[idx].position;

        curHp = initHp; //hp 초기화
        SetPlayerVisable(true); //플레이어 보이기
        cc.enabled = true; //움직임 허용
    }

    private void SetPlayerVisable(bool isVisable)
    {
        foreach(Renderer renderer in renderers)
        {
            renderer.enabled = isVisable;
        }
    }
}
