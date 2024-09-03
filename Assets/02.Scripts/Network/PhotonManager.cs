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
        PhotonNetwork.AutomaticallySyncScene = true; //������ Ŭ���̾�Ʈ �� �ڵ� ����ȭ �ɼ��̴�. (�ٸ� �����ڰ� ������Ŭ���̾�Ʈ�� �����ִ� ������ �ڵ����� ����ȭ ���ִ� �ɼ�)
        PhotonNetwork.GameVersion = Version; //�ڵ� ����ȭ �ɼ��� �ش� ���� ���� ���� ���� �������Ѵ�.
        Debug.Log(PhotonNetwork.SendRate); //���� �������� ������ �ʴ� ���� Ƚ���� ��Ÿ����.
        PhotonNetwork.ConnectUsingSettings(); //���� ������ �����Ѵ�.

        Room_Prefab = Resources.Load<GameObject>(Room_Prefab_Name); //������� awake�� �ۼ�
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
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); //Ŭ���̾�Ʈ�� �κ� �ȿ� ������ true -> ���⼱ false
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); //Ŭ���̾�Ʈ�� �κ� �ȿ� ������ true -> ���⼱ true
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRanndomRoom Failed: {returnCode}: {message}:");
        OnMakeRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room!");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}"); //���� ������ ���̸� ���
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}"); //��ȿ� ������ true���
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}"); //���� ��ȿ� �÷��̾� �� 

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName} : {player.Value.ActorNumber}"); //������ �÷��̾��� �г��Ӱ� �ش� �÷��̾ ���� �������� ��� (�г����� ������ �� ������ ActorNumber�� ���� �� �� ����. ��������.)
        }

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MainScene");
        }
    }

    //IEnumerator LoadScene() //���� ������ ������ ���� �Է��� �и����, �ΰ��� Ŭ���̾�Ʈ���� ���� �ε��ϴ� ��û�� �߻�, �̸� �����ϱ����� �� �ε带 õõ�� �߻� ��Ų��. 
    //{
    //    yield return new WaitForSeconds(5.0f);
    //    PhotonNetwork.LoadLevel("MainScene");
    //}

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        #region ���� ��� ������Ʈ ���
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

        #region ������� �ٲ� �� ������Ʈ
        GameObject tempRoom = null;

        foreach (var roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList == true) //���� ������ ���
            {
                rooms.TryGetValue(roomInfo.Name, out tempRoom); //��ųʸ����� ���̸����� �˻��� ����� ������ ����.

                Destroy(tempRoom); //������ ����

                rooms.Remove(roomInfo.Name); //��ųʸ��� �ش� �� �̸��� �����͸� ����
            }
            else
            {
                if (rooms.ContainsKey(roomInfo.Name) == false)
                {
                    GameObject roomPrefab = Instantiate(Room_Prefab, scrollContent.transform); //�� �������� scroll������ ����

                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;

                    rooms.Add(roomInfo.Name, roomPrefab); //��ųʸ� �ڷ����� ������ �߰�
                }
                else //�� �̸��� ��ųʸ��� ���� ��� ������ ����
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
        SetUserID(); //������ ����

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoom()
    {
        RoomOptions roomOption = new RoomOptions();
        roomOption.MaxPlayers = 4; //�ִ� �÷��̾��
        roomOption.IsOpen = true; //���� ������ ������ ��
        roomOption.IsVisible = true; //���� �� ����Ʈ�� ���̵��� �� ��

        PhotonNetwork.CreateRoom(SetRoomName(), roomOption); //������ ��� �����
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
