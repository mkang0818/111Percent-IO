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
        AttackText.text = GameManager.Instance.FormatNumber(playerController.playerstat.At); // ���ݷ� �ؽ�Ʈ ������Ʈ
        anim.SetBool("IsRun", true); // �ִϸ��̼� �۵�
    }


    private void OnTriggerEnter(Collider col)
    {
        // Prey������Ʈ�� �浹 ��
        if (col.transform.CompareTag("PreyAni"))
        {
            Vibration.Vibrate(1000); // 1�ʰ� ����
            AnimalCollision(col.gameObject);
        }
    }

    // �浹 ������Ʈ ���¿� ���� ����
    void AnimalCollision(GameObject col)
    {
        PreyAnimal prey = col.gameObject.GetComponent<PreyAnimal>();
        int Lv = prey.stat.Lv;


        // 1. ���� ������ �浹, 2. ���� ������ �浹, ���� ������ �浹

        // ���� �� ������ �浹
        if (Lv == playerController.playerstat.Lv)
        {
            SoundManager.Instance.SoundPlay("Eat", playerController.EatSound);
            
            playerController.playerstat.At += prey.stat.At; // ���ݷ� ����
            GameManager.Instance.Score += Lv; // ���� ȹ��
            ValueText(PlusText, Lv); // �ؽ�Ʈ ����Ʈ ����

            col.GetComponent<PoolObj>().ReleaseObject(); // ������Ʈ Ǯ ��ȯ
        }
        // ���� ���� ������ �浹
        else if (Lv >= playerController.playerstat.Lv)
        {
            if (prey.stat.At > playerController.playerstat.At) // ���� �������� ���� ��
            {
                SoundManager.Instance.SoundPlay("Damage", playerController.DamageSound);

                // ���ݹ��� �� 2�� �ð� ����
                int minusTime = 2;
                GameManager.Instance.CurTime -= minusTime;

                // ����Ʈ ���� �� ũ�� ����
                ValueText(TimerText, minusTime);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (prey.stat.At <= playerController.playerstat.At)  // ���� �������� ���� ��
            {
                SoundManager.Instance.SoundPlay("Eat", playerController.EatSound);

                playerController.playerstat.At += prey.stat.At; // ���ݷ� ����
                GameManager.Instance.Score += Lv;

                // ����Ʈ ���� �� ũ�� ����
                ValueText(PlusText, Lv);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (GameManager.Instance.Score >= GameManager.Instance.goalScore[prey.stat.Lv])  // ���� �������� ���� ��
            {
                //????????????????????
                print("���� ������ ����");
                SoundManager.Instance.SoundPlay("Upgrade", playerController.UpgradeSound);
                playerController.Upgrade();
                Destroy(col.gameObject);
            }
        }
        // ���� ���� ������ �浹
        else if (Lv <= playerController.playerstat.Lv)
        {
            SoundManager.Instance.SoundPlay("Eat", playerController.EatSound);

            playerController.playerstat.At += prey.stat.At; // ���ݷ� ����
            GameManager.Instance.Score += Lv; // ���� ȹ��

            // ����Ʈ ���� �� ũ�� ����
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
        Effect.GetComponent<VFXController>().ReleaseObj(); // Ǯ ������Ʈ ��ȯ
    }


    // �ؽ�Ʈ����Ʈ ����
    void ValueText(string TextName, double Value)
    {
        // ����Ʈ ��ǥ ����
        Vector3 playerPos = playerController.transform.position;
        int Xrand = Random.Range(-playerController.playerstat.Lv / 4, playerController.playerstat.Lv / 4);
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + playerController.playerstat.Lv, playerPos.z);

        // ����Ʈ ������Ʈ Ǯ Ȱ��ȭ
        GameObject TextObj = PoolManager.instance.GetGo(TextName);

        // ����Ʈ ��ǥ, Text ����
        TextObj.transform.position = spawnPos;
        TextObj.GetComponent<TextVFX>().Value = GameManager.Instance.FormatNumber(Value);
        TextObj.GetComponent<TextVFX>().TextAnim();
    }
}
