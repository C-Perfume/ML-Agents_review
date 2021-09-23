using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    public List<Transform> points = new List<Transform>();
    public List<GameObject> monsterPool = new List<GameObject>();
    public int maxPool = 10;
    public GameObject monsterPFeb;
    public float createTime = 3;
    private bool isOver = false;

    Transform playerTr;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        // 뒤에 변수 true를 넣어 비활성화 된 것도 가지고 온다.
        GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>(true, points);
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").transform;
        CreatePooling();
        InvokeRepeating("CreateMonster", 2, createTime);
    }


    void CreatePooling()
    {
        for (int i = 0; i < maxPool; i++)
        {
            GameObject monster = Instantiate<GameObject>(monsterPFeb);
            monster.name = $"Monster_{i:00}"; // 이게 "Monster_"+i.ToString("00")과 같다.
            monster.SetActive(false);
            monsterPool.Add(monster);
        }

    }
    void CreateMonster()
    {
        foreach (var monster in monsterPool)
        {
            if (monster.activeSelf == false)
            {
                int idx = Random.Range(1, points.Count);
                monster.transform.position = points[idx].position;
                //몬스터 회전각도
                Vector3 lookDir = playerTr.position - monster.transform.position;
                Quaternion rot = Quaternion.LookRotation(lookDir.normalized);
                monster.transform.rotation = rot;

                monster.SetActive(true);
                break;
            }
        }
        //GameObject mon = Instantiate(monsterPFeb, points[idx].position, points[idx].rotation);
    }

    public bool IsGameOver
    {
        get { return isOver; }
        set
        {
            isOver = value;
            if (isOver)
            {

                CancelInvoke("CreateMonster");
            }
        }
    }
}
