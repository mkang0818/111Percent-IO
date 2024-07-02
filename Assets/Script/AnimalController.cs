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
    string EatVFX = "EatVFX";

    string PlusText = "PlusText";
    string MinusText = "MinusText";
    string TimerText = "TimerText";
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
            ValueText(PlusText, prey.stat.At);

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

                ValueText(TimerText, minusTime);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (prey.stat.At <= playerController.playerstat.At)
            {
                print("���� ���� ���� ����");

                playerController.playerstat.At += prey.stat.At;
                GameManager.Instance.Score += prey.stat.At;

                ValueText(PlusText, prey.stat.At);
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
            playerController.playerstat.At += prey.stat.At;

            print("���� ȹ��");
            GameManager.Instance.Score += prey.stat.At;

            ValueText(PlusText, prey.stat.Lv);
            Setsize(prey.stat.Lv, EatVFX);
            col.GetComponent<PoolObj>().ReleaseObject();
        }
    }

    // ������Ʈ�� ���� ������ ����
    void Setsize(int index, string objText)
    {
        GameObject Effect = PoolManager.instance.GetGo(objText);
        Effect.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f) * GameManager.Instance.AnimalSize[index - 1];
        Effect.transform.position = transform.position;
        Effect.GetComponent<VFXController>().ReleaseObj();
    }


    // �ؽ�Ʈ ����
    void ValueText(string TextName, long Value)
    {
        Vector3 playerPos = playerController.transform.position;
        int Xrand = Random.Range(-playerController.playerstat.Lv / 2, playerController.playerstat.Lv / 2);
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + playerController.playerstat.Lv, playerPos.z);

        GameObject TextObj = PoolManager.instance.GetGo(TextName);
        TextObj.transform.position = spawnPos;
        TextObj.GetComponent<TextVFX>().Value = GameManager.Instance.FormatNumber(Value);
        TextObj.GetComponent<TextVFX>().TextAnim();
    }
}
