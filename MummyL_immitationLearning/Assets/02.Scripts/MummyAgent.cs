using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MummyAgent : Agent
{
    ///모방학습용 스크립트!
    // 게이머의 행동을 레코딩해야 한다.

    Rigidbody rb;
    Transform tr;

    public float moveSpd = 1.5f;
    public float turnSpd = 200;

    public StageManager stageManager;
    public Renderer floorRd;
    Material originMt;

    public Material goodMt, badMt;


    // 태그 비교를 위한 배열


    public override void Initialize()
    {
        MaxStep = 2000;
        rb = GetComponent<Rigidbody>();
        tr = transform;
        rb.velocity = rb.angularVelocity = Vector3.zero;
        originMt = floorRd.material;
    }


    public override void OnEpisodeBegin()
    {
        StartCoroutine(RevertMt());
        tr.localPosition = new Vector3(0, .05f, -4.5f);
        tr.localRotation = Quaternion.identity;
        stageManager.InitStage();
    }


    public override void CollectObservations(VectorSensor sensor)
    {

        //힌트 색상 정보 무기 한개의 정보를 전달할 때 쓴다??
        //sensor.AddOneHotObservation((int)stageManager.hintcolor, 4); // 1개정보
        //위 코드와 같은 의미다.         
        sensor.AddObservation((int)stageManager.hintcolor);

        sensor.AddObservation(stageManager.targetObjs[0].transform.localPosition);
        sensor.AddObservation(stageManager.targetObjs[1].transform.localPosition);
        sensor.AddObservation(stageManager.targetObjs[2].transform.localPosition);
        sensor.AddObservation(stageManager.targetObjs[3].transform.localPosition);
        sensor.AddObservation(tr.localPosition); //백터정보 3

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        var action = actions.DiscreteActions;

        switch (action[0])
        {
            case 1: dir = tr.forward; break;
            case 2: dir = -tr.forward; break;
        }

        switch (action[1])
        {
            case 1: rot = -tr.up; break;
            case 2: rot = tr.up; break;
        }

        tr.Rotate(rot * Time.fixedDeltaTime * turnSpd);
        rb.AddForce(dir * moveSpd, ForceMode.VelocityChange);
        SetReward(-1 / (float)MaxStep);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;

        //Branch 0
        if (Input.GetKey(KeyCode.W)) { actions[0] = 1; }
        if (Input.GetKey(KeyCode.S)) { actions[0] = 2; }


        //Branch 1
        if (Input.GetKey(KeyCode.A)) { actions[1] = 1; }
        if (Input.GetKey(KeyCode.D)) { actions[1] = 2; }

    }

    private void OnCollisionEnter(Collision other)
    {

        // 둘의 태그가 같으면
        if (other.collider.tag == stageManager.hintcolor.ToString())
        {
            floorRd.material = goodMt;
            SetReward(1);
            EndEpisode();
        }
        else
        {
            //태그가 벽이면
            if (other.collider.CompareTag("WALL") | other.gameObject.name == "Hint")
            { SetReward(-.1f); }
            else
            {

                floorRd.material = badMt;
                SetReward(-1);
                EndEpisode();

            }
        }
    }

    IEnumerator RevertMt()
    {

        yield return new WaitForSeconds(.2f);
        floorRd.material = originMt;

    }
}
