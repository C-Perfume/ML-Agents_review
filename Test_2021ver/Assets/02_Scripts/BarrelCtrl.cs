using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class BarrelCtrl : MonoBehaviour
{
int hitCount;
public GameObject expEFT;
MeshRenderer mr;
public Texture[] textures;
    void Start() 
    {
        mr = GetComponentInChildren<MeshRenderer>();    
        mr.material.mainTexture = textures[Random.Range(0, textures.Length)];
    }

    void OnCollisionEnter(Collision coll) {
    if(coll.collider.CompareTag("BULLET"))
    {
        if(++hitCount == 3){

            ExpBarrel();
        }
    }
}

    void ExpBarrel()
    {
        GameObject expG = Instantiate(expEFT, transform.position, Quaternion.identity);
        Destroy(expG, 6);
           Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 1200);
        Destroy(gameObject, 2);
    }
}
