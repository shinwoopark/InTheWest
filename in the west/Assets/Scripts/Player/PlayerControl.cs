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
    public UIManager UIManager;
    private EnemySystem _enemySystem;

    public GameObject IdleCollider, RollingCollider;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private int _playerDirection = 1;

    public float RollingSpeed;
    private float _rollingCooltime;
    private float _rollingTime;

    private string _weapon;
    private int _fireCount;
    private bool _bAddFireCount;
    private float _fireTime;
    private float _fireBlindlyTime;

    private float _loadPistolCooltime;
    private float _loadRifleCooltime;

    public LayerMask Eneny;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!GameInstance.instance.bPlaying) return;

        if (GameInstance.instance.bPause)
            _animator.speed = 0;
        else
            _animator.speed = 1;

        UpdateCollider();
        UpdateInput();
        UpdateFireRaycast();
        UpdateFire();
        UpdateFireBlindlyPistol();
        UpdateLoad();
    }

    private void FixedUpdate()
    {
        if (!GameInstance.instance.bPlaying || GameInstance.instance.bPause) return;

        UpdateRolling();
    }

    private void UpdateCollider()
    {
        if (CurrentState == PlayerState.Rolling)
        {
            IdleCollider.SetActive(false);
            RollingCollider.SetActive(true);
            _animator.SetBool("bRolling", true);
        }
        else
        {
            IdleCollider.SetActive(true);
            RollingCollider.SetActive(false);
            _animator.SetBool("bRolling", false);
        }
    }

    private void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (GameInstance.instance.PlayerWeapon == "Pistol")
                GameInstance.instance.PlayerWeapon = "Rifle";
            else
                GameInstance.instance.PlayerWeapon = "Pistol";

            UIManager.ChangeWeapon();
        }

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
                _rollingCooltime = 0.5f;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                if (GameInstance.instance.PlayerWeapon == "Pistol"
                    && GameInstance.instance.PistolBullets > 0)
                {
                    _weapon = GameInstance.instance.PlayerWeapon;
                    _bAddFireCount = false;
                    CurrentState = PlayerState.Fire;
                    GameInstance.instance.PistolBullets--;
                }
                else if (GameInstance.instance.PlayerWeapon == "Rifle"
                         && GameInstance.instance.RifleBullets > 0)
                {
                    _weapon = GameInstance.instance.PlayerWeapon;
                    _bAddFireCount = false;
                    CurrentState = PlayerState.Fire;
                    GameInstance.instance.RifleBullets--;
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
                CurrentState = PlayerState.Load;

            if (GameInstance.instance.PlayerWeapon == "Rifle"
               && GameInstance.instance.RifleBullets < GameInstance.instance.MaxRifleBullets)
                CurrentState = PlayerState.Load;
        }

        if (Input.GetKeyDown(KeyCode.A)
            && GameInstance.instance.ItemInventroy[0] > 0
            && !GameInstance.instance.Item1)
        {
            GameInstance.instance.Item1 = true;
            GameInstance.instance.ItemInventroy[0]--;
        }

        if (Input.GetKeyDown(KeyCode.S)
            && GameInstance.instance.ItemInventroy[1] > 0
            && !GameInstance.instance.Item2)
        {
            GameInstance.instance.Item2 = true;
            GameInstance.instance.ItemInventroy[1]--;
        }

        if (Input.GetKeyDown(KeyCode.D)
           && GameInstance.instance.ItemInventroy[2] > 0
           && !GameInstance.instance.Item3)
        {
            GameInstance.instance.Item3 = true;
            GameInstance.instance.ItemInventroy[2]--;
        }
    }

    private void UpdateRolling()
    {
        if (CurrentState == PlayerState.Rolling && _rollingTime > 0)
        {
            float rollingSpeed = 1;

            if (GameInstance.instance.Item2)
            {
                rollingSpeed = 3;
            }

            transform.position += Vector3.right * _playerDirection * RollingSpeed * rollingSpeed * Time.deltaTime;
            _rollingTime -= Time.deltaTime;

            if (_rollingTime <= 0)
            {
                GameInstance.instance.Item2 = false;
                CurrentState = PlayerState.Idle;
            }
        }
        else
        {
            _rollingTime = 0.3f;
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
                _enemySystem = hit.collider.gameObject.GetComponent<EnemySystem>();
                _enemySystem.Hit(_weapon);
                _fireCount--;
            }
        }
    }

    private void UpdateFire()
    {
        if (CurrentState == PlayerState.Fire)
        {
            _fireTime += Time.deltaTime;

            if (!_bAddFireCount && _fireTime >= 0.1f)
            {
                _fireCount++;
                _bAddFireCount = true;
            }

            if (_weapon == "Pistol")
            {
                _animator.SetBool("bFirePistol", true);

                if (_fireTime >= 0.25f)
                    CurrentState = PlayerState.Idle;
            }
            else
            {
                _animator.SetBool("bFireRifle", true);

                if (_fireTime >= 0.4f)
                    CurrentState = PlayerState.Idle;
            }
        }
        else
        {
            _fireTime = 0;
            _animator.SetBool("bFirePistol", false);
            _animator.SetBool("bFireRifle", false);
        }
    }

    private void UpdateFireBlindlyPistol()
    {
        if (CurrentState == PlayerState.FireBlindlyPistol)
        {
            _animator.SetBool("bFireBlindlyPistol", true);
            _fireBlindlyTime += Time.deltaTime;

            if (GameInstance.instance.Item3 && !_bAddFireCount && _fireBlindlyTime >= 0.2f)
            {
                UpgradeFireBlindlyPistol();
                GameInstance.instance.PistolBullets = 0;
                _bAddFireCount = true;
                GameInstance.instance.Item3 = false;
            }
            else
            {
                if (!_bAddFireCount && _fireBlindlyTime >= 0.2f)
                {
                    _fireCount = GameInstance.instance.PistolBullets;
                    GameInstance.instance.PistolBullets = 0;
                    _bAddFireCount = true;
                }
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

    private void UpgradeFireBlindlyPistol()
    {
        if (GameManager.manager.CurrentEnemyCount == 0) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemies.Length; i++)
        {
            Debug.Log(enemies.Length);
            _enemySystem = enemies[i].GetComponent<EnemySystem>();
            _enemySystem.Hit("Pistol");
        }
    }

    private void UpdateLoad()
    {
        if (CurrentState == PlayerState.Load)
        {
            if (GameInstance.instance.PlayerWeapon == "Pistol")
            {
                _loadPistolCooltime -= Time.deltaTime;

                if (GameInstance.instance.Item1)
                {
                    _animator.SetBool("bQuickLoadPistol", true);

                    if (_loadPistolCooltime / 2 <= 0)
                    {
                        GameInstance.instance.PistolBullets = GameInstance.instance.MaxPistolBullets;
                        GameInstance.instance.Item1 = false;
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

                if (GameInstance.instance.PistolBullets == GameInstance.instance.MaxPistolBullets)
                {
                    CurrentState = PlayerState.Idle;
                }
            }
            else
            {
                _loadRifleCooltime -= Time.deltaTime;

                if (GameInstance.instance.Item1)
                {
                    _animator.SetBool("bQuickLoadRifle", true);

                    if (_loadRifleCooltime / 2 <= 0)
                    {
                        int bullet;

                        if (GameInstance.instance.ItemInventroy[3] < 2)
                            bullet = 1;
                        else if (GameInstance.instance.ItemInventroy[3] < 3)
                            bullet = 2;
                        else
                            bullet = 3;

                        GameInstance.instance.RifleBullets += bullet;
                        GameInstance.instance.ItemInventroy[3] -= bullet;

                        for (; GameInstance.instance.RifleBullets > GameInstance.instance.MaxRifleBullets;)
                        {
                            GameInstance.instance.RifleBullets--;
                            GameInstance.instance.ItemInventroy[3]++;
                        }

                        GameInstance.instance.Item1 = false;
                    }
                }
                else
                {
                    _animator.SetBool("bLoadRifle", true);

                    if (_loadRifleCooltime <= 0 && GameInstance.instance.ItemInventroy[3] > 0)
                    {
                        GameInstance.instance.RifleBullets++;
                        GameInstance.instance.ItemInventroy[3]--;
                        _loadRifleCooltime = 0.5f;
                    }
                }

                if (GameInstance.instance.RifleBullets == GameInstance.instance.MaxRifleBullets
                    || GameInstance.instance.ItemInventroy[3] < 1)
                {
                    CurrentState = PlayerState.Idle;
                }
            }
        }
        else
        {
            _loadPistolCooltime = 0.6f;
            _loadRifleCooltime = 0.6f;
            _animator.SetBool("bLoadPistol", false);
            _animator.SetBool("bQuickLoadPistol", false);
            _animator.SetBool("bQuickLoadRifle", false);
            _animator.SetBool("bLoadRifle", false);
        }
    }
}