using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using TMPro;

public class TankCtrl : MonoBehaviour, IPunObservable
//오류 단축키 ctrl+. >> 인터페이스 구현
{
    //cache 처리하고 쓰기(속도가 빠르다.)
    Transform tr;
    public float spd = 10;
    PhotonView pv;
    Rigidbody rb;

    public TMP_Text nickNameTxt;

    CinemachineVirtualCamera vCam;
    new Camera camera;



    //포탄 쏠 방향에 쏴질 레이
    Ray ray;
    RaycastHit hit;
    // 포탄 각도
    public Transform turretTr;


    public Transform firePos;
    public GameObject cannon;

    public GameObject fireEft;


    //송수신용
    Vector3 currPos;
    Quaternion currRot;


    float hp = 100;


    void Start()
    {
        tr = GetComponent<Transform>();
        pv = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();

        vCam = GameObject.Find("VCam").GetComponent<CinemachineVirtualCamera>();
        camera = Camera.main;
        rb.mass = 1000;

        // 내꺼면 무게중심을 낮춘다. 오뚜기나 요트가 중심을 잘 잡는 이유
        if (pv.IsMine)
        {
            vCam.Follow = tr;
            vCam.LookAt = tr;

            rb.centerOfMass = new Vector3(0, -5, 0);
        }
        else rb.isKinematic = true;

        nickNameTxt.text = pv.Owner.NickName;
    }

    void Update()
    {
        if (!pv.IsMine)
        {
            //탱크가 죽고 재위치에서 살아날 때 차이가너무 큰데 쭈우욱 밀려보이게 됨
            //차이가 크면 lerp하지 말고 그냥 바뀌게 세팅
            //Dead Reckoning
            if (Vector3.Distance(tr.position, currPos) >= 2)
            {
                tr.position = currPos;
            }
            else
            {
                tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 20);
            }
            tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 20);
        }
        else
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            tr.Translate(Vector3.forward * Time.deltaTime * spd * v);
            tr.Rotate(Vector3.up * Time.deltaTime * 100 * h);

            ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
            {
                //탱크 포구 각도가 floor를 맞아 Y가 너무 낮아서 꼬구라짐.. Y를 포구로 수정했다.
                Vector3 hitPos = new Vector3(hit.point.x, turretTr.position.y, hit.point.z);
                var rot = Quaternion.LookRotation(hitPos - turretTr.position);
                //lerp의 각도버전 lerp는 균일하나 slerp는 원형이라 초반과 후반부가 느리고 중간은 빠른(폭이 큰) 형태를 보인다.
                turretTr.rotation = Quaternion.Slerp(turretTr.rotation, rot, 5 * Time.deltaTime);

            }
            //Debug.DrawRay(ray.origin, ray.direction * 30, Color.blue);

            if (Input.GetMouseButtonDown(0))
            {
                Fire(pv.Owner.ActorNumber);
                pv.RPC("Fire", RpcTarget.Others, pv.Owner.ActorNumber);
            }

        }
    }

    [PunRPC]
    void Fire(int actorNo)
    {
        var cann = Instantiate(cannon, firePos.position, firePos.rotation);
        cann.GetComponent<Cannon>().actorNumber = actorNo;
        Destroy(cann, 5);
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("CANNON"))
        {
            Destroy(other.gameObject);
            var obj = Instantiate(fireEft, other.transform.position, Quaternion.identity);
            Destroy(obj, 5);
            hp -= 20;
            if (hp <= 0)
            {
                if (pv.IsMine)
                {
                    int actN = other.collider.GetComponent<Cannon>().actorNumber;
                    //포탄의 넘버를 가지고 플레이어를 찾는다.
                    Player lastShooter = PhotonNetwork.CurrentRoom.GetPlayer(actN);
                    string msg = $"\n <color=#00ff00>[{pv.Owner.NickName}]</color> is killed by <color=#ff0000>{lastShooter.NickName}</color>";
                    // 다른 오브젝트의 포톤뷰 알피씨처리는 이렇게 한다...
                    GameObject.Find("GameManager").GetComponent<GameManager>().photonView.RPC("SendMsg", RpcTarget.All, msg);
                }
                StartCoroutine(TankDie());
            }

        }
    }

    IEnumerator TankDie()
    {
        TankVisible(false);
        yield return new WaitForSeconds(3);

        Vector3 pos = new Vector3(Random.Range(-150, 150f), 0, Random.Range(-150, 150f));
        tr.position = pos;
        hp = 100;
        TankVisible(true);

    }
    void TankVisible(bool visible)
    {
        //모든렌더러를 비활성화하자.
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var ren in renderers)
        {
            ren.enabled = visible;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //송신
        if (stream.IsWriting)
        {

            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);

        }
        //수신
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
