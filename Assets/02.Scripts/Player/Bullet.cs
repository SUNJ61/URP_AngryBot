using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject effect;

    public int actorNumber; //�� Ŭ���̾�Ʈ�� ���� ��ȣ�� �ִ´�.
    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 50.0f, ForceMode.Impulse); //�� �ڵ�� ���ڱ� ������ �� ���� ����Ѵ�. (ex.����) ForceMode�� VelocityChange�� ������ ���� ������ �ٲ� �� ����.
                                                                                                 //ForceMode.Impulse�� �ӵ��� �ſ� ������. �Ѿ˿� ���.
        Destroy(this.gameObject, 3.0f);
    }

    private void OnCollisionEnter(Collision col) //������Ʈ Ǯ�� ����. Destroy�� �� ������Ʈ Ǯ������ ��ü�ؾ� �Ѵ�.
    {
        var contact = col.GetContact(0); //col�� ù��°�� ���� ������ ����. col.contacts[0]�� ���� ��������� ������ �� ���� ���� �Ҵ��� �Ǿ���ؼ� GetContact(0)�� ����Ѵ�.
        var obj = Instantiate(effect, contact.point, Quaternion.LookRotation(-contact.normal)); //������ġ���� col�� �����ϴ� �ݴ� �������� ����Ʈ�� ��ȯ�Ѵ�.
        Destroy(obj, 2.0f);
        Destroy(this.gameObject);
    }
}
