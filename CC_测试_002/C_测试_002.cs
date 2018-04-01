using System.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CC_ServerDLL;
using System.Net;
namespace CC_测试_002 {
   public static class Program {
        static C_UdpClient o_44563, o_44573;
        static void Main() {
            o_44563 = C_UdpClient.S_启动监听(44563, S_UdpMessageHandle);
            o_44573 = C_UdpClient.S_启动监听(44573, S_UdpMessageHandle);
            while (true) {           
                Console.ReadLine();
            }
        }
        static void S_UdpMessageHandle(C_UdpClient zUdpClient,IPEndPoint zIPEndPoint,byte[] zBytes){
            Console.WriteLine("收到消息___"+ zUdpClient.o_通信对象.Client.LocalEndPoint+"_____" + zIPEndPoint);
            zUdpClient.S_发送消息(5, "ddddddddddd___"+ zIPEndPoint, zIPEndPoint);
        }
    }

}
