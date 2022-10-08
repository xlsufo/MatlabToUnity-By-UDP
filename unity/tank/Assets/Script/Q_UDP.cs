using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Q_UDP : MonoBehaviour
{
    private bool signalNew = false;  //定义接收到了新消息的信号
    static int recvDataNum = 48;   //定义接收的每个包的字节数
    UdpAgreement.SocketPackage mySocketPack;
    //定义接收端口
    Socket socketRecv;
    [Header("---------------------服务端设置--------------------")]

    [Header("使用的IP")]
    public string selfConnectIP = "127.0.0.1";
    [Header("使用的端口")]
    public int selfConnectPoint = 23333;
    EndPoint selfListenEnd;       //本机服务端所监听的ip端口对

    //监听线程
    Thread connectThread;

    [Header("------------------入站的字符串---------------")]
    public string recvStr;

    int recvLen = 0;       //定义接受byte数组的长度
    byte[] recvData = new byte[recvDataNum];      //接收到的byte数据

    void Start()
    {
        InitSocket();//网络通信初始化
        
    }

    void InitSocket()
    {
        //作为服务端    
        //本机服务器监听的ip端口对
        selfListenEnd = new IPEndPoint(IPAddress.Parse(selfConnectIP), selfConnectPoint);
        socketRecv = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socketRecv.Bind(selfListenEnd);
        //开启本机服务器监听线程
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
        print("开启监听");
    }

    //本机服务器监听
    void SocketReceive()
    {
        while (true)
        {
            recvData = new byte[recvDataNum];
            recvLen = socketRecv.ReceiveFrom(recvData, ref selfListenEnd);

           //将字节数组转结构体函数
            mySocketPack = (UdpAgreement.SocketPackage)BytesToStruct(recvData, mySocketPack.GetType());
            recvStr = mySocketPack.x.ToString();
            Debug.Log("信息来自" + selfListenEnd.ToString() + ":" + recvStr);
            signalNew = true;
        }
    }

    //连接关闭
    void SocketQuit()
    {
        //关闭线程
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最后关闭socket
        if (socketRecv != null)
            socketRecv.Close();

        StopAllCoroutines();
    }
    void OnApplicationQuit()
    {
        SocketQuit();
    }

    void Update()
    {
        if (signalNew == true)
        {
            signalNew = false;
            this.transform.position = new Vector3((float)mySocketPack.x, 0, 0);      
           
        }
    }

    /*字节数组转结构体函数*/  
    public object BytesToStruct(byte[] bytes, Type strcutType)
    {
        int size = Marshal.SizeOf(strcutType);
        IntPtr buffer = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.Copy(bytes, 0, buffer, size);
            return Marshal.PtrToStructure(buffer, strcutType);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }

    }

}
