using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;

public class BallCtrl : MonoBehaviour
{
    // 레드와 블루 플레이어를 담을 배열
    public Agent[] players;
    Rigidbody rb;

    int blueScore, redScore;
    public TMP_Text blueTxt, redTxt;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InitBall();

    }

    void InitBall()
    {
        //Rb초기화
        rb.velocity = rb.angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(0, 2f, 0);
    }

    void OnCollisionEnter(Collision other)
    {
        //블루골 들어가면 점수 획득 후 초기화
        if (other.collider.CompareTag("BLUE_GOAL"))
        {
            players[0].AddReward(-1);
            players[1].AddReward(1);
            players[0].EndEpisode();
            players[1].EndEpisode();
            redTxt.text = (++redScore).ToString();
            InitBall();
        }


        if (other.collider.CompareTag("RED_GOAL"))
        {
            players[0].AddReward(1);
            players[1].AddReward(-1);
            players[0].EndEpisode();
            players[1].EndEpisode();
            blueTxt.text = (++blueScore).ToString();
            InitBall();
        }

    }

}
