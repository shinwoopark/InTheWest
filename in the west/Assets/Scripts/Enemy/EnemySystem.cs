using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    public int Hp;
    public int MaxHp;

    [HideInInspector]
    public int Player_dir;
    private GameObject _player_gb;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameManager.manager.CurrentEnemyCount++;

        Hp = MaxHp;

        _player_gb = GameObject.Find("Player");
    }

    private void Update()
    {
        UpdateHp();
        UpdatePlayerPos();
    }

    private void UpdateHp()
    {
        if (Hp <= 0)
            Dead();

        if (Hp > MaxHp)
            Hp = MaxHp;
    }

    private void UpdatePlayerPos()
    {
        if (transform.position.x < _player_gb.transform.position.x)
            Player_dir = 1;
        else
            Player_dir = -1;
    }

    private void Dead()
    {
        gameObject.layer = 0;

        _spriteRenderer.color -= new Color(0, 0, 0, 1f) * Time.deltaTime;

        if (_spriteRenderer.color.a <= 0)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.manager.CurrentEnemyCount--;
    }

    public void Hit(string weapon)
    {
        if (weapon == "Pistol")
        {
            Hp--;
        }
        else
        {
            Hp -= 3;
        }
    }
}
