using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public GameObject[] NormalEnemy;
    public GameObject[] BossEnemy;

    private int _wave = 1;
    private float _time = 5;
    private int _enemyScore;
    private bool _bEnemySpawn = false;

    private void Update()
    {
        if (!_bEnemySpawn)
            _time += Time.deltaTime;

        UpdateWave();
    }

    private void UpdateWave()
    {
        if (_time < 7.5f)
            return;

        if(_wave >= 5)
        {
            SpawnBossEnemy(0, -1);
        }
        else if (_wave < 5)
        {
            int enemy1Count = 0;
            int enemy2Count = 0;

            _enemyScore = _wave;

            for (; 0 < _enemyScore;)
            {
                if (_enemyScore > 1 && Random.Range(0, 2) == 0)
                {
                    enemy2Count++;
                    _enemyScore = _enemyScore - 2;
                }
                else
                {
                    enemy1Count++;
                    _enemyScore--;
                }
            }

            StartCoroutine(SpawnEnemy(enemy1Count, enemy2Count));
            _wave++;      
        }
        _bEnemySpawn = true;
        _time = 0;
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

        _bEnemySpawn = false;
    }

    private void SpawnNormalEnemy(int enemy, int dir)
    {
        Instantiate(NormalEnemy[enemy], new Vector3(9.5f * dir, -1.5f, 0), Quaternion.identity);
    }

    private void SpawnBossEnemy(int enemy, int dir)
    {
        Instantiate(BossEnemy[enemy], new Vector3(9.5f * dir, -1.5f, 0), Quaternion.identity);
    }
}
