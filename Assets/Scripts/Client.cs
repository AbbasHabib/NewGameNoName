using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Unity;
using System.Collections;
using System.Collections.Generic;

class Client : MonoBehaviour
{
    private static Socket _clientSocket;
    private byte[] _receieve_buffer;
    private static int _CLIENT_ID;
    private Stack<String> _server_data;

    public GameObject summoned_player;
    public GameObject Gameplayer;

    private Dictionary<int, GameObject> _online_Players;

    private Client _Instance;

    public static int _CLIENT_ID_ { get => _CLIENT_ID; }
    public static Socket _CLIENT_SOCKET_ { get => _clientSocket; }
    bool _CONNECTED;
    String _server_data1 = "";
    private void Awake()
    {
        if (_Instance != null)
            Destroy(this);
        _Instance = this;
        _clientSocket = new Socket
        (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _CONNECTED = false;
        _receieve_buffer = new byte[1024];
        _server_data = new Stack<String>();
        _online_Players = new Dictionary<int, GameObject>();
        _CLIENT_ID = -1;
    }

    Thread ConnectSocketThread;
    private void Start()
    {
        ThreadStart ts = new ThreadStart(Connect);
        ConnectSocketThread = new Thread(ts);

        ConnectSocketThread.Start();
    }
    public void SendInputs(string mov)
    {
        if (_CONNECTED && _CLIENT_ID != -1)
        {
            try
            {
                byte[] data;
                string text = ",[client]," + _CLIENT_ID + ',' + Gameplayer.transform.position.x + '|' + Gameplayer.transform.position.y + ',';
                text += mov + ',';
                data = Encoding.ASCII.GetBytes(text);
                if (data.Length > 1)
                    _clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallBack), _clientSocket);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

    }
    private void ResendId()
    {
        byte[] data;
        string text = "{ME}";
        data = Encoding.ASCII.GetBytes(text);
        if (data.Length > 1)
            _clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallBack), _clientSocket);
    }
    private void Update()
    {
        if (_server_data1.Length > 0 && _CONNECTED)
            fetchBufferData();
        if (_CONNECTED && _CLIENT_ID == -1)
        {
            ResendId();
        }

    }

    public void fetchBufferData()
    {
        //String[] data = _server_data.Pop().Split(',');
        String[] data = _server_data1.Split(',');
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == "me")
            {
                if (i + 1 > data.Length)
                    continue;
                int id;
                if (int.TryParse(data[i + 1], out id))
                {
                    _CLIENT_ID = id;
                    Debug.Log("myId is " + id);
                }
                i += 1;
            }
            else if (data[i] == "new")
            {
                if (i + 2 > data.Length)
                    continue;
                int id;
                if (data[i + 1] == "client")
                {
                    if (int.TryParse(data[i + 2], out id))
                    {
                        bool existed = false;
                        foreach (var p in _online_Players)
                        {
                            if (id == p.Key)
                            {
                                existed = true;
                                break;
                            }
                        }
                        if (!existed)
                        {
                            Debug.Log("initializing new player");
                            _online_Players.Add(id, Instantiate(summoned_player, new Vector2(0, 0), Quaternion.identity) as GameObject);
                        }
                    }
                }
                i += 2;
            }
            else if (data[i] == "[client]")
            {
                if (i + 2 > data.Length)
                    continue;
                int id;
                if (int.TryParse(data[i + 1], out id))
                {
                    foreach (var player in _online_Players)
                    {
                        if (id == player.Key)
                        {
                            String[] data2 = data[i + 2].Split('|');
                            float x, y;
                            if (float.TryParse(data2[0], out x) && float.TryParse(data2[1], out y))
                            {
                                Vector2 v = new Vector2(x, y);
                                player.Value.transform.position = v;
                            }
                        }
                    }
                    i += 2;
                }
            }
        }
    }
    private void RecieveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int recievedDataSize = socket.EndReceive(ar);
            byte[] data = new byte[recievedDataSize];
            Array.Copy(_receieve_buffer, data, recievedDataSize);
            _server_data1 = Encoding.ASCII.GetString(data);
            _clientSocket.BeginReceive(_receieve_buffer, 0, _receieve_buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), _clientSocket);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log("!!!Cant connect to server!!!");
        }
    }


    public void SendCallBack(IAsyncResult AR)
    {
        try
        {
            Socket socket = (Socket)AR.AsyncState;
            if ((int)socket.EndSend(AR) > 0)
            {
                Debug.Log("message sent success");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void Connect()
    {
        try
        {
            if (_clientSocket.Connected)
            {
                _clientSocket.Shutdown(SocketShutdown.Both);
                _clientSocket.Close();
                _clientSocket.Dispose();
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
        }
        catch
        {
            Debug.Log("Cant dsipose old connection");
        }

        while (!_clientSocket.Connected) // Infinite loop FIX
        {
            int attempts = 0;

            attempts++;
            try
            {
                var ip = GetLocalIp();
                Debug.Log(ip.ToString());
                _clientSocket.Connect(ip, 5555);

                Debug.Log(ip.ToString());
            }
            catch (SocketException)
            {
                Debug.Log("cannot connect to server attemps: " + attempts.ToString());
            }
        }
        if (_clientSocket.Connected)
        {
            _CONNECTED = true;
            _clientSocket.BeginReceive(_receieve_buffer, 0, _receieve_buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), _clientSocket);
        }
    }

    private IPAddress GetLocalIp()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }
        }
        return IPAddress.Any;
    }



}
