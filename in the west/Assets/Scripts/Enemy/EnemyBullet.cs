using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private PlayerSystem _playerSystem;
    private GameObject _player;

    public int Damage;
    public float KnuckBack;
    public int Direction;

    public bool bHitDestroy;

    private bool _bHit;

    private void Awake()
    {
        _player = GameObject.Find("Player");
        _playerSystem = _player.GetComponent<PlayerSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 && !_bHit)
        {
            if (Direction == 0)
            {
                if (transform.position.x - collision.transform.position.x < 0)
                    Direction = 1;
                else
                    Direction = -1;
            }

            _playerSystem.Hit(Damage, KnuckBack, Direction);
            _bHit = true;

            if (bHitDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
