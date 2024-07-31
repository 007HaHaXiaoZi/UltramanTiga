using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using YZZ;

public class SelectCharacter : MonoBehaviour
{
    //ÓÃ»§°´Å¥
    public Button Character1;
    public Button Character2;

    void Start()
    {
        Character1.onClick.AddListener(() => { ClientDatas.SelectedCharacter = "Girl";SceneManager.LoadScene("ServerRoom"); });
        Character2.onClick.AddListener(() => { ClientDatas.SelectedCharacter = "RedGirl"; SceneManager.LoadScene("ServerRoom"); });
    }
}
