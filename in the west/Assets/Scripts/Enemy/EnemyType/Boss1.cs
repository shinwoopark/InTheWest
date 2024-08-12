using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State
{
    Idle,
    Move,
    Attack,
    Dead
}

public class Boss1 : MonoBehaviour
{
    private EnemySystem _enemySystem;

    private void Awake()
    {
        _enemySystem = GetComponent<EnemySystem>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }


}
