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

    void ResetView()
    {
        if (Players != null)
            foreach (InputsTestPlayerViewer p in Players)
                Destroy(p.gameObject);

        Players = new InputsTestPlayerViewer[GameState.players.Length];
        for (int i = 0; i < Players.Length; ++i)
        {
            Players[i] = Instantiate(PlayerPrefab, transform);
            Players[i].ID = ID;
        }
    }

    void UpdateView()
    {
        for (int i = 0; i < GameState.players.Length; ++i)
        {
            if (!Players[i].gameObject.activeSelf)
                Players[i].gameObject.SetActive(true);

            Players[i].UpdateVisuals(GameState.players[i]);
        }

        if (Players.Length > GameState.players.Length)
        {
            for (int i = GameState.players.Length; i < Players.Length; i++)
            {
                Players[i].gameObject.SetActive(false);
            }
        }
    }

    void ManageView()
    {
        if (Players == null || Players.Length != GameState.players.Length)
            ResetView();
        else
            UpdateView();
    }

    void Update()
    {
        GameState = (TestGameState)manager.sessionState;

        if (GameState != null)
            ManageView();
    }
}
