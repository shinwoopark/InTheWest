using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    public NormalEnemy NormalEnemy;
    public Boss1 Boss1;
    public Boss2 Boss2;

    public GameObject HeadShotImage;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    public int Hp;
    public float DeadTime;
    public bool bKunckBack;
    public int EXP;

    private float _knuckBackTiem;
    private float _knuckBack;
    private int _directoin;
    private bool _bdead;

    [HideInInspector]
    public int Player_dir;
    private GameObject _player_gb;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.manager.CurrentEnemyCount++;

        _player_gb = GameObject.Find("Player");
    }

    private void Update()
    {
        UpdateBorder();
        UpdatePlayerPos();

        if (_bdead)
            UpdateDead();
    }

    private void FixedUpdate()
    {
        UpdateKnuckBack();
    }

    private void UpdateBorder()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x < 0.0325f) pos.x = 0.0325f;
        if (pos.x > 0.9675f) pos.x = 0.9675f;

        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    private void UpdatePlayerPos()
    {
        if (transform.position.x < _player_gb.transform.position.x)
            Player_dir = 1;
        else
            Player_dir = -1;
    }

    private void UpdateKnuckBack()
    {
        if (bKunckBack && Hp > 0 && _knuckBackTiem > 0)
        {
            transform.position += Vector3.right * _knuckBack * _directoin * Time.deltaTime;
            _knuckBackTiem -= Time.deltaTime;
        }
    }

    private void UpdateDead()
    {
        gameObject.layer = 6;

        _animator.SetBool("bDead", true);

        _spriteRenderer.color -= new Color(0, 0, 0, DeadTime) * Time.deltaTime;

        if (_spriteRenderer.color.a <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        if (gameObject.tag == "BossEnemy")
        {
            GameInstance.instance.bBossSpawn = false;

            if (gameObject.name == "Boss2(Clone)")
            {
                GameManager.manager.GameClear();
                Destroy(gameObject);
                return;
            }
        }

        if (Random.Range(UpgradeManager.upgradeManager.UpgradeModule[5], 5) == 4)
        {
            int item = 0;

            if (!GameInstance.instance.bHatItem)
            {
                item = Random.Range(0, 5);

                if (item == 4)
                    GameInstance.instance.bHatItem = true;
                else
                    GameInstance.instance.ItemInventroy[item]++;
            }
            else
            {
                item = Random.Range(0, 4);

                GameInstance.instance.ItemInventroy[item]++;
            }

            SoundManager.soundManager.PlaySfx(SoundManager.Sfx.GetItem);
        }

        GameManager.manager.CurrentEnemyCount--;
        GameInstance.instance.PlayerEXP += EXP;

        Destroy(gameObject);
    }

    private void HeadShot()
    {
        Instantiate(HeadShotImage, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
    }

    private void Stern()
    {
        if (gameObject.tag == "BossEnemy")
        {
            if (gameObject.name == "Boss1(Clone)")
                Boss1.Stern();
            else if (gameObject.name == "Boss2(Clone)")
                Boss2.Stern();
        }
        else
            NormalEnemy.Stern();
    }

    public void Hit(string weapon, float direction, int additionalDamage)
    {
        if(GameInstance.instance.bPlaying && !GameInstance.instance.bPause)
        {
            int damage = 0;

            switch (weapon)
            {
                case "Pistol":
                    damage = 1;
                    _knuckBack = 5;
                    break;
                case "Rifle":
                    damage = 3;
                    _knuckBack = 10;
                    break;
                case "FlashBomb":
                    damage = UpgradeManager.upgradeManager.UpgradeModule[1];
                    _knuckBack = UpgradeManager.upgradeManager.UpgradeModule[1] * 3;
                    Stern();
                    break;
                case "Body":
                    _knuckBack = UpgradeManager.upgradeManager.UpgradeModule[8] * 15;
                    Stern();
                    break;
            }

            Hp -= damage + additionalDamage;

            if (direction - transform.position.x > 0)
                _directoin = -1;
            else
                _directoin = 1;

            _knuckBackTiem = 0.1f;

            if (Hp > 0)
                StartCoroutine(Blink());
            else
            {
                if (additionalDamage >= 100)
                    HeadShot();

                _bdead = true;
            }            
        }      
    }

    private IEnumerator Blink()
    {
        _spriteRenderer.color = new Color(1, 0.75f, 0.75f, 1);

        yield return new WaitForSeconds(0.1f);

        _spriteRenderer.color = Color.white;
    }
}
