using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadShot : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _spriteRenderer.color -= new Color(0, 0, 0, 1) * Time.deltaTime;

        if(_spriteRenderer.color.a <= 0)
            Destroy(gameObject);
    }
}
