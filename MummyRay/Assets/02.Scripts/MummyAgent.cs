using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MummyAgent : Agent
{

    //에이전트 변수
    Transform tr;
    Rigidbody rb;

    //전진,후진,회전 변수
    public float moveSpd = 1.5f;
    public float turnSpd = 200f;

    //색깔변수
    public Renderer floorRd;
    public Material goodMt;
    public Material badMt;
    Material originMt;

    //각 스테이지를 불러올 변수
    public StageManager stageManager;

    public override void Initialize()
    {

        tr = transform;
        rb = GetComponent<Rigidbody>();

        originMt = floorRd.material;

        //한 에피소드 당 시도회수
        MaxStep = 5000;
    }

    public override void OnEpisodeBegin()
    {

        //stage 초기화
        stageManager.SetStageObj();
        //물리엔진 초기화
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //에이전트 위치초기화
        tr.localPosition = new Vector3(Random.Range(-22, 22f), .05f, Random.Range(-22, 22f));
        //에이전트 회전값 불규칙하게
        tr.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;
        //0, 1번 값에 어떤 깂 나오는지 프린트
        //print($"[0]={action[0]}, [1]={action[1]}");

        //초기화 하자
        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        //Branch 0
        switch (action[0])
        {
            case 1: dir = tr.forward; break;
            case 2: dir = -tr.forward; break;
        }

        //Branch 1 좌우회전판단
        switch (action[1])
        {
            //반시계방향
            case 1: rot = -tr.up; break;
            //시계방향
            case 2: rot = tr.up; break;
        }


        tr.Rotate(rot, Time.fixedDeltaTime * turnSpd);
        rb.AddForce(dir * moveSpd, ForceMode.VelocityChange);

        //마이너스 리워드
        AddReward(-1 / (float)MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;

        //임의의 데이터가 있을 수 있어 클리어 하자.
        actions.Clear();

        // 기계는 0번이 전진인지 후진인지 멈추는건지 다 모른다.
        // 그래서 리워드를 통해 방향성을 알려준다.

        // Branch 0
        // 멈춤, 전진, 후진 값순서는 항상 0, 1, 2로 해야 된다! 
        if (Input.GetKey(KeyCode.W)) { actions[0] = 1; }
        if (Input.GetKey(KeyCode.S)) { actions[0] = 2; }
        // 위에서 클리어해주기 때문에 값은 자동으로 0이 되며 elseif문을 안써도 된다.

        // Branch 1
        // 멈춤, 좌, 우회전 0, 1, 2
        if (Input.GetKey(KeyCode.A)) { actions[1] = 1; }
        if (Input.GetKey(KeyCode.D)) { actions[1] = 2; }

    }

    private void OnCollisionEnter(Collision other)
    {
        //이번에는 먹을게 많아 endepisode 대신 초기화
        // 충돌 시 리워드
        if (other.collider.CompareTag("SLIME"))
        {
            floorRd.material = goodMt;

            rb.velocity = rb.angularVelocity = Vector3.zero;
            Destroy(other.gameObject);
            AddReward(1);
            StartCoroutine(RevertMaterial());
        }

        if (other.collider.CompareTag("BAD"))
        {
            floorRd.material = badMt;

            AddReward(-1);
            EndEpisode();
            StartCoroutine(RevertMaterial());
        }

        if (other.collider.CompareTag("WALL"))
        {
            // 벽에 부딪히면 계속 멈춰 붙어 있을 수 있으니 마이너스처리
            // 벽을 뚫고 지나갈 수 있다.. 확인필요
            AddReward(-.1f);
        }


    }


    IEnumerator RevertMaterial()
    {
        yield return new WaitForSeconds(.2f);
        floorRd.material = originMt;
    }
}
