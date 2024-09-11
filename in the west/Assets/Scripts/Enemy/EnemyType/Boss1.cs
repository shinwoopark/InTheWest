using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    enum State
    {
        Idle,
        Move,
        Attack,
        Dead
    }

    State CurrentState = State.Idle;

    private EnemySystem _enemySystem;
    private EnemyProjectile _enemyProjectile;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private PlayerSystem _playerSystem;
    private GameObject _player_gb;
    private Transform _player_tr;

    public GameObject Projectile;

    public Transform _jumpPos;

    public float JumpPower;
    public float RollSpeed;
    public int Damage;
    public float KnunkBack;

    private int _down = 0;

    private int _dir;
    private float _distance;

    private bool _bJump;
    private bool _bRoll;

    private bool _bAtk;
    private float _atkTime;
    private float _atkCoolTime = 1f;
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
        _player_tr = _player_gb.GetComponent<Transform>();
        _playerSystem = _player_gb.GetComponent<PlayerSystem>();
    }

    private void Update()
    {
        if (!GameInstance.instance.bPlaying) return;
        if (_enemySystem.Hp <= 0) return;

        UpdateDir();
        UpdatePlayerPos();
        UpdateChoosePattern();
        UpdateDistance();
        UpdateAtkBox();
    }

    private void FixedUpdate()
    {
        if (!GameInstance.instance.bPlaying) return;
        if (_enemySystem.Hp <= 0) return;

        UpdateMove();
    }

    private void UpdatePlayerPos()
    {
        _player_tr.position = _player_gb.transform.position;
    }

    private void UpdateDir()
    {
        if (CurrentState == State.Idle)
        {
            if (transform.position.x < _player_gb.transform.position.x)
            {
                _dir = 1;
                _spriteRenderer.flipX = false;
            }             
            else
            {
                _dir = -1;
                _spriteRenderer.flipX = true;
            }            
        }       
    }

    private void UpdateMove()
    {
        if (_bJump)
        {
            transform.position += new Vector3(-_enemySystem.Player_dir * JumpPower, 0, 0) * Time.deltaTime;
        }

        if (_bRoll)
        {
            transform.position += new Vector3(_enemySystem.Player_dir * RollSpeed, 0, 0) * Time.deltaTime;
        }
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
            int atkPattern = 0;

            if (_distance <= 2.5f)
            {
                if (_down == 2)
                {
                    atkPattern = Random.Range(0, 3);

                    if (atkPattern == 0)
                    {
                        StartCoroutine(Atk1());
                    }
                    else if (atkPattern == 1)
                    {
                        StartCoroutine(Atk2());
                    }
                    else
                    {
                        StartCoroutine(TrapCast());
                    }
                }
                else
                {
                    atkPattern = Random.Range(0, 4);

                    if (atkPattern == 0)
                    {
                        StartCoroutine(Atk1());
                    }
                    else if (atkPattern == 1)
                    {
                        StartCoroutine(Atk2());
                    }
                    else if (atkPattern == 2)
                    {
                        StartCoroutine(Atk3());
                    }
                    else
                    {
                        StartCoroutine(TrapCast());
                    }
                }

                CurrentState = State.Attack;
            }
            else if (_distance <= 7)
            {
                atkPattern = Random.Range(0, 2);

                if (atkPattern == 0)
                {
                    StartCoroutine(Roll());
                    CurrentState = State.Move;
                }
                else
                {
                    StartCoroutine(ProjectileCast());
                    CurrentState = State.Attack;
                }  
            }
            else
            {
                StartCoroutine(SpAtk());
                CurrentState = State.Attack;
            }     
        }
    }

    private IEnumerator Atk1()
    {
        _animator.SetBool("bAtk1", true);
        Damage = 1;
        KnunkBack = 10;
        AtkBosPos.transform.localPosition = new Vector3(_dir, 0, 0);
        AckBoxSize = new Vector2(2, 1);
        yield return new WaitForSeconds(0.5f);
        _bAtk = true;
        yield return new WaitForSeconds(0.1f);
        _bAtk = false;
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("bAtk1", false);       
        CurrentState = State.Idle;
        _atkCurTime = 0;
        _atkCoolTime = Random.Range(0.75f, 1.5f);
    }

    private IEnumerator Atk2()
    {
        _animator.SetBool("bAtk2", true);
        Damage = 1;
        KnunkBack = 10;
        AtkBosPos.transform.localPosition = new Vector3(_dir, 0, 0);
        AckBoxSize = new Vector2(2, 1);

        yield return new WaitForSeconds(0.5f);

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
        _atkCoolTime = Random.Range(0.75f, 1.5f);
    }

    private IEnumerator Atk3()
    {
        _animator.SetBool("bAtk3", true);
        Damage = 1;
        KnunkBack = 10;
        AtkBosPos.transform.localPosition = new Vector3(_dir, 0, 0);
        AckBoxSize = new Vector2(2, 1);
        yield return new WaitForSeconds(0.5f);

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
        _atkCoolTime = Random.Range(0.75f, 1.5f);
    }

    private IEnumerator TrapCast()
    {
        _animator.SetBool("bTrapCast", true);
        _bJump = true;

        yield return new WaitForSeconds(0.3f);

        _enemyProjectile = Instantiate(Projectile, transform.position, Quaternion.identity).GetComponent<EnemyProjectile>();

        _enemyProjectile.Dir = new Vector3(_dir, 0, 0);
        _enemyProjectile.Dir.Normalize();
        _enemyProjectile.SetDir();

        yield return new WaitForSeconds(0.3f);

        _animator.SetBool("bTrapCast", false);
        _bJump = false;
        CurrentState = State.Idle;
        _atkCurTime = 0;
        _atkCoolTime = Random.Range(0.75f, 1.5f);
    }

    private IEnumerator ProjectileCast()
    {
        _animator.SetBool("bProjectileCast", true);

        yield return new WaitForSeconds(0.3f);

        _enemyProjectile = Instantiate(Projectile, transform.position, Quaternion.identity).GetComponent<EnemyProjectile>();

        _enemyProjectile.Dir = new Vector3(_dir, 0, 0);
        _enemyProjectile.Dir.Normalize();
        _enemyProjectile.SetDir();

        yield return new WaitForSeconds(0.3f);

        _animator.SetBool("bProjectileCast", false);
        CurrentState = State.Idle;
        _atkCurTime = 0;
        _atkCoolTime = Random.Range(0.75f, 1.5f);
    }

    private IEnumerator Roll()
    {
        _animator.SetBool("bRoll", true);
        _bRoll = true;

        yield return new WaitForSeconds(0.5f);

        _animator.SetBool("bRoll", false);
        _bRoll = false;
        CurrentState = State.Idle;
        _atkCurTime = 0;
        _atkCoolTime = Random.Range(0.75f, 1.5f);
    }

    private IEnumerator SpAtk()
    {
        yield return new WaitForSeconds(0.5f);

        _animator.SetBool("bSpAtk", true);
        transform.position = _player_gb.transform.position + new Vector3(_dir * 1.5f, 0, 0);
        Damage = 1;
        KnunkBack = 5;
        AtkBosPos.transform.localPosition = Vector3.zero;
        AckBoxSize = new Vector2(3, 1);

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 2; i++)
        {
            float delay = 0;

            if (i == 1)
            {
                Damage = 2;
                KnunkBack = 20;
                AckBoxSize = new Vector2(7, 1);
                delay = 0.5f;
            }

            yield return new WaitForSeconds(delay);
            _bAtk = true;
            yield return new WaitForSeconds(0.1f);
            _bAtk = false;
            yield return new WaitForSeconds(0.4f);
        }

        _animator.SetBool("bSpAtk", false);

        CurrentState = State.Idle;
        _atkCurTime = 0;
        _atkCoolTime = Random.Range(0.75f, 1.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_enemySystem != null)
            Gizmos.DrawWireCube(AtkBosPos.position, AckBoxSize);
    }
}
