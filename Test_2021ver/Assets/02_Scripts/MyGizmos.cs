using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{

//커스텀 기즈모 만들기! 개별크기 변화를 위해 만들자.
public Color _color = Color.green;
public float _radius = .3f;

private void OnDrawGizmos() {
    //에디터 모드에서 보인다.
    Gizmos.color = _color;
    Gizmos.DrawSphere(transform.position, _radius);
}
}
