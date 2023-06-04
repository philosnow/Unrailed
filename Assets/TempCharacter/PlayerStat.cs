using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �̰� ��ũ���ͺ� ������Ʈ�� ¥�°� ������
public class PlayerStat : MonoBehaviour
{
    [Header("�̵�")]
    public float moveSpeed;
    public float dashSpeed;
    public float dashDuration;
    public float dashCoolTime;

    [Header("������ ��ȣ�ۿ�")]
    public int handAmount;
    public int stackAmount;
    public float stackinterval;

    [Header("���ҽ� ��ȣ�ۿ�")]
    public float interactiveCoolTime;

    [Header("���� �Ÿ�")]
    public float detectRange;

    [Header("���̾�")]
    public LayerMask blockLayer;
    public LayerMask waterLayer;
    public LayerMask attackableLayer;
    public LayerMask diggableLayer;
    public LayerMask detectableLayer;
}