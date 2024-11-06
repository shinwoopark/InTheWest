using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashBomb : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;

    public GameObject Explosion;

    public float MoveSpeed;
    public float RotationSpeed;

    public float BombTimer;

    private float UpgradeSpeed;

    [HideInInspector]
    public float Direction;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (UpgradeManager.upgradeManager.UpgradeModule[6] > 0)
        {
            for (int i = 0; i < UpgradeManager.upgradeManager.UpgradeModule[6]; i++)
            {
                _rigidbody2D.gravityScale -= 0.2f;
                UpgradeSpeed++;
            }
        }
    }

    private void Update()
    {
        UpdateMove();
    }

    private void UpdateBomb()
    {
        BombTimer -= Time.deltaTime;

        if (BombTimer <= 0)
            DestroyAndExplosion();
    }

    private void UpdateMove()
    {
        float moveSpeed = MoveSpeed + UpgradeSpeed;

        transform.position += new Vector3(moveSpeed * Direction, 0, 0) * Time.deltaTime;

        transform.eulerAngles += new Vector3(0, 0, RotationSpeed) * Time.deltaTime;
    }

    private void DestroyAndExplosion()
    {
        Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            MoveSpeed = 0;
            UpgradeSpeed = 0;
            RotationSpeed = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            DestroyAndExplosion();
        }
    }
}
