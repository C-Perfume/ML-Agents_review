using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    //
    public enum HINT_COLOR
    {
        BLACK, BLUE, GREEN, RED
    }

    public HINT_COLOR hintcolor = HINT_COLOR.BLACK;

    public Material[] hintMts;
    public string[] hintTags;

    int prevTag = -1;

    new Renderer renderer;

    //위치 랜덤 발생을 위한 타겟 배열
    public GameObject[] targetObjs;
    //위치 랜덤 발생을 위한 타겟 위치 배열
    public List<Vector3> targetObjPos = new List<Vector3>();

    //난수를 추출할 때마다 해당 인덱스 값을 삭제한다.(중복방지)
    List<Vector3> targetPos = new List<Vector3>();

    void Start()
    {
        // 힌트 메쉬 메테리얼 부여
        renderer = transform.Find("Hint").GetComponent<MeshRenderer>();
        InitStage();
    }

    public void InitStage()
    {
        int idx = 0;
        do
        {
            // 난수를 발생시킨다.
            idx = Random.Range(0, hintTags.Length);
            // 같은 숫자가 나오지 않게 한다.
        } while (idx == prevTag);

        // 이전 숫자를 기억한다
        prevTag = idx;
        // 힌트의 색상을 지정한다.
        renderer.material = hintMts[idx];
        // 그에 맞게 태그를 바꿔준다.
        renderer.gameObject.tag = hintTags[idx];
        // 그에 맞게 스테이지에 있는 목표 타겟의 값도 지정한다.
        hintcolor = (HINT_COLOR)idx;

        //set target position
        //매 에피소드마다 값을 다 뺐기 때문에 제일 처음 초기화 시킨다. 
        targetPos.AddRange(targetObjPos);

        foreach (var target in targetObjs)
        {
            int idxPos = Random.Range(0, targetPos.Count);
            //첫번째 위치값에 배치한다.
            target.transform.localPosition = targetPos[idxPos];
            //해당 값을 지워준다.
            targetPos.RemoveAt(idxPos);

        }
    }

}
