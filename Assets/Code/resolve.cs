using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

public class resolve : MonoBehaviour {


	// Use this for initialization
	void Start () {
        //IPHostEntry myHost = new IPHostEntry();
        //myHost = Dns.Resolve("www.baidu.com");
        //for (int i = 0;  i< myHost.AddressList.Length; i++)
        //{
        //    Debug.Log(myHost.HostName.ToString());
        //}
        GetHostByName();

    }
	void GetHostByName()
    {
        IPHostEntry myHost = new IPHostEntry();
        try
        {
            IPAddress iPAddress = IPAddress.Parse("180.97.33.107");
            myHost = Dns.GetHostEntry(iPAddress);

        }
        catch(Exception e) {
            Debug.Log(e.ToString());
        }
        for (int i = 0; i < myHost.AddressList.Length; i++)
        {
            Debug.Log(myHost.AddressList[i].ToString());
        }
    }
    /// <summary>
    /// send message to 127.0.0.1, using Socket send
    /// </summary>
    void SocketsendMessage() {
        IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint MyServer = new IPEndPoint(iPAddress, 2020);
        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        sock.Bind(MyServer);
        sock.Listen(50);
        Socket bbb = sock.Accept();
        Byte[] bytee = new Byte[64];
        string send = "the string go through the network via socket";
        bytee = System.Text.Encoding.BigEndianUnicode.GetBytes(send.ToCharArray());
        bbb.Send(bytee, bytee.Length, 0);
    }

    void NetworkStreamSendMessage() {
        IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint MyServer = new IPEndPoint(iPAddress, 2020);
        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        sock.Bind(MyServer);
        sock.Listen(50);
        Socket bbb = sock.Accept();
        NetworkStream stre = new NetworkStream(bbb);
        Byte[] ccc = new Byte[512];
        string send = "the string go through the network via NetWorkStream";
        ccc = System.Text.Encoding.BigEndianUnicode.GetBytes(send);
        stre.Write(ccc, 0, ccc.Length);
        
    }

    void ReveiveMessage() {
        IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint MyServer = new IPEndPoint(iPAddress, 2020);
        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        sock.Bind(MyServer);
        sock.Listen(50);
        Socket bbb = sock.Accept();
        NetworkStream stre = new NetworkStream(bbb);
        Byte[] ccc = new Byte[512];
        stre.Read(ccc, 0, ccc.Length);
        string readMessage = System.Text.Encoding.BigEndianUnicode.GetString(ccc);

    }

	// Update is called once per frame
	void Update () {
		
	}
}
