using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors; // collectObservations 함수 만들면 자동으로 생성
using Unity.MLAgents.Actuators;

//** 모노비헤이비어뿐 아니라 다른 여러가지 상속을 받은 Agent를 상속받는다.
public class MummyAgent : Agent
{
    //Agent의 역할
    //주변환경 관측(Observations)
    //policy(규율>>학습에 대한 결과물)에 따라 행동한다.(Actions)
    //보상(Rewards)


    Transform tr;
    Rigidbody rb;
    Transform targetTr;

    //초기화할 메테리얼
    Material originMt;
    public Material goodMt;
    public Material badMt;

    Renderer floorRd;

    //제일 먼저 초기화 작업을 한다. 1번만! awake 나 start로 생각하자
    public override void Initialize()
    {
        //이 세가지를 주변관측요소로 쓴다.
        tr = transform;
        rb = GetComponent<Rigidbody>();
        //?는 null값을 체크한다.
        targetTr = tr.parent.Find("Target")?.transform;
        floorRd = tr.parent.Find("Floor").GetComponent<MeshRenderer>();
        originMt = floorRd.material;
    }

    //트레이닝 하는 학습의 단위 = episode라고 지칭한다.
    //한번 학습 시 몇번 trial할지는 다르다.
    // 학습단위가 시작될 때마다 호출되는 학습이 있다.
    public override void OnEpisodeBegin()
    {
        //여기서 위치를 랜덤으로 해준다. 학습 시작!
        //Rigidbody 물리력 초기화
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //타겟 위치 변경(Stage의 자식이므로 로컬로)
        targetTr.localPosition = new Vector3(Random.Range(-4, 4f), .55f, Random.Range(-4, 4f));

        //Agent도 변경
        tr.localPosition = new Vector3(Random.Range(-4, 4f), .05f, Random.Range(-4, 4f));
        StartCoroutine(RevertMaterial());
    }

    IEnumerator RevertMaterial()
    {

        yield return new WaitForSeconds(.2f);
        floorRd.material = originMt;
    }

    //주변 환경 정보를 관측, 수집, 전달하는 메소드
    public override void CollectObservations(VectorSensor sensor)
    {
        //관측할 수치 데이터 작성하기 자료형.. 리스트... 등등등
        sensor.AddObservation(targetTr.localPosition); //3개의 관측데이터(xyz)
        sensor.AddObservation(tr.localPosition);

        //학습효율을 높이자.
        //sensor.AddObservation(rb.velocity)로 해도되나 Y는 사용하지 않기 때문에 x, z만 보낸다.
        sensor.AddObservation(rb.velocity.x); // 1개의 관측데이터
        sensor.AddObservation(rb.velocity.z);
        //그러므로 총 8개 데이터를 관측한다.
        //behaviour parameters의 stacked vectors == 누적좌표 why?
        // 현재 좌표 하나만 가지고 있으면 방향성을 모르기 때문이다.
        // stacked vectors는 방향성을 알려준다. 그치만 1로 두자. 

        //휴리스틱까지 코딩이 끝나면 관측데이터를 모아 행동하도록 명령을 내려야 한다.
        // decision requester 스크립트를 통해
    }

    //Policy (브레인)에게 전달 받은대로 행동
    public override void OnActionReceived(ActionBuffers actions)
    {
        //매개변수 액션이 규율을 받아온다.
        // 그 규칙대로 함수가 실행된다.
        //트레이닝 하지 않으면 값이 절대 안넘어온다.
        //그럼 어떻게 확인하느냐? 아래 함수로 확인

        // 옵저베이션 끝나면 이동처리하자.
        //좌우 화살표키 값 중 첫번째
        float h = actions.ContinuousActions[0];
        //상하 화살표키 중 두번째를 눌렀을 때의 리턴값 Getaxis이므로 연속적인 값(-1~1)을 리턴한다.
        float v = actions.ContinuousActions[1];

        Vector3 dir = new Vector3(h, 0, v);
        rb.AddForce(dir.normalized * 50);

        //희소보상원칙과 미묘한 마이너스 처벌로 학습을 독려하는 이론도 있다.
        //그러므로 미묘하게 -값을 주자.
        SetReward(-.001f);
    }

    //개발자가 미리 테스트 하기위한 메소드
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //모방학습을 할 때 (개발자가 시범을 먼저 보일 때)도 사용된다.
        //외부 규칙을 받지 않지만 받는 것처럼 처리한다.
        // 브레인에 연결하지 않을 때까진 이게 규율 역할 한다.

        //연속적 값을 전달 하기 때문에 continuousActions을 보내준다.
        // getaxisraw인 경우 discrete으로 넘겨준다.
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxis("Horizontal");
        actions[1] = Input.GetAxis("Vertical");

    }

    void OnCollisionEnter(Collision other)
    {

        //보상의 방법 2가지(set / add) 
        //set은 누적되지 않는다. add는 누적된다.(먹을게 많은 경우 사용)

        //보상은 정규화 된 값(1 or -1)을 준다. 넘어가면 잘못된 학습을 하게 된다.
        if (other.collider.CompareTag("DEAD_ZONE"))
        {
            SetReward(-1);
            floorRd.material = badMt;
        }

        if (other.collider.CompareTag("TARGET"))
        {
            SetReward(1);
            floorRd.material = goodMt;
        }

        //학습종료
        EndEpisode();
        //이게 호출되면 이후 episodebegin을 호출한다.
    }
}
