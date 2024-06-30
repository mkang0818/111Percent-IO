using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    Rigidbody rigid;

    // ������ �θ� ������Ʈ (�÷��̾� ������Ʈ)
    private GameObject playerObject;
    private PlayerController playerController;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        // �θ� ������Ʈ ����
        playerObject = transform.parent.gameObject;
        playerController = playerObject.GetComponent<PlayerController>();

        // �θ� ������Ʈ ���� �ʱ�ȭ
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("PreyAni"))
        {
            Vibration.Vibrate(1000);
            aa(col.gameObject);
        }
    }
    void aa(GameObject col)
    {
        PreyAnimal prey = col.gameObject.GetComponent<PreyAnimal>();
        int Lv = prey.stat.Lv;
        print("��� ���� : " + Lv);
        print("�÷��̾� ���� : "+playerController.playerstat.Lv);


        // ���� ���� ������ �浹 ��
        if (Lv == playerController.playerstat.Lv)
        {
            // �Ʊ����� �ڸ� �̵�
            print("���ݷ�����");
            playerController.playerstat.At += prey.stat.At;

            //����ٴϱ�
            prey.state = State.Follow;
            playerController.AllyList.Add(prey.gameObject);

            prey.gameObject.tag = "Player";

            int index = playerController.AllyList.Count;
            print(index);
            prey.target = playerController.FollowPos[index];
        }
        //���� ������ �浹 ��
        else if (Lv > playerController.playerstat.Lv)
        {
            if (prey.stat.At > playerController.playerstat.At)
            {
                print("ü�� ����");
                Destroy(col.gameObject);

            }
            else if (prey.stat.At < playerController.playerstat.At)
            {
                print("�ٸ� ������ ����");

                playerController.Upgrade();
                Destroy(col.gameObject);
            }
        }
        //���� ������ �浹 ��
        else if (Lv < playerController.playerstat.Lv) 
        {
            // ü�� ȸ��
            print("ü��ȸ��");
        }
    }
}
