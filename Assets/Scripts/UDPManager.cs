using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UdpManager : MonoBehaviour
{

    [Header("Receive")]
    public int receivePort = 5052;
    public bool startReceiving = true;
    public bool printDebug = false;
    public string data;

    [Header("Send")]
    public string targetIP = "127.0.0.1";
    public int targetPort = 5053;

    [Header("Debug")]
    public bool useMousePoint = true;

    private Thread _receiveThread;
    private UdpClient _receiveClient;
    private UdpClient _sendClient;
    private Vector2 _mousePosition;

    public const string DebugString =
        "1,1##1,1,1/2,1,2/3,1,3/4,1,4/5,1,5/6,1,6/7,1,7/8,1,8/9,1,9/10,1,10/" +
        "11,1,11/12,1,12/13,1,13/14,1,14/15,1,15/16,1,16/17,1,17/18,1,18/19,1,19/20,1,20/21,1,21";

    private const string CalibrationStartSignal = "0,0";
    private const string CalibrationEndSignal = "1,1";

    private string GetWindowSizeString()
    {
        return $"{Screen.width}, {Screen.height}";
    }

    private void StartReceive()
    {
        _receiveThread = new Thread(Receive);
        _receiveThread.IsBackground = true;
        _receiveThread.Start();
        Debug.Log("Start Receiving");
    }

    private void Receive()
    {
        _receiveClient = new UdpClient(receivePort);
        Debug.Log("Receive...");
        while (startReceiving)
        {
            if (useMousePoint)
            {
                var debugString = DebugString.Replace("1,1#", $"{_mousePosition.x},{_mousePosition.y}#");

                if (printDebug)
                {
                    Debug.Log(debugString);
                }

                FaceLandmark.DataProcessing(debugString);
                continue;
            }

            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = _receiveClient.Receive(ref anyIP);
                data = Encoding.UTF8.GetString(dataByte);

                if (printDebug)
                {
                    print(data);
                }

                FaceLandmark.DataProcessing(data);
            }
            catch (SocketException)
            {
                break;
            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }
    }

    private void StopReceive()
    {
        startReceiving = false;
        _receiveClient?.Close();
        if (_receiveThread != null && _receiveThread.IsAlive)
            _receiveThread.Join();
        Debug.Log("Stop Receiving");
    }

    private void StartSend()
    {
        StopSend();
        _sendClient = new UdpClient();
    }

    public void Send(string message)
    {
        try
        {
            var dataBytes = Encoding.UTF8.GetBytes(message);
            _sendClient.Send(dataBytes, dataBytes.Length, targetIP, targetPort);

            if (printDebug)
            {
                print($"Sent: {message} to {targetIP}:{targetPort}");
            }
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    private void StopSend()
    {
        _sendClient?.Close();
    }

    void Start()
    {
        StartSend();
        StartReceive();
        Send(CalibrationStartSignal);
    }

    void Update()
    {
        // 화면(스쿨린) 좌표
        Vector3 screenPos = Input.mousePosition;
        // 카메라 위치가 (0,0,-10) 이라면, Z=10 만큼 떨어진 평면(Z=0)에 투영
        screenPos.z = -Camera.main.transform.position.z;

        // 올바르게 월드 좌표로 변환
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        // 필요하다면 Z는 0으로 고정
        worldPos.z = 0f;
        _mousePosition = worldPos;
    }

    private void OnDisable()
    {
        StopSend();
        StopReceive();
    }
}