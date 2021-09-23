using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    readonly string gameVersion = "v1.0";
    string userId = "Star";

    public TMP_InputField nickNameIf;
    public TMP_InputField roomNameIf;


    //룸목록 저장용 딕셔너리
    Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    //룸 목록의 버튼 생성을 위한 프리펩
    public GameObject roomBtn;
    //버튼이 위치할 장소(스크롤뷰 컨텐츠)
    public Transform roomContents;


    private void Awake()
    {
        // 기본세팅
        // 방장의 씬을 전환 할 때 오토싱크로 할건지..
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;

        //포톤 클라우드 접속(한국서버로)
        //이전에는 아시아서버(싱가폴)로 이용해 TTL? PingTime이 늦어져 느렸다.
        //핑을 날려 가장 핑타임이 낮은 걸로 dev region이 설정됨

        PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.OfflineMode = true;

    }


    void Start()
    {
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 100):00}");
        nickNameIf.text = userId;


        PhotonNetwork.NickName = userId;
    }

    public override void OnConnectedToMaster()
    {
        print("Connected Network");
        //로비 접속 하자 - 방 목록 안볼거면 패스해도 됨
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        print("Entered Lobby");
        //PhotonNetwork.JoinRandomRoom();
        // 특정 방에 접속하면 콜백이 온다.


    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print($"Randomly Join Failed {returnCode} : {message}");
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;

        ro.MaxPlayers = 100;

        PhotonNetwork.CreateRoom("MyRoom", ro);
    }

    public override void OnCreatedRoom()
    {
        print("Room Created");
    }

    public override void OnJoinedRoom()
    {
        print("Entered The room");
        // Vector3 pos = new Vector3(Random.Range(-50, 50f), 0, Random.Range(-50, 50f));
        // // 그룹이 같아야 보여서 0으로 처리
        // PhotonNetwork.Instantiate("Tank", pos, Quaternion.identity, 0);

        if (PhotonNetwork.IsMasterClient)
        {
            //로비를 만들었기 때문에 해당 씬으로 이동하게 처리 >> 포톤 자체에서 기능이 있다.(load level함수에서 씬매니져를 사용한다.)
            PhotonNetwork.LoadLevel("BattleField");
        }
    }


    //룸 목록 콜백함수
    //로비에 있을 때만 호출된다.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        GameObject tempRoom;

        foreach (var room in roomList)
        {
            //            print($"Room Name : { room.Name}, Player : {room.PlayerCount} / { room.MaxPlayers}");

            // 룸 삭제된 경우
            if (room.RemovedFromList)
            {
                //딕셔너리 상 검색
                roomDict.TryGetValue(room.Name, out tempRoom);
                //룸버튼 삭제
                Destroy(tempRoom);
                //딕셔너리에서도 삭제
                roomDict.Remove(room.Name);

            }

            // 아닌 경우 룸정보 갱신
            else
            {

                //처음 생성된 경우
                if (!roomDict.ContainsKey(room.Name))
                {
                    GameObject _room = Instantiate(roomBtn, roomContents);
                    // 룸 정보 추가 표시 getset처리
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    // 딕셔너리 상 추가
                    roomDict.Add(room.Name, _room);
                }
                //기존에 있던게 움직여야 하는 경우 
                else
                {
                    //룸 정보 불러오기
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    //룸 정보 갱신
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }

            }
        }
    }


    #region UI_BUTTON_CALLBACK

    public void OnRandomJoin()
    {
        //입력된 닉네임을 지우고 버튼을 클릭하여 널값 인 경우
        if (string.IsNullOrEmpty(nickNameIf.text))
        {
            userId = $"User_{Random.Range(0, 100):00}";
            nickNameIf.text = userId;
        }
        PlayerPrefs.SetString("USER_ID", nickNameIf.text);
        PhotonNetwork.NickName = nickNameIf.text;
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoom()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = ro.IsVisible = true;
        ro.MaxPlayers = 100;

        if (string.IsNullOrEmpty(roomNameIf.text))
        {
            roomNameIf.text = $"ROOM_{Random.Range(0, 100):00}";
        }

        PhotonNetwork.CreateRoom(roomNameIf.text, ro);

    }

    #endregion
}
