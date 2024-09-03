using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Gamemanager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomName;
    public TMP_Text connectInfo;
    public TMP_Text msgList;

    public Button exitBtn;

    private void Awake()
    {
        StartCoroutine(CreatePlayer());
        SetRoomInfo();
        exitBtn.onClick.AddListener(() => OnExitClick());
    }

    IEnumerator CreatePlayer()
    {
        yield return new WaitForSeconds(1.0f);
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation);
    }

    private void SetRoomInfo() //�� ���� ���� ���
    {
        Room room = PhotonNetwork.CurrentRoom;
        roomName.text = room.Name;
        connectInfo.text = $"({room.PlayerCount}/{room.MaxPlayers})";
    }

    private void OnExitClick() //exit ��ư�� ������ �Լ�
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() //�뿡�� ���� ���� �� �ڵ� ȣ��Ǵ� �Լ�
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //�뿡 ���ο� ������ ���� �� ȣ��Ǵ� �Լ�
    {
        SetRoomInfo();
        string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> is joined room";
        msgList.text += msg;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //�뿡 �ִ� ������ ������ �� ȣ��Ǵ� �Լ�
    {
        SetRoomInfo();
        string msg = $"\n<color=#ff0000>{otherPlayer.NickName}</color> is left room";
        msgList.text += msg;
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }
}
