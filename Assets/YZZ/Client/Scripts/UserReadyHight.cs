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
    /// 确定身高文本提示(在出这个提示后，需要用户在5秒内站起并戴好头显设备，以便应用获取身高来调整模型大小)
    /// </summary>
    public Transform CertainHeightTip;
    public Button CertainHeightTipNext;
    /// <summary>
    /// 模型身高顶点描点(position.y就是模型身高)
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
        //期间更改模型大小
        //（头显位置/模型本身高度-Head差值）
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
