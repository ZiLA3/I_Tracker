using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading;
using Newtonsoft.Json;

public class UDPManager : MonoBehaviour
{
    [SerializeField] int listenPort = 5000, sendPort = 5001;
    UdpClient recvClient;
    UdpClient sendClient;
    IPEndPoint IpEndPoint;
    Thread receiveThread;
    Vector2 iris_position;

    bool[] current_corner = { false, false }; // 0: x, 1: y

    void Start()
    {
        recvClient = new UdpClient(listenPort);
        sendClient = new UdpClient();
        IpEndPoint = new IPEndPoint(IPAddress.Loopback, sendPort);

        receiveThread = new Thread(ReceiveLoop);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log($"[UDP] Listening on {listenPort}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SendScreenSize(1920, 1080);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (current_corner[0])
            {
                if (current_corner[1])
                {
                    SendCaptured("lt", iris_position);
                }
                else
                {
                    SendCaptured("lb", iris_position);
                }
            }
            else 
            {
                if (current_corner[1])
                { 
                    SendCaptured("rt", iris_position);
                }
                else
                {
                    SendCaptured("rb", iris_position);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            current_corner = new bool[] { false, false };
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            current_corner = new bool[] { false, true };
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            current_corner = new bool[] { true, false };
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            current_corner = new bool[] { true, true };
        }
    }

    void ReceiveLoop()
    {
        var remoteEP = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            try
            {
                byte[] data = recvClient.Receive(ref remoteEP);
                ProcessMessage(Encoding.UTF8.GetString(data));
            }
            catch { break; }
        }
    }

    void ProcessMessage(string json)
    {
        // 4) JSON �Ľ�
        JObject msg = JObject.Parse(json);
        string type = msg["type"]?.ToString();

        switch (type)
        {
            case "iris_position":
                // Ķ���극�̼� �� ȫä ��ǥ ��Ʈ����
                JArray p = (JArray)msg["position"];
                float ix = p?[0]?.Value<float>() ?? 0f;
                float iy = p?[1]?.Value<float>() ?? 0f;
                iris_position = new Vector2(ix, iy);
                Debug.Log($"Receive : Iris pos = ({ix:F1}, {iy:F1})");
                // TODO: ȭ�鿡 ��Ŀ �̵�
                break;
            case "start":
                // Ķ���극�̼� ����
                Debug.Log("Receive : ����");
                break;
            case "calibration_complete":
                // Ķ���극�̼� �Ϸ�
                Debug.Log("Receive : �Ϸ�");
                // TODO: ���� ����
                break;
            default:
                Debug.Log("Receive : ��Ȯ�� �޽���");
                break;
        }
    }

    /// <summary>
    /// ȭ�� ũ�� ���� ����
    /// </summary>
    void SendScreenSize(int w, int h)
    {
        var msg = new JObject
        {
            ["type"] = "screen_size",
            ["width"] = w,
            ["height"] = h
        };

        byte[] data = Encoding.UTF8.GetBytes(msg.ToString(Formatting.None));
        sendClient.Send(data, data.Length, IpEndPoint);
        Debug.Log("Send : Screen Size");
    }

    /// <summary>
    /// ĸó �Ϸ� ��ȣ ����
    /// </summary>
    public void SendCaptured(string corner, Vector2 irisPos)
    {
        var msg = new JObject
        {
            ["type"] = "captured",
            ["corner"] = corner,
            ["iris_pos"] = new JArray(irisPos.x, irisPos.y)
        };
        Send(msg);
    }

    /// <summary>
    /// �Ͻ����� ��ȣ ����
    /// </summary>
    public void SendPause()
    {
        var msg = new JObject { ["type"] = "pause" };
        Send(msg);
    }

    /// <summary>
    /// �簳 ��ȣ ����
    /// </summary>
    public void SendResume()
    {
        var msg = new JObject { ["type"] = "resume" };
        Send(msg);
    }

    /// <summary>
    /// ���� ��ȣ ����
    /// </summary>
    public void SendKill()
    {
        var msg = new JObject { ["type"] = "kill" };
        Send(msg);
    }

    /// <summary>
    /// ���� ���� ���� �޼���
    /// </summary>
    private void Send(JObject msg)
    {
        string json = msg.ToString(Newtonsoft.Json.Formatting.None);
        byte[] data = Encoding.UTF8.GetBytes(json);
        sendClient.Send(data, data.Length, IpEndPoint);
        Debug.Log($"[UdpSender] Sent: {json}");
    }

    /// <summary>
    /// ���� �ݱ�
    /// </summary>
    public void Close()
    {   
        sendClient.Close();
    }

    void OnDestroy()
    {
        // ���� �� ������� ���� ����
        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Abort();

        sendClient.Close();
    }
}
