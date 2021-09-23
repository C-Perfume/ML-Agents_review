using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{

    public GameObject sparkEFT;
    private void OnCollisionEnter(Collision coll) {
        // if(coll.gameObject.tag == "BULLET"){
        //     //이렇게 이름으로 찾는 경우 가비지컬렉션이 발생된다.
        //     // 메모리를 찾는데 사용하고 남은 메모리는 한쪽에서 모아둔다.
        //     // 그런다음 나중에 처리하게되는데 이렇게 되면 메모리 소모가 발생되서
        //     // 이런게 안되는 함수를 쓰는게 좋다.
        // }
        if(coll.collider.CompareTag("BULLET")){

           Destroy(coll.gameObject);
           ContactPoint contactP = coll.GetContact(0);
           Vector3 pos = contactP.point;
           //법선 벡터(~.normal)를 쿼터니언 값으로 변환
           Quaternion rot = Quaternion.LookRotation(-contactP.normal);
           GameObject sparkG = Instantiate(sparkEFT, pos, rot);
           Destroy(sparkG, 0.5f);
        }
    }


}
