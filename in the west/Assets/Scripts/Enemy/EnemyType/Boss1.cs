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
        Stern
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

    public float JumpPower;
    public float RollSpeed;

    private int _damage;
    private float _knunkBack;

    private int _down = 0;

    private int _dir;
    private float _distance;

    private bool _bJump;
    private bool _bRoll;

    private bool _bAtk;
    private float _atkTime;
    private float _atkCoolTime = 1f;
    private float _atkCurTime;

    private float _sternTime;

    public LayerMask Player;
    public Transform AtkBosPos;
    private Vector2 _ackBoxSize;

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
        if (GameInstance.instance.bPlaying
            && !GameInstance.instance.bPause
            && _enemySystem.Hp > 0)
        {
            UpdatePlayerPos();
            UpdateDistance();
            UpdateStern();

            if (CurrentState != State.Stern)
            {
                UpdateDir();
                UpdateChoosePattern();
                UpdateAtkBox();
            }
        }        
    }

    private void FixedUpdate()
    {
        if (GameInstance.instance.bPlaying
            && !GameInstance.instance.bPause
            && _enemySystem.Hp > 0
            && CurrentState != State.Stern)
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

        if (_bJump || _bRoll)
            gameObject.layer = 10;
        else
            gameObject.layer = 7;
    }

    private void UpdateDistance()
    {
        _distance = Vector3.Distance(transform.position, _player_gb.transform.position);
    }

    private void UpdateAtkBox()
    {
        Collider2D attackBox = Physics2D.OverlapBox(AtkBosPos.position, _ackBoxSize, 0, Player);

        if (attackBox != null && _bAtk)
        {
            _playerSystem.Hit(_damage, _knunkBack, transform.position.x);
            _bAtk = false;
        }
    }

    private void UpdateStern()
    {
        if (CurrentState == State.Stern)
        {
            _sternTime -= Time.deltaTime;

            if (_sternTime <= 0)
            {
                _spriteRenderer.color = new Color(1, 1, 1, 1);
                _animator.SetBool("bStern", false);
                CurrentState = State.Idle;
            }
        }
    }

    private void Blink()
    {
        if (CurrentState == State.Stern && _enemySystem.Hp > 0)
        {
            if (_spriteRenderer.color.a == 1)
                _spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            else
                _spriteRenderer.color = new Color(1, 1, 1, 1);

            Invoke("Blink", 0.25f);
        }
    }

    private void UpdateChoosePattern()
    {
        if(CurrentState != State.Stern)
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
        _damage = 1;
        _knunkBack = 10;
        AtkBosPos.transform.localPosition = new Vector3(_dir, 0, 0);
        _ackBoxSize = new Vector2(2, 1);
        yield return new WaitForSeconds(0.5f);
        _bAtk = true;
        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Swing);
        yield return new WaitForSeconds(0.1f);
        _bAtk = false;
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("bAtk1", false);
        EndPattern();
    }

    private IEnumerator Atk2()
    {
        _animator.SetBool("bAtk2", true);
        _damage = 1;
        _knunkBack = 10;
        AtkBosPos.transform.localPosition = new Vector3(_dir, 0, 0);
        _ackBoxSize = new Vector2(2, 1);

        yield return new WaitForSeconds(0.5f);

        for(int i = 0; i < 2; i++)
        {
            _bAtk = true;
            SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Swing);
            yield return new WaitForSeconds(0.1f);
            _bAtk = false;
            yield return new WaitForSeconds(0.2f);
        }

        _animator.SetBool("bAtk2", false);
        EndPattern();
    }

    private IEnumerator Atk3()
    {
        _animator.SetBool("bAtk3", true);
        _damage = 1;
        _knunkBack = 10;
        AtkBosPos.transform.localPosition = new Vector3(_dir, 0, 0);
        _ackBoxSize = new Vector2(2, 1);
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 2; i++)
        {
            _bAtk = true;
            SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Swing);
            yield return new WaitForSeconds(0.1f);
            _bAtk = false;
            yield return new WaitForSeconds(0.2f);
        }

        _damage = 2;
        _knunkBack = 15;
        _bAtk = true;
        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Swings);
        yield return new WaitForSeconds(0.1f);
        _bAtk = false;
        yield return new WaitForSeconds(0.6f);
        _animator.SetBool("bAtk3", false);
        EndPattern();
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

        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Throw);

        yield return new WaitForSeconds(0.3f);

        _animator.SetBool("bTrapCast", false);
        _bJump = false;
        EndPattern();
    }

    private IEnumerator ProjectileCast()
    {
        _animator.SetBool("bProjectileCast", true);

        yield return new WaitForSeconds(0.3f);

        _enemyProjectile = Instantiate(Projectile, transform.position, Quaternion.identity).GetComponent<EnemyProjectile>();

        _enemyProjectile.Dir = new Vector3(_dir, 0, 0);
        _enemyProjectile.Dir.Normalize();
        _enemyProjectile.SetDir();

        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Throw);

        yield return new WaitForSeconds(0.3f);

        _animator.SetBool("bProjectileCast", false);
        EndPattern();
    }

    private IEnumerator Roll()
    {
        _animator.SetBool("bRoll", true);
        _bRoll = true;
        SoundManager.soundManager.PlaySfx( SoundManager.Sfx.Roll);

        yield return new WaitForSeconds(0.5f);

        _animator.SetBool("bRoll", false);
        _bRoll = false;
        EndPattern();
    }

    private IEnumerator SpAtk()
    {
        yield return new WaitForSeconds(0.5f);

        _animator.SetBool("bSpAtk", true);
        transform.position = _player_gb.transform.position + new Vector3(_dir * 1.5f, 0, 0);
        _damage = 1;
        _knunkBack = 5;
        AtkBosPos.transform.localPosition = Vector3.zero;
        _ackBoxSize = new Vector2(3, 1);

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 2; i++)
        {
            float delay = 0;

            if (i == 1)
            {
                _damage = 2;
                _knunkBack = 20;
                _ackBoxSize = new Vector2(7, 1);
                delay = 0.5f;
            }

            yield return new WaitForSeconds(delay);
            _bAtk = true;

            if (i == 0)
                SoundManager.soundManager.PlaySfx(SoundManager.Sfx.SpAtk1);
            else
                SoundManager.soundManager.PlaySfx(SoundManager.Sfx.SpAtk2);

            yield return new WaitForSeconds(0.1f);
            _bAtk = false;
            yield return new WaitForSeconds(0.4f);
        }

        _animator.SetBool("bSpAtk", false);

        EndPattern();
    }

    private void EndPattern()
    {
        if(CurrentState != State.Stern)
            CurrentState = State.Idle;

        _atkCurTime = 0;
        _atkCoolTime = Random.Range(0.75f, 1.5f);
    }

    public void Stern()
    {
        _sternTime = 1;
        CurrentState = State.Stern;
        Blink();
        _animator.SetBool("bStern", true);
        _animator.SetBool("bAttack", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_enemySystem != null)
            Gizmos.DrawWireCube(AtkBosPos.position, _ackBoxSize);
    }
}
