using CC_ServerDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace CC_水果大作战Server_9020_9018 {
    public enum E_红蓝方阵 {
        e60_蓝方 = 60,
        e61_红方 = 61,
        e62_中立 = 62,
    }
    public enum ET {
        e_0020管理器心跳 = 20,
        e_0101群组心跳 = 101,
        e_0116_网格群发 = 116,//-0雇主服务器IP-&-1雇主玩家ID-&-2消息类型-0&0--#3---msg消息内容--
        e_0120_网格群发_发送给目标玩家 = 120,//-0雇主服务器IP-&-1雇主玩家ID-&-2目标服务器IP-&-3目标玩家ID-&-4消息类型-&0&0--#3---msg消息内容
        //e_0125_网格群发_发送给附近的人=125,
        e_0130_网格群发_玩家群发=130,
        e_0310玩家心跳 = 310,
        e_0350登陆数据服务器 = 350,
        e_0360校验数据 = 360,
        e_0380充值 = 380,
        e_0401玩家数据 = 401,
        e_0500单人匹配 = 500,
        e_0501组队匹配 = 501,
        e_0503取消匹配 = 503,
        e_0505进入选择英雄界面 = 505,
        e_0508_请求创建组队房间=508,
        e_0510_请求加入队伍 = 510,
        e_0512同意加入队伍 = 512,
        e_0514收到队伍消息 = 514,
        e_0516退出或解散队伍 = 516,
        e_0522_断线重连 = 522,
        e_0526解散房间 = 526,
        e_0530指定服务器玩家 = 530,
        e_0540强制退出游戏 = 540,
        e_0550投降信息 = 550,
        e_0560游戏房间结束 = 560,
        e_0610玩家信息 = 610,
        e_0630玩家选择武器英雄 = 630,
        e_0631玩家确定选择武器英雄 = 631,
        e_0633玩家选择技能 = 633,
        e_0640进入场景 = 640,
        e_0810_申请添加好友 = 810,
        e_0811_回复添加好友 = 811,
        e_0813_收到好友消息 = 813,
        e_0815_发送好友是否在线 = 815,
        e_0817_回复好友是否在线=817,
        e_0850_附近的玩家=850,
        e_1065批量创建水果 = 1065,
        e_1070复活 = 1070,
        e_1080角色死亡 = 1080,
    }
    public class C_Main {
        public static C_Main ooo = new C_Main();
        public C_UdpClient o_Udp通信;
        static void Main(string[] sss) {
            C_aa数据库.Ooo.S_10分钟刷新库名();
            C_网格Serve.S_启动监听(9020);
            C_TcpClient2.S_启动监听(9018, new C_aa玩家());
            ooo.o_Udp通信 = C_UdpClient.S_启动监听(9018, C_Udp管理器.S_接口_消息处理);
            C_网格管理Serve.S_初始化();  
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
                Console.ReadKey();
                Console.WriteLine("水果大作战Server_9002___请勿点击---------");
            }
        }
        #region MyRegion--计时事件------------------
        static void S_5秒事件() {
            try {
                C_aa游戏房间.S_5秒_重置Serv玩家();
            } catch (Exception ex) {
                Console.WriteLine("出错___" + ex.Message + "___" + ex.StackTrace);
            }
        }
        static void S_10秒事件() {
            Console.WriteLine("S_10秒事件---------");
            try {
                C_aa匹配房间.S_10秒_匹配刷新();
            } catch (Exception ex) {
                Console.WriteLine("游戏房间-10匹配___" + ex.Message + "___" + ex.StackTrace);
            }
            try {
                C_aa游戏房间.S_10秒_清楚房间();
            } catch (Exception ex) {
                Console.WriteLine("游戏房间-10清除___" + ex.Message + "___" + ex.StackTrace);
            }
        }
        static void S_20秒事件() {
            //Console.WriteLine("S_20秒事件---------");
        }
        static void S_30秒事件() {
            try { 
                C_aa玩家.S_30秒_清楚离线玩家();
            } catch (Exception ex) {
                Console.WriteLine("玩家-30清除___" + ex.Message + "___" + ex.StackTrace);
            }
            try {
                C_网格Serve.S_30秒_清除Ser列表();
            } catch (Exception ex) {
                Console.WriteLine("网格-30清除___" + ex.Message + "___" + ex.StackTrace);
            }
            try {
                C_aa矩阵地图.Ooo.S_30秒_清除刷新();
            } catch (Exception ex) {
                Console.WriteLine("网格-30清除___" + ex.Message + "___" + ex.StackTrace);
            }
            try {
                C_网格Serve.S_30秒_发送心跳();
            } catch (Exception ex) {
                Console.WriteLine("网格-30心跳___" + ex.Message + "___" + ex.StackTrace);
            }
            try {
                C_网格管理Serve.S_30秒_发送心跳();
            } catch (Exception ex) {
                Console.WriteLine("网格管理器-30心跳___" + ex.Message + "___" + ex.StackTrace);
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
                C_aa数据库.Ooo.S_10分钟刷新库名();
            } catch (Exception ex) {
                Console.WriteLine("数据库-600刷新库名___" + ex.Message + "___" + ex.StackTrace);
            }
        }
        #endregion
    }
    public class C_网格管理Serve:C_TcpClient2.C_逻辑端 {
        public static C_网格管理Serve ooo;
        static string o_域名IP;
        public static void S_初始化() {
            if (ooo == null) {
                try {
                    IPHostEntry z解析域名 = Dns.GetHostEntry(@"www.aaa9000.com");
                    IPAddress[] zip组 = z解析域名.AddressList;
                    string z域名IP = zip组[0].ToString();
                    o_域名IP = "192.168.1.59";//z局域网-----------
                    o_域名IP = "47.100.29.122";//z公司购买的阿里云服务器--
                    Console.WriteLine("o_域名IP:____________" + o_域名IP);
                    S_主动链接();
                } catch (Exception ex) {
                    Console.WriteLine("z获取IP地址出错:" + ex.Message+"___"+ex.StackTrace);
                }
            }
        }
        static void S_主动链接() {
            Console.WriteLine("网格管理====发送主动链接==网格管理Ser------------" + o_域名IP);
            new C_TcpClient2(o_域名IP, 9023, 0, new C_网格管理Serve(), delegate(C_TcpClient2.C_逻辑端 z逻辑端) {
                Console.WriteLine("网格管理---主动链接网格_________");
                z逻辑端.o_通信器.S_发送消息(20, C_网格Serve.o_本机服务器数据);
                ooo = z逻辑端 as C_网格管理Serve;
            });
        }
        public static void S_30秒_发送心跳() {//z数据用户数量--z等待匹配数量--z在线玩家数量-----
             ooo.o_通信器.S_发送消息(20, C_网格Serve.o_本机服务器数据);
        }
        public override void S_接口_消息处理(ushort z消息类型, string msg, byte[] zData) {
            string ss = "";
            string[] sss, sss0;
            switch ((ET)z消息类型) {
                case ET.e_0020管理器心跳:
                    Console.WriteLine("网格管理----收到网格地址列表____" + msg);
                    sss = msg.Split('@');
                    sss0 = sss[0].Split('&');
                    int z分割位置 = int.Parse(sss[1]);
                    C_网格Serve.o_本机IP = sss0[z分割位置];
                    int z当期时间 = C_Toot.S_Get时间搓int();
                    for (int i = 0; i < sss0.Length; i++) {
                        if (i < z分割位置) {
                            if (sss0[i] == C_网格Serve.o_本机IP) {
                                continue;
                            }
                            try {
                                if (C_网格Serve.o_网格列表.ContainsKey(sss0[i]) == false) {
                                    C_网格Serve.S_主动链接(sss0[i]);
                                }
                            } catch (Exception ex) {
                                Console.WriteLine("网格管理----链接出错__" + ex.Message+"___"+ex.StackTrace);
                            }
                        }
                    }
                    break;
            }
        }
    }
    public class C_网格Serve:C_TcpClient2.C_逻辑端 {
        public int o_在线时间, o_用户数量, o_匹配的用户数量, o_在线玩家数量;
        public static Dictionary<string, C_网格Serve> o_网格列表 = new Dictionary<string, C_网格Serve>();
        public static string o_本机服务器数据 = "0&0&0",o_本机IP;
        public static void S_启动监听(int z端口) {
            C_TcpClient2.S_启动监听(z端口, new C_网格Serve());
        }
        public static void S_主动链接(string zIP) {
            new C_TcpClient2(zIP, 9020, 6, new C_网格Serve(), delegate(C_TcpClient2.C_逻辑端 mm) {
                //Console.WriteLine("网格---主动链接网格_________" + zIP);
                mm.o_通信器.S_发送消息((int)ET.e_0101群组心跳, o_本机服务器数据);
                (mm as C_网格Serve).o_在线时间 = C_Toot.S_Get时间搓int();
            });
        }
        public static void S_30秒_清除Ser列表() {
            int z当前时间 = C_Toot.S_Get时间搓int();
            List<string> z清除列表 = new List<string>();
            foreach (var n in o_网格列表) {
                int z时间差 = z当前时间 - n.Value.o_在线时间;
                Console.WriteLine("网格---------清除网格____" + n.Key + "___时间差:" + z时间差);
                if (z时间差 > 62) {
                    z清除列表.Add(n.Key);
                    n.Value.o_通信器.S_关闭();
                }
            }
            for (int i = 0; i < z清除列表.Count; i++) {
                o_网格列表.Remove(z清除列表[i]);
            }
            Dictionary<string, C_网格Serve> z在线玩家列表 = o_网格列表.Where(n => n.Key != n.Value.o_通信器.o_IP端口.O_IP).ToDictionary(n => n.Key, n => n.Value);
            foreach (var n in z在线玩家列表) {
                o_网格列表.Remove(n.Key);
                o_网格列表[n.Value.o_通信器.o_IP端口.O_IP] = n.Value;
            }
        }   
        public static void S_30秒_发送心跳() {
            Console.WriteLine("网格================网格数-------------------" + o_网格列表.Count);
            o_本机服务器数据 = C_aa数据库.Ooo.o_玩家库名集合.Count + "&" + (C_aa匹配房间.o_单人匹配列表.Count + C_aa匹配房间.o_团队匹配列表.Count) + "&" + C_aa玩家.o_在线玩家列表.Count;
            foreach (var n in o_网格列表) {
                try {
                    n.Value.o_通信器.S_发送消息((int)ET.e_0101群组心跳, o_本机服务器数据);
                    //Console.WriteLine("网格------------------发送网格心跳-------------------" + n.Key);
                } catch (Exception ex) {
                    Console.WriteLine("出错___" + ex.Message + "___" + ex.StackTrace);
                }
            }
        }
        public override void S_接口_消息处理(ushort z消息类型, string zMsg, byte[] zData) {
            if (o_网格列表.ContainsKey(o_通信器.o_IP端口.O_IP) == false) {
                o_网格列表[o_通信器.o_IP端口.O_IP] = this;
                o_通信器.S_发送消息((int)ET.e_0101群组心跳, o_本机服务器数据);
                //Console.WriteLine("网格-----添加新网格--" + o_通信器.o_IP端口.O_String);
            }
            //Console.WriteLine("网格-------" + z消息类型+"____" + zMsg);
            o_在线时间 = C_Toot.S_Get时间搓int();
            string[] sss,sss0,sss1,sssA;
            string ss2, ss;
            long zID,z目标ID;
            ushort z消息类型0;
            switch ((ET)z消息类型) {
                case ET.e_0101群组心跳:
                    sss = zMsg.Split('&');
                    o_用户数量 = int.Parse(sss[0]);
                    o_匹配的用户数量 = int.Parse(sss[1]);
                    o_在线玩家数量 = int.Parse(sss[2]);
                    //Console.WriteLine("网格------------------收到-网格心跳_______" + o_通信器.o_IP端口.O_String);
                    break;
                case ET.e_0116_网格群发://-0雇主服务器IP-&-1雇主玩家ID-&-2消息类型-0&0--#3---msg消息内容--
                    Console.WriteLine("网格收到--116网格群搜索-------------" + zMsg);
                    sssA = zMsg.Split(new string[] { "#3" }, StringSplitOptions.RemoveEmptyEntries);// 
                    sss = zMsg.Split('&');
                    z消息类型0 = ushort.Parse(sss[2]);
                    switch ((ET)z消息类型0) {
                        #region MyRegion
                        case ET.e_0815_发送好友是否在线:
                            #region MyRegion
                            //sss1 = sssA[1].Split('@');
                            //ss = "";
                            //for (int i = 0; i < sss1.Length; i++) {//z将在线好友聚集起来--z发送到定向接口---------
                            //    //sss0 = sss1[i].Split('&');
                            //    zID = long.Parse(sss1[i]);
                            //    //--0zID,1z姓名,2z头像,3z服务器IP-4z战斗力-5zX经度--6zY纬度
                            //    if (C_aa玩家.o_在线玩家列表.ContainsKey(zID) && C_aa数据库.Ooo.o_玩家库名集合.ContainsKey(zID)) {
                            //        C_aa玩家 nn = C_aa玩家.o_在线玩家列表[zID];
                            //        ss += string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}@", zID, nn.o_姓名, nn.o_头像, C_网格Serve.o_本机IP, nn.o_战斗力, nn.o_x经度, nn.o_y纬度);//z在线--
                            //    }
                            //}
                            //if (ss.Length > 3) {
                            //    ss = ss.Remove(ss.Length - 1, 1);
                            //    o_通信器.S_发送消息((int)ET.e_0120_网格群发_发送给目标玩家, string.Format("0&0&{0}&{1}&{2}&0&0#3{3}", sss[0], sss[1], (int)ET.e_0817_回复好友是否在线, ss));
                            //}
                            string[] zIDIP列表 = sssA[2].Split('@');
                            ss=  C_aa玩家.S_好友_相互通信(zIDIP列表, long.Parse(sss[1]), sssA[1], false, false);
                            if (ss.Length > 3) {
                                //ss = ss.Remove(ss.Length - 1, 1);
                                o_通信器.S_发送消息((int)ET.e_0120_网格群发_发送给目标玩家, string.Format("0&0&{0}&{1}&{2}&0&0#3{3}", sss[0], sss[1], (int)ET.e_0817_回复好友是否在线, ss));
                            }
                            #endregion
                            break;
                        case ET.e_0850_附近的玩家://--0zID,1z姓名,2z头像,3z服务器IP-4z战斗力-5zX经度--6zY纬度
                            sss1 = sssA[1].Split('&');
                            try {
                                ushort zx经度0 = (ushort)(float.Parse(sss1[5])*100);
                                ushort zy纬度0 = (ushort)(float.Parse(sss1[6])*100);
                                ss=C_aa矩阵地图.Ooo.S_附近的人(zx经度0, zy纬度0, sssA[1]);
                                //ss = C_aa矩阵地图.Ooo.S_获取附近的好友(ushort.Parse(sss1[0]), ushort.Parse(sss1[1]));
                                Console.WriteLine(sss[1] + "___www获取附近的人___" + ss);
                                if (ss.Length > 5) {
                                    o_通信器.S_发送消息((int)ET.e_0120_网格群发_发送给目标玩家, string.Format("0&0&{0}&{1}&{2}&0&0#3{3}", sss[0], sss[1], (int)ET.e_0850_附近的玩家, ss));
                                }
                            } catch (Exception ex) {
                                Console.WriteLine("S_获取附近的好友____"+ex.Message+"___"+ex.StackTrace);
                            }
                            break; 
                        #endregion
                    }
                    break;
                case ET.e_0120_网格群发_发送给目标玩家:
                    #region MyRegion
		            Console.WriteLine("网格收到--120指定玩家-------------" + zMsg);
                    //-0雇主服务器IP-&-1雇主玩家ID-&-2B服务器IP-&-3B玩家ID-&-4消息类型-&0&0--#3---msg消息内容
                    //sss=msg.Split(new string[] { "#3" }, StringSplitOptions.RemoveEmptyEntries);//      
                    sss = zMsg.Split('&');
                    z目标ID = long.Parse(sss[3]);
                    z消息类型0 = ushort.Parse(sss[4]);
                    if (C_aa玩家.o_在线玩家列表.ContainsKey(z目标ID)) {
                        C_aa玩家.o_在线玩家列表[z目标ID].S_接口_消息处理(z消息类型0, zMsg, zData);
                    } 
	                #endregion
                    break;
                //case ET.e_0125_网格群发_发送给附近的人:
                //    #region MyRegion
                //    sssA = zMsg.Split(new string[] { "#3" }, StringSplitOptions.RemoveEmptyEntries);// 
                //    sss = zMsg.Split('&');
                //    ushort zx经度0 = ushort.Parse(sss[0]);
                //    ushort zy纬度0 = ushort.Parse(sss[1]);
                //    z消息类型0 = ushort.Parse(sss[2]);
                //    List<C_aa玩家> z玩家列表 = C_aa矩阵地图.Ooo.S_获取附近的人(zx经度0, zy纬度0, true);
                //    for (int i = 0; i < z玩家列表.Count; i++) {
                //        try {
                //            z玩家列表[i].o_通信器.S_发送消息(z消息类型, sssA[1]);
                //        } catch (Exception ex) {
                //            Console.WriteLine("125发送出错__" + ex.Message + "____" + ex.StackTrace);
                //        }
                //    } 
                //    #endregion
                //    break;
                case ET.e_0130_网格群发_玩家群发:
                    #region MyRegion
                    //z消息类型 &0&0 #3 msg消息内容
                    sss = zMsg.Split('&');
                    z消息类型0 = ushort.Parse(sss[0]);
                    foreach (var n in C_aa玩家.o_在线玩家列表.Values) {
                        //n.S_发送消息((ET)z消息类型0, zMsg);
                        n.S_接口_消息处理(z消息类型0, zMsg, zData);
                    } 
                    #endregion
                    break;
            }
        }
        public static void S_116_网格群发(long z雇主玩家ID, ET z消息类型, string z消息内容) {
            //-0雇主服务器IP-&-1雇主玩家ID-&-2消息类型-0&0--#3---msg消息内容--
            //Console.WriteLine("网格搜索___" + z消息类型 + "___" + z消息内容);
            foreach (var n in o_网格列表) {
                n.Value.o_通信器.S_发送消息((int)ET.e_0116_网格群发, string.Format("{0}&{1}&{2}&0&0#3{3}", o_本机IP, z雇主玩家ID, (int)z消息类型, z消息内容));
            }
        }
        public static void S_120_网格群发_发送给目标玩家(long z雇主玩家ID, long z目标玩家ID, ET z消息类型, string z消息内容) {
            //-0雇主服务器IP-&-1雇主玩家ID-&-2目标服务器IP-&-3目标玩家ID-&-4消息类型---#3---msg消息内容--
            //Console.WriteLine("网格发送___" + z消息类型 + "___" + z消息内容);
            foreach (var n in o_网格列表) {
                n.Value.o_通信器.S_发送消息((int)ET.e_0120_网格群发_发送给目标玩家, string.Format("{0}&{1}&{2}&{3}&{4}&0&0#3{5}", o_本机IP, z雇主玩家ID, n.Key, z目标玩家ID, (int)z消息类型, z消息内容));
            }
        }
        //public static void S_125_网格群发_发送给附近的人(ushort zx经度0, ushort zy纬度0, ET z消息类型, string z消息内容) {
        //    foreach (var n in o_网格列表) {
        //        n.Value.o_通信器.S_发送消息((int)ET.e_0125_网格群发_发送给附近的人, string.Format("{0}&{1}&{2}&0&0#3{3}", zx经度0, zy纬度0,(int)z消息类型, z消息内容));
        //    }
        //}
        public static void S_130_网格群发_玩家群发(ushort z消息类型,string z消息内容) {
            string ss = z消息类型 + "&0&0#3" + z消息内容;
            foreach(var n in o_网格列表.Values){
                n.o_通信器.S_发送消息((int)ET.e_0130_网格群发_玩家群发, ss);
            }
        }
    }
    public class C_aa玩家:C_TcpClient2.C_逻辑端 {
        public long o_ID;
        public int o_在线时间;
        public float o_x经度,o_y纬度;
        public C_aa游戏房间 o_游戏房间;
        public IPEndPoint o_Ucp地址;
        public bool o_是否可被清楚;
        public string o_姓名;
        public byte o_玩家等级, o_玩家位置;
        public ushort o_头像, o_战斗力, o_x经度0, o_y纬度0;
        public static Dictionary<long, C_aa玩家> o_在线玩家列表 = new Dictionary<long, C_aa玩家>();//z玩家ID-玩家-------
        public C_aa玩家() {}
        public C_aa玩家(long zID) {
            o_ID = zID;
            C_aa玩家.o_在线玩家列表[zID] = this;
            // Console.WriteLine("临时新用户_____________" + zID);
        }
        static List<C_aa玩家> o_电脑玩家列表;
        public static List<C_aa玩家> O_电脑玩家列表 {
            get {
                if (o_电脑玩家列表 == null) {
                    o_电脑玩家列表 = new List<C_aa玩家>();
                    for (int i = 0; i < 30; i++) {
                        o_电脑玩家列表.Add(new C_aa玩家(i));
                    }
                }
                return C_aa玩家.o_电脑玩家列表;
            }
        }
        bool S_列表_重叠校验(long zID) {
            bool z是否相同 = true;
            if (o_在线玩家列表.ContainsKey(zID)) {
                C_aa玩家 mm = o_在线玩家列表[zID];
                if(Object.ReferenceEquals(this,mm)==false) {
                    o_Ucp地址 = mm.o_Ucp地址;
                    o_玩家等级 = mm.o_玩家等级;
                    o_游戏房间 = mm.o_游戏房间;
                    o_玩家位置 = mm.o_玩家位置;
                    Console.WriteLine(this.GetHashCode()+"______"+mm.GetHashCode()+"________________rRRRR_____________" + mm.o_通信器.o_IP端口.O_String);
                    mm.o_通信器.S_关闭();
                    if (mm.o_游戏房间 != null) {
                        mm.o_游戏房间.o_玩家列表[mm.o_玩家位置] = this;
                    }
                    if (C_aa数据库.Ooo.o_玩家库名集合.ContainsKey(o_ID)) {
                        try {
                            C_aa矩阵地图.Ooo.o_玩家集合地图[mm.o_x经度0][mm.o_y纬度0].Remove(mm);
                            C_aa矩阵地图.Ooo.o_玩家集合地图[o_x经度0][o_y纬度0].Add(this);
                        } catch (Exception ex) {
                            C_Debug.S_Add("矩阵地图_移除出错_", ex);
                        }
                    }
                    //mm.o_通信器 = o_通信器;
                    z是否相同 = false;
                }
            } else {
                z是否相同 = false;
            }
            o_在线玩家列表[zID] = this;
            return z是否相同;
        }
        public static string S_好友_相互通信(string[] zIDIP列表, long zID, string z标准数据, bool z是否第1用户, bool z找不到的好友是否继续搜索) {
            string ss = "";
            if (zIDIP列表[0].Length < 3) {
                return "0";
            }
            Dictionary<string, List<string>> z远程访问列表 = new Dictionary<string, List<string>>();
            for (int i = 0; i < zIDIP列表.Length; i++) {
                string[] sss = zIDIP列表[i].Split('&');
                long z目标ID = long.Parse(sss[0]);
                //--0zID,1z姓名,2z头像,3z服务器IP-4z战斗力-5zX经度--6zY纬度
                if (C_aa玩家.o_在线玩家列表.ContainsKey(z目标ID) && C_aa数据库.Ooo.o_玩家库名集合.ContainsKey(z目标ID)) {
                    C_aa玩家 nn = C_aa玩家.o_在线玩家列表[z目标ID];
                    ss += string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}@", z目标ID, nn.o_姓名, nn.o_头像, C_网格Serve.o_本机IP, nn.o_战斗力, nn.o_x经度, nn.o_y纬度);//z在线--
                    nn.S_发送消息(ET.e_0817_回复好友是否在线, z标准数据);
                } else  if (z是否第1用户){
                    //ss2 += sss[i]+"@";
                    if (z远程访问列表.ContainsKey(sss[1]) == false) {
                        z远程访问列表[sss[1]] = new List<string>();
                    }
                    z远程访问列表[sss[1]].Add(zIDIP列表[i]);
                }
            }
            if (z是否第1用户 && z远程访问列表.Count > 0){//--z不在线好友去其他服务器搜索---
                foreach (var n in z远程访问列表) {
                    string z远程IDIP表 = "";
                    for (int i = 0; i < n.Value.Count; i++) {
                        if (i == 0) {
                            z远程IDIP表 += n.Value[i];
                        } else {
                            z远程IDIP表 += "@" + n.Value[i];
                        }
                    }
                    if (C_网格Serve.o_网格列表.ContainsKey(n.Key)) {
                        C_网格Serve.o_网格列表[n.Key].o_通信器.S_发送消息(116, string.Format("{0}&{1}&{2}&0&0#3{3}#3{4}", C_网格Serve.o_本机IP, zID, 815,z标准数据,z远程IDIP表));
                    } else if(z找不到的好友是否继续搜索){
                        C_网格Serve.S_116_网格群发(zID, ET.e_0815_发送好友是否在线,z标准数据+"#3"+ z远程IDIP表);
                    }
                }
            }
            return ss;
        }
        static string S_附近的人_互相通信(C_aa玩家 nn) {
            string z标准消息 = string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}", nn.o_ID, nn.o_姓名, nn.o_头像, C_网格Serve.o_本机IP, nn.o_战斗力, nn.o_x经度, nn.o_y纬度);
            if (nn.o_x经度0 != 0) {
                string ss= C_aa矩阵地图.Ooo.S_附近的人(nn.o_x经度0, nn.o_y纬度0, z标准消息);
                nn.S_发送消息(ET.e_0850_附近的玩家, ss);
                C_网格Serve.S_116_网格群发(nn.o_ID, ET.e_0850_附近的玩家, z标准消息);
            }
            return z标准消息;
        }
        public static void S_30秒_清楚离线玩家() {
            List<C_aa玩家> z清楚列表 = new List<C_aa玩家>();
            int z当前时间 = C_Toot.S_Get时间搓int();
            foreach (var n in o_在线玩家列表) {
                if (n.Key > 300) {
                    if (n.Value.o_通信器 != null) {//z在游戏房间中的用户是无法掉线的---只能是通信断开--
                        if (z当前时间 - n.Value.o_在线时间 > 40 && n.Value.o_游戏房间 == null) {                 
                            z清楚列表.Add(n.Value);
                        }
                    } else {
                        z清楚列表.Add(n.Value);
                    }
                }
            }
            for (int i = 0; i < z清楚列表.Count; i++) {
                C_aa玩家 mm = o_在线玩家列表[z清楚列表[i].o_ID];
                mm.o_是否可被清楚 = true;
                if (mm.o_Ucp地址 != null && C_Udp管理器.o_地址玩家列表.ContainsKey(mm.o_Ucp地址)){
                    C_Udp管理器.o_地址玩家列表.Remove(mm.o_Ucp地址);
                }
                try {
                    mm.o_通信器.S_关闭();
                } catch (Exception ex) {
                    Console.WriteLine("出错___" + ex.Message + "___" + ex.StackTrace);
                }
                o_在线玩家列表.Remove(z清楚列表[i].o_ID);
                //Console.WriteLine("玩家__" + z清楚列表[i] + "______被清除_______");
            }
            Dictionary<long, C_aa玩家> z在线玩家列表 = o_在线玩家列表.Where(n => n.Key != n.Value.o_ID).ToDictionary(n => n.Key, n => n.Value);
            foreach (var n in z在线玩家列表) {
                o_在线玩家列表.Remove(n.Key);
                o_在线玩家列表[n.Value.o_ID] = n.Value;
            }
        }
        public override void S_接口_消息处理(ushort z消息类型, string zMsg, byte[] zData) {
            Console.WriteLine("玩家消息__" + (ET)z消息类型 + "___" + zMsg);
            string ss = "";
            string[] sss, sss0, sss1, sss2, sssA;
            long z目标ID,zID;
            switch ((ET)z消息类型) {
                case ET.e_0310玩家心跳:
                    #region MyRegion------收到的域名Server消息-----心跳回应-------------
                    S_发送消息(ET.e_0310玩家心跳, "0");
                    o_在线时间 = C_Toot.S_Get时间搓int();
                    sss1 = zMsg.Split('@');
                    sss = sss1[0].Split('&');
                    o_是否可被清楚 = false;
                    o_ID = long.Parse(sss[0]);
                    try {
                        o_姓名 = sss[1];
                        o_头像 = ushort.Parse(sss[2]);
                        o_战斗力 = ushort.Parse(sss[3]);
                        o_x经度 = float.Parse(sss[4]);
                        o_y纬度 = float.Parse(sss[5]);
                        o_x经度0 = (ushort)(o_x经度 * 100);
                        o_y纬度0 = (ushort)(o_y纬度 * 100);
                        if (o_ID != 10000&&C_aa数据库.Ooo.o_玩家库名集合.ContainsKey(o_ID)) {
                            C_aa矩阵地图.Ooo.S_添加玩家(this);
                        }
                    } catch (Exception ex) {
                        Console.WriteLine("基础消息解析出错___" + ex.Message + "___" + ex.StackTrace);
                    }
                    if (S_列表_重叠校验(o_ID)) {
                        S_附近的人_互相通信(this);
                    }
                    if (sss1.Length > 2) {
                        if (o_游戏房间 != null) {
                            o_游戏房间.S_竞技数据上传到房间(sss1);
                            foreach (var n in o_游戏房间.o_玩家列表) {
                                if (n.o_ID > 300 && n.o_ID!=o_ID) {
                                    n.S_发送消息(ET.e_0310玩家心跳, zMsg);
                                }
                            }
                        }
                    }
                    #endregion
                    break;
                case ET.e_0350登陆数据服务器://z玩家ID,z姓名,z头像,z战斗力,z精度X,z纬度Y---------------
                    #region MyRegion---------------玩家精确登陆----------
                    Console.WriteLine("玩家精确登陆-----------在线玩家数量__" + C_aa玩家.o_在线玩家列表.Count);
                    sss1 = zMsg.Split('&');
                    o_是否可被清楚 = false;
                    o_ID = long.Parse(sss1[0]);
                    try {
                        o_姓名 = sss1[1];
                        o_头像 = ushort.Parse(sss1[2]);
                        o_战斗力 = ushort.Parse(sss1[3]);
                        o_x经度 = float.Parse(sss1[4]);
                        o_y纬度 = float.Parse(sss1[5]);
                        o_x经度0 = (ushort)(o_x经度 * 100);
                        o_y纬度0 = (ushort)(o_y纬度 * 100);
                    } catch (Exception ex) {
                        Console.WriteLine("基础消息解析出错___" + ex.Message + "___" + ex.StackTrace);
                    }
                    if (C_aa数据库.Ooo.o_玩家库名集合.ContainsKey(o_ID)) {
                        ss = C_aa数据库.Ooo.S_获取玩家数据2(o_ID);
                        S_发送消息(ET.e_0350登陆数据服务器, "1@" + ss);
                    } else {
                        Console.WriteLine("玩家不在数据库中_" + zMsg);
                        if (o_ID == 10000) {
                            Random random = new Random();
                            do {
                                o_ID =9000000+random.Next(90483645);
                            } while (C_aa数据库.Ooo.o_玩家库名集合.ContainsKey(o_ID));
                            Console.WriteLine("玩家___ID==10000__________________生成新ID_____ " + o_ID);
                        }
                        Console.WriteLine("玩家不存在_____注册到数据库_______________________ " + o_ID);
                        C_aa数据库.Ooo.S_用户注册(o_ID);
                        S_发送消息(ET.e_0350登陆数据服务器, o_ID + "");
                    }
                    S_列表_重叠校验(o_ID);
                    o_在线时间 = C_Toot.S_Get时间搓int();
                    S_附近的人_互相通信(this);
                    #endregion
                    break;
                case ET.e_0360校验数据:
                    break;
                case ET.e_0380充值:
                    S_发送消息(ET.e_0380充值, zMsg);
                    break;
                case ET.e_0401玩家数据:
                    Console.WriteLine("e_101玩家数据________ " + zMsg);
                    sss1 = zMsg.Split('@');
                    z目标ID = long.Parse(sss1[0]);
                    C_aa数据库.Ooo.S_保存玩家数据(z目标ID, sss1[1]);
                    break;
                case ET.e_0500单人匹配://----单人匹配-----------------------            
                    S_发送消息(ET.e_0500单人匹配, "0");//进入匹配界面-----------
                    if (C_aa匹配房间.o_单人匹配列表.Contains(this) == false) {
                        C_aa匹配房间.o_单人匹配列表.Add(this);
                    }
                    break;
                case ET.e_0501组队匹配://zID &zID.... @ zID &zID........//z组队匹配--------------
                    #region MyRegion------组队请求5v5匹配--------//zID &zID ........
                    sss = zMsg.Split('&');
                    int z房间号 = int.Parse(sss[0]);
                    if (C_aa匹配房间.o_团队匹配列表.ContainsKey(z房间号)) {
                        C_aa匹配房间 z房间 = C_aa匹配房间.o_团队匹配列表[z房间号];
                        z房间.o_是否开始系统匹配 = true;
                        foreach (var n in z房间.o_房间玩家列表) {
                            n.Key.S_发送消息(ET.e_0500单人匹配, "0");
                        }
                    }
                    #endregion
                    break;
                case ET.e_0503取消匹配://取消匹配等待-------------------
                    #region MyRegion--------取消匹配等待-------------------
                    if (C_aa匹配房间.o_单人匹配列表.Contains(this)) {
                        C_aa匹配房间.o_单人匹配列表.Remove(this);
                        S_发送消息(ET.e_0503取消匹配, "00");
                    } else {
                        foreach (var n in C_aa匹配房间.o_团队匹配列表) {
                            if (n.Value.o_房间玩家列表.ContainsKey(this)) {
                                foreach (var m in n.Value.o_房间玩家列表) {
                                    m.Key.S_发送消息(ET.e_0503取消匹配, "00");
                                }
                            }
                        }
                        //for (int i = 0; i < C_aa匹配房间.o_团队匹配列表.Count; i++) {
                        //    bool z是否存在 = false;
                        //    for (byte r = 0; r < C_aa匹配房间.o_团队匹配列表[i].Count; r++) {
                        //        if (C_aa匹配房间.o_团队匹配列表[i][r].Contains(this) == true) {
                        //            z是否存在 = true;
                        //            break;
                        //        }
                        //    }
                        //    if (z是否存在) {
                        //        for (byte r = 0; r < C_aa匹配房间.o_团队匹配列表[i].Count; r++) {
                        //            List<C_aa玩家> nn = C_aa匹配房间.o_团队匹配列表[i][r];
                        //            for (int x = 0; x < nn.Count; x++) {
                        //                nn[x].S_发送消息(ET.e_0503取消匹配, "00");
                        //            }
                        //        }
                        //        C_aa匹配房间.o_团队匹配列表.RemoveAt(i);
                        //        break;
                        //    }
                        //}
                    }
                    #endregion
                    break;
                case ET.e_0508_请求创建组队房间:
                    C_aa匹配房间 z匹配房间 = new C_aa匹配房间(this);
                    S_发送消息(ET.e_0508_请求创建组队房间, z匹配房间.o_房间号 + "");
                    break;
                case ET.e_0510_请求加入队伍://"z雇主ID,z雇主姓名,z雇主头像,z目标ID,z房间号"--------
                    //-0雇主服务器IP-&-1雇主玩家ID-&-2目标服务器IP-&-3目标玩家ID-&-4消息类型-&0&0--#3---msg消息内容
                    #region MyRegion---z请求玩家加入队伍---------//z房主ID,z房主头像,z房主姓名,z请求ID
                    sssA = zMsg.Split(new string[] { "#3" }, StringSplitOptions.RemoveEmptyEntries);// 
                    if (sssA.Length == 2) {//z网格消息------
                        S_发送消息(ET.e_0510_请求加入队伍, zMsg);
                    }else{
                        sss = zMsg.Split('&');
                        z目标ID = long.Parse(sss[3]);
                        if (C_aa玩家.o_在线玩家列表.ContainsKey(z目标ID) == true) {
                            C_aa玩家.o_在线玩家列表[z目标ID].S_发送消息(ET.e_0510_请求加入队伍, zMsg);
                        } else {//--------z网格群发----------------
                            C_网格Serve.S_120_网格群发_发送给目标玩家(o_ID, z目标ID, ET.e_0510_请求加入队伍, zMsg);
                        }
                    }
                    #endregion
                    break;
                case ET.e_0512同意加入队伍://z房间号 & zID-z姓名-z头像---
                    #region MyRegion-------z玩家同意加入队伍-----
                    sss = zMsg.Split('&');
                    z房间号 = int.Parse(sss[0]);
                    o_ID = long.Parse(sss[1]);
                    S_列表_重叠校验(o_ID);
                    try {
                        o_姓名 = sss[2];
                        o_头像 = ushort.Parse(sss[3]);
                        o_战斗力 = ushort.Parse(sss[4]);
                    } catch (Exception ex) {
                        Console.WriteLine("出错___" + ex.Message + "___" + ex.StackTrace);
                    }
                    if(C_aa匹配房间.o_团队匹配列表.ContainsKey(z房间号)){
                        C_aa匹配房间 z房间 = C_aa匹配房间.o_团队匹配列表[z房间号];
                        z房间.o_房间玩家列表[this] = 0;
                        ss = "-10&0";
                        foreach (var n in z房间.o_房间玩家列表) {
                            //z主ID &,z消息 @ zID,z姓名,z头像,z位置 @zID,z姓名,z头像,z位置@ .....
                            ss += string.Format("@{0}&{1}&{2}&{3}", n.Key.o_ID, n.Key.o_姓名, n.Key.o_头像, n.Value);
                        }
                        foreach (var n in z房间.o_房间玩家列表.Keys) {
                            n.S_发送消息(ET.e_0514收到队伍消息, ss);
                        }
                    } else {
                        Console.WriteLine("尴尬____你同意进入房间了，但是房间不存在 >_<   "+C_Toot.S_当前行号());
                    }
                    #endregion
                    break;
                case ET.e_0514收到队伍消息://z0房间号,z1ID,z2消息类型,z3消息------
                    #region MyRegion---z收到队伍消息---
                    sss = zMsg.Split('&');
                    z房间号 = int.Parse(sss[0]);
                    if (C_aa匹配房间.o_团队匹配列表.ContainsKey(z房间号)) {
                        C_aa匹配房间 z房间 = C_aa匹配房间.o_团队匹配列表[z房间号];
                        zID = long.Parse(sss[1]);
                        if (C_aa玩家.o_在线玩家列表.ContainsKey(zID)) {
                            C_aa玩家 z玩家 = C_aa玩家.o_在线玩家列表[zID];
                            if (z房间.o_房间玩家列表.ContainsKey(z玩家)) {
                                if (int.Parse(sss[2]) == 0) {
                                    ss = -10 + "&" + sss[3];
                                    z房间.o_房间玩家列表[z玩家] = byte.Parse(sss[3]);
                                } else {
                                    ss = sss[1] + "&" + sss[3];
                                }
                                foreach (var n in z房间.o_房间玩家列表) {
                                    //z主ID &,z消息 @ zID,z姓名,z头像,z位置 @zID,z姓名,z头像,z位置@ .....
                                    ss += string.Format("@{0}&{1}&{2}&{3}", n.Key.o_ID, n.Key.o_姓名, n.Key.o_头像, n.Value);
                                }
                                foreach (var n in z房间.o_房间玩家列表.Keys) {
                                    n.S_发送消息(ET.e_0514收到队伍消息, ss);
                                }
                            }
                        }
                    }
                    #endregion
                    break;
                case ET.e_0516退出或解散队伍://退出队伍-------------z房间号 & zID----
                    #region MyRegion----退出队伍----
                    sss = zMsg.Split('&');
                    bool z是否处理失败 = false;
                    z房间号 = int.Parse(sss[0]);
                    if (C_aa匹配房间.o_团队匹配列表.ContainsKey(z房间号)) {
                        C_aa匹配房间 z房间 = C_aa匹配房间.o_团队匹配列表[z房间号];
                        zID = long.Parse(sss[1]);
                        if (C_aa玩家.o_在线玩家列表.ContainsKey(zID)) {
                            C_aa玩家 z玩家 = C_aa玩家.o_在线玩家列表[zID];
                            if (z房间.o_房主 == z玩家) {
                                ss = -10 + "&0@";
                                foreach (var n in z房间.o_房间玩家列表.Keys) {
                                    n.S_发送消息(ET.e_0514收到队伍消息, ss);
                                }
                                C_aa匹配房间.o_团队匹配列表.Remove(z房间.o_房间号);
                            }else{
                                if (z房间.o_房间玩家列表.ContainsKey(z玩家)) {
                                    z房间.o_房间玩家列表.Remove(z玩家);
                                }
                                ss = -10 + "&0";
                                foreach (var n in z房间.o_房间玩家列表) {
                                    //z主ID &,z消息 @ zID,z姓名,z头像,z位置 @zID,z姓名,z头像,z位置@ .....
                                    ss += string.Format("@{0}&{1}&{2}&{3}", n.Key.o_ID, n.Key.o_姓名, n.Key.o_头像, n.Value);
                                }
                                foreach (var n in z房间.o_房间玩家列表.Keys) {
                                    n.S_发送消息(ET.e_0514收到队伍消息, ss);
                                }
                                if (z房间.o_房间玩家列表.Count == 0) {
                                    z房间.o_房间玩家列表.Clear();
                                }
                            }
                        } else {
                            z是否处理失败 = true;
                        }
                    } else {
                        z是否处理失败 = true;
                    }
                    if (z是否处理失败) {
                        S_发送消息(ET.e_0514收到队伍消息, "-10&0@");
                    }
                    #endregion
                    break;
                case ET.e_0522_断线重连://z房间号 z玩家位置 z是否数据网络 @ zID 姓名 头像 z战斗力---
                    #region MyRegion
		            sssA = zMsg.Split('@');
                    sss0 = sssA[0].Split('&');
                    z房间号 = int.Parse(sss0[0]);
                    o_玩家位置 = byte.Parse(sss0[1]);
                    byte z是否数据网络 = byte.Parse(sss0[2]);
                    bool z房间出错 = false;
                    if (o_游戏房间==null&&C_aa游戏房间.o_房间集合.ContainsKey(z房间号)) {
                        o_游戏房间 = C_aa游戏房间.o_房间集合[z房间号];
                    }
                    if (o_游戏房间 != null) {
                        ss = o_游戏房间.S_Get房间数据(o_玩家位置, true);
                        if (ss.Length > 10) {
                            S_发送消息(ET.e_0522_断线重连, ss);                    
                            sss1 = sssA[1].Split('&');
                            try {
                                o_ID = long.Parse(sss1[0]);
                                o_姓名 = sss1[1];
                                o_头像 = ushort.Parse(sss1[2]);
                                o_战斗力 = ushort.Parse(sss1[3]);
                                o_x经度 = float.Parse(sss1[4]);
                                o_y纬度 = float.Parse(sss1[5]);
                                o_x经度0 = (ushort)(o_x经度 * 100);
                                o_y纬度0 = (ushort)(o_y纬度 * 100);
                            } catch (Exception ex) {
                                Console.WriteLine("出错___" + ex.Message + "___" + ex.StackTrace);
                            }
                            S_列表_重叠校验(o_ID);
                        } else {
                            z房间出错 = true;
                        }
                    } else {
                        z房间出错 = true;
                    }
                    if (z房间出错) {
                        S_发送消息(ET.e_0522_断线重连, "0");
                    } 
	                #endregion
                    break;
                case ET.e_0526解散房间://解散房间-----------------
                    #region MyRegion-------解散房间---房间号----
                    z房间号 = int.Parse(zMsg);
                    if (C_aa游戏房间.o_房间集合.ContainsKey(z房间号) == true) {
                        Console.WriteLine("解散房间__" + zMsg);
                        foreach (var n in o_游戏房间.o_玩家列表) {
                            if (n.o_ID > 300) {
                                n.o_通信器.S_发送消息(zData);
                                n.o_游戏房间 = null;
                            }
                        }
                        C_aa游戏房间.o_房间集合.Remove(z房间号);
                    } else {
                        Console.WriteLine("房间__" + zMsg + "_____不存在，无法解散");
                    }
                    #endregion
                    break;
                case ET.e_0540强制退出游戏:
                    Console.WriteLine("-----------------------------------玩家想要强制退出游戏__mm");
                    #region MyRegion
                    S_游戏房间_群发(z消息类型, zData);
                    sss = zMsg.Split('&');
                    int z位置 = int.Parse(sss[0]);
                    if (o_游戏房间 != null) {
                        C_aa玩家 z电脑玩家 = C_aa玩家.O_电脑玩家列表[z位置];
                        o_游戏房间.o_玩家列表[z位置] = z电脑玩家;
                        o_游戏房间.o_玩家数据表[z位置] = zMsg;
                        if (o_游戏房间.o_Serv玩家 == this) {
                            o_游戏房间.o_Serv玩家 = z电脑玩家;
                        }
                        byte z真实玩家 = 0;
                        foreach (var n in o_游戏房间.o_玩家列表) {
                            if (n.o_ID > 300) {
                                z真实玩家++;
                            }
                        }
                        if (z真实玩家 > 0) {
                            o_游戏房间 = null;
                        } else {
                            o_游戏房间.o_房间状态 = 6;
                        }
                    }
                    #endregion
                    break;
                case ET.e_0550投降信息://z投降信息-----
                    S_游戏房间_群发(z消息类型, zData);
                    break;
                case ET.e_0560游戏房间结束://游戏房间号----0==正常游戏/2==匹配不成功(平局)/2==玩家投降认输(玩家位置胜负)/4==boss死亡(玩家位置胜负)/5==游戏结束(数据胜负)/6==强制退出(玩家位置胜负)
                    S_游戏房间_群发(z消息类型, zData);
                    sss1 = zMsg.Split('&');
                    if (o_游戏房间 != null) {
                        o_游戏房间.o_房间状态 = byte.Parse(sss1[1]);
                    }
                    break;
                case ET.e_0610玩家信息://z玩家位置 &z玩家姓名 &z初始金币-----------z收到玩家姓名----------
                    S_游戏房间_群发(z消息类型, zData);
                    break;
                case ET.e_0630玩家选择武器英雄://z收到玩家选择的英雄----
                    S_游戏房间_群发(z消息类型, zData);
                    break;
                case ET.e_0631玩家确定选择武器英雄://z玩家确定选择的英雄----
                    S_游戏房间_群发(z消息类型, zData);
                    break;
                case ET.e_0633玩家选择技能://z玩家选择技能----
                    S_游戏房间_群发(z消息类型, zData);
                    break;
                case ET.e_0640进入场景:
                    S_游戏房间_群发(z消息类型, zData);
                    break;
                case ET.e_0810_申请添加好友:
                    #region MyRegion------申请添加好友-------
                    //-0z申请者ID,1z申请者姓名,2z申请者头像,3z申请者服务器IP-4z战斗力-5zX经度--6zY纬度 @,z目标ID----
                    sssA = zMsg.Split(new string[] { "#3" }, StringSplitOptions.RemoveEmptyEntries);// 
                    if (sssA.Length == 2) {//z网格消息------
                        S_发送消息(ET.e_0810_申请添加好友, zMsg);
                    } else {
                        sss = zMsg.Split('@');//z本地服务器--发来的消息--  
                        z目标ID = long.Parse(sss[1]);
                        if (C_aa玩家.o_在线玩家列表.ContainsKey(z目标ID) == true) {
                            C_aa玩家.o_在线玩家列表[z目标ID].S_发送消息(ET.e_0810_申请添加好友, zMsg);
                        } else {
                            C_网格Serve.S_120_网格群发_发送给目标玩家(o_ID, z目标ID, ET.e_0810_申请添加好友, zMsg);
                        }
                    }      
                    #endregion
                    break;
                case ET.e_0811_回复添加好友://----------------z添加好友------------------
                    //-0雇主服务器IP-&-1雇主玩家ID-&-2目标服务器IP-&-3目标玩家ID-&-4消息类型-&0&0--#3---msg消息内容
                    #region MyRegion------添加好友-----------z申请者ID,z申请者姓名,z申请者头像,@,z目标ID,z目标姓名,z目标头像----
                    sssA = zMsg.Split(new string[] { "#3" }, StringSplitOptions.RemoveEmptyEntries);// 
                    if (sssA.Length == 2) {//z网格消息--  
                        sss = sssA[0].Split('&');
                        long z雇主ID=long.Parse(sss[1]);
                        z目标ID=long.Parse(sss[3]);
                        if (sss[2] == C_网格Serve.o_本机IP) {//z本地服务器--发来的网格消息--  
                            if (C_aa玩家.o_在线玩家列表.ContainsKey(z目标ID) == true) {
                                C_aa玩家.o_在线玩家列表[z目标ID].S_发送消息(ET.e_0811_回复添加好友, zMsg);
                            } else {
                                Console.WriteLine("尴尬___e_0811_回复添加好友__目标网格是本机_本地玩家还不存在__" + C_Toot.S_当前行号());
                            }
                        } else {
                            if (C_网格Serve.o_网格列表.ContainsKey(sss[2])) {
                                C_网格Serve.o_网格列表[sss[2]].o_通信器.S_发送消息((int)ET.e_0120_网格群发_发送给目标玩家, zMsg);
                            } else {
                                Console.WriteLine("尴尬___e_0811_回复添加好友__目标网格不在网格列表中____" + C_Toot.S_当前行号());
                            }
                        }
                    } else {
                        sss = zMsg.Split('@');//z本地服务器--发来的消息--  
                        z目标ID = long.Parse(sss[0]);
                        if (C_aa玩家.o_在线玩家列表.ContainsKey(z目标ID) == true) {
                            C_aa玩家.o_在线玩家列表[z目标ID].S_发送消息(ET.e_0811_回复添加好友, zMsg);
                        } else {
                            Console.WriteLine("尴尬___e_0811_回复添加好友__玩家不在列表中____" + C_Toot.S_当前行号());
                        }
                    } 
                    #endregion
                    break;
                case ET.e_0813_收到好友消息://z收到好友消息--------z发送者ID,z接收者ID,z表情编号,z消息内容------
                    //-0雇主服务器IP-&-1雇主玩家ID-&-2目标服务器IP-&-3目标玩家ID-&-4消息类型-&0&0--#3---msg消息内容
                    #region MyRegion----z收到好友消息--------z发送者ID,z接收者ID,z表情编号,z消息内容------
                    sssA = zMsg.Split(new string[] { "#3" }, StringSplitOptions.RemoveEmptyEntries);// 
                    if (sssA.Length == 2) {//z网格消息--  
                        o_通信器.S_发送消息((int)ET.e_0813_收到好友消息, sssA[1]);
                    } else {
                        sss1 = zMsg.Split('&');
                        z目标ID = long.Parse(sss1[1]);
                        if (C_aa玩家.o_在线玩家列表.ContainsKey(z目标ID) == true) {
                            C_aa玩家.o_在线玩家列表[z目标ID].S_发送消息(ET.e_0813_收到好友消息, zMsg);
                        } else {
                            C_网格Serve.S_120_网格群发_发送给目标玩家(o_ID, z目标ID, ET.e_0813_收到好友消息, zMsg);
                        }
                    }
                    #endregion
                    break;
                case ET.e_0815_发送好友是否在线://z好友是否在线------z好友ID & z服务器IP @ z好友ID & z服务器IP...------------
                    #region MyRegion--z好友是否在线------z好友ID,z是否在心@ ...------------
                    string z标准数据= S_附近的人_互相通信(this);
                    sssA = zMsg.Split(new string[] { "#3" }, StringSplitOptions.RemoveEmptyEntries);// 
                    bool z找不到的好友是否继续搜索=false;
                    string[] zIDIP列表;
                    if (sssA.Length > 1) {
                        z找不到的好友是否继续搜索=true;
                        zIDIP列表 = sssA[1].Split('@');
                    } else {
                        zIDIP列表 = sssA[0].Split('@');
                    }
                    ss = S_好友_相互通信(zIDIP列表, o_ID, z标准数据, true, z找不到的好友是否继续搜索);
                    if (ss.Length > 3) {//--z在线好友直接返回------------
                        //ss = ss.Remove(ss.Length - 1, 1);
                        S_发送消息(ET.e_0817_回复好友是否在线, ss);
                    }
                    //    sss = zMsg.Split('@');
                    //    ss = "";
                    //    Dictionary<string, List<string>> z网格访问列表 = new Dictionary<string, List<string>>();
                    //    for (int i = 0; i < sss.Length; i++) {
                    //        sss0 = sss[i].Split('&');
                    //        z目标ID = long.Parse(sss0[0]);
                    //        //--0zID,1z姓名,2z头像,3z服务器IP-4z战斗力-5zX经度--6zY纬度
                    //        if (C_aa玩家.o_在线玩家列表.ContainsKey(z目标ID) && C_aa数据库.Ooo.o_玩家库名集合.ContainsKey(z目标ID)) {
                    //            C_aa玩家 nn = C_aa玩家.o_在线玩家列表[z目标ID];
                    //            ss += string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}@", z目标ID, nn.o_姓名, nn.o_头像, C_网格Serve.o_本机IP, nn.o_战斗力, nn.o_x经度, nn.o_y纬度);//z在线--
                    //        } else {
                    //            //ss2 += sss[i]+"@";
                    //            if (z网格访问列表.ContainsKey(sss0[1]) == false) {
                    //                z网格访问列表[sss0[1]] = new List<string>();
                    //            }
                    //            z网格访问列表[sss0[1]].Add(sss0[0]);
                    //        }
                    //    }
                    //    if (ss.Length > 3) {//--z在线好友直接返回------------
                    //        ss = ss.Remove(ss.Length - 1, 1);
                    //        S_发送消息(ET.e_0817_回复好友是否在线, ss);
                    //    }
                    //    if (z网格访问列表.Count > 0) {//--z不在线好友去其他服务器搜索---
                    //        foreach (var n in z网格访问列表) {
                    //            string ss2 = "";
                    //            for (int i = 0; i < n.Value.Count; i++) {
                    //                if (i == 0) {
                    //                    ss2 += n.Value[i];
                    //                } else {
                    //                    ss2 += "@" + n.Value[i];
                    //                }
                    //            }
                    //            if (C_网格Serve.o_网格列表.ContainsKey(n.Key)) {
                    //                C_网格Serve.o_网格列表[n.Key].o_通信器.S_发送消息(116, string.Format("{0}&{1}&{2}&0&0#3{3}", C_网格Serve.o_本机IP, o_ID, 815, ss2));
                    //            } else {
                    //                C_网格Serve.S_116_网格群发(o_ID, ET.e_0815_发送好友是否在线, ss2);
                    //            }
                    //        }
                    //    }
                    //}
                    //Console.WriteLine(o_姓名 + "___获取附近的人*****************************  o_x经度==" + o_x经度);
                    //if (o_x经度0 != 0) {
                    //   ss = C_aa矩阵地图.Ooo.S_获取附近的好友(o_x经度0,o_y纬度0);
                    //   Console.WriteLine(o_姓名+"___获取附近的人___" + ss);
                    //   if (ss.Length > 5) {
                    //       S_发送消息(ET.e_0850_附近的玩家,ss);
                    //   }
                    //   C_网格Serve.S_116_网格群发(o_ID, ET.e_0850_附近的玩家, o_x经度0 + "&" + o_y纬度0);
                    //}                  
                
                    #endregion
                    break;
                case ET.e_0817_回复好友是否在线:  //z玩家ID,z服务器IP,z头像,z战斗力---------------
                    //-0雇主服务器IP-&-1雇主玩家ID-&-2目标服务器IP-&-3目标玩家ID-&-4消息类型-&0&0--#3---msg消息内容
                    sssA = zMsg.Split(new string[] { "#3" }, StringSplitOptions.RemoveEmptyEntries);// 
                    S_发送消息(ET.e_0817_回复好友是否在线, sssA[1]);
                    break;
                case ET.e_0850_附近的玩家:
                    //-0雇主服务器IP-&-1雇主玩家ID-&-2目标服务器IP-&-3目标玩家ID-&-4消息类型-&0&0--#3---msg消息内容
                    sssA = zMsg.Split(new string[] { "#3" }, StringSplitOptions.RemoveEmptyEntries);// 
                    S_发送消息(ET.e_0850_附近的玩家, sssA[1]);
                    break;
                case ET.e_1065批量创建水果:
                    #region MyRegion
                    S_游戏房间_群发(z消息类型, zData);
                    if (o_游戏房间 != null) {
                        sss1 = zMsg.Split('@');
                        for (int i = 0; i < sss1.Length; i++) {
                            sss = sss1[i].Split('&');
                            int z序号 = int.Parse(sss[0]);
                            o_游戏房间.o_角色数据列表[z序号] = sss1[i];
                        }
                    }
                    #endregion
                    break;
                case ET.e_1070复活:
                    S_游戏房间_群发(z消息类型, zData);
                    break;
                case ET.e_1080角色死亡:
                    S_游戏房间_群发(z消息类型, zData);
                    break;
            }
        }
        public void S_发送消息(ET z消息类型, string z内容) {
            o_通信器.S_发送消息((ushort)z消息类型, z内容);
        }
        void S_游戏房间_群发(int z消息类型, byte[] zData) {
            if (o_游戏房间 == null){
                Console.WriteLine("游戏房间不行了____________0________o_游戏房间 == null");
                return;
            } else if (o_游戏房间.o_玩家列表.Count == 0) {
                Console.WriteLine("游戏房间不行了____________0________o_游戏房间.o_玩家列表.Count == 0");
                return;
            }
            foreach (var n in o_游戏房间.o_玩家列表) {
                if (n.o_ID > 300) {
                    n.o_通信器.S_发送消息(zData);
                }
            }
        }
        void S_游戏房间_群发(int z消息类型, string msg) {
            if (o_游戏房间 == null) {
                Console.WriteLine("游戏房间不行了____________1________o_游戏房间 == null");
                return;
            } else if (o_游戏房间.o_玩家列表.Count == 0) {
                Console.WriteLine("游戏房间不行了____________1________o_游戏房间.o_玩家列表.Count == 0");
                return;
            }
            foreach (var n in o_游戏房间.o_玩家列表) {
                if (n.o_ID > 300) {
                    n.o_通信器.S_发送消息(z消息类型, msg);
                }
            }
        }
        void S_游戏房间_群发_自身特别发(ET z消息类型, string z自身消息, string z群体消息) {
            if (o_游戏房间 == null) {
                Console.WriteLine("游戏房间不行了_____________2_______o_游戏房间 == null");
                return;
            } else if (o_游戏房间.o_玩家列表.Count == 0) {
                Console.WriteLine("游戏房间不行了_____________2_______o_游戏房间.o_玩家列表.Count == 0");
                return;
            }
            foreach (var n in o_游戏房间.o_玩家列表) {
                if (n.o_ID > 300) {
                    if (n == this) {
                        n.o_通信器.S_发送消息((int)z消息类型, z自身消息);
                    } else {
                        n.o_通信器.S_发送消息((int)z消息类型, z群体消息);
                    }
                }
            }
        }
        void S_游戏房间_群发_自身特别发(ET z消息类型, string z自身消息, byte[] z群体消息) {
            if (o_游戏房间 == null) {
                Console.WriteLine("游戏房间不行了_____________3_______o_游戏房间 == null");
                return;
            } else if (o_游戏房间.o_玩家列表.Count == 0) {
                Console.WriteLine("游戏房间不行了______________3______o_游戏房间.o_玩家列表.Count == 0");
                return;
            }
            foreach (var n in o_游戏房间.o_玩家列表) {
                if (n.o_ID > 300) {
                    if (n == this) {
                        n.o_通信器.S_发送消息((int)z消息类型, z自身消息);
                    } else {
                        n.o_通信器.S_发送消息((int)z消息类型, z群体消息);
                    }
                }
            }
        }
    }
    public class C_Udp管理器 {
        public static Dictionary<IPEndPoint, C_aa玩家> o_地址玩家列表 = new Dictionary<IPEndPoint, C_aa玩家>();
        public static void S_接口_消息处理(byte[] zData, IPEndPoint z地址) {
            if (zData[0] == 10) {
                C_Main.ooo.o_Udp通信.S_发送消息(zData, z地址);
                string ss = Encoding.UTF8.GetString(zData, 1, zData.Length - 1);
                string[] sss = ss.Split('&');
                long zID = long.Parse(sss[0]);
                if (C_aa玩家.o_在线玩家列表.ContainsKey(zID)) {
                    C_aa玩家 mm = C_aa玩家.o_在线玩家列表[zID];
                    mm.o_Ucp地址 = z地址;
                    o_地址玩家列表[z地址] = mm;
                }
                C_UdpClient.ooo.S_发送消息(10,C_Toot.S_Get时间搓int() + "", z地址);
            } else if(zData[0] <60) {//连接---------------玩家目录
                S_广播(zData, z地址);
                if (zData[0] == 100 || zData[0] == 101) {
                    //z游戏房间号,& o_雇主.o_自身序号 + "&" + z自身位置.x + "&" + z自身位置.z + "&" + z目标点.x + "&" + z目标点.z
                    try {
                        string[] sss = Encoding.UTF8.GetString(zData, 1, zData.Length - 1).Split('&');
                        int z房间号 = int.Parse(sss[0]);
                        int z位置 = int.Parse(sss[1]);
                        if (C_aa游戏房间.o_房间集合.ContainsKey(z房间号)) {
                            C_aa游戏房间.o_房间集合[z房间号].o_角色位置列表[z位置] = string.Format("{0}&{1}&{2}", sss[1], sss[4], sss[5]);
                        }
                    } catch (Exception ex) {
                        Console.WriteLine("出错___" + ex.Message + "___" + ex.StackTrace);
                    }
                }
            }
        }
        static void S_广播(byte[] zData, IPEndPoint z地址) {
            if (o_地址玩家列表.ContainsKey(z地址) == true) {
                C_aa玩家 mm = o_地址玩家列表[z地址];
                if (mm.o_游戏房间 != null) {
                    foreach (var n in mm.o_游戏房间.o_玩家列表) {
                        if (n.o_ID > 300 && mm != n) {
                            C_UdpClient.ooo.S_发送消息(zData, n.o_Ucp地址);
                        }
                    }
                }
            }
        }
    }
    public class C_aa匹配房间 {
        public int o_房间号;
        public bool o_是否开始系统匹配;
        public C_aa玩家 o_房主;
        public Dictionary<C_aa玩家, byte> o_房间玩家列表 = new Dictionary<C_aa玩家, byte>();//z玩家--z红蓝方0:1---
        public static Dictionary<long, C_aa匹配房间> o_团队匹配列表 = new Dictionary<long, C_aa匹配房间>();
        public static List<C_aa玩家> o_单人匹配列表 = new List<C_aa玩家>();//玩家地址----
        static byte o_队伍人员数量 = 5;
        public C_aa匹配房间(C_aa玩家 z房主) {
            o_房主 = z房主;
            Random ran = new Random();
            do { 
                o_房间号 = ran.Next(100000);//房间号--10长度数字--- 
            } while (C_aa游戏房间.o_房间集合.ContainsKey(o_房间号) == true||C_aa匹配房间.o_团队匹配列表.ContainsKey(o_房间号));
            o_团队匹配列表.Add(o_房间号, this);
            o_房间玩家列表[o_房主] = 0;
            o_是否开始系统匹配 = false;
        }
        public static void S_10秒_匹配刷新() {
            List<C_aa匹配房间> z团队匹配列表 = o_团队匹配列表.Values.Where(n => n.o_是否开始系统匹配 == true).ToList();
            foreach(var n in z团队匹配列表){
                if (C_aa游戏房间.o_房间集合.Count > 10000) {
                    Console.WriteLine("房间数已达到10001个___不能在创建-----------");
                    break;
                }
                S_房间匹配(n, o_队伍人员数量);
                o_团队匹配列表.Remove(n.o_房间号);
            }
            while (o_单人匹配列表.Count > 0) {
                if (C_aa游戏房间.o_房间集合.Count > 10000) {
                    Console.WriteLine("房间数已达到10001个___不能在创建-----------");
                    break;
                }
                Dictionary<long, C_aa玩家> z新集合 = new Dictionary<long, C_aa玩家>();
                if (o_单人匹配列表.Count > 10) {
                    for (int i = 0; i < 10; i++) {
                        z新集合.Add(o_单人匹配列表[0].o_ID,o_单人匹配列表[0]);
                        o_单人匹配列表.RemoveAt(0);
                    }
                } else {
                    for (int i = 0; i < o_单人匹配列表.Count; i++) {
                        z新集合.Add(o_单人匹配列表[0].o_ID, o_单人匹配列表[0]);
                        o_单人匹配列表.RemoveAt(0);
                    }
                    o_单人匹配列表.Clear();
                }
                Random ran = new Random();
                while (z新集合.Count < 10) {
                    int z英雄编号 = ran.Next(C_aa玩家.O_电脑玩家列表.Count);
                    if (z新集合.ContainsKey(z英雄编号) == false) {
                        C_aa玩家 nn = C_aa玩家.O_电脑玩家列表[z英雄编号];
                        z新集合.Add(z英雄编号, nn);
                    }
                }
                S_创建竞技房间(z新集合.Values.ToList().S_随机排序(), o_队伍人员数量, -10);
            }
        }
        static void S_房间匹配(C_aa匹配房间 z匹配房间, byte z队伍人员数量) {
            Console.WriteLine("开始匹配________S_匹配组____");
            #region MyRegion------//z添加单个匹配的人-----------
            Dictionary<long, C_aa玩家> z列表1 = new Dictionary<long, C_aa玩家>();
            Dictionary<long, C_aa玩家> z列表2 = new Dictionary<long, C_aa玩家>();
            foreach (var n in z匹配房间.o_房间玩家列表) {
                if (n.Value == 0) {
                    z列表1.Add(n.Key.o_ID, n.Key);
                } else {
                    z列表2.Add(n.Key.o_ID, n.Key);
                }
            }
            bool z是否需要添加 = false;
            if (z列表1.Count < z队伍人员数量 || z列表2.Count < z队伍人员数量) {
                z是否需要添加 = true;
            }
            do {
                if (z列表1.Count < z队伍人员数量 && o_单人匹配列表.Count > 0) {
                    z列表1.Add(o_单人匹配列表[0].o_ID,o_单人匹配列表[0]);
                    o_单人匹配列表.RemoveAt(0);
                }
                if (z列表2.Count < z队伍人员数量 && o_单人匹配列表.Count > 0) {
                    z列表2.Add(o_单人匹配列表[0].o_ID,o_单人匹配列表[0]);
                    o_单人匹配列表.RemoveAt(0);
                }
                if (z列表1.Count < z队伍人员数量 || z列表2.Count < z队伍人员数量) {
                    z是否需要添加 = true;
                } else {
                    z是否需要添加 = false;
                }
            } while (o_单人匹配列表.Count > 0 && z是否需要添加); 
            #endregion
            Console.WriteLine("z列表1.Count==" + z列表1.Count + "________z列表2.Count==" + z列表2.Count);
            #region MyRegion----添加电脑-------------
            Random ran = new Random();
            do {
                if (z列表1.Count < z队伍人员数量) {
                    int z英雄编号 = ran.Next(C_aa玩家.O_电脑玩家列表.Count);
                    if (z列表1.ContainsKey(z英雄编号) == false) {
                        C_aa玩家 nn = C_aa玩家.O_电脑玩家列表[z英雄编号];
                        z列表1.Add(z英雄编号,nn);
                    }
                }
                if (z列表2.Count < z队伍人员数量) {
                    int z英雄编号 = ran.Next(C_aa玩家.O_电脑玩家列表.Count);
                    if (z列表2.ContainsKey(z英雄编号) == false) {
                        C_aa玩家 nn = C_aa玩家.O_电脑玩家列表[z英雄编号];
                        z列表2.Add(z英雄编号,nn);
                    }
                }
                if (z列表1.Count < z队伍人员数量 || z列表2.Count < z队伍人员数量) {
                    z是否需要添加 = true;
                } else {
                    z是否需要添加 = false;
                }
            } while (z是否需要添加); 
            #endregion
            Console.WriteLine("z匹配房间_____整理完成______z列表1.Count==" + z列表1.Count + "________z列表2.Count==" + z列表2.Count);
            List<C_aa玩家> zm列表1 = z列表1.Values.ToList().S_随机排序();
            List<C_aa玩家> zm列表2 = z列表2.Values.ToList().S_随机排序();
            List<C_aa玩家> z新列表 = new List<C_aa玩家>();
            for (int i = 0; i < z队伍人员数量; i++) {
                z新列表.Add(zm列表1[i]);
                z新列表.Add(zm列表2[i]);
            }
            S_创建竞技房间(z新列表, z队伍人员数量, z匹配房间.o_房间号);
        }        
        static void S_创建竞技房间(List<C_aa玩家> z新列表, byte z队伍人员数量,int z房间号) {
            C_aa游戏房间 mm = new C_aa游戏房间(z新列表, z队伍人员数量, z房间号);
            //string ss = mm.o_房间号.ToString("0000000000") + "@";
            string ss = mm.o_房间号.ToString("00000") + "*0*" + mm.o_Serv玩家.o_玩家位置 + "@";
            for (byte i = 0; i < z新列表.Count; i++) {
                if (i != 0) {
                    ss += "&";
                }
                ss += z新列表[i].o_ID;
            }
            foreach (var n in mm.o_玩家列表) {
                if (n.o_ID > 300) {
                    try {
                        n.S_发送消息(ET.e_0505进入选择英雄界面, ss + "@" + n.o_玩家位置);//房间号*已用时间*Serv玩家位置 @玩家ID&玩家ID... @z自身位置
                    } catch (Exception ex) {
                        Console.WriteLine("出错___" + ex.Message + "___" + ex.StackTrace);
                    }
                }
            }
        }
    }
    public class C_aa游戏房间 {
        public int o_房间号;
        public byte o_队伍人员数量, o_房间状态;//游戏房间号----0==正常游戏/2==匹配不成功(平局)/3==玩家投降认输(玩家位置胜负)/4==boss死亡(玩家位置胜负)/5==游戏结束(数据胜负)
        public bool o_数据第1次填充完成;
        public List<C_aa玩家> o_玩家列表 = new List<C_aa玩家>();
        public Dictionary<int, string> o_玩家数据表 = new Dictionary<int, string>();
        public C_aa玩家 o_Serv玩家;
        public Dictionary<int, string> o_角色数据列表 = new Dictionary<int, string>();
        public Dictionary<int, string> o_角色位置列表 = new Dictionary<int, string>();
        public int o_游戏开始时间;
        public static Dictionary<int, C_aa游戏房间> o_房间集合 = new Dictionary<int, C_aa游戏房间>();//z房间号-房间---
        public C_aa游戏房间(List<C_aa玩家> z玩家列表, byte z队伍人员数量,int z房间号) {
            o_队伍人员数量 = z队伍人员数量;
            if (z房间号 > 0) {
                o_房间号 = z房间号;
            } else {
                Random ran = new Random();
                do {
                    o_房间号 = ran.Next(100000);//房间号--10长度数字--- 
                } while (C_aa游戏房间.o_房间集合.ContainsKey(o_房间号) == true || C_aa匹配房间.o_团队匹配列表.ContainsKey(o_房间号));
            }
            o_玩家列表 = z玩家列表;
            for (byte i = 0; i < z玩家列表.Count; i++) {
                z玩家列表[i].o_游戏房间 = this;
                z玩家列表[i].o_玩家位置 = i;
                if (z玩家列表[i].o_ID > 300 && z玩家列表[i].o_通信器 != null) {
                    o_Serv玩家 = z玩家列表[i];
                }
                o_玩家数据表[i] = "";
            }
            C_aa游戏房间.o_房间集合.Add(o_房间号, this);
            o_游戏开始时间 = C_Toot.S_Get时间搓int();
            Console.WriteLine(o_房间号 + "_____房间创建完成-----");
        }
        public void S_竞技数据上传到房间(string[] sss) {
            Console.WriteLine("玩家上传数据_____");
            string[] sss1 = sss[1].Split('*');//z玩家数据---
            for (int i = 0; i < sss1.Length; i++) {
                string[] sss0 = sss1[i].Split('&');
                int z序号 = int.Parse(sss0[0]);
                o_玩家数据表[z序号] = sss1[i];
            }
            string[] sss2 = sss[2].Split('*');//角色数据---
            for (int i = 0; i < sss2.Length; i++) {
                string[] sss0 = sss2[i].Split('&');
                int z序号 = int.Parse(sss0[0]);
                o_角色数据列表[z序号] = sss2[i];
            }
            if (o_角色数据列表.Count > 11) {
                o_数据第1次填充完成 = true;
            }
        }
        public string S_Get房间数据(byte z玩家位置, bool z是否包含位置数据) {
            //房间号 @玩家ID&玩家ID... @z自身位置----@--*--：--&
            //z玩家IDz玩家姓名,z英雄,z武器,z等级
            if (o_数据第1次填充完成) {
                string ss = o_房间号.ToString("00000") + "*" + (C_Toot.S_Get时间搓int() - o_游戏开始时间) + "*" + o_Serv玩家.o_玩家位置 + "@";//---0房间信息----
                o_玩家数据表 = o_玩家数据表.OrderBy(n => n.Key).ToDictionary(n => n.Key, n => n.Value);
                List<string> z列表 = o_玩家数据表.Values.ToList();
                for (int i = 0; i < z列表.Count; i++) {//---1玩家数据----
                    if (i != 0) {
                        ss += "*";
                    }
                    ss += z列表[i];
                }
                ss += "@";
                z列表 = o_角色数据列表.Values.ToList();//----2角色数据------
                for (int i = 0; i < z列表.Count; i++) {
                    if (i != 0) {
                        ss += "*";
                    }
                    ss += z列表[i];
                }
                if (z是否包含位置数据) {
                    ss += "@";
                    z列表 = o_角色位置列表.Values.ToList();//----3角色位置------
                    for (int i = 0; i < z列表.Count; i++) {
                        if (i != 0) {
                            ss += "*";
                        }
                        ss += z列表[i];
                    }
                }
                ss += "@" + z玩家位置;//---------4第1玩家位置-----
                return ss;
            } else {
                return "";
            }
        }
        public static void S_10秒_清楚房间() {
            List<C_aa游戏房间> z要死亡的房间列表 = new List<C_aa游戏房间>();
            foreach (var n in o_房间集合) {
                int z当前时间 = C_Toot.S_Get时间搓int();
                if (n.Value.o_数据第1次填充完成) {
                    if (z当前时间 - n.Value.o_游戏开始时间 > 660) {
                        n.Value.o_房间状态 = 5;
                    }
                } else {
                    if (z当前时间 - n.Value.o_游戏开始时间 > 80) {
                        n.Value.o_房间状态 = 2;
                    }
                }
                if (n.Value.o_房间状态 != 0) {
                    z要死亡的房间列表.Add(n.Value);
                }
                int z真实玩家数 = 0;//z游戏房间没有真实玩家---
                foreach (var a in n.Value.o_玩家列表) {
                    if (a.o_ID > 300) {
                        z真实玩家数++;
                    }
                }
                if (z真实玩家数 == 0) {
                    z要死亡的房间列表.Add(n.Value);
                }
            }
            for (int i = 0; i < z要死亡的房间列表.Count; i++) {
                foreach (var m in z要死亡的房间列表[i].o_玩家列表) {
                    if (m.o_ID > 300) {//游戏房间号----0==正常游戏/2==匹配不成功(平局)/3==玩家投降认输(玩家位置胜负)/4==boss死亡(玩家位置胜负)/5==游戏结束(数据胜负)---
                        m.S_发送消息(ET.e_0560游戏房间结束, z要死亡的房间列表[i].o_房间号 + "&" + z要死亡的房间列表[i].o_房间状态 + "&0");
                        m.o_游戏房间 = null;
                    }
                }
                z要死亡的房间列表[i].o_玩家列表.Clear();
                o_房间集合.Remove(z要死亡的房间列表[i].o_房间号);
            }
        }
        public static void S_5秒_重置Serv玩家() {
            int z当前时间 = C_Toot.S_Get时间搓int();
            foreach (var n in o_房间集合.Values) {  
                if (z当前时间 - n.o_Serv玩家.o_在线时间 >= 9) {
                    Console.WriteLine(n.o_房间号 + "___________需要更换Serv玩家___" + n.o_Serv玩家.o_ID);
                    try {
                        List<C_aa玩家> z玩家列表 = n.o_玩家列表.Where(m => m.o_ID > 300 && (z当前时间 - m.o_在线时间) < 10).ToList();
                        if (z玩家列表.Count > 0) {
                            n.o_Serv玩家 = z玩家列表[0];
                            for (int i = 0; i < n.o_玩家列表.Count; i++) {
                                if (n.o_玩家列表[i].o_ID > 300) {
                                    try {
                                        n.o_玩家列表[i].S_发送消息(ET.e_0530指定服务器玩家, n.o_Serv玩家.o_玩家位置 + "");
                                    } catch (Exception ex) {
                                        Console.WriteLine(ex.Message + "_______" + ex.StackTrace);
                                    }
                                }
                            }
                        }
                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message + "_______" + ex.StackTrace);
                    }
                }
            }
        }
    }
    public class C_aa数据库 {
        string o_本游戏库名关键词 = "9018_";
        static C_aa数据库 ooo;
        public static C_aa数据库 Ooo {
            get {
                if (ooo == null) {
                    ooo = new C_aa数据库();
                }
                return ooo;
            }
        }
        C_aa数据库() {
            C_mysql.S_链接信息设置("127.0.0.1", "3306", "root", "ASDfgh132");
        }
        public Dictionary<long, string> o_玩家库名集合 = new Dictionary<long, string>();
        string o_玩家数量不饱和的数据库 = "";
        public void S_10分钟刷新库名() {
            List<string> z所有库名列表 = C_mysql.S_查询("show databases");
            List<string> o_本游戏的库名列表 = new List<string>();
            for (int i = 0; i < z所有库名列表.Count; i++) {
                Console.WriteLine("---------------数据库: " + z所有库名列表[i]);
                if (z所有库名列表[i].Contains(o_本游戏库名关键词)) {
                    o_本游戏的库名列表.Add(z所有库名列表[i]);
                    Console.WriteLine("------当前可用的数据库_______________" + z所有库名列表[i]);
                }
            }
            if (o_本游戏的库名列表.Count > 0) {//添加表到列表----------------
                foreach (string z库名 in o_本游戏的库名列表) {
                    //9002_15545    8618202565800
                    C_mysql.S_操作("use " + z库名);
                    List<string> z表名列表 = C_mysql.S_查询("show tables");
                    for (int i = 0; i < z表名列表.Count; i++) {
                        try {
                            string ss = z表名列表[i];
                            ss = ss.Remove(0, 1);
                            Console.WriteLine("数据库玩家ID____" + ss);
                            o_玩家库名集合[long.Parse(ss)] = z库名;
                        } catch (Exception ex) {
                            Console.WriteLine("数据库玩家ID__解析出错__" + z表名列表[i]);
                        }
                    }
                    if (z表名列表.Count < 10000) {
                        o_玩家数量不饱和的数据库 = z库名;
                    } else {
                        o_玩家数量不饱和的数据库 = "";
                    }
                }
            } else if (o_本游戏的库名列表.Count == 0 || o_玩家数量不饱和的数据库.Length < 2) {
                S_创建新库();
            }
        }
        void S_创建新库() {
            o_玩家数量不饱和的数据库 = o_本游戏库名关键词 + DateTime.Now.S_Get时间搓();
            //"CREATE DATABASE IF NOT EXISTS hdu CHARACTER SET 'utf8' COLLATE 'gbk_chinese_ci'"
            C_mysql.S_操作("CREATE DATABASE `" + o_玩家数量不饱和的数据库 + "` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci");
            Console.WriteLine(">--------创建库---> " + o_玩家数量不饱和的数据库);
        }
        public bool S_用户注册(long zID) {
            if (o_玩家库名集合.ContainsKey(zID)) {
                return true;
            }
            C_mysql.S_操作("use " + o_玩家数量不饱和的数据库);
            string z新表名 = "t" + zID;
            string ling = "create table " + z新表名 + "(Id bigint primary key,Kee longtext) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=1";
            C_mysql.S_操作(ling);
            o_玩家库名集合.Add(zID, o_玩家数量不饱和的数据库);
            return true;
        }
        public string S_获取玩家数据2(long zID) {
            Dictionary<long, string> zmm = S_获取玩家数据(zID);
            StringBuilder sb = new StringBuilder();
            foreach (var n in zmm) {
                sb.AppendFormat("{0}#0{1}#1", n.Key, n.Value);
            }
            string ss = sb.ToString();
            if (ss.Length > 2) {
                ss = ss.Remove(ss.Length - 2, 2);
            }
            return ss;
        }
        public Dictionary<long, string> S_获取玩家数据(long zID) {
            if (o_玩家库名集合.ContainsKey(zID) == false) {
                return null;
            }
            string z表路径 = o_玩家库名集合[zID] + ".t" + zID;
            return C_mysql.S_玩家数据_键值对查询表(z表路径);
        }
        public bool S_保存玩家数据(long zID, Dictionary<long, string> z数据集合) {
            string z玩家数据库地址 = o_玩家库名集合[zID] + ".t" + zID;
            string ss = "REPLACE INTO " + z玩家数据库地址 + " (Id,Kee) values";
            foreach (var n in z数据集合) {
                ss += "('" + n.Key + "','" + n.Value + "'),";
            }
            ss = ss.Remove(ss.Length - 1, 1);
            return S_玩家数据_更新或插入(zID, ss);
        }
        public bool S_保存玩家数据(long zID, string ss) {
            //Dictionary<long, string> z数据集合 = new Dictionary<long, string>();
            //string[] sss = ss.Split(new string[] { "@#" }, StringSplitOptions.RemoveEmptyEntries);//
            //for (int i = 0; i < sss.Length; i++) {
            //    string[] sss0 = ss.Split(new string[] { "$#" }, StringSplitOptions.RemoveEmptyEntries);//
            //    z数据集合.Add(long.Parse(sss0[0]), sss0[1]);
            //}
            ss = ss.Replace("#0", "','");
            ss = "('" + ss.Replace("#1", "'),('") + "')";
            S_玩家数据_更新或插入(zID, ss);
            return true;
        }
        bool S_玩家数据_更新或插入(long zID, string ss0) {
            if (o_玩家库名集合.ContainsKey(zID) == false) {
                S_用户注册(zID);
            }
            string z玩家数据库地址 = o_玩家库名集合[zID] + ".t" + zID;
            string ss = "REPLACE INTO " + z玩家数据库地址 + " (Id,Kee) values " + ss0;
            Console.WriteLine(ss);
            return C_mysql.S_操作(ss);
        }
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
        public Dictionary<ushort, Dictionary<ushort, List<C_aa玩家>>> o_玩家集合地图 = new Dictionary<ushort, Dictionary<ushort, List<C_aa玩家>>>();
        public void S_添加玩家(C_aa玩家 z玩家) {
            if (o_玩家集合地图.ContainsKey(z玩家.o_x经度0) == false) {
                o_玩家集合地图[z玩家.o_x经度0] = new Dictionary<ushort, List<C_aa玩家>>();
            }
            if (o_玩家集合地图[z玩家.o_x经度0].ContainsKey(z玩家.o_y纬度0) == false) {
                o_玩家集合地图[z玩家.o_x经度0][z玩家.o_y纬度0] = new List<C_aa玩家>();
            }
            if (o_玩家集合地图[z玩家.o_x经度0][z玩家.o_y纬度0].Contains(z玩家) == false) {
                o_玩家集合地图[z玩家.o_x经度0][z玩家.o_y纬度0].Add(z玩家);
                Console.WriteLine("添加**************  附近的人_____" + z玩家.o_x经度0 + "____" + z玩家.o_y纬度0 + "________" + o_玩家集合地图[z玩家.o_x经度0][z玩家.o_y纬度0].Count);
            }
        }
        public void S_30秒_清除刷新() {
            Dictionary<ushort, ushort> z列表 = new Dictionary<ushort, ushort>();
            List<C_aa玩家> z玩家更换位置列表 = new List<C_aa玩家>();
            foreach (var n in o_玩家集合地图) {
                foreach (var m in n.Value) {
                    for (int i = 0; i < m.Value.Count; i++) {
                        bool z是否可移除 = false;
                        if (m.Value[i].o_是否可被清楚 == true) {
                            z是否可移除 = true;       
                        } else if(m.Value[i].o_x经度0!=n.Key||m.Value[i].o_y纬度0!=m.Key){
                            z玩家更换位置列表.Add(m.Value[i]);
                            z是否可移除 = true;
                        }
                        if (z是否可移除) {
                            Console.WriteLine("移除**************  附近的人_" + m.Value[i].o_ID + "____________" + n.Key + "_____" + m.Key);
                            m.Value.RemoveAt(i);
                            i--;
                            if (m.Value.Count == 0) {
                                z列表[n.Key] = m.Key;
                            }
                        }
                    }
                }
            }
            foreach (var n in z列表) {
                if (o_玩家集合地图.ContainsKey(n.Key) && o_玩家集合地图[n.Key].ContainsKey(n.Value)) {
                    o_玩家集合地图[n.Key].Remove(n.Value);
                    if (o_玩家集合地图[n.Key].Count == 0) {
                        o_玩家集合地图.Remove(n.Key);
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
                    ss2 += string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}@", z玩家列表[i].o_ID, z玩家列表[i].o_姓名, z玩家列表[i].o_头像, C_网格Serve.o_本机IP, z玩家列表[i].o_战斗力, z玩家列表[i].o_x经度, z玩家列表[i].o_y纬度);
                }
            }
            return ss2;
        }
        //public string S_获取附近的好友(ushort xff, ushort yff) {
        //    return S_获取附近好友(xff, yff, true,80);
        //}
        //string S_获取附近好友(ushort xff, ushort yff, bool z是否1级查询,byte z最大数量) {
        //    string ss = "";
        //    if (o_玩家集合地图.ContainsKey((ushort)xff) && o_玩家集合地图[(ushort)xff].ContainsKey((ushort)yff)) {
        //        if (o_玩家集合地图[(ushort)xff][(ushort)yff].Count > 0) {
        //            List<C_aa玩家> z列表 = o_玩家集合地图[(ushort)xff][(ushort)yff].Where((C_aa玩家 nn6, int ii) => {
        //                return ii < z最大数量;
        //            }).ToList();                
        //            foreach (var n in z列表) { //--0zID,1z姓名,2z头像,3z服务器IP-4z战斗力-5zX经度--6zY纬度
        //                ss += string.Format("@{0}&{1}&{2}&{3}&{4}&{5}&{6}", n.o_ID, n.o_姓名, n.o_头像, C_网格Serve.o_本机IP, n.o_战斗力, n.o_x经度, n.o_y纬度);
        //            }
        //        }
        //        if (z是否1级查询 == true && o_玩家集合地图[(ushort)xff][(ushort)yff].Count < z最大数量) {
        //            ss += S_获取附近好友((ushort)(xff + 1), (ushort)(yff - 1), false,30);
        //            ss += S_获取附近好友((ushort)(xff + 1), yff, false, 30);
        //            ss += S_获取附近好友((ushort)(xff + 1), (ushort)(yff + 1), false, 30);
        //            ss += S_获取附近好友(xff, (ushort)(yff - 1), false, 30);
        //            ss += S_获取附近好友(xff, (ushort)(yff + 1), false, 30);
        //            ss += S_获取附近好友((ushort)(xff - 1), (ushort)(yff - 1), false, 30);
        //            ss += S_获取附近好友((ushort)(xff - 1), yff, false, 30);
        //            ss += S_获取附近好友((ushort)(xff - 1), (ushort)(yff + 1), false, 30);
        //            return ss;
        //        } else {
        //            return ss;
        //        }
        //    } else {
        //        return "";
        //    }
        //}
        public List<C_aa玩家> S_获取附近的人(ushort xff, ushort yff, bool z是否包含周围) {
            List<C_aa玩家> z玩家列表=new List<C_aa玩家>();
            if (o_玩家集合地图.ContainsKey((ushort)xff) && o_玩家集合地图[(ushort)xff].ContainsKey((ushort)yff)) {
                z玩家列表.AddRange(o_玩家集合地图[(ushort)xff][(ushort)yff]);
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
    public static class C_扩展方法_jdsdk{
        public static string S_网格群发头调换(this string[] zz,string z消息内容) {
            //-0A服务器IP-&-1A玩家ID-&-2B服务器IP-&-3B玩家ID-&-4消息类型-&0&0--#3---msg消息内
            //return new string[] { zz[2], zz[3], zz[0], zz[1], zz[4] };
            return string.Format("{0}&{1}&{2}&{3}&{4}&0&0#3{5}", zz[2], zz[3], zz[0], zz[1], zz[4], z消息内容);
        }
    }
}
