using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class AsynchronousSocketClient : MonoBehaviour {
    public InputField HostName,Port,SendText;
    public Text DebugText, MessageText;
    private string debugtex,messagetex;
    private IPAddress myIP = IPAddress.Parse("127.0.0.1");
    private IPEndPoint MyServer;
    private Socket mySocket;
    Thread thread;
    private static ManualResetEvent connectReset = new ManualResetEvent(false);
    private static ManualResetEvent sendReset = new ManualResetEvent(false);
	// Use this for initialization
	void Start () {
		
	}

    public void Connect()
    {
        try
        {
          //  IPHostEntry myHost = new IPHostEntry();
           // myHost = Dns.GetHostEntry(HostName.text);
         //   string IPstring = myHost.AddressList[0].ToString();
            myIP = IPAddress.Parse(HostName.text);
        }
        catch 
        {
            debugtex += "输入的IP地址格式不正确";
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

          debugtex  += e.Message;
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
            debugtex += "IP ADDRESS"+HostName.text +"PORT"+ Port.text + "建立连接" + "\n";
           thread = new Thread(new ThreadStart(target));
            thread.Start();
           connectReset.Set();

        }
        catch 
        {

          
        }
    }

    public void DisConnect() {
        try
        {
            mySocket.Close();
           debugtex+= "IP ADDRESS" + HostName.text + "PORT" + Port.text + "断开连接" + "\n";
        }
        catch (Exception)
        {
            debugtex += "连接尚未建立，断开无效";
        }
    }

    public void SendTheMessage() {
        byte[] byteDate = System.Text.Encoding.BigEndianUnicode.GetBytes(SendText.text);
        mySocket.BeginSend(byteDate, 0, byteDate.Length, 0, new AsyncCallback(SendCallback), mySocket);
    }

    private void SendCallback(IAsyncResult ar) {
        try
        {
            Socket client = (Socket)ar.AsyncState;
            sendReset.Set();
        }
        catch (Exception e)
        {

            debugtex += e.ToString();
        }
    }
    private void target() {
        try
        {
            StateObject state = new StateObject();
            state.workSocket = mySocket;
            mySocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {

            debugtex += e.Message+"\n";
        }
    }

    private void ReceiveCallback(IAsyncResult ar) {
        try
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;
            int bytesRead = client.EndReceive(ar);
            state.sb.Append(System.Text.Encoding.BigEndianUnicode.GetString(state.buffer, 0, bytesRead));
            string aa = state.sb.ToString();
            state.sb.Remove(0, aa.Length);
            messagetex += aa + "\n";
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }
        catch 
        {


        }
    }

    // Update is called once per frame
    void Update () {
        DebugText.text = debugtex;
        MessageText.text = messagetex;
    }
}
