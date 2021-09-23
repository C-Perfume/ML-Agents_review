using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{

    TMP_Text roomInfoTxt;
    RoomInfo roomInfo;
    public RoomInfo RoomInfo
    {
        get
        {
            return roomInfo;
        }
        set
        {
            roomInfo = value;
            roomInfoTxt.text = $"{roomInfo.Name} ( {roomInfo.PlayerCount} / {roomInfo.MaxPlayers} )";
            //버튼 클릭 이벤트 스크립트로 연결하기
            GetComponent<Button>().onClick.AddListener(() => OnEnterRoom(roomInfo.Name));
        }


    }


    void Awake()
    {
        roomInfoTxt = GetComponentInChildren<TMP_Text>();
    }


    void OnEnterRoom(string Name)
    {

        PhotonNetwork.JoinRoom(Name);
    }
}
