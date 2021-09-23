using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public TMP_Text msgTxt;
    public TMP_Text connectInfoTxt;
    public TMP_InputField chatMsg;

    PhotonView pv;
    private void Awake()
    {
        Vector3 pos = new Vector3(Random.Range(-150, 150f), 0, Random.Range(-150, 150f));
        // 그룹이 같아야 보여서 0으로 처리
        PhotonNetwork.Instantiate("Tank", pos, Quaternion.identity, 0);
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        info();
    }

    void info()
    {
        var roominfo = PhotonNetwork.CurrentRoom;
        connectInfoTxt.text = $"( {roominfo.PlayerCount} / {roominfo.MaxPlayers} )";

    }

    public void OnExitClick()
    {
        // 네트워크 방을 나가기 전 모든 데이터 클린업 과정 필요
        PhotonNetwork.LeaveRoom();
    }

    public void OnSend()
    {
        string _msg = $"\n<color=#00ff00>[{PhotonNetwork.NickName}]</color> {chatMsg.text}";
        pv.RPC("SendMsg", RpcTarget.AllBufferedViaServer, _msg);
    }

    [PunRPC]
    void SendMsg(string msg)
    {
        msgTxt.text += msg;
    }

    // 클린 업 후 받을 수 있는 콜백 함수  >>> 콜백이므로 모노비헤이비어 콜백
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //새로 입장한 자의 정보를 알 수 있다.
        string msg = $"\n <color=#0000ff> {newPlayer.NickName} </color> joined room";
        msgTxt.text += msg;
        info();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string msg = $"\n <color=#ff0000> {otherPlayer.NickName} </color> has left room";
        msgTxt.text += msg;
        info();

    }
}
