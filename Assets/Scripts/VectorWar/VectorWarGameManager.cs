using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PleaseResync.Unity;

public class VectorWarGameManager : PleaseResyncManager
{
    public override void OnlineGame(uint maxPlayers, uint ID)
    {
        StartOnlineGame(new VectorWar((int)maxPlayers), maxPlayers, ID);
    }

    public override void LocalGame(uint maxPlayers)
    {
        StartOfflineGame(new VectorWar((int)maxPlayers), maxPlayers);
    }
}
