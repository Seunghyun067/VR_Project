using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeLimit : MonoBehaviour
{
    public TextMeshProUGUI gameTimeUI;

    public float curMin;
    private int min;
    private float sec;
    bool isDie = false;
    public  bool isStop = false;


    private void Awake()
    {
        GameManager.Instance.setTime = curMin * 60;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isStop)
        { 
            TimeLimi();
        }

        
    }
    public void TimeLimi()
    {
        if (GameManager.Instance.setTime >= 60f)               //�����ð��� 60�ʺ��� Ŭ ��
        {
            min = (int)GameManager.Instance.setTime / 60;    //���� �����ð��� 60���� ������ ���� ��
            sec = (float)(Math.Truncate((GameManager.Instance.setTime % 60) * 1) / 1);         //�ʴ� �����ð��� 60���� ������ ���� ������
            gameTimeUI.text = min + ":" + sec;
        }
        if (GameManager.Instance.setTime < 60)
        {
            gameTimeUI.text = (int)GameManager.Instance.setTime + " Sec";
        }
        if (!isDie && GameManager.Instance.setTime <= 0)
        {
            TimeOver();
            gameTimeUI.text = "0 Sec";
        }
    }
    public void TimeOver()
    {
        isDie = true;
        GameManager.Instance.IsTimeGo = false;
        //�����ϰ�
        StartCoroutine(IsDie());
    }

    
    private IEnumerator IsDie()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.Die();
    }
}
