using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using UnityEngine;

public class PlayFabUsage
{
    #region 使用游戏数据（TitleData）

    public static void GetTitleData()
    {
        var request = new PlayFab.ClientModels.GetTitleDataRequest();
        PlayFab.PlayFabClientAPI.GetTitleData(request, result =>
        {
            foreach (var dataPair in result.Data)
            {
                Debug.Log($"GetTitleData == {dataPair.Key} == {dataPair.Value}");
            }
        },
            error =>
            {
                Debug.LogError($"GetTitleData == {error.GenerateErrorReport()}");
            }
        );
    }

    #endregion

    #region 使用用户数据（UserData）

    public static void UpdateUserDataRequest(string userId, string loginTime)
    {
        var updateUserDataRequest = new PlayFab.ClientModels.UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"UserId", userId},
                {"LoginTime", loginTime}
            }
        };

        PlayFab.PlayFabClientAPI.UpdateUserData(updateUserDataRequest, result =>
        {
            Debug.Log($"UpdateUserDataRequest == Success {result.DataVersion}");
        },
            error =>
            {
                Debug.LogError($"UpdateUserDataRequest == {error.GenerateErrorReport()}");
            }
        );
    }

    public static void GetUserDataRequest()
    {
        var request = new PlayFab.ClientModels.GetUserDataRequest();

        PlayFab.PlayFabClientAPI.GetUserData(request, result =>
        {
            foreach (var dataPair in result.Data)
            {
                Debug.Log($"GetUserDataRequest == {dataPair.Key} == {dataPair.Value.Value}");
            }
        },
            error =>
            {
                Debug.LogError($"GetUserDataRequest == {error.GenerateErrorReport()}");
            }
        );
    }

    #endregion

    #region 使用实体数据（EntityData）

    public static void SetEntityObject(string entityKey, string entityType)
    {
        var debugData = new Dictionary<string, object>()
        {
            {"Atk", 100},
            {"Hp", 1000},
        };
        var dataList = new List<SetObject>()
        {
            new SetObject()
            {
                ObjectName = "MyPlayerData",
                DataObject = debugData,
            },
        };

        var newSetObjectRequest = new SetObjectsRequest()
        {
            Entity = new PlayFab.DataModels.EntityKey() { Id = entityKey, Type = entityType },
            Objects = dataList,
        };
        PlayFabDataAPI.SetObjects(newSetObjectRequest, setResult =>
        {
            Debug.Log($"SetEntityObject == Success with version {setResult.ProfileVersion}");
        },
            error =>
            {
                Debug.LogError(error.ErrorMessage);
            });
    }

    public static void GetEntityObject(string entityKey, string entityType)
    {
        var newGetObjectRequest = new GetObjectsRequest()
        {
            Entity = new PlayFab.DataModels.EntityKey() { Id = entityKey, Type = entityType },
        };
        PlayFabDataAPI.GetObjects(newGetObjectRequest, getResult =>
        {
            Debug.Log($"GetEntityObject == Success with version {getResult.ProfileVersion}");
            foreach (var dataPair in getResult.Objects)
            {
                Debug.Log($"GetEntityObject == {dataPair.Key} == {dataPair.Value.DataObject}");
            }
        },
            error =>
            {
                Debug.LogError(error.ErrorMessage);
            });
    }

    #endregion

    #region 购买物品

    public static void PurchaseHealthPotion(Action<string> onPurchaseFinish)
    {
        var purchaseItemRequest = new PlayFab.ClientModels.PurchaseItemRequest
        {
            CatalogVersion = "Items",
            ItemId = "HealthPotion",
            Price = 10,
            VirtualCurrency = "CN"
        };

        PlayFab.PlayFabClientAPI.PurchaseItem(purchaseItemRequest, result =>
        {
            Debug.Log($"PurchaseHealthPotion == {result.Request.GetType().Name} Success {result.Items[0].ItemId}");
            onPurchaseFinish?.Invoke(result.Items[0].ItemInstanceId);
        },
            error =>
            {
                Debug.LogError($"PurchaseHealthPotion == {error.GenerateErrorReport()}");
                onPurchaseFinish?.Invoke(string.Empty);
            }
        );
    }

    public static void GetUserInventory()
    {
        var request = new PlayFab.ClientModels.GetUserInventoryRequest();

        PlayFab.PlayFabClientAPI.GetUserInventory(request, result =>
        {
            foreach (var item in result.Inventory)
            {
                Debug.Log($"GetUserInventory == {item.ItemId} == {item.DisplayName} : {item.ItemInstanceId} count: {(item.RemainingUses.HasValue ? item.RemainingUses.Value : 0)}");
            }
        },
            error =>
            {
                Debug.LogError($"GetUserInventory == {error.GenerateErrorReport()}");
            }
        );
    }

    public static void ConsumePotion(string itemInstanceId)
    {
        var consumeItemRequest = new PlayFab.ClientModels.ConsumeItemRequest
        {
            ItemInstanceId = itemInstanceId,
            ConsumeCount = 1
        };

        PlayFab.PlayFabClientAPI.ConsumeItem(consumeItemRequest, result =>
        {
            Debug.Log($"ConsumePotion == {result.Request.GetType().Name}-{itemInstanceId} Success");
        },
            error =>
            {
                Debug.LogError($"ConsumePotion == {error.GenerateErrorReport()}");
            }
        );
    }

    #endregion

    #region 使用排行榜

    public static void SubmitHighScore(int highScore)
    {
        var request = new PlayFab.ClientModels.UpdatePlayerStatisticsRequest
        {
            Statistics = new List<PlayFab.ClientModels.StatisticUpdate>
            {
                new PlayFab.ClientModels.StatisticUpdate
                {
                    StatisticName = "Daily High Score",
                    Value = highScore
                }
            }
        };

        PlayFab.PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
        {
            Debug.Log($"SubmitHighScore == {result.Request.GetType().Name} Success");
        },
            error =>
            {
                Debug.LogError($"SubmitHighScore == {error.GenerateErrorReport()}");
            }
        );
    }

    public static void GetLeaderboard(int highScoreCount)
    {
        var request = new PlayFab.ClientModels.GetLeaderboardRequest
        {
            StatisticName = "Daily High Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFab.PlayFabClientAPI.GetLeaderboard(request, result =>
        {
            Debug.Log($"GetLeaderboard == {result.Request.GetType().Name} Success");

            foreach (var player in result.Leaderboard)
            {
                Debug.Log($"GetLeaderboard == {player.Position} == {player.PlayFabId} == {player.StatValue}");
            }
        },
            error =>
            {
                Debug.LogError($"GetLeaderboard == {error.GenerateErrorReport()}");
            }
        );
    }

    public static void GetLeaderboardAroundPlayer(int highScoreCount)
    {
        var request = new PlayFab.ClientModels.GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "Daily High Score",
            MaxResultsCount = highScoreCount
        };

        PlayFab.PlayFabClientAPI.GetLeaderboardAroundPlayer(request, result =>
        {
            Debug.Log($"GetLeaderboardAroundPlayer == {result.Request.GetType().Name} Success");

            foreach (var player in result.Leaderboard)
            {
                Debug.Log($"GetLeaderboardAroundPlayer == {player.Position} == {player.PlayFabId} == {player.StatValue}");
            }
        },
            error =>
            {
                Debug.LogError($"GetLeaderboardAroundPlayer == {error.GenerateErrorReport()}");
            }
        );
    }

    #endregion
}