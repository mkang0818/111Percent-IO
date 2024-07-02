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

    Animator anim;
    string EatVFX = "EatVFX";
    string PlusText = "PlusText";
    string TimerText = "TimerText";
    void Start()
    {
        // �θ� ������Ʈ ����
        playerObject = transform.parent.gameObject;
        playerController = playerObject.GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        AttackText.text = GameManager.Instance.FormatNumber(playerController.playerstat.At);
        anim.SetBool("IsRun", true);
    }
    private void OnTriggerEnter(Collider col)
    {
        // ���� ������Ʈ�� �浹 ��
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
        int playerlv = playerController.playerstat.Lv;

        // ���� ���� ������ �浹 ��
        if (Lv == playerController.playerstat.Lv && playerController.playerstat.At >= prey.stat.At)
        {
            // �Ʊ����� �ڸ� �̵�
            print("���ݷ�����");
            SoundManager.Instance.SoundPlay("Eat", playerController.EatSound);
            
            playerController.playerstat.At += prey.stat.At;

            GameManager.Instance.Score += Lv;
            ValueText(PlusText, Lv);
            prey.gameObject.tag = "Player";

            col.GetComponent<PoolObj>().ReleaseObject();
        }
        //���� ������ �浹 ��
        else if (Lv >= playerController.playerstat.Lv)
        {
            if (prey.stat.At > playerController.playerstat.At)
            {
                print("���� �������� ���ݹ���");
                SoundManager.Instance.SoundPlay("Damage", playerController.DamageSound);

                // ���ݹ��� �� 2�� �ð� ����
                int minusTime = 2;
                GameManager.Instance.CurTime -= minusTime;

                ValueText(TimerText, minusTime);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (prey.stat.At <= playerController.playerstat.At)
            {
                print("���� ���� ���� ����");
                SoundManager.Instance.SoundPlay("Eat", playerController.EatSound);

                playerController.playerstat.At += prey.stat.At;
                GameManager.Instance.Score += Lv;

                ValueText(PlusText, Lv);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (GameManager.Instance.Score >= GameManager.Instance.goalScore[prey.stat.Lv])
            {

                print("���� ������ ����");
                SoundManager.Instance.SoundPlay("Upgrade", playerController.UpgradeSound);
                playerController.Upgrade();
                Destroy(col.gameObject);
            }
        }
        //���� ������ �浹 ��
        else if (Lv <= playerController.playerstat.Lv)
        {
            print("���ݷ� ����");
            SoundManager.Instance.SoundPlay("Eat", playerController.EatSound);
            playerController.playerstat.At += prey.stat.At;

            print("���� ȹ��");
            GameManager.Instance.Score += Lv;

            ValueText(PlusText, Lv);
            Setsize(prey.stat.Lv, EatVFX);
            col.GetComponent<PoolObj>().ReleaseObject();
        }
    }

    // ������Ʈ�� ���� ������ ����
    void Setsize(int index, string objText)
    {
        GameObject Effect = PoolManager.instance.GetGo(objText);
        float size = index * 0.1f;
        Effect.transform.localScale = new Vector3(size, size, size);
        Effect.transform.position = transform.position;
        Effect.GetComponent<VFXController>().ReleaseObj();
    }


    // �ؽ�Ʈ ����
    void ValueText(string TextName, long Value)
    {
        Vector3 playerPos = playerController.transform.position;
        int Xrand = Random.Range(-playerController.playerstat.Lv / 4, playerController.playerstat.Lv / 4);
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + playerController.playerstat.Lv, playerPos.z);

        GameObject TextObj = PoolManager.instance.GetGo(TextName);
        TextObj.transform.position = spawnPos;
        TextObj.GetComponent<TextVFX>().Value = GameManager.Instance.FormatNumber(Value);
        TextObj.GetComponent<TextVFX>().TextAnim();
    }
}
