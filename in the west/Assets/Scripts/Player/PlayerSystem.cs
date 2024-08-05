using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerSystem : MonoBehaviour
{
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
        UpdateHp();
    }

    private void FixedUpdate()
    {
        UpdateKnuckBack();
    }

    private void UpdateHp()
    {
        if (GameInstance.instance.PlayerHp <= 0)
        {
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

    public void Hit(int damage, float KnuckBack, int direction)
    {
        GameInstance.instance.PlayerHp -= damage;
        _knuckBack = KnuckBack;
        _directoin = direction;
        _knuckBackTiem = 0.1f;
    }
}