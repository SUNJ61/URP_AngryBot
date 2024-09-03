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

    private void SetRoomInfo() //룸 접속 정보 출력
    {
        Room room = PhotonNetwork.CurrentRoom;
        roomName.text = room.Name;
        connectInfo.text = $"({room.PlayerCount}/{room.MaxPlayers})";
    }

    private void OnExitClick() //exit 버튼의 연결할 함수
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() //룸에서 퇴장 했을 때 자동 호출되는 함수
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //룸에 새로운 유저가 들어올 때 호출되는 함수
    {
        SetRoomInfo();
        string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> is joined room";
        msgList.text += msg;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //룸에 있던 유저가 나갔을 때 호출되는 함수
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
