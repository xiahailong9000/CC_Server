using CC_ServerDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CC_水果大作战Server管理器_9023 {
    public class C_水果Serve管理器_9023 {
        static void Main(string[] args) {
            Console.WriteLine("________域名链接服务器__________");
            C_TcpClient2.S_启动监听(9023, new C_Tcp服务器());
            Thread th = new Thread(delegate() {
                #region MyRegion----计时事件-----------------
                int ii = 0;
                Random ran = new Random();
                while (true) {
                    int z震荡率 = ran.Next(100);
                    try {
                        Thread.Sleep(4950 + z震荡率);
                        ii += 5;
                        S_5秒事件();
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
                        if (ii >= 600) {
                            ii = 0;
                            S_10分钟事件();
                        }
                    } catch (Exception) {}
                }
                #endregion
            });
            th.IsBackground = true;
            th.Start();
            Console.ReadKey();
        }
        #region MyRegion--计时事件------------------
        static void S_5秒事件() {
        }
        static void S_10秒事件() {
        }
        static void S_20秒事件() {
        }
        static void S_30秒事件() {
            try {
                C_Tcp服务器.S_30秒_刷新网格列表();
            } catch (Exception ex) {
                Console.WriteLine("S_30秒_刷新网格列表_______" + ex.Message + "____" + ex.StackTrace);
            }
        }
        static void S_60秒事件() {
        }
        static void S_3分钟事件() {
        }
        static void S_10分钟事件() {
        }
        #endregion
    }
    public class C_Tcp服务器 : C_TcpClient2.C_逻辑端 {
        public int o_用户数量, o_在线时间, o_匹配的用户数量, o_在线玩家数量;
        public bool o_是否公网地址;
        public static Dictionary<string, C_Tcp服务器> o_网格列表 = new Dictionary<string,C_Tcp服务器>();
        public override void S_接口_消息处理(ushort z消息类型, string msg, byte[] zData) {
            switch (z消息类型) {
                case 20://z搜到服务器发来的心跳---z数据用户数量--z等待匹配数量--z在线玩家数量-----
                    if (o_网格列表.ContainsKey(o_通信器.o_IP端口.O_IP) == false) {//------z首次链接--------------
                        o_网格列表.Add(o_通信器.o_IP端口.O_IP, this);
                        o_通信器.S_发送消息(20, S_群发服务器地址(false) + "@" + (o_网格列表.Count-1));
                        Console.WriteLine("TTT______网格链接成功_____________________");
                    }
                    Console.WriteLine("收到心跳_____" + o_通信器.o_IP端口.O_String + "___________" + msg);
                    string[] sss = msg.Split('&');
                    o_用户数量 = int.Parse(sss[0]);
                    o_匹配的用户数量 = int.Parse(sss[1]);
                    o_在线玩家数量 = int.Parse(sss[2]);
                    o_在线时间 = C_Toot.S_Get时间搓int();
                    break;
            }
        }
        public static string S_群发服务器地址(bool z是否群发) {
            StringBuilder ssb = new StringBuilder();
            List<C_Tcp服务器> z列表 = o_网格列表.Values.ToList();
            for (int i = 0; i < z列表.Count; i++) {
                try {
                    if (i == 0) {
                        ssb.Append(z列表[i].o_通信器.o_IP端口.O_IP);
                    } else {
                        ssb.Append("&" + z列表[i].o_通信器.o_IP端口.O_IP);
                    }
                } catch (Exception) {}
            }
            if (z是否群发) {
                for (int i = 0; i < z列表.Count; i++) {
                    z列表[i].o_通信器.S_发送消息(20, ssb.ToString() + "@" + i);
                }
            }
            return ssb.ToString();
        }
        public static void S_30秒_刷新网格列表() {
            Console.WriteLine("网格数----------------------" + o_网格列表.Count);
            int z当前时间 = C_Toot.S_Get时间搓int();
            List<string> z清除列表 = new List<string>();
            foreach (var n in o_网格列表) {
                if (n.Value.o_通信器 == null || n.Value.o_通信器.o_通信对象 == null) {
                    z清除列表.Add(n.Key);
                }
                if (z当前时间 - n.Value.o_在线时间 > 62) {
                    Console.WriteLine("清除网格__" + n.Value.o_通信器.o_IP端口.O_String + "___时间差:" + (z当前时间 - n.Value.o_在线时间));
                    z清除列表.Add(n.Key);
                }
            }
            for (int i = 0; i < z清除列表.Count; i++) {
                o_网格列表.Remove(z清除列表[i]);
            }
            Dictionary<string, C_Tcp服务器> z在线玩家列表 = o_网格列表.Where(n => n.Key != n.Value.o_通信器.o_IP端口.O_IP).ToDictionary(n => n.Key, n => n.Value);
            foreach (var n in z在线玩家列表) {
                o_网格列表.Remove(n.Key);
                o_网格列表[n.Value.o_通信器.o_IP端口.O_IP] = n.Value;
            }
            S_群发服务器地址(true);
        }
    }
}
