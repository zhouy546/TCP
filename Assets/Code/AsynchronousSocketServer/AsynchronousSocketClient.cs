using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class AsynchronousSocketClient : MonoBehaviour {
    public InputField HostName,Port;
    public Text DebugText;
    private IPAddress myIP = IPAddress.Parse("127.0.0.1");
    private IPEndPoint MyServer;
    private Socket mySocket;
    private static ManualResetEvent connectReset = new ManualResetEvent(false);
    private static ManualResetEvent sendReset = new ManualResetEvent(false);
	// Use this for initialization
	void Start () {
		
	}

    public void Connect()
    {
        try
        {
            IPHostEntry myHost = new IPHostEntry();
            myHost = Dns.GetHostEntry(HostName.text);
            string IPstring = myHost.AddressList[0].ToString();
            myIP = IPAddress.Parse(IPstring);
        }
        catch 
        {
            DebugText.text += "输入的IP地址格式不正确";
        }
        try
        {
            MyServer = new IPEndPoint(myIP, Int32.Parse(Port.text));

            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mySocket.BeginConnect(MyServer,new AsyncCallback(connectCallback),mySocket);
            connectReset.WaitOne();
        }
        catch (Exception e)
        {

            DebugText.text += e.Message;
        }
    }

    void connectCallback(IAsyncResult ar) {
        try
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndConnect(ar);
            try
            {
                byte[] byteData = System.Text.Encoding.BigEndianUnicode.GetBytes("准备完毕，可以通话" + "\n");
                mySocket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), mySocket);
            }
            catch (Exception ee)
            {
                DebugText.text += ee.Message;
            }
            DebugText.text += "IP ADDRESS"+HostName.text +"PORT"+ Port.text + "建立连接" + "\n";
            Thread thread = new Thread(new ThreadStart(target));
            thread.Start();
            connectReset.Set();

        }
        catch (Exception)
        {

            throw;
        }
    }

    private void SendCallback(IAsyncResult ar) { }
    private void target() { }
	// Update is called once per frame
	void Update () {
		
	}
}
