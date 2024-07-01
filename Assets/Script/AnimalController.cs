using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimalController : MonoBehaviour
{
    public TextMeshProUGUI AttackText;

    // ������ �θ� ������Ʈ (�÷��̾� ������Ʈ)
    private GameObject playerObject;
    private PlayerController playerController;

    // ������ ũ�Ⱑ Ŀ�� ���� FollowPos ���� �þ����
    public Transform[] FollowPos;

    void Start()
    {
        // �θ� ������Ʈ ����
        playerObject = transform.parent.gameObject;
        playerController = playerObject.GetComponent<PlayerController>();

        // �θ� ������Ʈ ���� �ʱ�ȭ
    }
    private void Update()
    {
        AttackText.text = GameManager.Instance.FormatNumber(playerController.playerstat.At); 
    }
    private void OnTriggerEnter(Collider col)
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
        print("�÷��̾� ���� : " + playerController.playerstat.Lv);


        // ���� ���� ������ �浹 ��
        if (Lv == playerController.playerstat.Lv)
        {
            // �Ʊ����� �ڸ� �̵�
            print("���ݷ�����");
            playerController.playerstat.At += prey.stat.At;
            AllyTextVFX(prey.stat.At);

            //����ٴϱ�
            prey.state = State.Follow;
            playerController.AllyList.Add(prey.gameObject);

            prey.gameObject.tag = "Player";

            int index = playerController.AllyList.Count + 1;
            print(index);
            prey.target = FollowPos[index];
            // �ε��� �Ѿ ����ó��

        }
        //���� ������ �浹 ��
        else if (Lv >= playerController.playerstat.Lv)
        {
            if (prey.stat.At > playerController.playerstat.At)
            {
                print("ü�� ����");
                playerController.playerstat.CurHP -= prey.stat.Lv;
                DamageTextVFX(playerController.DamageText, prey.stat.Lv);
                Destroy(col.gameObject);
            }
            else if (prey.stat.At < playerController.playerstat.At)
            {
                //print("�ٸ� ������ ����");
                playerController.Upgrade();
                Destroy(col.gameObject);
            }
        }
        //���� ������ �浹 ��
        else if (Lv <= playerController.playerstat.Lv)
        {
            // ü�� ȸ��
            print("ü��ȸ��");
            
            playerController.playerstat.CurHP += prey.stat.Lv;

            DamageTextVFX(playerController.HeelText, prey.stat.Lv);
            Destroy(col.gameObject);
        }
    }

    void DamageTextVFX(GameObject VFXText, float Value)
    {
        Vector3 playerPos = playerController.transform.position;
        int Xrand = Random.Range(-playerController.playerstat.Lv/2, playerController.playerstat.Lv/2);
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + playerController.playerstat.Lv, playerPos.z);

        GameObject damageVFX = Instantiate(VFXText, spawnPos,Quaternion.identity);

        damageVFX.transform.localScale = new Vector3(playerController.playerstat.Lv, playerController.playerstat.Lv, playerController.playerstat.Lv);
        Transform canvasTransform = VFXText.transform.Find("Canvas");
        canvasTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Value.ToString();
    }
    void AllyTextVFX(long Value)
    {
        int Xrand = Random.Range(-playerController.playerstat.Lv / 2, playerController.playerstat.Lv / 2);
        Vector3 playerPos = playerController.transform.position;
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + playerController.playerstat.Lv, playerPos.z);

        GameObject AllyVFX = Instantiate(playerController.AllyText, spawnPos, Quaternion.identity);

        AllyVFX.transform.localScale = new Vector3(playerController.playerstat.Lv, playerController.playerstat.Lv, playerController.playerstat.Lv);
        Transform canvasTransform = playerController.AllyText.transform.Find("Canvas");
        canvasTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + GameManager.Instance.FormatNumber(Value);
    }
}
