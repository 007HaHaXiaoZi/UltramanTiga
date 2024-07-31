using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform ainTransform;

    public Transform item;
    void Start()
    {
        ainTransform.DORotate(item.eulerAngles, 5.0f);
        ainTransform.DOMove(item.position, 5.0f).SetEase(Ease.Linear);
    }

    void Update()
    {
        
    }
}
