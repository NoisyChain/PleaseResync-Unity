using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PleaseResync;

public class VectorWarGameManager : PleaseResyncManager
{
    public override void OnlineGame(uint maxPlayers, uint ID)
    {
        StartOnlineGame(new VectorWar((int)maxPlayers, controls), maxPlayers, ID);
    }

    public override void LocalGame(uint maxPlayers)
    {
        StartOfflineGame(new VectorWar((int)maxPlayers, controls), maxPlayers);
    }

    public override void ReplayMode(uint maxPlayers)
    {
        StartReplay(new VectorWar((int)maxPlayers, controls), maxPlayers);
    }
}
