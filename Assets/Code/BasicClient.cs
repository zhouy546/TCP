using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;

public class BasicClient : MonoBehaviour {

    public InputField ip,port,MessageToSend;
    public Text DebugBox,ReceiveText;
    string RecMessage;
    public static string st = "127.0.0.1";
    private IPAddress myIP;
    private IPEndPoint MyServer;
    private Socket connectSock;
    private bool check = true;

	// Use this for initialization
	void Start () {

    }

    public void getConnection()
    {
        try
        {
            myIP = IPAddress.Parse(ip.text);
        }
        catch 
        {
            DebugBox.text += "IP 地址不正确！"+"\n";
        }

        try
        {
            MyServer = new IPEndPoint(myIP, Int32.Parse(port.text));
            connectSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connectSock.Connect(MyServer);
            DebugBox.text +="与主机"+ip.text+"port"+port.text+"成功连接"+"\n";
            Thread thread = new Thread(new ThreadStart(receive));
            thread.Start();
        }
        catch (Exception e)
        {
            DebugBox.text += e.Message + "\n";
        }

    }

    public void StopConnection() {
        try
        {
            connectSock.Close();
            DebugBox.text += "与主机" + ip.text + "port" + port.text + "断开连接" + "\n";
        }
        catch
        {
            DebugBox.text += "连接尚未建立断开无效" + "\n";
        }
    }

    public void send() {
        try
        {
            Byte[] sendByte = new Byte[64];
            string send = MessageToSend.text;
            NetworkStream netStream = new NetworkStream(connectSock);
            sendByte = System.Text.Encoding.BigEndianUnicode.GetBytes(send.ToCharArray());
            netStream.Write(sendByte, 0, sendByte.Length);
            netStream.Flush();
        }
        catch (Exception)
        {
            DebugBox.text += "连接尚未建立无法发送" + "\n";
        }
    }

    public void receive() {
        while (check)
        {
            Byte[] Rec = new Byte[64];
            NetworkStream networkStream = new NetworkStream(connectSock);
            networkStream.Read(Rec, 0, Rec.Length);
            RecMessage = System.Text.Encoding.BigEndianUnicode.GetString(Rec);
           
        }
    }


	// Update is called once per frame
	void Update () {
        ReceiveText.text = RecMessage;
    }
}
