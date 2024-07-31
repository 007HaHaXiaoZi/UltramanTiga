using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserReadyHight : MonoBehaviour
{
    /*public Transform XROrigin;
    public Transform CameraOffset;*/
    public Transform MainCamera;
    public Transform Head;

    public Transform Model;

    /*public Text text_XROrigin;
    public Text text_CameraOffset;
    public Text text_MainCamera;
    public Text text_Head;*/

    //public Text text_Scale;

    /*public Button button_add;
    public Button button_minus;*/

    /// <summary>
    /// ȷ������ı���ʾ(�ڳ������ʾ����Ҫ�û���5����վ�𲢴���ͷ���豸���Ա�Ӧ�û�ȡ���������ģ�ʹ�С)
    /// </summary>
    public Transform CertainHeightTip;
    public Button CertainHeightTipNext;
    /// <summary>
    /// ģ����߶������(position.y����ģ�����)
    /// </summary>
    public Transform HeightItem;



    private void Start()
    {
        /*button_add.onClick.AddListener(() => { Model.localScale = new Vector3(Model.localScale.x + 0.1f, Model.localScale.y + 0.1f, Model.localScale.y + 0.1f); });
        button_minus.onClick.AddListener(() => { Model.localScale = new Vector3(Model.localScale.x - 0.1f, Model.localScale.y - 0.1f, Model.localScale.y - 0.1f); });*/

        CertainHeightTip.gameObject.SetActive(true);
        CertainHeightTipNext.onClick.AddListener(CertainHeight);
    }

    void CertainHeight()
    {
        //�ڼ����ģ�ʹ�С
        //��ͷ��λ��/ģ�ͱ���߶�-Head��ֵ��
        var newScale = (MainCamera.position.y / HeightItem.position.y - Head.localPosition.y) * Model.localScale.y;
        Model.localScale = new Vector3(newScale, newScale,newScale);
        CertainHeightTip.gameObject.SetActive(false);
    }

    /*void Update()
    {
        text_XROrigin.text = "XROrigin position:" + XROrigin.position + "   localposition:" + XROrigin.localPosition;
        text_CameraOffset.text = "CameraOffset position:" + CameraOffset.position + "   localposition:" + CameraOffset.localPosition;
        text_MainCamera.text = "MainCamera position:" + MainCamera.position + "   localposition:" + MainCamera.localPosition;
        text_Head.text = "Head position:" + Head.position + "   localposition:" + Head.localPosition;
        text_Scale.text = "Model.localScale:" + Model.localScale;
    }*/
}
