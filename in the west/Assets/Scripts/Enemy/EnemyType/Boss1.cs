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

    public GameObject Projectile, Trap;

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
        Collider2D attackBox = Physics2D.OverlapBox(AtkBosPos.position, AckBoxSize, 0, Player);

        if (attackBox != null && _bAtk)
        {
            _playerSystem.Hit(Damage, KnunkBack, transform.position.x);
            _bAtk = false;
        }
    }

    private void UpdateChoosePattern()
    {
        _atkCurTime += Time.deltaTime;



        if (_atkCurTime >= _atkCoolTime && CurrentState == State.Idle)
        {
            if (_distance <= 3)
            {

            }
            else if(_distance <= 6)
            {

            }
            else
            {

            }


            CurrentState = State.Attack;
            StartCoroutine(ProjectileCast());
        }
    }

    private IEnumerator Atk1()
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
        _atkCurTime = 0;
    }

    private IEnumerator Atk2()
    {
        _animator.SetBool("bAtk2", true);
        Damage = 1;
        KnunkBack = 10;
        AtkBosPos.transform.localPosition = new Vector3(_enemySystem.Player_dir, 0, 0);
        AckBoxSize = new Vector2(2, 1);

        yield return new WaitForSeconds(0.2f);

        for(int i = 0; i < 2; i++)
        {
            _bAtk = true;
            yield return new WaitForSeconds(0.1f);
            _bAtk = false;
            yield return new WaitForSeconds(0.2f);
        }

        _animator.SetBool("bAtk2", false);
        CurrentState = State.Idle;
        _atkCurTime = 0;
    }

    private IEnumerator Atk3()
    {
        _animator.SetBool("bAtk3", true);
        Damage = 1;
        KnunkBack = 10;
        AtkBosPos.transform.localPosition = new Vector3(_enemySystem.Player_dir, 0, 0);
        AckBoxSize = new Vector2(2, 1);
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < 2; i++)
        {
            _bAtk = true;
            yield return new WaitForSeconds(0.1f);
            _bAtk = false;
            yield return new WaitForSeconds(0.2f);
        }

        Damage = 2;
        KnunkBack = 15;
        _bAtk = true;
        yield return new WaitForSeconds(0.1f);
        _bAtk = false;
        yield return new WaitForSeconds(0.6f);
        _animator.SetBool("bAtk3", false);
        CurrentState = State.Idle;
        _atkCurTime = 0;
    }

    private IEnumerator ProjectileCast()
    {
        _animator.SetBool("bProjectileCast", true);

        yield return new WaitForSeconds(0.3f);

        Instantiate(Projectile, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.3f);

        _animator.SetBool("bProjectileCast", false);
        CurrentState = State.Idle;
        _atkCurTime = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_enemySystem != null)
            Gizmos.DrawWireCube(AtkBosPos.position, AckBoxSize);
    }
}
