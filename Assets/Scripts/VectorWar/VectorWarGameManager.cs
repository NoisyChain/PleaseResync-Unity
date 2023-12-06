using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PleaseResync.Unity;

public class VectorWarGameManager : PleaseResyncManager
{
    protected PlayerInputs controls;

    void Awake()
    {
        controls = new PlayerInputs();
    }
    public void OnEnable()
    {
        controls.Enable();
    }
    public new void OnDisable()
    {
        CloseGame();
        controls.Disable();
    }

    public override void OnlineGame(uint maxPlayers, uint ID)
    {
        StartOnlineGame(new VectorWar((int)maxPlayers, controls), maxPlayers, ID);
    }

    public override void LocalGame(uint maxPlayers)
    {
        StartOfflineGame(new VectorWar((int)maxPlayers, controls), maxPlayers);
    }
}
