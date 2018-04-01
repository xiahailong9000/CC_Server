using CC_ServerDLL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CC_好友地图Server_12000 {
    class C_Main {
        public static C_Main ooo = new C_Main();
        static void Main(string[] sss) {
            C_TcpClient2.S_启动监听(12000, new C_aa玩家());
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
                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message + "__***********\n__" + ex.StackTrace);
                    }
                }
                #endregion
            });
            th.IsBackground = true;
            th.Start();
            while (true) {
                C_Debug.Ooo.S_命令保存();
                //Console.ReadKey();
                //Console.WriteLine("水果大作战Server_9002___请勿点击---------");
            }
        }
        #region MyRegion--计时事件------------------
        static void S_5秒事件() {
            try {
                C_Debug.S_Add("dddddddddddddddddddddddddddddddddddddddddsssssssssss+");
            } catch (Exception ex) {
                Console.WriteLine("出错___" + ex.Message + "___" + ex.StackTrace);
            }
        }
        static void S_10秒事件() {
            Console.WriteLine("S_10秒事件---------");
            try {
            } catch (Exception ex) {
                Console.WriteLine("游戏房间-10匹配___" + ex.Message + "___" + ex.StackTrace);
            }
            try {
            } catch (Exception ex) {
                Console.WriteLine("游戏房间-10清除___" + ex.Message + "___" + ex.StackTrace);
            }
        }
        static void S_20秒事件() {
            //Console.WriteLine("S_20秒事件---------");
        }
        static void S_30秒事件() {
            try {
                C_aa矩阵地图.Ooo.S_30秒_玩家更换位置();
            } catch (Exception ex) {
                Console.WriteLine("网格-30清除___" + ex.Message + "___" + ex.StackTrace);
            }
        }
        static void S_60秒事件() {
            //Console.WriteLine("S_60秒事件---------");
        }
        static void S_3分钟事件() {
            //Console.WriteLine("S_3分钟事件---------");
        }
        static void S_10分钟事件() {
            try {
            } catch (Exception ex) {
                Console.WriteLine("数据库-600刷新库名___" + ex.Message + "___" + ex.StackTrace);
            }
        }
        #endregion
    }
    public class C_aa矩阵地图 {
        static C_aa矩阵地图 ooo;
        public static C_aa矩阵地图 Ooo {
            get {
                if (ooo == null) {
                    ooo = new C_aa矩阵地图();
                }
                return C_aa矩阵地图.ooo;
            }
        }
        public Dictionary<ushort, Dictionary<ushort, Dictionary<long, C_aa玩家>>> o_玩家集合地图 = new Dictionary<ushort, Dictionary<ushort, Dictionary<long,C_aa玩家>>>();
        public void S_添加玩家(C_aa玩家 z玩家) {
            if (o_玩家集合地图.ContainsKey(z玩家.o_x经度0) == false) {
                o_玩家集合地图[z玩家.o_x经度0] = new Dictionary<ushort, Dictionary<long,C_aa玩家>>();
            }
            if (o_玩家集合地图[z玩家.o_x经度0].ContainsKey(z玩家.o_y纬度0) == false) {
                o_玩家集合地图[z玩家.o_x经度0][z玩家.o_y纬度0] = new Dictionary<long,C_aa玩家>();
            }
            if (o_玩家集合地图[z玩家.o_x经度0][z玩家.o_y纬度0].ContainsKey(z玩家.o_ID) == false) {
                o_玩家集合地图[z玩家.o_x经度0][z玩家.o_y纬度0].Add(z玩家.o_ID, z玩家);
                Console.WriteLine("添加**************  附近的人_____" + z玩家.o_x经度0 + "____" + z玩家.o_y纬度0 + "________" + o_玩家集合地图[z玩家.o_x经度0][z玩家.o_y纬度0].Count);
            }
        }
        public void S_30秒_玩家更换位置() {
            Dictionary<ushort, ushort> z列表 = new Dictionary<ushort, ushort>();
            List<C_aa玩家> z玩家更换位置列表 = new List<C_aa玩家>();
            foreach (var n in o_玩家集合地图) {
                foreach (var m in n.Value) {
                    foreach (var o in m.Value) {
                         if (o.Value.o_x经度0 != n.Key || o.Value.o_y纬度0 != m.Key) {
                            z玩家更换位置列表.Add(o.Value);
                        }
                    }
                }
            }
            foreach (var n in z玩家更换位置列表) {
                if (o_玩家集合地图.ContainsKey(n.o_x经度0) && o_玩家集合地图[n.o_x经度0].ContainsKey(n.o_y纬度0) && o_玩家集合地图[n.o_x经度0][n.o_y纬度0].ContainsKey(n.o_ID)) {
                    o_玩家集合地图[n.o_x经度0][n.o_y纬度0].Remove(n.o_ID);
                    if (o_玩家集合地图[n.o_x经度0][n.o_y纬度0].Count == 0) {
                        o_玩家集合地图[n.o_x经度0].Remove(n.o_y纬度0);
                    }
                    if (o_玩家集合地图[n.o_x经度0].Count == 0) {
                        o_玩家集合地图.Remove(n.o_x经度0);
                    }
                }
            }
            for (int i = 0; i < z玩家更换位置列表.Count; i++) {
                S_添加玩家(z玩家更换位置列表[i]);
            }
        }
        public string S_附近的人(ushort zx经度0, ushort zy纬度0, string z标准消息) {
            List<C_aa玩家> z玩家列表 = C_aa矩阵地图.Ooo.S_获取附近的人(zx经度0, zy纬度0, true);
            string ss2 = "";
            for (int i = 0; i < z玩家列表.Count; i++) {
                z玩家列表[i].o_通信器.S_发送消息(850, z标准消息);
                if (i < 100) {
                    //ss2 += string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}@", z玩家列表[i].o_ID, z玩家列表[i].o_姓名, z玩家列表[i].o_头像, C_网格Serve.o_本机IP, z玩家列表[i].o_战斗力, z玩家列表[i].o_x经度, z玩家列表[i].o_y纬度);
                }
            }
            return ss2;
        }
        public List<C_aa玩家> S_获取附近的人(ushort xff, ushort yff, bool z是否包含周围) {
            List<C_aa玩家> z玩家列表 = new List<C_aa玩家>();
            if (o_玩家集合地图.ContainsKey((ushort)xff) && o_玩家集合地图[(ushort)xff].ContainsKey((ushort)yff)) {
                z玩家列表.AddRange(o_玩家集合地图[(ushort)xff][(ushort)yff].Values.ToList());
                if (z是否包含周围 == true) {
                    z玩家列表.AddRange(S_获取附近的人((ushort)(xff + 1), (ushort)(yff - 1), false));
                    z玩家列表.AddRange(S_获取附近的人((ushort)(xff + 1), yff, false));
                    z玩家列表.AddRange(S_获取附近的人((ushort)(xff + 1), (ushort)(yff + 1), false));
                    z玩家列表.AddRange(S_获取附近的人(xff, (ushort)(yff - 1), false));
                    z玩家列表.AddRange(S_获取附近的人(xff, (ushort)(yff + 1), false));
                    z玩家列表.AddRange(S_获取附近的人((ushort)(xff - 1), (ushort)(yff - 1), false));
                    z玩家列表.AddRange(S_获取附近的人((ushort)(xff - 1), yff, false));
                    z玩家列表.AddRange(S_获取附近的人((ushort)(xff - 1), (ushort)(yff + 1), false));
                }
            }
            return z玩家列表;
        }
    }
    public class C_aa玩家 : C_TcpClient2.C_逻辑端 {
        public long o_ID;
        public ushort o_x经度0, o_y纬度0;
        public string o_消息内容;
    }
}
