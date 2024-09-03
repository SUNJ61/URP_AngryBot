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
    #region ������� �� ���
    private RoomInfo _roomInfo;
    private TMP_Text roomInfoText; //�ؽ�Ʈ �Ѱ��� ��
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
            // �� ���� ǥ��
            roomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";
            // ��ư Ŭ�� �̺�Ʈ�� �Լ� ����
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
        // ������ ����
        photonManager.SetUserID();
        // �� ����
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion
}
