using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpUi : MonoBehaviour
{
    public RectTransform Panel;

    public float MoveSpeed;
    private float _wheelSpeed;

    private void Update()
    {
        UpdatePanelMove();
        UpdateScroll();
    }

    private void UpdatePanelMove()
    {
        Panel.transform.position += new Vector3(0, MoveSpeed * -_wheelSpeed, 0) * Time.deltaTime;

        if (Panel.anchoredPosition.y <= 0)
            Panel.anchoredPosition = Vector3.zero;
        else if (Panel.anchoredPosition.y >= 1180)
            Panel.anchoredPosition = new Vector3(0, 1180, 0);
    }

    private void UpdateScroll()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");

        if(wheelInput != 0)
            _wheelSpeed += wheelInput * Time.deltaTime;
        else
            _wheelSpeed = 0;
    }

    public void SetPosition()
    {
        Panel.transform.position = Panel.anchoredPosition = Vector3.zero;
    }
}
