using Photon.Deterministic;
using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoadPlayerSpawnPosition : QuantumEntityViewComponent
{
    bool loadAllIPlayerSpawnPosition;

    private void Start()
    {
        loadAllIPlayerSpawnPosition = false;
    }

    private void Update()
    {
        if (_entityView == null || loadAllIPlayerSpawnPosition)
        {
            return;
        }

        var playerSpanwer = VerifiedFrame.GetSingleton<PlayerSpawnPosition>();
        var playerSpawnPositions = VerifiedFrame.FindAsset(playerSpanwer.PlayerSpawnPositions);

        playerSpawnPositions.waitingPositions = new();
        playerSpawnPositions.inGamePositions = new();

        LoadPositionFromTransform("Waiting", playerSpanwer.WaitingPosition.Length, playerSpawnPositions.waitingPositions);
        LoadPositionFromTransform("InGame", playerSpanwer.InGamePosition.Length, playerSpawnPositions.inGamePositions);

        loadAllIPlayerSpawnPosition = true;
    }

    private void LoadPositionFromTransform(string namePath, int length, List<FPVector2> assetPositions)
    {
        for (var i = 0; i < length; i++)
        {
            assetPositions.Add(transform.Find($"{namePath}/{namePath}Position - ({i})").position.ToFPVector2());
        }
    }
}
