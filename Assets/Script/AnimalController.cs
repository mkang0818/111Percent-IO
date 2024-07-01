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
    Animator anim;
    public GameObject EatVFX;

    void Start()
    {
        // �θ� ������Ʈ ����
        playerObject = transform.parent.gameObject;
        playerController = playerObject.GetComponent<PlayerController>();
        anim = GetComponent<Animator>();

        // �θ� ������Ʈ ���� �ʱ�ȭ
    }
    private void Update()
    {
        AttackText.text = GameManager.Instance.FormatNumber(playerController.playerstat.At);

        anim.SetBool("IsRun", true);
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("PreyAni"))
        {
            Vibration.Vibrate(1000);
            AnimalCollision(col.gameObject);
        }
    }

    // ���� ������Ʈ �浹 �� 
    void AnimalCollision(GameObject col)
    {
        PreyAnimal prey = col.gameObject.GetComponent<PreyAnimal>();
        int Lv = prey.stat.Lv;


        // ���� ���� ������ �浹 ��
        if (Lv == playerController.playerstat.Lv)
        {
            // �Ʊ����� �ڸ� �̵�
            print("���ݷ�����");
            playerController.playerstat.At += prey.stat.At;
            GameManager.Instance.Score += prey.stat.At;
            ValueText(GameManager.Instance.PlusText, prey.stat.At);

            //����ٴϱ�
            prey.state = State.Follow;
            prey.RoundVFX.SetActive(true);

            playerController.AllyList.Add(prey.gameObject);

            prey.gameObject.tag = "Player";

            int index = playerController.AllyList.Count + 1;
            // �ε��� �Ѿ ����ó��
            if (FollowPos.Length > index) prey.target = FollowPos[index];
        }
        //���� ������ �浹 ��
        else if (Lv >= playerController.playerstat.Lv)
        {
            if (prey.stat.At > playerController.playerstat.At)
            {
                print("���� �������� ���ݹ���");

                // ���ݹ��� �� 2�� �ð� ����
                int minusTime = 2;
                playerController.playerstat.TimeLimit -= minusTime;

                ValueText(GameManager.Instance.MinusText, minusTime);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (prey.stat.At <= playerController.playerstat.At)
            {
                print("���� ���� ���� ����");

                playerController.playerstat.At += prey.stat.At;
                GameManager.Instance.Score += prey.stat.At;

                ValueText(GameManager.Instance.PlusText, prey.stat.At);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (GameManager.Instance.Score >= GameManager.Instance.goalScore[prey.stat.Lv])
            {
                print("���� ������ ����");

                playerController.Upgrade();
                Destroy(col.gameObject);
            }
        }
        //���� ������ �浹 ��
        else if (Lv <= playerController.playerstat.Lv)
        {
            print("���ݷ� ����");

            float PlayerAt = playerController.playerstat.At;
            PlayerAt += prey.stat.At;

            print("���� ȹ��");
            GameManager.Instance.Score += prey.stat.At;

            ValueText(GameManager.Instance.PlusText, prey.stat.Lv);
            Setsize(prey.stat.Lv, EatVFX);
            col.GetComponent<PoolObj>().ReleaseObject();
        }
    }

    // ������Ʈ�� ���� ������ ����
    void Setsize(int index,GameObject obj)
    {
        GameObject Effect = Instantiate(obj, transform.position,Quaternion.identity);
        Effect.transform.localScale *= GameManager.Instance.AniSize[index-1];
    }


    // �ؽ�Ʈ ����
    void ValueText(GameObject VFXText, long Value)
    {
        Vector3 playerPos = playerController.transform.position;
        int Xrand = Random.Range(-playerController.playerstat.Lv / 2, playerController.playerstat.Lv / 2);
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + playerController.playerstat.Lv, playerPos.z);

        GameObject damageVFX = Instantiate(VFXText, spawnPos, Quaternion.identity);
        damageVFX.GetComponent<TextVFX>().Value = GameManager.Instance.FormatNumber(Value);

        //damageVFX.transform.localScale = new Vector3(playerController.playerstat.Lv, playerController.playerstat.Lv, playerController.playerstat.Lv);
        //Transform canvasTransform = VFXText.transform.Find("Canvas");
        //canvasTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameManager.Instance.FormatNumber(Value);
    }
}
