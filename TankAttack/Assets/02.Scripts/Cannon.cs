using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    //캐논 발사 한 오너가 누군지 알아보자
    // 탱크 컨트롤의 punrpc에서 지정해준다.
    public int actorNumber;
    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 3000);
    }

}
