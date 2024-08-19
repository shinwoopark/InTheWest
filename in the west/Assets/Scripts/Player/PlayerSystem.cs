using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerSystem : MonoBehaviour
{
    public UIManager UIManager;

    private Animator _animator;

    private float _knuckBackTiem;
    private float _knuckBack;
    private int _directoin;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateBorder();
        UpdateHp();
    }

    private void FixedUpdate()
    {
        if (!GameInstance.instance.bPlaying) return;

        UpdateKnuckBack();
    }

    private void UpdateBorder()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x < 0.0325f) pos.x = 0.0325f;
        if (pos.x > 0.9675f) pos.x = 0.9675f;

        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    private void UpdateHp()
    {
        if (GameInstance.instance.PlayerHp <= 0)
        {
            GameInstance.instance.PlayerHp = 0;
            GameManager.manager.GameOver();
            _animator.SetBool("bTakeDown", true);
        }         
    }

    private void UpdateKnuckBack()
    {
        if (GameInstance.instance.PlayerHp > 0 && _knuckBackTiem > 0)
        {
            transform.position += Vector3.right * _knuckBack * _directoin * Time.deltaTime;
            _knuckBackTiem -= Time.deltaTime;
        }
    }

    public void Hit(int damage, float KnuckBack, float direction)
    {
        if (GameInstance.instance.PlayerHp <= 0)
            return;

        if (GameInstance.instance.Item4)
        {
            GameInstance.instance.Item4 = false;
            return;
        }

        GameInstance.instance.PlayerHp -= damage;
        _knuckBack = KnuckBack;

        if (direction - transform.position.x > 0)
            _directoin = -1;
        else
            _directoin = 1;

        _knuckBackTiem = 0.1f;

        UIManager.ChangePlayerHp();

        if (GameInstance.instance.PlayerHp <= 0)
        {
            GameManager.manager.GameOver();
            _animator.SetBool("bTakeDown", true);
        }
    }
}