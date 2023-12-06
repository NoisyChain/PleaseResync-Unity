using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PleaseResync.Unity;

public class InputsTestGameManager : PleaseResyncManager
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
        StartOnlineGame(new TestGameState(maxPlayers, controls), maxPlayers, ID);
    }

    public override void LocalGame(uint maxPlayers)
    {
        StartOfflineGame(new TestGameState(maxPlayers, controls), maxPlayers);
    }
}
