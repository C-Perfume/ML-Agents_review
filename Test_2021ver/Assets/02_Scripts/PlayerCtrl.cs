
// #pragma warning disable IDE0051 이걸 적으면 함수 워닝(IED0051 = Start함수의 번호)이 안나온다. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//vscode이용 시 C# , debugger for unity, unity tools, unity snipppets를 다운로드 받고
//preferences에 external tool에서 바꾸고 체크 모두 하고 regenerate 버튼 클릭한다.

[System.Serializable]
public class PlayerAmim
{

    public AnimationClip _idle;
    public AnimationClip _runF;
    public AnimationClip _runL;
    public AnimationClip _runR;
    public AnimationClip _runB;
    public AnimationClip[] _dies;


}
public class PlayerCtrl : MonoBehaviour
{
//    public :

    //위에껀 안되고 아래 레인지는 인스펙터 창이 토글로 바뀐다.
    [Range(3, 8)] 
    public float moveSpd = 8;
    public float initHp = 100;
    public float curHp = 100;
    public PlayerAmim playerAmim;
    //private :
    float h;
    float v;
    float r;
    float turnSpd = 0;
    Animation anim;

    //델리게이트 통한 이벤트로 몬스터들에게 전부 플레이어 죽음을 전달하자
    public delegate void PlayerDieHandler();
    //언제든 불러올 수 있어야 한다. 그래서 static
    //지금은 이벤트가 델리게이트 변수를 담고 있어 결국 이 이벤트는 담겨진 함수를 발생하게 된다.
    public static event PlayerDieHandler OnPlayerDie;

void Awake()
{
// 비활성화 상태라도 호출이 된다.
// 다른 스크립트의 스타트 함수보다도 제일 먼저 호출되어 초기화 할 때 쓴다.
// 만약 두개의 스크립트 모두 awake를 사용한다면 어느게 먼저 호출될까?
//프로젝트 세팅의 스크립 익스큐션오더에서 우선순위가능함.
//이건 코루틴으로 호출 불가능
}


       // void Start()
 IEnumerator Start()
    {
        // 이걸 void 아닌 코루틴으로 시작할 수 있다.
        anim = GetComponent<Animation>();
        anim.Play(playerAmim._idle.name);
        yield return new WaitForSeconds(.2f);
        turnSpd = 100;
    }

    void Update()
    {
        //화면을 렌더링하는 주기
        //정규화 백터(Normalized vector), 단위 백터(unit vector)
        //우리가 주로 쓰는 Vector3.forward; Vector3.left; Vector3.right; Vector3.back; Vector3.one; Vector3.zero;
        //다섯가지가 정규화 백터라고 한다. 
        
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");
        Vector3 dir = new Vector3(h,0,v);
        //v = Input.GetAxisRaw("") 이건 1.0, 0.0, -1.0만되어 부드럽게 나오지 않는다.
        //dir.Normalize();
        //transform.position += dir * moveSpd * Time.deltaTime;
        transform.Translate(dir.normalized*moveSpd*Time.deltaTime);
        transform.Rotate(Vector3.up*Time.deltaTime * turnSpd * r);

        if(v >= .1f)
        { 
            //전진
            anim.CrossFade(playerAmim._runF.name, .3f);
        }
        
        else if(v <= -.1f)
        { 
            //전진
            anim.CrossFade(playerAmim._runB.name, .3f);
        }

        else if(h >= .1f)
        { 
            //전진
            anim.CrossFade(playerAmim._runR.name, .3f);
        }

        else if(h <= -.1f)
        { 
            //전진
            anim.CrossFade(playerAmim._runL.name, .3f);
        }
        else{
            anim.CrossFade(playerAmim._idle.name, .3f);
        }


    }

    void fixedUpdate(){
//발생하는 주기가 균일하다
//.02초이며 수정가능 함
// 물리엔진이 주기적으로 값을 입력해야하는 경우
//물리엔진의 계산주기(중력 / 마찰계수 / ? )
    }

    void lateUpdate()
    {
//Update의 로직이 끝난 뒤 바로 호출됨
//Update와 쌍으로 생각하면 좋음
// 업데이트에서 값을 받아 후처리를 해야 하는 경우
// 주인공이 이동하는 경우 그 좌표값을 가지고 카메라가 이동하게 할 때 등 사용
// 모든 다른 스크립트의 업데이트가 끝난 뒤 호출된다.
    }

     void OnTriggerEnter(Collider other) {
        if(curHp > 0 && other.CompareTag("FIST")){

                curHp-=10;
                if(curHp<=0)
                {
                    PlayerDie();

                }
        }
    }


    void PlayerDie()
    {
        // 이벤트 발생
        //GameObject.Find("GameManager").GetComponent<GameManager>().IsGameOver = true;
        OnPlayerDie();
        GameManager.instance.IsGameOver = true;
        #region 1번 방법
        // print("die");
        // GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
        // foreach (var monster in monsters)
        // {
        //     //monster.GetComponent<MonsterCtrl>().YouWin();
        //     //저렇게 함수 부르지 말고 샌드메세지로 처리할 수 있다
        //     //루프를 돌며 메세지를 보낸다.
        //     // 근데 느리다는 의견이 있다. 그래서 많지 않을 때 좋다.
        //     // 뒤의 파라미터는 리턴을 안받겠다는 의미
        //     monster.SendMessage("YouWin", SendMessageOptions.DontRequireReceiver);
        // }
#endregion
    }
}
