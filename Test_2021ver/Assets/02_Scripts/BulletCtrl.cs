using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // relative는 로컬좌표계 기준 힘은 뉴튼
        rb.AddRelativeForce(Vector3.forward*800);
    }

// 매프레임마다 사용하지 않는 콜백함수를 나왔다 들어가기 때문에 지워줘야 한다.
}
