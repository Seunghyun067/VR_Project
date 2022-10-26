using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeLimit : MonoBehaviour
{
    public TextMeshProUGUI gameTimeUI;

    public float curMin;
    private float setTime;
    private int min;
    private float sec;


    private void Awake()
    {
        setTime = curMin * 60;
    }

    // Update is called once per frame
    void Update()
    {
        setTime -= Time.deltaTime;

        if (setTime >= 60f)               //�����ð��� 60�ʺ��� Ŭ ��
        {
            min = (int)setTime / 60;    //���� �����ð��� 60���� ������ ���� ��
            sec = (float)(Math.Truncate((setTime % 60) * 1) / 1);         //�ʴ� �����ð��� 60���� ������ ���� ������
            gameTimeUI.text =  min + ":"  + sec;
        }
        if (setTime < 60)
        {
            gameTimeUI.text = setTime + "��";
        }
        if (setTime <= 0)
        {
            gameTimeUI.text = "0��";
        }
    }
}
