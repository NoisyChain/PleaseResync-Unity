using UnityEngine;
using TMPro;

public class ShowFPS : MonoBehaviour 
{
    [SerializeField] private TextMeshProUGUI fpsText;
    private float deltaTime;

    void Update () 
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = "" + Mathf.Ceil (fps).ToString () + " FPS";
    }
}