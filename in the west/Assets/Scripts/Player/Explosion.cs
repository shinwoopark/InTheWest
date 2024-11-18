using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private EnemySystem _enemySystem;

    private SpriteRenderer _spriteRenderer;

    public float LifeTime;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SoundManager.soundManager.PlaySfx(SoundManager.Sfx.Explosion);

        if (UpgradeManager.upgradeManager.UpgradeModule[1] > 0)
            _spriteRenderer.color = new Color(0.7f, 0.2f, 0.2f);
    }

    private void Update()
    {
        LifeTime -= Time.deltaTime;

        if (LifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 7)
        {
            _enemySystem = collision.gameObject.GetComponent<EnemySystem>();
            _enemySystem.Hit("FlashBomb", transform.position.x, 0);
        }
    }
}
