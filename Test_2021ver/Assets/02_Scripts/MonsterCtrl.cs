using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{

    public enum STATE
    {

        IDLE,
        TRACE,
        ATTACK,
        DIE
    }

    public STATE state = STATE.IDLE;
    public float hp = 100;

    Transform trP;
    Transform trM;

    WaitForSeconds ws;
    NavMeshAgent nv;
    Animator anim;
    int hashTrace;
    int hashAttack;
    int hashHit;
    int hashPlayerDie;

    public bool isDead = false;
    public float attackDistM = 2;
    public float traceDistM = 10;

    private void Awake()
    {
        // 스타트에서 처리했던 코루틴을 온이네이블로 넘겨서 널 오류 발생 >> 어웨이크로 변경
        trP = GameObject.FindGameObjectWithTag("PLAYER").transform;
        trM = transform;
        ws = new WaitForSeconds(.3f);
        nv = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        //animator controller - parameter 값 >> 불값에 스트링을 넣는 것보다 속도가 조금 더 빠르다 
        //왜냐면 유니티 자체에서 스트링 값으로 해쉬테이블을 찾는 작업을 덜하기 때문이다.
        hashTrace = Animator.StringToHash("IsTraced");
        hashAttack = Animator.StringToHash("IsAttacked");
        hashHit = Animator.StringToHash("Hit");
        hashPlayerDie = Animator.StringToHash("PlayerDie");

    }

    // OnEnable과 OnEable의 차이를 기억하자. 스펠링 문제로 안될 수 있다.
    public void OnEnable()
    {
        // 비활성화 하고 활성화 시 호출된다.
        // 이벤트를 활성화 시킬 때
        // 코루틴 재가동할 때 쓴다.

        //쥬인공이 사망 시 호출되는 이벤트
        PlayerCtrl.OnPlayerDie += YouWin;
        StartCoroutine(CheckStateMon());
        StartCoroutine(ActionMon());
    }

    public void OnDisable()
    {
        //주인공이 죽은 뒤 호출되는 이벤트를 해지한다.
        PlayerCtrl.OnPlayerDie -= YouWin;

    }

    IEnumerator CheckStateMon()
    {

        while (!isDead)
        {
            float distance = Vector3.Distance(trP.position, trM.position);
            if (distance <= attackDistM)
            {

                state = STATE.ATTACK;
            }
            else if (distance <= traceDistM)
            {

                state = STATE.TRACE;

            }
            else
            {

                state = STATE.IDLE;

            }
            yield return ws;
        }
    }

    IEnumerator ActionMon()
    {

        while (!isDead)
        {

            switch (state)
            {
                case STATE.IDLE:
                    nv.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;

                case STATE.TRACE:
                    nv.SetDestination(trP.position);
                    nv.isStopped = false;
                    anim.SetBool(hashAttack, false);
                    anim.SetBool(hashTrace, true);
                    break;

                case STATE.ATTACK:
                    nv.isStopped = true;
                    anim.SetBool(hashAttack, true);
                    break;

                case STATE.DIE:
                    break;

            }
            yield return ws;
        }
    }

    public void Damage(float damage)
    {

        anim.SetTrigger(hashHit);
        hp -= damage;
        if (hp <= 0)
        {

            MonsterDie();
        }

    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            Destroy(coll.gameObject);
        }
    }

    int hashDie = Animator.StringToHash("Die");
    void MonsterDie()
    {

        StopAllCoroutines();
        nv.isStopped = true;
        // 캡슐콜라이더가 활성화 된 경우 계속 총을 맞아 죽어도 다시 죽는 애니메이션이 나온다.
        GetComponent<CapsuleCollider>().enabled = false;
        anim.SetTrigger(hashDie);
        Invoke("ReturnPooling", 3);
    }

    void ReturnPooling()
    {
        isDead = false;
        hp = 100;
        GetComponent<CapsuleCollider>().enabled = true;
        state = STATE.IDLE;
        gameObject.SetActive(false);
    }

    public void YouWin()
    {
        StopAllCoroutines();
        nv.isStopped = true;
        anim.SetFloat("DanceSpd", Random.Range(.8f, 2));
        anim.SetTrigger(hashPlayerDie);
        // 캡슐콜라이더가 활성화 된 경우 계속 총을 맞아 죽어도 다시 죽는 애니메이션이 나온다.

    }

    //GI(Global Illumination) 전역조명
    //Direct Light, Indirect Light 직접광원, 간접광원
    //실시간 조명인 경우 직접광원만 가능하다.
    //간접광원 >> 바운스(두번 튕김) 모두 베이크 하는게 GI굽는 것.
}
