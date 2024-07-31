using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabHello : MonoBehaviour
{
    private string _loginUserId = string.Empty;
    private string _loginTime = string.Empty;
    private string _itemInstanceId = string.Empty;

    private string _entityId = string.Empty;
    private string _entityType = string.Empty;

    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            //如果没有设置TitleId，就设置一个默认值
            PlayFabSettings.staticSettings.TitleId = "91135";
        }

        var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayFabUsage.GetTitleData();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (string.IsNullOrEmpty(_loginUserId) || string.IsNullOrEmpty(_loginTime))
            {
                Debug.LogError("请先登录");
                return;
            }

            PlayFabUsage.UpdateUserDataRequest(_loginUserId, _loginTime);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayFabUsage.GetUserDataRequest();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (string.IsNullOrEmpty(_entityId) || string.IsNullOrEmpty(_entityType))
            {
                Debug.LogError("请先登录");
                return;
            }

            PlayFabUsage.SetEntityObject(_entityId, _entityType);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (string.IsNullOrEmpty(_entityId) || string.IsNullOrEmpty(_entityType))
            {
                Debug.LogError("请先登录");
                return;
            }

            PlayFabUsage.GetEntityObject(_entityId, _entityType);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayFabUsage.PurchaseHealthPotion(itemInstanceId =>
            {
                _itemInstanceId = itemInstanceId;
            });
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayFabUsage.GetUserInventory();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (string.IsNullOrEmpty(_itemInstanceId))
            {
                Debug.LogError("请先购买物品");
                return;
            }

            PlayFabUsage.ConsumePotion(_itemInstanceId);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            PlayFabUsage.SubmitHighScore(UnityEngine.Random.Range(0, 1000));
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayFabUsage.GetLeaderboard(10);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayFabUsage.GetLeaderboardAroundPlayer(10);
        }
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log($"Hello PlayFab！ {result.PlayFabId}");

        _loginUserId = result.PlayFabId;
        _loginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        _entityId = result.EntityToken.Entity.Id;
        _entityType = result.EntityToken.Entity.Type;
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}