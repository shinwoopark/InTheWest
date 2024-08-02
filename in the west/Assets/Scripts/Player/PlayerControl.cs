using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

enum PlayerState
{
    Idle,
    Rolling,
    Fire,
    FireBlindlyPistol,
    Load
}

public class PlayerControl : MonoBehaviour
{
    PlayerState CurrentState = PlayerState.Idle;

    private EnemySystem _enemySystem;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private int _playerDirection = 1;

    public float RollingSpeed;
    private float _rollingCooltime;
    private float _rollingTime;

    private int _fireCount;
    private bool _bAddFireCount;
    private float _fireTime;
    private float _fireBlindlyTime;

    private bool _bQuickLoad;
    private float _loadPistolCooltime;

    public LayerMask Eneny;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateInput();
        UpdateFireRaycast();
        UpdateFire();
        UpdateFireBlindlyPistol();
        UpdateLoad();
    }

    private void FixedUpdate()
    {
        UpdateRolling();
    }

    private void UpdateInput()
    {
        if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Load)
        {
            if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                _spriteRenderer.flipX = true;
                _playerDirection = -1;
            }
            if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            {
                _spriteRenderer.flipX = false;
                _playerDirection = 1;
            }

            _rollingCooltime -= Time.deltaTime;
            if (_rollingCooltime <= 0
                && Input.GetKeyDown(KeyCode.Space))
            {
                CurrentState = PlayerState.Rolling;
                _rollingCooltime = 1;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                if (GameInstance.instance.PlayerWeapon == "Pistol"
                    && GameInstance.instance.PistolBullets > 0)
                {
                    _bAddFireCount = false;
                    CurrentState = PlayerState.Fire;
                    GameInstance.instance.PistolBullets--;
                }
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (GameInstance.instance.PlayerWeapon == "Pistol"
                    && GameInstance.instance.PistolBullets > 0)
                {
                    _bAddFireCount = false;
                    CurrentState = PlayerState.FireBlindlyPistol;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.C) && CurrentState == PlayerState.Idle)
        {
            if (GameInstance.instance.PlayerWeapon == "Pistol"
               && GameInstance.instance.PistolBullets < GameInstance.instance.MaxPistolBullets)
            {
                _bQuickLoad = GameInstance.instance.Item1;
                CurrentState = PlayerState.Load;
            }
        }
    }

    private void UpdateRolling()
    {
        if (CurrentState == PlayerState.Rolling && _rollingTime > 0)
        {
            transform.position += Vector3.right * _playerDirection * RollingSpeed * Time.deltaTime;
            _rollingTime -= Time.deltaTime;

            if (_rollingTime <= 0)
                CurrentState = PlayerState.Idle;
        }
        else
        {
            _rollingTime = 0.05f;
        } 
    }

    private void UpdateFireRaycast()
    {
        if (CurrentState == PlayerState.Fire || CurrentState == PlayerState.FireBlindlyPistol)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.right * _playerDirection, 20, Eneny);
            Debug.DrawRay(transform.position, Vector3.right * _playerDirection * 20, Color.blue);

            if (_fireCount > 0 && hit.collider != null)
            {
                Debug.Log(hit.collider);
                _enemySystem = hit.collider.gameObject.GetComponent<EnemySystem>();
                _enemySystem.Hit();
                _fireCount--;
            }
        }          
    }

    private void UpdateFire()
    {
        if (CurrentState == PlayerState.Fire)
        {
            _fireTime += Time.deltaTime;

            if (GameInstance.instance.PlayerWeapon == "Pistol")
            {
                _animator.SetBool("bFirePistol", true);

                if (!_bAddFireCount && _fireTime >= 0.1f)
                {
                    _fireCount++;
                    _bAddFireCount = true;
                }
                    

                if (_fireTime >= 0.3f)
                    CurrentState = PlayerState.Idle;
            }        
        }
        else
        {
            _fireTime = 0;
            _animator.SetBool("bFirePistol", false);
        }           
    }

    private void UpdateFireBlindlyPistol()
    {
        if(CurrentState == PlayerState.FireBlindlyPistol)
        {
            _animator.SetBool("bFireBlindlyPistol", true);
            _fireBlindlyTime += Time.deltaTime;          
            
            if (!_bAddFireCount && _fireBlindlyTime >= 0.2f)
            {
                _fireCount = GameInstance.instance.PistolBullets;
                GameInstance.instance.PistolBullets = 0;
                _bAddFireCount = true;
            }

            if (_fireBlindlyTime >= 0.5f)
                CurrentState = PlayerState.Idle;
        }
        else
        {
            _fireBlindlyTime = 0;
            _animator.SetBool("bFireBlindlyPistol", false);
        }
    }

    private void UpdateLoad()
    {
        if(CurrentState == PlayerState.Load)
        {
            _loadPistolCooltime -= Time.deltaTime;

            if (GameInstance.instance.PlayerWeapon == "Pistol")
            {
                if (_bQuickLoad)
                {
                    _animator.SetBool("bQuickLoadPistol", true);

                    if (_loadPistolCooltime / 2 <= 0)
                    {
                        GameInstance.instance.PistolBullets = GameInstance.instance.MaxPistolBullets;
                    }         
                }
                else
                {
                    _animator.SetBool("bLoadPistol", true);

                    if (_loadPistolCooltime <= 0)
                    {
                        GameInstance.instance.PistolBullets++;
                        _loadPistolCooltime = 0.5f;
                    }
                }

                if(GameInstance.instance.PistolBullets == GameInstance.instance.MaxPistolBullets)
                {
                    CurrentState = PlayerState.Idle;
                }
            }          
        }
        else
        {
            _loadPistolCooltime = 0.6f;
            _animator.SetBool("bLoadPistol", false);
            _animator.SetBool("bQuickLoadPistol", false);
        }
    }
}
