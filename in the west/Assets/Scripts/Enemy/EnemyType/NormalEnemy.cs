using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class NormalEnemy : MonoBehaviour
{
    enum State
    {
        Move,
        Attack,
        Stern
    }

    State CurrentState = State.Move;

    private EnemySystem _enemySystem;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private PlayerSystem _playerSystem;
    private GameObject _player_gb;

    public float MoveSpeed;
    public int Damage;
    public float KnunkBack;

    public float RayLenth;
    public LayerMask Player;
    public Vector2 AttackBoxSize;

    private int _dir;

    private bool _bSwing;

    private float _attackTime;
    private float _sternTime;

    private void Awake()
    {
        _enemySystem = GetComponent<EnemySystem>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _player_gb = GameObject.Find("Player");
        _playerSystem = _player_gb.GetComponent<PlayerSystem>();
    }

    private void Update()
    {
        if (GameInstance.instance.bPlaying
                && _enemySystem.Hp > 0)
        {
            UpdateRaycaset();
            UpdateAttack();
            UpdateStern();
        }          
    }

    private void FixedUpdate()
    {
        if (GameInstance.instance.bPlaying
                && _enemySystem.Hp > 0)
        UpdateMove();
    }

    private void UpdateMove()
    {
        if(CurrentState == State.Move)
        {
            transform.position += new Vector3(_enemySystem.Player_dir, 0, 0) * MoveSpeed * Time.deltaTime;

            if (_enemySystem.Player_dir == 1)
            {
                _dir = 1;
                _spriteRenderer.flipX = true;
            }              
            else
            {
                _dir = -1;
                _spriteRenderer.flipX = false;
            }           
        }     
    }

    private void UpdateAttack()
    {
        if(CurrentState == State.Attack)
        {
            _attackTime += Time.deltaTime;

            _animator.SetBool("bAttack", true);

            if (_attackTime >= 0.4 && !_bSwing)
            {
                Collider2D attackBox = Physics2D.OverlapBox(transform.position + new Vector3(_dir / 1.5f, 0, 0), AttackBoxSize, 0, Player);

                if (attackBox != null)
                {
                    _playerSystem.Hit(Damage, KnunkBack, transform.position.x);
                }

                SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Swing);
                _bSwing = true;
            }
            if (_attackTime >= 0.8)
            {
                _animator.SetBool("bAttack", false);
                CurrentState = State.Move;
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
                CurrentState = State.Move;
                _spriteRenderer.color = new Color(1, 1, 1, 1);
                _animator.SetBool("bStern", false);
            }
        }
    }

    private void Blink()
    {
        if (CurrentState == State.Stern  && _enemySystem.Hp > 0)
        {
            if (_spriteRenderer.color.a == 1)
                _spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            else
                _spriteRenderer.color = new Color(1, 1, 1, 1);

            Invoke("Blink", 0.25f);
        }           
    }

    private void UpdateRaycaset()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.right * _dir, RayLenth, Player);
        Debug.DrawRay(transform.position, Vector3.right * _dir * RayLenth, Color.red);

        if (CurrentState == State.Move && hit.collider != null)
        {
            _attackTime = 0;
            _bSwing = false;
            CurrentState = State.Attack;
        }
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
            Gizmos.DrawWireCube(transform.position + new Vector3(_dir / 1.5f, 0, 0), AttackBoxSize);
    }
}
