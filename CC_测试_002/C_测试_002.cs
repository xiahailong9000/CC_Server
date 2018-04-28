using System.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CC_ServerDLL;
using System.Net;
namespace CC_测试_002 {
   public static class Program {
        static void Main() {
            C_TcpClient.S_启动监听(8088, new C_Tcc());
            while (true) {           
                Console.ReadLine();
            }
        }
        public class C_TestUdp打洞Server {
            static C_UdpClient o_44563, o_44573;
            public static void S_Start() {
                o_44563 = C_UdpClient.S_启动监听(44563, S_UdpMessageHandle);
                o_44573 = C_UdpClient.S_启动监听(44573, S_UdpMessageHandle);
            }
            static void S_UdpMessageHandle(C_UdpClient zUdpClient, IPEndPoint zIPEndPoint, byte[] zBytes) {
                Console.WriteLine("收到消息___" + zUdpClient.o_通信对象.Client.LocalEndPoint + "_____" + zIPEndPoint);
                zUdpClient.S_发送消息(5, "ddddddddddd___" + zIPEndPoint, zIPEndPoint);
            }
        }
        public class C_Tcc: C_TcpClient.C_逻辑端 {
            public override void S_接口_消息处理(ushort z消息类型, string msg, byte[] zData) {
                Console.WriteLine("收到消息__" + z消息类型 + "___" + msg);
            }
        }
    }

}
