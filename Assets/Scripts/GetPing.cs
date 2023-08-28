using UnityEngine;
using TMPro;
using System.Collections;

public class GetPing : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pingText;
    [SerializeField] private string pingIP = "127.0.0.1";

    void Update()
    {
        StartCoroutine(Ping());
    }

    IEnumerator Ping()
    {
        Ping GetPing = new Ping(pingIP);
        while (!GetPing.isDone) yield return null;
        if (pingText != null) pingText.text = "Ping: " + GetPing.time.ToString() + " ms";
    }
}
