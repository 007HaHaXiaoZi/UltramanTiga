using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YZZ;
using RootMotion.FinalIK;

public class ClientPrefab : MonoBehaviour
{
    public Transform Head;//ͷ���ڵ�
    public Transform LeftHand;//���ֽڵ�
    public Transform RightHand;//���ֽڵ�

    //������ɫ���ƶ�
    void LateUpdate()
    {

    }

    /// <summary>
    /// ��Ϊһ��Ԥ������ƶ�����������ת���������ƶ�
    /// </summary>
    void PrefabMove(Transform target,Vector3 pos,Vector3 angle,float time)
    {
        target.transform.DORotate(angle, time);
        target.transform.DOMove(pos, time).SetEase(Ease.Linear);
    }

}
