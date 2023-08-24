using System.Collections;
using System.Collections.Generic;
using PleaseResync;
using UnityEngine;
using TMPro;

namespace PleaseResync.Unity
{
    public class PleaseResyncPlayerViewer : MonoBehaviour
    {
        public int ID;
        public MeshRenderer mesh;
        public TextMeshPro positionViewer;

        public void UpdateVisuals(TestPlayer player)
        {
            Color finalColor = ID == 0 ? Color.red : Color.blue;
            mesh.material.color = finalColor;
            transform.position = (Vector2)player.position;
            positionViewer.text = player.position.ToString();
        }
    }
}
