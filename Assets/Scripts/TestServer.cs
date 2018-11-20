using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;  //网络传输
using System.Net.Sockets;  //套接字命名空间
using System.IO;
using System.Threading;

public class TestServer : MonoBehaviour {
    private IPAddress ip;  //服务器IP；
    private EndPoint port; //1090
    private Socket tcpServer;  //用于连接客户端的套接字:
                               //用于连接服务器和客户端IP和端口号的传输层，
                               //并且可以通过套接字发送和接受数据协议（字节流）
	// Use this for initialization
	void Start () {
        InitSocket();
        Thread t = new Thread(StartServer);
        t.Start();
	}

    private void InitSocket()
    {
        //创建tcp Socket
        tcpServer = new Socket(AddressFamily.InterNetwork,//互联网传输模式
            SocketType.Stream, //以字节流形式传输
            ProtocolType.Tcp); //连接模式为TCP长连接

        ip = new IPAddress(new byte[] { 192, 168, 0, 128 });

        port = new IPEndPoint(ip, 1090);
    }
	

    public void  StartServer()
    {
        //向计算机申请端口号（检测端口号是否有socket占用，没有占用绑定端口号）
        tcpServer.Bind(port);

        //开启socket监听模式
        tcpServer.Listen(100); 
        Debug.Log("等待连接。。。");
        while(true)
        {
            Socket clientSocket = tcpServer.Accept();
            Debug.Log("有人连接成功了");

            //创建字节流缓冲区
            MemoryStream ms = new MemoryStream();
            //创建字节书写流
            BinaryWriter bw = new BinaryWriter(ms);
            //赋值发送内容
            Login l = new Login();
            l.name = "张三";
            l.password = "123456";

            //打包发送内容为字节流的形式
            bw.Write(l.name);
            bw.Write(l.password);
            bw.Flush();
            bw.Close();

            clientSocket.Send(ms.ToArray());


            ///////////解包////////////
            //定义接受协议字节流，大小限制为 1kb
            byte[] receiveData = new byte[1024];
            clientSocket.Receive(receiveData);  //这里如果用tcpServer则服务器接收不到客户端的消息
            MemoryStream newMs = new MemoryStream(receiveData);
            //将字节流的读取位置重新挪回起始位置（索引为0）
            newMs.Seek(0, SeekOrigin.Begin);

            //解析协议包
            BinaryReader br = new BinaryReader(newMs);
            string result = br.ReadString();
            uint RoleID = br.ReadUInt32();
            br.Close();

            Debug.Log(string.Format("接收到了回复协议：Result：{0},RoleID：{1}", result, RoleID));

            clientSocket.Close();
            clientSocket = null;
        }                     
    }

	// Update is called once per frame
	void Update () {
		 
	}
}

public class Login
{
    public string name;
    public string password;
}

public class LoginAck
{
    public string Result;
    public string RoleID;
}
