using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimalStat
{
    public int Lv = 0;
    public string Name = "";
    public int HP = 0;
    public int At = 0;
    public int Speed = 0;
    public int MaxExp = 0;
    public int CurExp = 0;
}

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public AnimalStat playerstat;

    private VariableJoystick joy;
    public float speed;

    Rigidbody rigid;
    Animator anim;
    Vector3 moveVec;

    void Start()
    {
        joy = GameManager.Instance.Joystick;
        playerstat = new AnimalStat();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
    private void LateUpdate()
    {
        //�ִϸ��̼�
        print("���� ���� : " + playerstat.Lv);
        print("���� �̸� : " + playerstat.Name);
        print("���� ü�� : " + playerstat.HP);
        print("���� ���ݷ� : " + playerstat.At);
        //print(stat.MaxExp);
        //print(stat.CurExp);
    }
    void Movement()
    {
        float x = joy.Horizontal;
        float y = joy.Vertical;

        // �̵� ���� ���͸� ���
        Vector3 inputDirection = new Vector3(x, 0, y).normalized;

        // ���̽�ƽ�� ������ ��쳪 �Է°��� ���� ��
        if (inputDirection == Vector3.zero)
        {
            // ���� �������� ��� �̵�
            rigid.MovePosition(rigid.position + moveVec);
        }
        else
        {
            // ���ο� �̵� ���� ���� ���
            moveVec = inputDirection * playerstat.Speed * Time.deltaTime;

            // ĳ���͸� �̵� �������� �̵�
            rigid.MovePosition(rigid.position + moveVec);

            // ȸ�� ���� ���
            Quaternion dirQuat = Quaternion.LookRotation(moveVec);
            Quaternion moveQuat = Quaternion.Slerp(rigid.rotation, dirQuat, 0.3f);

            // ĳ���� ȸ��
            rigid.MoveRotation(moveQuat);
        }
    }


    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("PreyAni"))
        {
            Vibration.Vibrate(1000);
            print("�浹");
            aa(col.gameObject);
        }
    }

    void aa(GameObject col)
    {
        PreyAnimal prey = col.gameObject.GetComponent<PreyAnimal>();
        int Lv = prey.stat.Lv;

        // ���� ���� ������ �浹 ��
        if (Lv == playerstat.Lv)
        {
            // �Ʊ����� �ڸ� �̵�
            print("���ݷ�����");
            playerstat.At += prey.stat.At;
        }
        //�ٸ� ���� ������ �浹 ��
        else
        {
            if (prey.stat.At > playerstat.At)
            {
                print("ü�� ����");
            }
            else if (prey.stat.At < playerstat.At)
            {
                print("����ġ ����");
            }
        }

        Destroy(col.gameObject);
    }
}