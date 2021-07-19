using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies; // behaviourParameters를 쓰기 위한 조건
using Unity.MLAgents.Actuators;

public class PlayerAgents : Agent
{

    public enum TEAM
    {
        BLUE = 0, RED
    }

    public TEAM team = TEAM.BLUE;

    //플레이어 색상 변경
    public Material[] materials;
    Rigidbody rb;
    public Vector3 initPosBlue = new Vector3(-5.5f, .5f, 0);
    public Vector3 initPosRed = new Vector3(5.5f, .5f, 0);
    Quaternion initRotBlue = Quaternion.Euler(Vector3.up * 90);
    Quaternion initRotRed = Quaternion.Euler(Vector3.up * -90);


    BehaviorParameters bps;

    //슛 파워
    float moveSpd = 1;
    float kickForce = 800;

    public override void Initialize()
    {
        bps = GetComponent<BehaviorParameters>();
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.mass = 10;
        bps.TeamId = (int)team; // 블루면 블루숫자, 레드면 레드숫자로 팀아이디 세팅
        // material도 팀순서 맞게 넣어준다.
        GetComponent<Renderer>().material = materials[(int)team];

        MaxStep = 10000;
    }


    public override void OnEpisodeBegin()
    {

        //위치값 세팅
        InitPlayer();
        //물리력 초기화
        rb.velocity = rb.angularVelocity = Vector3.zero;
    }


    void InitPlayer()
    {
        // 블루팀이면 블루포지션으로 아니면 레드 포지션으로
        transform.localPosition = (team == TEAM.BLUE) ? initPosBlue : initPosRed;
        transform.localRotation = (team == TEAM.BLUE) ? initRotBlue : initRotRed;
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        // 브레인에 전달받는 키 값 
        int forwardIdx = actions.DiscreteActions[0];
        int rightIdx = actions.DiscreteActions[1];
        int rotateIdx = actions.DiscreteActions[2];

        //방향 설정
        switch (forwardIdx)
        {
            case 1: dir = transform.forward; break;
            case 2: dir = -transform.forward; break;
        }

        switch (rightIdx)
        {
            case 1: dir = -transform.right; break;
            case 2: dir = transform.right; break;
        }

        switch (rotateIdx)
        {
            case 1: rot = -transform.up; break;
            case 2: rot = transform.up; break;
        }

        //실제 이동로직
        transform.Rotate(rot, Time.fixedDeltaTime * 100);
        rb.AddForce(dir * moveSpd, ForceMode.VelocityChange);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;
        //파라미터 초기화
        actions.Clear();


        //키 값 설정
        //정 전 후진
        if (Input.GetKey(KeyCode.W)) actions[0] = 1;
        if (Input.GetKey(KeyCode.S)) actions[0] = 2;

        //정 좌 우 이동
        if (Input.GetKey(KeyCode.Q)) actions[1] = 1;
        if (Input.GetKey(KeyCode.E)) actions[1] = 2;

        // 정지 반시계 시계 (Y축 회전)
        if (Input.GetKey(KeyCode.A)) actions[2] = 1;
        if (Input.GetKey(KeyCode.D)) actions[2] = 2;

        //print($"[0]={actions[0]}, [1]={actions[1]}, [2]={actions[2]} ");
    }

    //공이 상대편 골대로 들어가도 +
    //내 골대로 들어가면 -

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("ball"))
        {
            //공을 차면 +리워드
            AddReward((float)1 / MaxStep);
            //킥 방향 설정 공A 플레이어B a-b
            Vector3 kickDir = other.GetContact(0).point - transform.position;
            other.gameObject.GetComponent<Rigidbody>().AddForce(kickDir * kickForce);
        }



    }
}
