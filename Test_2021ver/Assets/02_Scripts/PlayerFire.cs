using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFire : MonoBehaviour
{

    public GameObject bulletPrefeb;

    //파이어포스 아래에 머즐플래쉬
    MeshRenderer mrMuzzle;
    public Transform firePos;

    public AudioClip sfx;

    AudioSource aud;

    void Start()
    {
        aud = GetComponent<AudioSource>();
        mrMuzzle = firePos.GetComponentInChildren<MeshRenderer>();
        mrMuzzle.enabled = false;

    }
    void Update()
    {
        // Debug.DrawRay(firePos.position, firePos.forward*10, Color.green);
        if (Input.GetButtonDown("Fire1"))
        {
            //찾아낼 오브젝트 갯수가 가변젹이면 raycast, all을 쓰고 확정된 숫자인 경우 가비지 컬랙션을 안만들게 nonallow를 쓰자
            if (Physics.Raycast(firePos.position, firePos.forward, out RaycastHit hit, 10, 1 << LayerMask.NameToLayer("MONSTER_BOBY")))
            {
                //                Debug.Log(hit.collider.name);
                hit.collider.GetComponent<MonsterCtrl>().Damage(25);
            }
            GunFire();
        }
    }


    void GunFire()
    {

        Instantiate(bulletPrefeb, firePos.position, firePos.rotation);
        aud.PlayOneShot(sfx, .8f);
        StartCoroutine(ShowMuzzleFlash());

    }

    IEnumerator ShowMuzzleFlash()
    {
        //다른모양
        //x, y = ., .5 둘만 나와야 함
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2) * .5f);

        mrMuzzle.enabled = true;
        mrMuzzle.material.mainTextureOffset = offset;

        //회전
        float angle = Random.Range(0, 360);
        mrMuzzle.transform.localRotation = Quaternion.Euler(Vector3.forward * angle);

        //크기
        float scale = Random.Range(1.0f, 3);
        mrMuzzle.transform.localScale = Vector3.one * scale;

        //서브루프 양보시간
        yield return new WaitForSeconds(.1f);
        mrMuzzle.enabled = false;
    }
}
