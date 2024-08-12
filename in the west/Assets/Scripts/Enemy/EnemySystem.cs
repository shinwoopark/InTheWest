using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    public int Hp;
    public int MaxHp;
    public float DeadTime;

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

        Hp = MaxHp;

        _player_gb = GameObject.Find("Player");
    }

    private void Update()
    {
        UpdatePlayerPos();

        if (_bdead)
            UpdateDead();
    }

    private void FixedUpdate()
    {
        UpdateKnuckBack();
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
        if (Hp > 0 && _knuckBackTiem > 0)
        {
            transform.position += Vector3.right * _knuckBack * _directoin * Time.deltaTime;
            _knuckBackTiem -= Time.deltaTime;
        }
    }

    private void UpdateDead()
    {
        gameObject.layer = 6;

        _spriteRenderer.color -= new Color(0, 0, 0, DeadTime) * Time.deltaTime;
        _animator.SetBool("bDead", true);

        if (_spriteRenderer.color.a <= 0)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.manager.CurrentEnemyCount--;
    }

    public void Hit(string weapon, float direction)
    {
        if (weapon == "Pistol")
        {
            Hp--;
            _knuckBack = 5;
        }
        else
        {
            Hp -= 3;
            _knuckBack = 10;
        }

        if (direction - transform.position.x > 0)
            _directoin = -1;
        else
            _directoin = 1;

        _knuckBackTiem = 0.1f;

        if (Hp > 0)
            StartCoroutine(Blink());
        else
            _bdead = true;
    }

    private IEnumerator Blink()
    {
        _spriteRenderer.color = new Color(1, 0.75f, 0.75f, 1);

        yield return new WaitForSeconds(0.1f);

        _spriteRenderer.color = Color.white;
    }
}
