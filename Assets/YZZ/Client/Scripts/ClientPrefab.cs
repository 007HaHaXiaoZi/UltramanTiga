using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YZZ;
using RootMotion.FinalIK;

public class ClientPrefab : MonoBehaviour
{
    public Transform Head;//头部节点
    public Transform LeftHand;//左手节点
    public Transform RightHand;//右手节点

    //其他角色自移动
    void LateUpdate()
    {

    }

    /// <summary>
    /// 作为一个预制体的移动方法，来先转动方向再移动
    /// </summary>
    void PrefabMove(Transform target,Vector3 pos,Vector3 angle,float time)
    {
        target.transform.DORotate(angle, time);
        target.transform.DOMove(pos, time).SetEase(Ease.Linear);
    }

}
