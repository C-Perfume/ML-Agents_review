using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    public GameObject goodObj;
    public GameObject badObj;

    public int goodCnt = 30;
    public int badCnt = 10;

    List<GameObject> goodList = new List<GameObject>();
    List<GameObject> badList = new List<GameObject>();

    void Start()
    {
        SetStageObj();
    }

    public void SetStageObj()
    {
        //리스트 초기화
        foreach (var item in goodList)
        {
            Destroy(item);
        }
        foreach (var item in badList)
        {
            Destroy(item);
        }

        goodList.Clear();
        badList.Clear();

        //Goodobj생성
        for (int i = 0; i < goodCnt; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-22, 22f), .05f, Random.Range(-22, 22f));
            Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);

            // stage의 자식으로 한다. 그리고 위치값을 변경하기 위해 로컬 포지션처리해줘야 해서 스테이지의 포지션을 더한다.
            goodList.Add(Instantiate(goodObj, transform.position + pos, rot, transform));
        }
        //badObj생성
        for (int i = 0; i < badCnt; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-22, 22f), .05f, Random.Range(-22, 22f));
            Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);

            // stage의 자식으로 한다. 그리고 위치값을 변경하기 위해 로컬 포지션처리해줘야 해서 스테이지의 포지션을 더한다.
            badList.Add(Instantiate(badObj, transform.position + pos, rot, transform));
        }

    }

}

