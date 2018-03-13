using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;

public class AsynchronousSocketServerSc : MonoBehaviour {
    public InputField Port,serverName,TextToSend;
    public Text DebugText, richTextBox1;
    private string debugtex, richtex; 
    private IPAddress myIP = IPAddress.Parse("127.0.0.1");
    private IPEndPoint Myserver;
    private Socket mySocket;
    private Socket handler;
    private static ManualResetEvent myReset = new ManualResetEvent(false);
	// Use this for initialization
	void Start () {
		
	}

    public void StartListening()
    {
        try
        {
           // IPHostEntry myHost = new IPHostEntry();
          //  myHost = Dns.GetHostEntry(serverName.text);
         //   string IPstring = myHost.AddressList[0].ToString();
            myIP = IPAddress.Parse(serverName.text);
        }
        catch {
            debugtex+="输入的IP地址格式不正确，请重新输入";
        }

        try
        {
            Myserver = new IPEndPoint(myIP, Int32.Parse(Port.text));
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mySocket.Bind(Myserver);
            mySocket.Listen(50);
            debugtex += "主机" + serverName.text + "端口" + Port.name + "开始监听。。。。" + "\n";
            Thread thread = new Thread(new ThreadStart(target));
            thread.Start();

        }
        catch (Exception e) {
            debugtex += e.Message + "\n";
        }

    }
    //线程同步方法
    private void target() {
        while (true) {
            myReset.Reset();
            mySocket.BeginAccept(new AsyncCallback(AcceptCallback), mySocket);
            myReset.WaitOne();
        }
    }

    private void AcceptCallback(IAsyncResult ar)
    {
        myReset.Set();
        Socket listener = (Socket)ar.AsyncState;
        handler = listener.EndAccept(ar);

        StateObject state = new StateObject();
        state.workSocket = handler;
        debugtex += "与客户建立链接" + "\n";

        try {
            byte[] byteDate = System.Text.Encoding.BigEndianUnicode.GetBytes("已经主备好，请通话" + "\n");
            //开始发送数据
            handler.BeginSend(byteDate, 0, byteDate.Length,0,new AsyncCallback(SendCallback),handler);

        }
        catch(Exception e) {
            debugtex += e.Message;
        }
        Thread thread = new Thread(new ThreadStart(begReceive));
        thread.Start();
    }

    private void SendCallback(IAsyncResult ar) {
        try
        {
            handler = (Socket)ar.AsyncState;
            int bytesSent = handler.EndSend(ar);
        }
        catch (Exception e)
        {

            debugtex += e.Message.ToString();
        }
    }

    private void begReceive() {
        StateObject state = new StateObject();
        state.workSocket = handler;
        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback) , state);
    }

    private void ReadCallback(IAsyncResult ar) {
        StateObject state = (StateObject)ar.AsyncState;
        Socket tt = state.workSocket;

        //结束读取并获取读取字节数
        int byteesRead = handler.EndReceive(ar);
        state.sb.Append(System.Text.Encoding.BigEndianUnicode.GetString(state.buffer, 0, byteesRead));
        string content = state.sb.ToString();
        if (content == "") {
            return;
        }
        state.sb.Remove(0, content.Length);
        richtex += content + "\n";
        Debug.Log(content);
        //重新开始读取数据
        tt.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
    }

   public  void SendtheMessage() {
        try
        {
            byte[] byteData = System.Text.Encoding.BigEndianUnicode.GetBytes(TextToSend.text);
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);

        }
        catch (Exception e)
        {
            debugtex += e.Message;
        }
    }

    public void StopListening() {
        try
        {
            mySocket.Close();
            debugtex += "主机" + serverName.text + "端口" + Port.text + "停止监听。。。。" + "\n";
            
        }
        catch{

            debugtex += "监听尚未开始，关闭无效";
        }
    }
	// Update is called once per frame
	void Update () {
        DebugText.text = debugtex;
        richTextBox1.text = richtex;
	}
}
