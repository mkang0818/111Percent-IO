using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimalController : MonoBehaviour
{
    public TextMeshProUGUI attackText;

    // ������ �θ� ������Ʈ (�÷��̾� ������Ʈ)
    private GameObject playerObject;
    private PlayerController _playerController;

    Animator anim;
    string eatVFX = "EatVFX";
    string plusText = "PlusText";
    string timerText = "TimerText";

    void Start()
    {
        // �θ� ������Ʈ ����
        playerObject = transform.parent.gameObject;
        _playerController = playerObject.GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        attackText.text = GameManager.Instance.FormatNumber(_playerController.PlayerStat.attack); // ���ݷ� �ؽ�Ʈ ������Ʈ
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
        if (Lv == _playerController.PlayerStat.lv)
        {
            SoundManager.Instance.SoundPlay("Eat", _playerController.eatSound);

            _playerController.PlayerStat.attack += prey.stat.At; // ���ݷ� ����
            GameManager.Instance.curScore += Lv; // ���� ȹ��
            ValueText(plusText, Lv); // �ؽ�Ʈ ����Ʈ ����

            col.GetComponent<PoolObj>().ReleaseObject(); // ������Ʈ Ǯ ��ȯ
        }
        // ���� ���� ������ �浹
        else if (Lv >= _playerController.PlayerStat.lv)
        {
            if (prey.stat.At > _playerController.PlayerStat.attack) // ���� �������� ���� ��
            {
                SoundManager.Instance.SoundPlay("Damage", _playerController.damageSound);

                // ���ݹ��� �� 2�� �ð� ����
                int minusTime = 2;
                GameManager.Instance.curTime -= minusTime;

                // ����Ʈ ���� �� ũ�� ����
                ValueText(timerText, minusTime);
                Setsize(prey.stat.Lv, eatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (prey.stat.At <= _playerController.PlayerStat.attack)  // ���� �������� ���� ��
            {
                SoundManager.Instance.SoundPlay("Eat", _playerController.eatSound);

                _playerController.PlayerStat.attack += prey.stat.At; // ���ݷ� ����
                GameManager.Instance.curScore += Lv;

                // ����Ʈ ���� �� ũ�� ����
                ValueText(plusText, Lv);
                Setsize(prey.stat.Lv, eatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (GameManager.Instance.curScore >= GameManager.Instance.goalScore[prey.stat.Lv])  // ���� �������� ���� ��
            {
                //????????????????????
                print("���� ������ ����");
                SoundManager.Instance.SoundPlay("Upgrade", _playerController.upgradeSound);
                _playerController.Upgrade();
                Destroy(col.gameObject);
            }
        }
        // ���� ���� ������ �浹
        else if (Lv <= _playerController.PlayerStat.lv)
        {
            SoundManager.Instance.SoundPlay("Eat", _playerController.eatSound);

            _playerController.PlayerStat.attack += prey.stat.At; // ���ݷ� ����
            GameManager.Instance.curScore += Lv; // ���� ȹ��

            // ����Ʈ ���� �� ũ�� ����
            ValueText(plusText, Lv);
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
        Vector3 playerPos = _playerController.transform.position;
        int Xrand = Random.Range(-_playerController.PlayerStat.lv / 4, _playerController.PlayerStat.lv / 4);
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + _playerController.PlayerStat.lv, playerPos.z);

        // ����Ʈ ������Ʈ Ǯ Ȱ��ȭ
        GameObject TextObj = PoolManager.instance.GetGo(TextName);

        // ����Ʈ ��ǥ, Text ����
        TextObj.transform.position = spawnPos;
        TextObj.GetComponent<TextVFX>().Value = GameManager.Instance.FormatNumber(Value);
        TextObj.GetComponent<TextVFX>().TextAnim();
    }
}
