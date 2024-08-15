using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private PlayerSystem _playerSystem;
    private GameObject _player;

    public int Damage;
    public float KnuckBack;

    public bool bHitDestroy;

    private bool _bHit;

    private void Awake()
    {
        _player = GameObject.Find("Player");
        _playerSystem = _player.GetComponent<PlayerSystem>();
    }

    private void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 && !_bHit)
        {
            _playerSystem.Hit(Damage, KnuckBack, transform.position.x);
            _bHit = true;

            if (bHitDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
