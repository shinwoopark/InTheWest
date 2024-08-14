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
    State CurrentState = State.Idle;

    private EnemySystem _enemySystem;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private PlayerSystem _playerSystem;
    private GameObject _player_gb;

    public float MoveSpeed;
    public int Damage;
    public float KnunkBack;

    private float _distance;

    private bool _bAtk;
    private float _atkTime;
    private float _atkCoolTime = 2;
    private float _atkCurTime;

    public LayerMask Player;
    public Transform AtkBosPos;
    public Vector2 AckBoxSize;

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
        UpdateChoosePattern();
        UpdateDistance();
        UpdateAtkBox();
    }

    private void UpdateDistance()
    {
        _distance = Vector3.Distance(transform.position, _player_gb.transform.position);
    }

    private void UpdateAtkBox()
    {
       

        Collider2D attackBox = Physics2D.OverlapBox(AtkBosPos.position + new Vector3(_enemySystem.Player_dir, 0, 0), AckBoxSize, 0, Player);

        if (attackBox != null && _bAtk)
        {
            _playerSystem.Hit(Damage, KnunkBack, transform.position.x);
            _bAtk = false;
        }
    }

    private void UpdateChoosePattern()
    {
        _atkCurTime += Time.deltaTime;

        if (_atkCurTime >= _atkCoolTime)
        {
            CurrentState = State.Attack;
            StartCoroutine(Pattern1());
            _atkCurTime = 0;
        }
    }

    private IEnumerator Pattern1()
    {
        _animator.SetBool("bAtk1", true);
        Damage = 1;
        KnunkBack = 10;
        AtkBosPos.transform.localPosition = new Vector3(_enemySystem.Player_dir, 0, 0);
        AckBoxSize = new Vector2(2, 1);
        yield return new WaitForSeconds(0.2f);
        _bAtk = true;
        yield return new WaitForSeconds(0.1f);
        _bAtk = false;
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("bAtk1", false);       
        CurrentState = State.Idle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_enemySystem != null)
            Gizmos.DrawWireCube(AtkBosPos.position, AckBoxSize);
    }
}
