using System.Collections;
using System.Collections.Generic;
using PleaseResync.Unity;
using UnityEngine;
using TMPro;

public class InputsTestPlayerViewer : MonoBehaviour
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
