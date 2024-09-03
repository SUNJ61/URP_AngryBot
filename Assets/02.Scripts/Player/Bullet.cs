using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject effect;

    public int actorNumber; //각 클라이언트의 고유 번호를 넣는다.
    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 50.0f, ForceMode.Impulse); //이 코드는 갑자기 움직일 때 많이 사용한다. (ex.점프) ForceMode중 VelocityChange는 움직임 도중 방향을 바꿀 수 있음.
                                                                                                 //ForceMode.Impulse는 속도가 매우 빠르다. 총알에 사용.
        Destroy(this.gameObject, 3.0f);
    }

    private void OnCollisionEnter(Collision col) //오브젝트 풀링 생략. Destroy을 다 오브젝트 풀링으로 교체해야 한다.
    {
        var contact = col.GetContact(0); //col이 첫번째로 맞은 정보를 저장. col.contacts[0]도 같은 기능이지만 가져올 때 마다 동적 할당이 되어야해서 GetContact(0)를 사용한다.
        var obj = Instantiate(effect, contact.point, Quaternion.LookRotation(-contact.normal)); //맞은위치에서 col이 진행하던 반대 방향으로 이펙트를 소환한다.
        Destroy(obj, 2.0f);
        Destroy(this.gameObject);
    }
}
