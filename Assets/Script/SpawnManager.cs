using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public string EnemyText = "Prey_Lv1";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("spawnPrey");
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator spawnPrey()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            var preyObj = PoolManager.instance.GetGo(EnemyText);
            //print(preyObj);
            float Xpos = Random.Range(-5, 5);
            float Zpos = Random.Range(-5, 5);
            Vector3 spawnPos = new Vector3(Xpos, 0.1f, Zpos);
            preyObj.transform.position = spawnPos;

            string SheetTxt = GameManager.Instance.StatData;
            PreyAnimal prey = preyObj.GetComponent<PreyAnimal>();
            GameManager.Instance.PreyInitStat(SheetTxt, prey.AniCode, prey);

            //print("»ý¼º");
            yield return new WaitForSeconds(1);
        }
    }
}
