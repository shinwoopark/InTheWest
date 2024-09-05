using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private PlayerSystem _playerSystem;
    private GameObject _player;

    public float MoveSpeed;

    public int Damage;
    public float KnuckBack;

    public bool bHitDestroy;

    private bool _bHit;

    [HideInInspector]
    public Vector3 Dir;

    private void Awake()
    {
        _player = GameObject.Find("Player");
        _playerSystem = _player.GetComponent<PlayerSystem>();
    }

    private void FixedUpdate()
    {
        UpdateMove();
    }

    public void SetDir()
    {
        float rotZ = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    private void UpdateMove()
    {
        transform.position += Dir * MoveSpeed * Time.deltaTime;
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
