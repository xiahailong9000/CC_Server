using System.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CC_ServerDLL;
using System.Net;
namespace CC_测试_002 {
   public static class Program {
        static void Main() {
            C_55点 nn1 = new C_55点(2);
            C_55点 nn2 = new C_55点(5);
            Console.WriteLine(nn1== nn2);
            Console.WriteLine(nn1 != nn2);
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
        public class C_55点 {
            public int o_序号;
            public C_55点(int ii) {
                o_序号 = ii;
            }
            public static bool operator ==(C_55点 lhs, C_55点 rhs) {
                return lhs.o_序号 == rhs.o_序号;
            }
            public static bool operator !=(C_55点 lhs, C_55点 rhs) {
                return lhs.o_序号 != rhs.o_序号;
            }
        }
    }

}
