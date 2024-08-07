using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class NormalEnemy1 : MonoBehaviour
{
    private EnemySystem _enemySystem;

    public float MoveSpeed;

    private int _dir;

    private void Awake()
    {
        _enemySystem = GetComponent<EnemySystem>();
    }

    private void Update()
    {
        UpdateRaycaset();
    }

    private void FixedUpdate()
    {
        UpdateMove();
    }

    private void UpdateMove()
    {
        transform.position += new Vector3(_enemySystem.Player_dir, 0, 0) * MoveSpeed * Time.deltaTime;


    }

    private void UpdateRaycaset()
    {
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.right * _enemySystem.Player_dir, 20, Eneny);
        //Debug.DrawRay(transform.position, Vector3.right * _enemySystem.Player_dir * 20, Color.blue);
    }
}
