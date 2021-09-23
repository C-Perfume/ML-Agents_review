using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    Transform camTr;
    Transform tr;

    void Start()
    {
        camTr = Camera.main.transform;
        tr = transform;
    }


    //탱크 움직임 처리 후 변경되게 하기 위해 update 아닌 lateupdate
    private void LateUpdate()
    {
        //        tr.forward = camTr.forward;
        tr.LookAt(camTr);

    }
}
