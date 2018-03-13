using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
using System;

public class BasicServer : MonoBehaviour {
    private IPAddress myIPaddress;
    private IPEndPoint MyServer;
    private Socket sock;
    private bool check = true;
    private Socket accSock;
    public InputField ipInput,SendMessageBox,Port;
    public Text DebugText,getMessageBox;

    public void StartListening()
    {
        try
        {
            myIPaddress = IPAddress.Parse(ipInput.text);
            Debug.Log(myIPaddress);
        }
        catch {
            DebugText.text = "\n"+"您输入的IP地址格式不正确，请重新输入";
        }

        try
        {
            Thread thread = new Thread(new ThreadStart(Accp));
            thread.Start();
        }
        catch(Exception e) {
            DebugText.text += e.ToString();
        }
    }

    private void Accp() {
        MyServer = new IPEndPoint(myIPaddress, Int32.Parse(Port.text));
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        sock.Bind(MyServer);
        sock.Listen(50);
        DebugText.text += "\n"+"主机：" + ipInput.text + "  " + "端口：" + Port.text + "  " + "开始监听";          
         while (check) {
            accSock = sock.Accept();
            if (accSock.Connected)
            {
                DebugText.text += "\n" + "与客户建立链接";
                Thread thread = new Thread(new ThreadStart(round));
                thread.Start();
            }
         }
    }

    private void round() {
        while (true) {
            Byte[] rec = new Byte[64];
            NetworkStream networkStream = new NetworkStream(accSock);
            networkStream.Read(rec, 0, rec.Length);
            string readMessage = System.Text.Encoding.BigEndianUnicode.GetString(rec);
            getMessageBox.text = readMessage;
        }
    }

    public void SendMessage() {
        try
        {
            Byte[] sendByte = new Byte[64];
            string send = SendMessageBox.text;
            NetworkStream netStream = new NetworkStream(accSock);
            sendByte = System.Text.Encoding.BigEndianUnicode.GetBytes(send.ToCharArray());
            netStream.Write(sendByte, 0, sendByte.Length);
        }
        catch {
            DebugText.text += "\n" + "尚未建立链接";
        }
    }

    public void StopListening() {
        try {
            sock.Close();
            DebugText.text += "\n" + "主机：" + ipInput.text + "  " + "端口：" + Port.text + "  " + "停止监听";

        }
        catch { DebugText.text += "\n" + "尚未开始监听"; }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
