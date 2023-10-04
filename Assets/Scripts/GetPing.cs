using System.Net.NetworkInformation;
using UnityEngine;
using TMPro;
using System.Threading;

public class GetPing : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pingText;
    [SerializeField] private string pingIP = "127.0.0.1";

    System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
    PingReply r;
    int pingWindow;

    Thread PingThread;

    void Start()
    {
        PingThread = new Thread(() => Ping(pingIP));
        PingThread.IsBackground = true;
        PingThread.Start();
    }

    void Update()
    {
        if (r == null) return;
        if (r.Status == IPStatus.Success)
            pingText.text = $"Ping: {r.RoundtripTime} ms";
    }

    void Ping(string PingIP)
    {
        while(true)
            r = p.Send(PingIP);
    }
}
