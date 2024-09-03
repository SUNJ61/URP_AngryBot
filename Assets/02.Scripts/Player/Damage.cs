using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Player = Photon.Realtime.Player; //�����߿� �ǽð����� ������ �÷��̾ ã�´�.
public class Damage : MonoBehaviourPun
{
    private Renderer[] renderers;
    private Animator animator;
    private CharacterController cc;
    private Gamemanager gamemanager; //���ӸŴ����� �����ϱ� ���� ����. �α׸� �߰��ϱ�����

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

                    Player lastShooterPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNum); //�Ѿ˿��� �� ������ȣ�� ���� �뿡 �ִ� �÷��̾ ã�´�.

                    string msg = string.Format //���� ����� �г����� ���� �������� �Ѿ��� ���� �÷��̾��� �г����� ����.
                        ("\n<color=#00ff00>{0}</color> is killed by <color=#ff0000>{1}</color>",photonView.Owner.NickName, lastShooterPlayer.NickName);

                    photonView.RPC("KillMessge", RpcTarget.AllBufferedViaServer, msg); //���ۿ� �ش�޼��� ������ ���� ����
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
        cc.enabled = false; //�������� ���ϵ��� ��Ʈ�ѷ� ��Ȱ��ȭ
        animator.SetTrigger(hashDie);
        animator.SetBool(hashRespawn, false);

        yield return new WaitForSeconds(3.0f); //�״� ����� ���� ��Ȱ��ȭ

        SetPlayerVisable(false); //ĳ���� ���� ó��
        animator.SetBool(hashRespawn, true);

        yield return new WaitForSeconds(2.0f); //�� ������ �ð� 5��

        Transform[] points = GameObject.Find(SpawnPiontTag).GetComponentsInChildren<Transform>(); //���� ��ġ �������� ã��
        int idx = Random.Range(1, points.Length);
        transform.position = points[idx].position;

        curHp = initHp; //hp �ʱ�ȭ
        SetPlayerVisable(true); //�÷��̾� ���̱�
        cc.enabled = true; //������ ���
    }

    private void SetPlayerVisable(bool isVisable)
    {
        foreach(Renderer renderer in renderers)
        {
            renderer.enabled = isVisable;
        }
    }
}
