using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
public class RoomData : MonoBehaviourPun
{
    //[HideInInspector]
    //public string roomID = string.Empty;
    //[HideInInspector]
    //public int connectPlayer = 0;
    //[HideInInspector]
    //public int maxPlayer = 0;

    //public TMP_Text textRoomID;
    //public TMP_Text textConnectInfo;

    //public void DisplayRoomData()
    //{
    //    textRoomID.text = roomID;
    //    textConnectInfo.text = $"({connectPlayer.ToString()}/{maxPlayer.ToString()})";
    //}
    #region 강사님이 한 방법
    private RoomInfo _roomInfo;
    private TMP_Text roomInfoText; //텍스트 한개만 씀
    private PhotonManager photonManager;
    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            // 룸 정보 표시
            roomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";
            // 버튼 클릭 이벤트에 함수 연결
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }

    private void Awake()
    {
        roomInfoText = GetComponentInChildren<TMP_Text>();
        photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
    }
    void OnEnterRoom(string roomName)
    {
        // 유저명 설정
        photonManager.SetUserID();
        // 룸 접속
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion
}
