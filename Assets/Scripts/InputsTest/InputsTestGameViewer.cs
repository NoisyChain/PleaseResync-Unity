using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PleaseResync.Unity;

public class InputsTestGameViewer : MonoBehaviour
{
    public int ID;
    public PleaseResyncManager manager;
    public InputsTestPlayerViewer PlayerPrefab;

    private InputsTestPlayerViewer[] Players;
    private TestGameState GameState;

    void Update()
    {
        if (manager.sessionState == null) return;

        if (GameState == null)
            GameState = (TestGameState)manager.sessionState;

        if (Players == null)
        {
            Players = new InputsTestPlayerViewer[GameState.players.Length];
            for (int i = 0; i < GameState.players.Length; ++i)
            {
                Players[i] = Instantiate(PlayerPrefab, transform);
                Players[i].ID = ID;
            }
        }

        for (int i = 0; i < GameState.players.Length; ++i)
            Players[i].UpdateVisuals(GameState.players[i]);
    }
}
