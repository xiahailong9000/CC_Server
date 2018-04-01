using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using CC_ServerDLL;
namespace CC_部落大战Server管理器_9800 {
    public class C_LinkServer {
        public static C_LinkServer ooo;
        public List<C_Serv客户端> o_服务器列表 = new List<C_Serv客户端>();
        public C_UdpClient o_Udp;
        static void Main(string[] args) {
            ooo = new C_LinkServer();
            Console.WriteLine("_________________________域名链接服务器:");
            C_TcpClient.S_启动监听(9800, new C_Serv客户端());
            ooo.o_Udp = C_UdpClient.S_启动监听(9800, C_Udp管理器.S_接口_消息处理);
            Thread th = new Thread(delegate() {
                #region MyRegion----计时事件-----------------
                int ii = 0;
                while (true) {
                    Thread.Sleep(2000);
                    ii += 2;
                    if (ii % 2 == 0) {
                        S_2秒事件();
                    }
                    if (ii % 10 == 0) {
                        S_10秒事件();
                    }
                    if (ii % 20 == 0) {
                        S_20秒事件();
                    }
                    if (ii % 30 == 0) {
                        S_30秒事件();
                    }
                    if (ii % 60 == 0) {
                        S_60秒事件();
                    }
                    if (ii % 180 == 0) {
                        S_3分钟事件();
                    }
                    if (ii % 600 == 0) {
                        ii = 0;
                        S_10分钟事件();
                    }
                }
                #endregion
            });
            th.IsBackground = true;
            th.Start();
            Console.ReadKey();
        }
        #region MyRegion--计时事件------------------
        static void S_2秒事件() {
            //Console.WriteLine("S_2秒事件---------");
            C_待机事件器.S_清楚过时的待机事件();
        }
        static void S_10秒事件() {
            Console.WriteLine("S_10秒事件---------");
        }
        static void S_20秒事件() {
            //Console.WriteLine("S_20秒事件---------");
        }
        static void S_30秒事件() {
            //Console.WriteLine("S_30秒事件---------");
            C_LinkServer.ooo.o_服务器列表 = C_LinkServer.ooo.o_服务器列表.OrderBy(n => n.o_用户数量).ToList();
        }
        static void S_60秒事件() {
            //Console.WriteLine("S_60秒事件---------");
            List<C_Serv客户端> z清除列表 = new List<C_Serv客户端>();
            foreach (var n in ooo.o_服务器列表) {
                if (C_Toot.S_Get时间搓int() - n.o_在线时间 > 70) {
                    z清除列表.Add(n);
                }
            }
            for (int i = 0; i < z清除列表.Count; i++) {
                ooo.o_服务器列表.Remove(z清除列表[i]);
            }
        }//离线服务器清除-------------
        static void S_3分钟事件() {
            //Console.WriteLine("S_3分钟事件---------");
        }
        static void S_10分钟事件() {
            //Console.WriteLine("S_10分钟事件---------");
        }
        #endregion
    }
    public class C_Serv客户端 : C_TcpClient.C_逻辑端 {
        public int o_用户数量, o_在线时间;
        //public override C_TcpClient.C_逻辑端 S_接口_初始化(C_TcpClient nn) {
        //    if (o_通信器 != null) {
        //        C_Serv客户端 mm = new C_Serv客户端();
        //        return mm.S_接口_初始化(nn);
        //    } else {
        //        o_通信器 = nn;
        //        return this;
        //    }
        //}
        public override void S_接口_消息处理(ushort z消息类型, string msg, byte[] zData) {
            //base.S_接口_消息处理(z消息类型, msg, zData);
            switch (z消息类型) {
                case 1://------------初次登陆-回应-------- z玩家地址-------
                    //string ss = Encoding.UTF8.GetString(zData, 1, zData.Length - 1);
                    Console.WriteLine("发送——初次登陆-回应----" + msg);
                    string[] sss = msg.Split('&');
                    C_LinkServer.ooo.o_Udp.S_发送消息(1, o_通信器.o_IP端口.O_IP, sss[1]);//zIP地址-------------
                    C_待机事件器.S_解除待机(new m_IP(sss[1]), 1, true);
                    break;
                case 3://------------注册-回应------------ 1 &玩家地址  -------
                    sss = msg.Split('&');
                    C_LinkServer.ooo.o_Udp.S_发送消息(3, o_通信器.o_IP端口.O_IP + "&" + sss[0], sss[1]);//zIP地址  &1-------------
                    break;
                case 10:
                    #region MyRegion-------收到的Server--心跳消息---------------------------
                    if (C_LinkServer.ooo.o_服务器列表.Contains(this) == false) {
                        C_LinkServer.ooo.o_服务器列表.Add(this);
                    }
                    Console.WriteLine("服务器_心跳__" + o_通信器.o_IP端口.O_String + "__________________用户数量:_" + msg);
                    o_用户数量 = int.Parse(msg);
                    o_在线时间 = C_Toot.S_Get时间搓int();
                    #endregion
                    break;
            }
        }
        public override void S_接口_关闭() {
        }
    }
    public class C_Udp管理器  {
        public static void S_接口_消息处理(C_UdpClient zUdpClient, IPEndPoint z地址, byte[] zData) {
            switch (zData[0]) {
                case 1://----------初次-登陆--------------------------z玩家ID-------------------
                    string ss = Encoding.UTF8.GetString(zData, 1, zData.Length - 1);
                    foreach (var n in C_LinkServer.ooo.o_服务器列表) {//z玩家ID  &玩家地址-------
                        n.o_通信器.S_发送消息(1, ss + "&" + z地址.ToString());
                    }
                    new C_待机事件器(zData, z地址, true);
                    Console.WriteLine("初次-登陆_____" + z地址.ToString() + "__________" + ss);
                    break;
                case 3://--------玩家注册--------------//z玩家ID &姓名 &密码--------------
                    ss = Encoding.UTF8.GetString(zData, 1, zData.Length - 1);
                    if (C_LinkServer.ooo.o_服务器列表.Count > 0) {
                        C_LinkServer.ooo.o_服务器列表[0].o_通信器.S_发送消息(3, ss + "&" + z地址.ToString());//z玩家ID &姓名 &密码 &玩家地址-------------
                    }
                    new C_待机事件器(zData, z地址, true);
                    Console.WriteLine("玩家注册_________" + ss);
                    break;
            }
        }
    }
    public class C_待机事件器 {
        public byte[] o_Data;
        public int o_在线时间;
        public bool o_是否Udp;
        public C_待机事件器(byte[] zData, IPEndPoint z地址, bool z是否Udp) {
            o_Data = zData;
            o_在线时间 = C_Toot.S_Get时间搓int();
            o_是否Udp = z是否Udp;
            m_IP o_地址 = new m_IP(z地址.ToString());
            if (o_待机事件集合.ContainsKey(o_地址) == false) {
                o_待机事件集合[o_地址] = new List<C_待机事件器>();
            }
            o_待机事件集合[o_地址].Add(this);
        }
        public void S_发送死亡消息(m_IP z地址) {
            if (o_是否Udp == false) {

            } else {
                switch (o_Data[0]) {
                    case 1:
                        C_Serv客户端 z第1Server = C_LinkServer.ooo.o_服务器列表[0];
                        C_LinkServer.ooo.o_Udp.S_发送消息(1, z第1Server.o_通信器.o_IP端口.O_IP, z地址.O_String);//zIP地址-------
                        break;
                }
            }
        }
        static Dictionary<m_IP, List<C_待机事件器>> o_待机事件集合 = new Dictionary<m_IP, List<C_待机事件器>>();
        public static void S_解除待机(m_IP z地址, byte z消息类型, bool z是否Udp) {
            if (o_待机事件集合.ContainsKey(z地址)) {
                List<C_待机事件器> nn = o_待机事件集合[z地址];
                nn = nn.Where(n => {
                    return n.o_Data[0] == z消息类型 && n.o_是否Udp == z是否Udp;
                }).ToList();
                for (int i = 0; i < nn.Count; i++) {
                    o_待机事件集合[z地址].Remove(nn[0]);
                }
                if (o_待机事件集合[z地址].Count == 0) {
                    o_待机事件集合.Remove(z地址);
                }
            }
        }
        public static void S_清楚过时的待机事件() {
            int z当前时间 = C_Toot.S_Get时间搓int();
            List<m_IP> z清楚列表 = new List<m_IP>();
            foreach (var n in o_待机事件集合) {
                if (n.Value.Count > 0) {
                    for (int i = 0; i < n.Value.Count; i++) {
                        if (z当前时间 - n.Value[i].o_在线时间 > 3) {
                            n.Value[i].S_发送死亡消息(n.Key);
                            n.Value.RemoveAt(i);
                        }
                    }
                } else {
                    z清楚列表.Add(n.Key);
                }
            }
            for (int i = 0; i < z清楚列表.Count; i++) {
                o_待机事件集合.Remove(z清楚列表[i]);
            }
        }
    }
}
