using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class PhotonManager : MonoBehaviourPunCallbacks
{
    private GameObject Room_Prefab;
    [SerializeField] private GameObject RoomList_Panel;

    public TMP_InputField UserID;
    public TMP_InputField RoomID;
    public GameObject scrollContent;

    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();

    private string userID;

    private readonly string Version = "1.0.1";
    private readonly string Room_Prefab_Name = "Room";
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; //마스터 클라이언트 씬 자동 동기화 옵션이다. (다른 접속자가 마스터클라이언트가 보고있는 씬으로 자동으로 동기화 해주는 옵션)
        PhotonNetwork.GameVersion = Version; //자동 동기화 옵션이 해당 게임 버전 보다 먼저 쓰여야한다.
        Debug.Log(PhotonNetwork.SendRate); //포톤 서버와의 데이터 초당 전송 횟수를 나타낸다.
        PhotonNetwork.ConnectUsingSettings(); //포톤 서버에 접속한다.

        Room_Prefab = Resources.Load<GameObject>(Room_Prefab_Name); //강사님은 awake에 작성
    }
    private void Start()
    { 
        userID = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1,21):00}");

        UserID.text = userID;
        PhotonNetwork.NickName = userID;
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect To Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); //클라이언트가 로비 안에 있으면 true -> 여기선 false
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); //클라이언트가 로비 안에 있으면 true -> 여기선 true
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRanndomRoom Failed: {returnCode}: {message}:");
        OnMakeRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room!");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}"); //현재 접속한 룸이름 출력
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}"); //룸안에 있으면 true출력
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}"); //현재 룸안에 플레이어 수 

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName} : {player.Value.ActorNumber}"); //접속한 플레이어의 닉네임과 해당 플레이어가 가진 고유값을 출력 (닉네임은 동일할 수 있지만 ActorNumber는 동일 할 수 없음. 고유값임.)
        }

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MainScene");
        }
    }

    //IEnumerator LoadScene() //포톤 서버가 느려서 서버 입력이 밀릴경우, 두개의 클라이언트에서 씬을 로드하는 요청이 발생, 이를 방지하기위해 씬 로드를 천천히 발생 시킨다. 
    //{
    //    yield return new WaitForSeconds(5.0f);
    //    PhotonNetwork.LoadLevel("MainScene");
    //}

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        #region 기존 목록 업데이트 방법
        //foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM"))
        //{
        //    Destroy(obj);
        //}

        //foreach (RoomInfo roomInfo in roomList)
        //{
        //    if (roomInfo.RemovedFromList)
        //    {
        //        continue;
        //    }

        //    GameObject room = (GameObject)Instantiate(Room_Prefab);
        //    room.transform.SetParent(RoomList_Panel.transform, false);

        //    RoomData roomData = room.GetComponent<RoomData>();
        //    roomData.roomID = roomInfo.Name;
        //    roomData.connectPlayer = roomInfo.PlayerCount;
        //    roomData.maxPlayer = roomInfo.MaxPlayers;
        //    roomData.DisplayRoomData();

        //    roomData.GetComponent<Button>().onClick.AddListener(delegate { OnClickRoomItem(roomData.roomID); });
        //}
        #endregion

        #region 강사님이 바꾼 룸 업데이트
        GameObject tempRoom = null;

        foreach (var roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList == true) //룸이 삭제된 경우
            {
                rooms.TryGetValue(roomInfo.Name, out tempRoom); //딕셔너리에서 룸이름으로 검색해 저장된 프리팹 추출.

                Destroy(tempRoom); //프리팹 삭제

                rooms.Remove(roomInfo.Name); //딕셔너리에 해당 룸 이름의 데이터를 삭제
            }
            else
            {
                if (rooms.ContainsKey(roomInfo.Name) == false)
                {
                    GameObject roomPrefab = Instantiate(Room_Prefab, scrollContent.transform); //룸 프리팹을 scroll하위에 생성

                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;

                    rooms.Add(roomInfo.Name, roomPrefab); //딕셔너리 자료형에 데이터 추가
                }
                else //룸 이름에 딕셔너리에 없는 경우 룸정보 갱신
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
            }
            Debug.Log($"Room = {roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})");
        }
        #endregion
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    #region UI_BUTTON_EVENT

    public void OnRandomJoin()
    {
        SetUserID(); //유저명 저장

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoom()
    {
        RoomOptions roomOption = new RoomOptions();
        roomOption.MaxPlayers = 4; //최대 플레이어수
        roomOption.IsOpen = true; //공개 방으로 설정할 지
        roomOption.IsVisible = true; //룸을 룸 리스트에 보이도록 할 지

        PhotonNetwork.CreateRoom(SetRoomName(), roomOption); //설정한 대로 방생성
    }

    public void OnClickRoomItem(string roomID)
    {
        PhotonNetwork.NickName = UserID.text;
        PlayerPrefs.SetString("USER_ID", UserID.text);
        PhotonNetwork.JoinRoom(roomID);
    }

    #endregion

    public void SetUserID()
    {
        if(string.IsNullOrEmpty(UserID.text))
        {
            userID = $"USER_{Random.Range(1, 21):00}";
        }
        else
        {
            userID = UserID.text;
        }
        PlayerPrefs.GetString("USER_ID", userID);
        PhotonNetwork.NickName = userID;
    }

    private string SetRoomName()
    {
        if(string.IsNullOrEmpty(RoomID.text))
        {
            RoomID.text = $"ROOM_{Random.Range(1, 101):000}";
        }
        return RoomID.text;
    }
}
