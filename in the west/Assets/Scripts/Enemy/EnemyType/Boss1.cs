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
        ProjectileCast,
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

    public GameObject Projectile, Trap;

    public Transform _jumpPos;

    public float MoveSpeed;
    public float JumpPower;
    public int Damage;
    public float KnunkBack;

    private int _phase;

    private int _dir;
    private float _distance;

    private bool _bJump;

    float timer = 0;
    int movement = 0;
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
        _player_tr = _player_gb.GetComponent<Transform>();
        _playerSystem = _player_gb.GetComponent<PlayerSystem>();
    }

    private void Start()
    {
        _phase = 1;
    }

    private void Update()
    {
        Debug.Log(_dir);

        UpdateMove();
        UpdatePlayerPos();
        UpdateChoosePattern();
        UpdateDistance();
        UpdateAtkBox();
        UpdateProjectileCast();
    }

    private void FixedUpdate()
    {
        UpdateBackJump();
    }

    private void UpdatePlayerPos()
    {
        _player_tr.position = _player_gb.transform.position;
    }

    private void UpdateMove()
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

    private void UpdateBackJump()
    {
        if (_bJump)
        {
            transform.position += new Vector3(-_enemySystem.Player_dir * JumpPower, 0, 0) * Time.deltaTime;
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
            //int atkPattern = 0;

            //if (_distance <= 3)
            //{
            //    if(_phase == 1)
            //    {
            //        atkPattern = Random.Range(0, 5);

            //        if (atkPattern < 2)
            //        {
            //            StartCoroutine(Atk1());
            //        }
            //        else if (atkPattern < 4)
            //        {
            //            StartCoroutine(Atk2());
            //        }
            //        else
            //        {

            //        }
            //    }
            //    else
            //    {
            //        atkPattern = Random.Range(0, 7);

            //        if (atkPattern < 2)
            //        {
            //            StartCoroutine(Atk1());
            //        }
            //        else if (atkPattern < 4)
            //        {
            //            StartCoroutine(Atk2());
            //        }
            //        else if (atkPattern < 6)
            //        {
            //            StartCoroutine(Atk3());
            //        }
            //        else
            //        {

            //        }
            //    }
            //}
            //else if(_distance <= 6)
            //{

            //}
            //else
            //{

            //}


            CurrentState = State.Attack;
            StartCoroutine(ProjectileCast());
        }
    }

    private IEnumerator Atk1()
    {
        _animator.SetBool("bAtk1", true);
        Damage = 1;
        KnunkBack = 10;
        AtkBosPos.transform.localPosition = new Vector3(_dir, 0, 0);
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
        AtkBosPos.transform.localPosition = new Vector3(_dir, 0, 0);
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
        AtkBosPos.transform.localPosition = new Vector3(_dir, 0, 0);
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

    private IEnumerator TrapCast()
    {
        _animator.SetBool("bTrapCast", true);
        _bJump = true;

        yield return new WaitForSeconds(0.3f);

        Instantiate(Projectile, transform.position, Quaternion.identity);

        _enemyProjectile = Projectile.GetComponent<EnemyProjectile>();
        _enemyProjectile.Dir = _player_tr.position - transform.position;

        yield return new WaitForSeconds(0.3f);

        _animator.SetBool("bTrapCast", false);
        _bJump = false;
        CurrentState = State.Idle;
        _atkCurTime = 0;
    }

    private IEnumerator ProjectileCast()
    {
        _animator.SetBool("bProjectileCast", true);

        yield return new WaitForSeconds(0.3f);

        Instantiate(Projectile, transform.position, Quaternion.identity);
        _enemyProjectile = Projectile.GetComponent<EnemyProjectile>();
        _enemyProjectile.Dir = new Vector3(_dir, 0, 0);

        yield return new WaitForSeconds(0.3f);

        _animator.SetBool("bProjectileCast", false);
        CurrentState = State.Idle;
        _atkCurTime = 0;
    }

    private void UpdateProjectileCast()
    {
        if(CurrentState == State.ProjectileCast)
        {
            timer += Time.deltaTime;

            if (movement == 0)
            {
                _animator.SetBool("bProjectileCast", true);
                movement++;
            }
            

            if (timer >= 0.3f && movement == 1)
            {
                //FireProjectile();
                movement++;
            }

            if (timer >= 0.6f && movement == 2)
            {
                _animator.SetBool("bProjectileCast", false);               
                _atkCurTime = 0;
                timer = 0;
                movement = 0;
                CurrentState = State.Idle;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_enemySystem != null)
            Gizmos.DrawWireCube(AtkBosPos.position, AckBoxSize);
    }
}
