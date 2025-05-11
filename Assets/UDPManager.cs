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
            SendCalibrationRequest("lt");
            current_corner = new bool[] { false, false };
        }


        if (Input.GetKeyDown(KeyCode.F2))
        {
            SendCalibrationRequest("lb");
            current_corner = new bool[] { false, true };
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SendCalibrationRequest("rt");
            current_corner = new bool[] { true, false };
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            SendCalibrationRequest("rb");
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
        // 4) JSON 파싱
        JObject msg = JObject.Parse(json);
        string type = msg["type"]?.ToString();

        switch (type)
        {
            case "start":
                // 화면 크기 정보
                JArray sz = (JArray)msg["screen_size"];
                int w = sz?[0]?.Value<int>() ?? 1920;
                int h = sz?[1]?.Value<int>() ?? 1080;
                Debug.Log($"[UDPReceive] Start: screen size = {w}×{h}");
                break;

            case "iris_position":
                // 캘리브레이션 중 홍채 좌표 스트리밍
                JArray p = (JArray)msg["position"];
                float ix = p?[0]?.Value<float>() ?? 0f;
                float iy = p?[1]?.Value<float>() ?? 0f;
                iris_position = new Vector2(ix, iy);
                Debug.Log($"[UDPReceive] Iris pos = ({ix:F1}, {iy:F1})");
                // TODO: 화면에 마커 이동
                break;

            case "captured":
                // 캘리브레이션 지점 캡처 알림
                string corner = msg["corner"]?.ToString();
                JArray ip = (JArray)msg["iris_pos"];
                Debug.Log($"[UDPReceive] Captured {corner} = ({ip[0]}, {ip[1]})");
                break;

            case "calibration_complete":
                Debug.Log("[UDPReceive] Calibration complete");
                // TODO: 트래킹 모드로 UI 전환
                break;

            case "position":
                // 트래킹 중 매핑된 화면 좌표 수신
                JArray sp = (JArray)msg["screen_pos"];
                Debug.Log($"[UDPReceive] Screen pos = ({sp[0]}, {sp[1]})");
                // TODO: 게임 오브젝트 이동
                break;

            case "pause":
                Debug.Log("[UDPReceive] Paused");
                // TODO: 게임 또는 UI 일시정지 처리
                break;

            case "resume":
                Debug.Log("[UDPReceive] Resumed");
                break;

            case "kill":
                Debug.Log("[UDPReceive] Kill signal received");
                // TODO: 애플리케이션 종료 or 씬 리셋
                break;

            default:
                Debug.Log($"[UDPReceive] Unknown type: {type}");
                break;
        }
    }

    /// <summary>
    /// 화면 크기 정보 전송
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
        Debug.Log($"[UDP] Sent → {msg}");
    }

    /// <summary>
    /// 캘리브레이션 요청 전송 (corner: "lt","lb","rt","rb")
    /// </summary>
    public void SendCalibrationRequest(string corner)
    {
        var msg = new JObject
        {
            ["type"] = "calibration_request",
            ["corner"] = corner
        };
        Send(msg);
    }

    /// <summary>
    /// 캡처 완료 신호 전송
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
    /// 일시정지 신호 전송
    /// </summary>
    public void SendPause()
    {
        var msg = new JObject { ["type"] = "pause" };
        Send(msg);
    }

    /// <summary>
    /// 재개 신호 전송
    /// </summary>
    public void SendResume()
    {
        var msg = new JObject { ["type"] = "resume" };
        Send(msg);
    }

    /// <summary>
    /// 종료 신호 전송
    /// </summary>
    public void SendKill()
    {
        var msg = new JObject { ["type"] = "kill" };
        Send(msg);
    }

    /// <summary>
    /// 내부 공통 전송 메서드
    /// </summary>
    private void Send(JObject msg)
    {
        string json = msg.ToString(Newtonsoft.Json.Formatting.None);
        byte[] data = Encoding.UTF8.GetBytes(json);
        sendClient.Send(data, data.Length, IpEndPoint);
        Debug.Log($"[UdpSender] Sent: {json}");
    }

    
    /// <summary>
    /// 소켓 닫기
    /// </summary>
    public void Close()
    {
        sendClient.Close();
    }

    void OnDestroy()
    {
        // 종료 시 스레드와 소켓 정리
        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Abort();
        sendClient.Close();
    }
}
