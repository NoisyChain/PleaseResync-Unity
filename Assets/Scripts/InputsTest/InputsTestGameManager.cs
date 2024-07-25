using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PleaseResync;

public class InputsTestGameManager : PleaseResyncManager
{
    public override void OnlineGame(uint maxPlayers, uint ID)
    {
        StartOnlineGame(new TestGameState(maxPlayers, controls), maxPlayers, ID);
    }

    public override void LocalGame(uint maxPlayers)
    {
        StartOfflineGame(new TestGameState(maxPlayers, controls), maxPlayers);
    }

    public override void ReplayMode(uint maxPlayers)
    {
        StartReplay(new TestGameState(maxPlayers, controls), maxPlayers);
    }
}
