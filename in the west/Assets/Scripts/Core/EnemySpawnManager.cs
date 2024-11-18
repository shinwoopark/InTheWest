using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public GameObject[] NormalEnemy;
    public GameObject[] BossEnemy;

    private int _stage = 1;
    private int _wave = 0;
    private float _time = 0;
    private int _enemyScore = 1;
    private int _stageCount;

    private void Update()
    {
        if (!GameInstance.instance.bBossSpawn)
            _time += Time.deltaTime;

       UpdateWave();
    }

    private void UpdateWave()
    {
        if (_time >= Random.Range(4, 7))
        {
            if (_wave == 3)
            {
                if (_stage == 2 || _stage == 4)
                {
                    GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

                    if (enemys.Length == 0)
                    {
                        switch (_stage)
                        {
                            case 2:
                                SpawnBossEnemy(0, -1);
                                GameInstance.instance.bBossSpawn = true;
                                break;
                            case 4:
                                SpawnBossEnemy(1, -1);
                                GameInstance.instance.bBossSpawn = true;
                                break;
                        }
                    }
                }
                else
                    _enemyScore++;

                _stage++;
                _wave = 0;
            }
            else
                ChooseEnemy();

            _time = 0;
        }
    }

    private void ChooseEnemy()
    {
        int enemyScore = _enemyScore;
        int enemy1Count = 0;
        int enemy2Count = 0;

        for (; 0 < enemyScore;)
        {
            if (enemyScore > 1 && Random.Range(0, 2) == 0)
            {
                enemy2Count++;
                enemyScore = enemyScore - 2;
            }
            else
            {
                enemy1Count++;
                enemyScore--;
            }
        }

        StartCoroutine(SpawnEnemy(enemy1Count, enemy2Count));
        _wave++;
    }

    private IEnumerator SpawnEnemy(int enemy1Count, int enemy2Count)
    {
        int enemySpaneCount = 0;
        int dir = 1;

        for (int i = 0; i < enemy1Count; i++)
        {
            if (enemySpaneCount % 2 == 1)
            {
                dir *= -1;
            }
            else if (Random.Range(0, 2) == 0)
            {
                dir = -1;
            }

            SpawnNormalEnemy(0, dir);
            enemySpaneCount++;

            yield return new WaitForSeconds(Random.Range(0.25f, 0.75f));
        }

        for (int i = 0; i < enemy2Count; i++)
        {
            if (enemySpaneCount % 2 == 1)
            {
                dir *= -1;
            }
            else if (Random.Range(0, 2) == 0)
            {
                dir = -1;
            }

            SpawnNormalEnemy(1, dir);
            enemySpaneCount++;

            yield return new WaitForSeconds(Random.Range(0.25f, 0.75f));
        }
    }

    private void SpawnNormalEnemy(int enemy, int dir)
    {
        Instantiate(NormalEnemy[enemy], new Vector3(9.5f * dir, -3f, 0), Quaternion.identity);
    }

    private void SpawnBossEnemy(int enemy, int dir)
    {
        Instantiate(BossEnemy[enemy], new Vector3(9.5f * dir, -3f, 0), Quaternion.identity);
    }
}
