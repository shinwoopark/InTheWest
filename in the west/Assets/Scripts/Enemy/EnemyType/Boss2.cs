using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    enum State
    {
        Idle,
        Run,
        Roll,
        Defend,
        Atk,
        SpAtk,
        Stern
    }

    State CurrentState = State.Idle;

    private EnemySystem _enemySystem;
    private PlayerSystem _playerSystem;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private GameObject _player_gb;
    private Transform _player_tr;

    public float RunSpeed;
    public float RollSpeed;

    private bool _bAtk;
    private int _damage;
    private float _knunkBack;

    private int _dir;
    private float _distance;

    public float RayLenth;

    private bool _bRun;

    private int _spAtkCount;
    private int _movePatternCount;

    private float _sternTime;
    private bool _bReservationPattern;

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

    private void Start()
    {
        Invoke("PatternStart", 1);
    }

    private void PatternStart()
    {
        ChoosePattern();
    }

    private void Update()
    {
        if (GameInstance.instance.bPlaying
            && !GameInstance.instance.bPause
            && _enemySystem.Hp > 0)
        {
            UpdatePlayerPos();
            UpdateDir();
            UpdateDistance();
            UpdateLayer();
            UpdateAtkBox();
            UpdateRun();
            UpdateStern();
            Blink();
        }
    }

    private void FixedUpdate()
    {
        if (GameInstance.instance.bPlaying
            && !GameInstance.instance.bPause
            && _enemySystem.Hp > 0)
            UpdateMove();
    }

    private void UpdatePlayerPos()
    {
        _player_tr.position = _player_gb.transform.position;
    }

    private void UpdateDir()
    {
        if (CurrentState == State.Idle
            || CurrentState == State.Run)
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

    private void UpdateDistance()
    {
        _distance = Vector3.Distance(transform.position, _player_gb.transform.position);
    }

    private void UpdateMove()
    {
        if (CurrentState == State.Run)
        {
            transform.position += new Vector3(RunSpeed * _dir, 0, 0) * Time.deltaTime;
        }
        else if (CurrentState == State.Roll)
        {
            transform.position += new Vector3(RollSpeed * _dir, 0, 0) * Time.deltaTime;
        }
    }

    private void UpdateRun()
    {
        if (_bRun)
        {
            _animator.SetBool("bRun", true);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.right * _dir, RayLenth, Player);
            Debug.DrawRay(transform.position, Vector3.right * _dir * RayLenth, Color.red);

            if (hit.collider != null)
            {
                _animator.SetBool("bRun", false);
                _bRun = false;
                EndPattern();
            }
        }
    }

    private void UpdateLayer()
    {
        if (CurrentState == State.Roll
            || CurrentState == State.Defend
            || CurrentState == State.SpAtk)
            gameObject.layer = 10;
        else
            gameObject.layer = 7;
    }

    private void UpdateAtkBox()
    {
        Collider2D attackBox = Physics2D.OverlapBox(AtkBosPos.position, _ackBoxSize, 0, Player);

        if (attackBox != null && _bAtk)
        {
            if(CurrentState == State.Atk || CurrentState == State.SpAtk)
            {
                _playerSystem.Hit(_damage, _knunkBack, transform.position.x);
                _bAtk = false;
            }          
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

                if (_bReservationPattern)
                {
                    ChoosePattern();
                    _bReservationPattern = false;
                }
                
                if(_bRun)
                    CurrentState = State.Run;
                else
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

    private void ChoosePattern()
    {
        if (GameInstance.instance.bPlaying
            && !GameInstance.instance.bPause
            && _enemySystem.Hp > 0)
        {
            if (_spAtkCount == 3)
            {
                StartCoroutine(SpAtk());
                CurrentState = State.SpAtk;
                _spAtkCount = 0;
            }
            else if (_distance <= 2.5f)
            {
                if (Random.Range(0, 2) == 0)
                    StartCoroutine(Atk1());
                else
                    StartCoroutine(Atk2());

                CurrentState = State.Atk;
                _spAtkCount++;
            }
            else
            {
                switch (_movePatternCount)
                {
                    case 0:
                        _movePatternCount = Random.Range(1, 3);
                        break;
                    case 1:
                        if (Random.Range(0, 2) == 0)
                            _movePatternCount = 0;
                        else
                            _movePatternCount = 2;
                        break;
                    case 2:
                        _movePatternCount = Random.Range(0, 2);
                        break;
                }

                switch (_movePatternCount)
                {
                    case 0:
                        Run();
                        break;
                    case 1:
                        StartCoroutine(Roll());
                        break;
                    case 2:
                        StartCoroutine(Defend());
                        break;
                }
            }
        }       
    }

    private void Run()
    {
        CurrentState = State.Run;
        _bRun = true;
    }

    private IEnumerator Roll()
    {
        _animator.SetBool("bRoll", true);
        CurrentState = State.Roll;
        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Roll);
        yield return new WaitForSeconds(0.4f);
        _animator.SetBool("bRoll", false);
        EndPattern();
    }

    private IEnumerator Defend()
    {
        _animator.SetBool("bDefend", true);
        CurrentState = State.Defend;
        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Defend);
        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("bDefend", false);
        EndPattern();
    }

    private IEnumerator Atk1()
    {
        _animator.SetBool("bAtk1", true);
        _damage = 1;
        _knunkBack = 10;
        AtkBosPos.transform.localPosition = new Vector3(_dir, 0, 0);
        _ackBoxSize = new Vector2(2, 1);
        yield return new WaitForSeconds(0.3f);
        _bAtk = true;
        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Swing);
        yield return new WaitForSeconds(0.1f);
        _bAtk = false;
        yield return new WaitForSeconds(0.4f);
        _animator.SetBool("bAtk1", false);
        EndPattern();
    }

    private IEnumerator Atk2()
    {
        _animator.SetBool("bAtk2", true);
        _damage = 1;
        _knunkBack = 5;
        AtkBosPos.transform.localPosition = new Vector3(_dir, 0, 0);
        _ackBoxSize = new Vector2(2, 1);

        yield return new WaitForSeconds(0.3f);
        _bAtk = true;
        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Swing);
        yield return new WaitForSeconds(0.1f);
        _bAtk = false;
        yield return new WaitForSeconds(0.3f);
        _knunkBack = 2.5f;
        AtkBosPos.transform.localPosition = new Vector3(_dir * 1.5f, 0, 0);
        _ackBoxSize = new Vector2(3, 1);
        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Swings);

        for (int i = 0; i < 3; i++)
        {
            _bAtk = true;
            yield return new WaitForSeconds(0.1f);
            _bAtk = false;
            yield return new WaitForSeconds(0.1f);
        }     

        yield return new WaitForSeconds(0.3f);
        _animator.SetBool("bAtk2", false);
        EndPattern();
    }

    private IEnumerator SpAtk()
    {
        _animator.SetBool("bSpAtk", true);
        _damage = 1;
        _knunkBack = 0;
        AtkBosPos.transform.localPosition = new Vector3(0, 0, 0);
        _ackBoxSize = new Vector2(2, 2);
        float startPos = gameObject.transform.position.x;
        yield return new WaitForSeconds(0.5f);     

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.35f);
            transform.position = _player_tr.position;
            yield return new WaitForSeconds(0.4f);
            SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Swing);
            _bAtk = true;
            yield return new WaitForSeconds(0.1f);
            _bAtk = false;
        }
        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("bSpAtk", false);
        transform.position = new Vector3(startPos, -3, 0);
        yield return new WaitForSeconds(0.3f);
        CurrentState = State.Idle;
        yield return new WaitForSeconds(0.2f);
        EndPattern();
    }

    private void EndPattern()
    {
        if (CurrentState != State.Stern)
        {
            CurrentState = State.Idle;
            ChoosePattern();
        }
        else
            _bReservationPattern = true;
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
