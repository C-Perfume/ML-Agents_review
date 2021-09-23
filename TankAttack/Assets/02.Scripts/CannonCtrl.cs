using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCtrl : MonoBehaviour
{
    Transform tr;
    public Photon.Pun.PhotonView pv;
    void Start()
    {
        tr = transform;
    }

    void Update()
    {
        if (!pv.IsMine) return;
        if (Input.GetButton("Fire2"))
        {
            tr.Rotate(Vector3.right * Time.deltaTime * 10);
        }

        if (Input.GetButton("Fire3"))
        {
            tr.Rotate(-Vector3.right * Time.deltaTime * 10);
        }

    }
}
