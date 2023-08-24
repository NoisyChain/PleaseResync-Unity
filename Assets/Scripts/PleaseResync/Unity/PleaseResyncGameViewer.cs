using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PleaseResync.Unity
{
    public class PleaseResyncGameViewer : MonoBehaviour
    {
        public PleaseResyncManager manager;
        public PleaseResyncPlayerViewer[] Players;

        private TesteGameState GameState;

        void Update()
        {
            if (manager.sessionState == null) return;

            if (GameState == null)
                GameState = (TesteGameState)manager.sessionState;

            for (int i = 0; i < GameState.players.Length; ++i)
                Players[i].UpdateVisuals(GameState.players[i]);
        }
    }
}
