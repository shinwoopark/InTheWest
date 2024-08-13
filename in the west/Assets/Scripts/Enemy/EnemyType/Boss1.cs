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
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private PlayerSystem _playerSystem;
    private GameObject _player_gb;

    private void Awake()
    {
        _enemySystem = GetComponent<EnemySystem>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _player_gb = GameObject.Find("Player");
        _playerSystem = _player_gb.GetComponent<PlayerSystem>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void ChoosePattern()
    {

    }

    private void Pattern1()
    {

    }
}
