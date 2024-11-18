using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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
    private FlashBomb _flashBomb;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    public GameObject FlashBomb;
    public Transform ThrowPos;

    private int _playerDirection = 1;

    public float RollingSpeed;
    private float _rollingCooltime;
    private float _rollingTime;

    private string _weapon = "Pistol";
    private bool _bFire;
    private bool _bFireBlind;
    private int _fireCount;
    private bool _bAddFireCount;
    private float _fireTime;
    private float _fireBlindlyTime;

    private float _loadPistolCooltime;
    private float _loadRifleCooltime;
    private bool _bQuickReload;

    private float _quickFire;
    private bool _bHittingBody;

    public LayerMask Eneny;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!GameInstance.instance.bPlaying || GameInstance.instance.bPause) return;

        UpdateCollider();
        UpdateInput();
        UpdateFireRaycast();
        UpdateFire();
        UpdateFireBlindlyPistol();
        UpdateHittingbody();
        UpdateLoad();

        _quickFire -= Time.deltaTime;
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
            gameObject.layer = 9;
            _animator.SetBool("bRolling", true);
        }
        else
        {
            gameObject.layer = 3;
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

            UiManager.uiManager.MainUi.ChangeWeapon();
            SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Swich);
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
                SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Roll);

                //빠른 사격 타이머
                if (UpgradeManager.upgradeManager.UpgradeModule[3] > 0)
                    _quickFire = 0.6f;

                //작은 선물
                if (UpgradeManager.upgradeManager.UpgradeModule[7] > 0
                    && Random.Range(UpgradeManager.upgradeManager.UpgradeModule[7], 6) == 5)
                {
                    _flashBomb = Instantiate(FlashBomb, transform.position, Quaternion.identity).GetComponent<FlashBomb>();
                    _flashBomb.Direction = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                if (GameInstance.instance.PlayerWeapon == "Pistol"
                    && GameInstance.instance.PistolBullets > 0)
                {
                    _weapon = GameInstance.instance.PlayerWeapon;                   
                    GameInstance.instance.PistolBullets--;
                    CurrentState = PlayerState.Fire;
                    SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Pistol);
                    StartCoroutine(Fire(0.1f));
                }
                else if (GameInstance.instance.PlayerWeapon == "Rifle"
                         && GameInstance.instance.RifleBullets > 0)
                {
                    _weapon = GameInstance.instance.PlayerWeapon;
                    GameInstance.instance.RifleBullets--;
                    CurrentState = PlayerState.Fire;
                    SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Rifle);
                    StartCoroutine(Fire(0.2f));
                }
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (GameInstance.instance.PlayerWeapon == "Pistol"
                    && GameInstance.instance.PistolBullets > 0)
                {
                    StartCoroutine(FireBlindlyPistol(0.2f));
                    CurrentState = PlayerState.FireBlindlyPistol;
                    SoundManager.soundManager.PlaySfx(SoundManager.Sfx.FireBlindlyPistol);
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
           && GameInstance.instance.ItemInventroy[2] > 0)
        {
            StartCoroutine(Flashingbullets());
            GameInstance.instance.ItemInventroy[2]--;
            SoundManager.soundManager.PlaySfx(SoundManager.Sfx.FlashBomb);
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.right * _playerDirection, 20, Eneny);
        Debug.DrawRay(transform.position, Vector3.right * _playerDirection * 20, Color.blue);

        if (_bFire)
        {
            if (hit.collider != null)
            {
                int additionalDamage = 0;

                _enemySystem = hit.collider.gameObject.GetComponent<EnemySystem>();

                //마지막 한 발
                if (GameInstance.instance.PistolBullets == 0 && UpgradeManager.upgradeManager.UpgradeModule[0] > 0)
                {
                    for (int i = 0; i < UpgradeManager.upgradeManager.UpgradeModule[0]; i++)
                    {
                        additionalDamage++;
                    }                  
                }

                //헤드샷
                if (hit.collider.gameObject.tag == "Enemy" && UpgradeManager.upgradeManager.UpgradeModule[2] > 0)
                {
                    switch (UpgradeManager.upgradeManager.UpgradeModule[2])
                    {
                        case 1:
                            if (Random.Range(0, 5) == 0)
                                additionalDamage = 100;
                            break;
                        case 2:
                            if (Random.Range(0, 4) == 0)
                                additionalDamage = 100;
                            break;
                        case 3:
                            if (Random.Range(0, 3) == 0)
                                additionalDamage = 100;
                            break;
                    }
                }

                //빠른사격
                if (_quickFire > 0)
                {
                    additionalDamage = UpgradeManager.upgradeManager.UpgradeModule[3];
                    _quickFire = 0;
                }

                //광전사
                if (GameInstance.instance.PlayerHp == 1 && UpgradeManager.upgradeManager.UpgradeModule[4] > 0)
                    additionalDamage = UpgradeManager.upgradeManager.UpgradeModule[4];

                _enemySystem.Hit(_weapon, transform.position.x, additionalDamage);
                _bFire = false;
            }
            else
                _bFire = false;
        }

        if (_bFireBlind && _fireCount > 0)
        {
            if (hit.collider != null)
            {
                for (; 0 < _fireCount;)
                {
                    _enemySystem = hit.collider.gameObject.GetComponent<EnemySystem>();
                    _enemySystem.Hit(_weapon, transform.position.x,0);
                    _fireCount--;
                }
                _bFireBlind = false;
            }
            else
            {
                _fireCount = 0;
                _bFireBlind = false;           
            }
        }
    }

    private IEnumerator Fire(float delay)
    {
        yield return new WaitForSeconds(delay);
        _bFire = true;
    }

    private void UpdateFire()
    {
        if (CurrentState == PlayerState.Fire)
        {
            _fireTime += Time.deltaTime;

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

    private IEnumerator FireBlindlyPistol(float delay)
    {
        yield return new WaitForSeconds(delay);
        _bFireBlind = true;
    }

    private void UpdateFireBlindlyPistol()
    {
        if (CurrentState == PlayerState.FireBlindlyPistol)
        {
            _animator.SetBool("bFireBlindlyPistol", true);
            _fireBlindlyTime += Time.deltaTime;

            if (_bFireBlind)
            {
                _fireCount = GameInstance.instance.PistolBullets;
                GameInstance.instance.PistolBullets = 0;
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

    private IEnumerator Flashingbullets()
    {
        _animator.SetBool("bThrow", true);

        yield return new WaitForSeconds(0.2f);

        _flashBomb = Instantiate(FlashBomb, ThrowPos.position + new Vector3(0.5f * _playerDirection, 0, 0), Quaternion.identity).GetComponent<FlashBomb>();
        _flashBomb.Direction = _playerDirection;

        yield return new WaitForSeconds(0.2f);

        _animator.SetBool("bThrow", false);       
    }

    private void UpdateHittingbody()
    {
        if (UpgradeManager.upgradeManager.UpgradeModule[8] > 0 && CurrentState != PlayerState.Rolling)
        {
            if (GameInstance.instance.HittingCoolBodyTime < 5)
                GameInstance.instance.HittingCoolBodyTime += Time.deltaTime;
            else
            {
                GameInstance.instance.HittingCoolBodyTime = 5;
                _bHittingBody = true;
            }
        }       
        else
            GameInstance.instance.HittingCoolBodyTime = 0;
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

                    if (!_bQuickReload)
                    {
                        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.QuickReload);
                        _bQuickReload = true;
                    }

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
                        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Reload);
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
                        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Reload);
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
            _bQuickReload = false;
            _animator.SetBool("bLoadPistol", false);
            _animator.SetBool("bQuickLoadPistol", false);
            _animator.SetBool("bQuickLoadRifle", false);
            _animator.SetBool("bLoadRifle", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            if(collision.gameObject.name == "Left")
            {
                _spriteRenderer.flipX = false;
                _playerDirection = 1;
            }
            else if (collision.gameObject.name == "Right")
            {
                _spriteRenderer.flipX = true;
                _playerDirection = -1;
            }
        }

        if (CurrentState == PlayerState.Rolling && _bHittingBody)
        {
            if(collision.gameObject.layer == 7)
            {
                _enemySystem = collision.gameObject.GetComponent<EnemySystem>();
                _enemySystem.Hit("Body", transform.position.x, 0);             
            }
            _bHittingBody = false;
        }
    }
}