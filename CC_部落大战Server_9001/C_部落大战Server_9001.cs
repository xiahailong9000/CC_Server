using CC_ServerDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CC_部落大战Server_9001 {
    public enum E_红蓝方阵 {
        e60_蓝方 = 60,
        e61_红方 = 61,
        e62_中立 = 62,
    }
    public class C_Server {
        public static C_Server ooo = new C_Server();
        public C_数据库 o_数据库 = new C_数据库();
        public C_UdpClient o_Udp通信;
        public C_TcpClient o_域名Serv;
        static void Main(string[] sss) {
            ooo.o_数据库.S_刷新数据库();
            C_TcpClient.S_启动监听(9001, new C_Tcp客户端());
            ooo.o_Udp通信 = C_UdpClient.S_启动监听(9001, C_Udp管理器.S_接口_消息处理);
            //ooo.o_域名Serv = new C_TcpClient("127.0.0.1", 9800, new C_Tcp客户端());
            //ooo.o_域名Serv = new C_TcpClient("47.89.180.28", 9800, new C_Tcp客户端());
            ooo.o_域名Serv.S_发送消息(10, C_玩家.o_玩家总列表.Count + "");
            Thread th = new Thread(delegate() {
                #region MyRegion----计时事件-----------------
                int ii = 0;
                while (true) {
                    Thread.Sleep(10000);
                    ii += 10;
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
            while (true) {
                Console.ReadKey();
                Console.WriteLine("AA农场服务器___请勿点击---------");
            }
        }
        #region MyRegion--计时事件------------------
        static void S_10秒事件() {
            Console.WriteLine("S_10秒事件---------");
        }
        static void S_20秒事件() {
            //Console.WriteLine("S_20秒事件---------");
            ooo.o_域名Serv.S_发送消息(10, C_玩家.o_玩家总列表.Count + "");
        }
        static void S_30秒事件() {
            C_玩家.S_30秒_清楚临时玩家();
            C_游戏房间.S_30秒_匹配刷新();
        }
        static void S_60秒事件() {
            //Console.WriteLine("S_60秒事件---------");
        }
        static void S_3分钟事件() {
            //Console.WriteLine("S_3分钟事件---------");
        }
        static void S_10分钟事件() {
            //Console.WriteLine("S_10分钟事件---------");
        }
        #endregion
        public class C_数据库 {
            string o_不饱和的数据库 = "";
            public void S_刷新数据库() {
                List<string> zDB名列表 = C_mysql.S_查询("show databases");
                List<string> o_用户库列表 = new List<string>();
                for (int i = 0; i < zDB名列表.Count; i++) {
                    Console.WriteLine("数据库: " + zDB名列表[i]);
                    if (zDB名列表[i].Contains("d1707_")) {
                        o_用户库列表.Add(zDB名列表[i]);
                        Console.WriteLine("当前可用的数据库_______________" + zDB名列表[i]);
                    }
                }
                if (o_用户库列表.Count > 0) {//添加表到列表----------------
                    foreach (string z库名 in o_用户库列表) {
                        //d1707_osdjsk    t8618202565800
                        C_mysql.S_操作("use " + z库名);
                        List<string> z表名列表 = C_mysql.S_查询("show tables");
                        if (z表名列表.Count < 10000) {
                            o_不饱和的数据库 = z库名;
                        }
                        for (int i = 0; i < z表名列表.Count; i++) {
                            try {
                                string ss = z表名列表[i].Remove(0, 1);
                                long zID = long.Parse(ss);
                                if (C_玩家.o_玩家总列表.ContainsKey(zID) == false) {
                                    new C_玩家(z库名, zID, false);
                                }
                            } catch {
                            }
                        }
                    }
                } else if (o_用户库列表.Count == 0 || o_不饱和的数据库.Length < 2) {
                    S_创建新库();
                }
            }
            void S_创建新库() {
                o_不饱和的数据库 = "d1707_" + DateTime.Now.S_Get时间搓().S_10进制到62进制();
                C_mysql.S_操作("create database " + o_不饱和的数据库);
                Console.WriteLine("创建库 " + o_不饱和的数据库);
            }
            public string S_用户登陆(long zID, string z密码) {
                string z表路径 = C_玩家.o_玩家总列表[zID].o_库名 + ".t" + zID;
                string Ling = "select a.Kee from " + z表路径 + " a where Id='1'";
                Console.WriteLine("MySql语句__" + Ling);
                string ss = C_mysql.S_查询(Ling)[0];
                string[] sss = ss.Split('&');
                if (sss[1] == z密码) {
                    return sss[0];
                } else {
                    return "-5";//密码错误----
                }
            }
            public C_玩家 S_用户注册(long zID, string z用户名, string z密码) {
                C_mysql.S_操作("use " + o_不饱和的数据库);
                List<string> z列表 = C_mysql.S_查询("show tables");
                if (z列表.Count > 10000) {
                    S_创建新库();
                    C_mysql.S_操作("use " + o_不饱和的数据库);
                }
                string z新表名 = "t" + zID;
                string ling = "create table " + z新表名 + "(Id bigint primary key,Kee longtext)";
                C_mysql.S_操作(ling);
                ling = string.Format("insert into {0} (Id,Kee)  values ('1','{1}&{2}')", z新表名, z用户名, z密码);
                C_mysql.S_操作(ling);
                return new C_玩家(o_不饱和的数据库, zID, false);
            }
            public void S_用户保存资源(string z数据目录, string[] sss0) {
                string ling = "insert " + z数据目录 + " (Id,Kee) values ";
                for (int i = 0; i < sss0.Length; i++) {
                    string[] sss = sss0[i].Split(new char[] { '&' }, 4);
                    string zID = sss[0] + "&" + sss[1] + "&" + sss[2];
                    ling += "('" + zID + "','" + sss[3] + "'),";
                }
                ling = ling.Remove(ling.Length - 1, 1);
                C_mysql.S_操作(ling);
            }
            public string S_用户获取资源(string z数据目录) {
                string Ling = "select * from " + z数据目录;
                List<string> z列表 = C_mysql.S_查询(Ling);
                if (z列表.Count < 10) {
                    return z数据目录 + "@0";//0@玩家目录--表示让玩家自己创建场景
                }
                string ss = z数据目录;
                for (int i = 0; i < z列表.Count; i++) {
                    ss += "@" + z列表[i];
                }
                return ss;
            }
        }
    }
    //public class C_游戏房间 {
    //    //o_房间号==-1(房间超载)/-2(创建者场景不存在)
    //    public int o_房间号, o_房间类型, o_加载时间, o_是否开始游戏;
    //    public string o_创建者目录;
    //    public List<C_玩家> o_蓝方玩家列表 = new List<C_玩家>(),
    //        o_红方玩家列表 = new List<C_玩家>();
    //    //public C_游戏房间(int z房间类型,string z数据目录) {
    //    //    string ss= ooo.o_数据库.S_用户获取资源(z数据目录);
    //    //    if(ss.Length<50) {
    //    //        o_房间号=-2;//创建者场景不存在
    //    //        return;
    //    //    }
    //    //    o_创建者目录=z数据目录;
    //    //    if(ooo.o_游戏房间列表.Count<7000) {
    //    //        o_房间类型=z房间类型;
    //    //        int z房间号;
    //    //        Random ran = new Random();
    //    //        do {
    //    //            z房间号=ran.Next(10000);
    //    //        } while(ooo.o_游戏房间列表.ContainsKey(z房间号)==true);
    //    //        o_房间号=z房间号;
    //    //        ooo.o_游戏房间列表.Add(o_房间号,this);
    //    //    } else {
    //    //        o_房间号=-1;//房间超载
    //    //    }
    //    //    Thread z延时销毁线程=new Thread(delegate() {
    //    //        #region MyRegion
    //    //        for(int i=0;i<120;i++) {
    //    //            Thread.Sleep(1000);
    //    //            o_加载时间=i;
    //    //        }
    //    //        if(o_蓝方玩家列表.Count!=o_房间类型||o_红方玩家列表.Count!=o_房间类型) {
    //    //            Console.WriteLine("房间被销毁=---- "+o_房间号);
    //    //            S_销毁房间();
    //    //        }
    //    //    });
    //    //    z延时销毁线程.IsBackground=true;
    //    //        #endregion
    //    //    z延时销毁线程.Start();
    //    //}
    //    //public void S_销毁房间() {
    //    //    ooo.o_游戏房间列表.Remove(o_房间号);
    //    //    S_群发(59,o_房间号+"","ss");
    //    //    o_蓝方玩家列表.Clear();
    //    //    o_红方玩家列表.Clear();
    //    //    Console.WriteLine("销毁房间___"+o_房间号);
    //    //}
    //    //public int S_玩家加入(C_玩家 nn) {
    //    //    if(o_房间类型>o_红方玩家列表.Count) {
    //    //        if(o_蓝方玩家列表.Count>o_红方玩家列表.Count) {
    //    //            nn.o_我方方阵=E_红蓝方阵.e61_红方;
    //    //            o_红方玩家列表.Add(nn);
    //    //        } else {
    //    //            nn.o_我方方阵=E_红蓝方阵.e60_蓝方;
    //    //            o_蓝方玩家列表.Add(nn);
    //    //        }
    //    //        nn.o_房间=this;
    //    //        return o_房间号;
    //    //    } else {
    //    //        return -1;//人数已满
    //    //    }
    //    //}
    //    //public void S_玩家离开(C_玩家 nn) {
    //    //    if(o_是否开始游戏!=1) {
    //    //        if(o_蓝方玩家列表.Contains(nn)) {
    //    //            o_蓝方玩家列表.Remove(nn);
    //    //        }
    //    //        if(o_红方玩家列表.Contains(nn)) {
    //    //            o_红方玩家列表.Remove(nn);
    //    //        }
    //    //        if(o_蓝方玩家列表.Count==0&&o_红方玩家列表.Count==0) {
    //    //            S_销毁房间();
    //    //        } else {
    //    //            S_群发(57,o_房间号+"&"+o_房间类型+"&"+o_加载时间+S_房间玩家数据(true),"00");
    //    //        }
    //    //    }
    //    //}
    //    //public string S_房间玩家数据(bool z是否加载技能) {
    //    //    string ss="";
    //    //    for(int i=0;i<o_蓝方玩家列表.Count;i++) {
    //    //        ss+="@"+o_蓝方玩家列表[i].o_数据目录+o_蓝方玩家列表[i].S_技能列表(z是否加载技能);
    //    //        if(i+1<=o_红方玩家列表.Count) {
    //    //            ss+="@"+o_红方玩家列表[i].o_数据目录+o_蓝方玩家列表[i].S_技能列表(z是否加载技能);
    //    //        }
    //    //    }
    //    //    return ss;
    //    //}
    //    //public void S_群发(int z类型,string msg,string z自身) {
    //    //    foreach(C_玩家 nn in o_蓝方玩家列表) {
    //    //        if(nn.o_数据目录!=z自身) {
    //    //            nn.o_Tcp通信.o_通信对象.S_发送消息(z类型,msg);
    //    //        }
    //    //    }
    //    //    foreach(C_玩家 nn in o_红方玩家列表) {
    //    //        if(nn.o_数据目录!=z自身) {
    //    //            nn.o_Tcp通信.o_通信对象.S_发送消息(z类型,msg);
    //    //        }
    //    //    }
    //    //}
    //    //public bool S_是否全部玩家都已经确定技能() {
    //    //    if(o_蓝方玩家列表.Count!=o_房间类型||o_红方玩家列表.Count!=o_房间类型) {
    //    //        return false;
    //    //    }
    //    //    foreach(C_玩家 nn in o_蓝方玩家列表) {
    //    //        if(nn.o_是否确定技能==0) {
    //    //            return false;
    //    //        }
    //    //    }
    //    //    foreach(C_玩家 nn in o_红方玩家列表) {
    //    //        if(nn.o_是否确定技能==0) {
    //    //            return false;
    //    //        }
    //    //    }                   
    //    //    return true;
    //    //}


    //}
    public class C_玩家 {
        public string o_库名;//"0"==电脑玩家/"1"==临时玩家/"d1707_issi3"==正常玩家
        public long o_ID;
        public C_游戏房间 o_游戏房间;
        public C_Tcp客户端 o_Tcp通信;
        public m_IP o_Ucp地址;
        public static Dictionary<long, C_玩家> o_玩家总列表 = new Dictionary<long, C_玩家>();//玩家ID-玩家
        public static List<long> o_临时玩家列表 = new List<long>();
        public C_玩家(string z库名, long zID, bool z是否临时玩家) {
            o_库名 = z库名;
            o_ID = zID;
            if (z是否临时玩家) {
                C_玩家.o_临时玩家列表.Add(zID);
                C_玩家.o_玩家总列表.Add(zID, this);
                Console.WriteLine("临时新用户_____________" + zID);
            } else {
                C_玩家.o_玩家总列表.Add(zID, this);
                Console.WriteLine("新用户_____________" + zID);
            }
        }
        static List<C_玩家> o_电脑玩家列表;
        public static List<C_玩家> O_电脑玩家列表 {
            get {
                if (o_电脑玩家列表 == null) {
                    o_电脑玩家列表 = new List<C_玩家>();
                    for (int i = 0; i < 30; i++) {
                        o_电脑玩家列表.Add(new C_玩家("0", i, false));
                    }
                }
                return C_玩家.o_电脑玩家列表;
            }
        }
        public static void S_30秒_清楚临时玩家() {
            List<long> z清楚列表 = new List<long>();
            int z当前时间 = C_Toot.S_Get时间搓int();
            foreach (var n in o_临时玩家列表) {
                if (o_玩家总列表.ContainsKey(n) == true) {
                    C_玩家 nn = o_玩家总列表[n];
                    if (nn.o_Tcp通信 != null) {
                        if (z当前时间 - o_玩家总列表[n].o_Tcp通信.o_在线时间 > 40) {
                            z清楚列表.Add(n);
                        }
                    } else {
                        z清楚列表.Add(n);
                    }
                }
            }
            o_临时玩家列表.Clear();
            for (int i = 0; i < z清楚列表.Count; i++) {
                o_玩家总列表.Remove(z清楚列表[i]);
                Console.WriteLine("玩家__" + z清楚列表[i] + "______被清除_______");
            }
        }
    }
    public class C_Tcp客户端 : C_TcpClient.C_逻辑端 {
        public int o_在线时间;
        C_玩家 o_雇主;
        //public override C_TcpClient.C_逻辑端 S_接口_初始化(C_TcpClient nn) {
        //    if (o_通信器 != null) {
        //        C_Tcp客户端 mm = new C_Tcp客户端();
        //        return mm.S_接口_初始化(nn);
        //    } else {
        //        o_通信器 = nn;
        //        return this;
        //    }
        //}
        public override void S_接口_消息处理(ushort z消息类型, string zMsg, byte[] zData) {
            if (z消息类型 > 50 && o_雇主 == null) {
                Console.WriteLine(z消息类型 + "___Tcp非法登录________________" + zMsg);
                return;
            }
            string ss;
            switch (z消息类型) {
                case 1://z玩家ID  &玩家地址---
                    #region MyRegion----初次登陆-----------收到的域名Server消息-----------------
                    string[] sss = zMsg.Split('&');
                    long zID = long.Parse(sss[0]);
                    if (C_玩家.o_玩家总列表.ContainsKey(zID) == true) {
                        o_通信器.S_发送消息(1, sss[1]);//玩家地址-------
                        Console.WriteLine("玩家探测登陆________存在_____" + C_玩家.o_玩家总列表.Count);
                    } else {
                        Console.WriteLine("玩家探测登陆______用户不存在_______" + C_玩家.o_玩家总列表.Count);
                    }
                    #endregion
                    break;
                case 3://z玩家ID &姓名 &密码 &玩家地址---------
                    #region MyRegion-------玩家注册--------收到的域名Server消息-------------------
                    sss = zMsg.Split('&');
                    zID = long.Parse(sss[0]);
                    if (C_玩家.o_玩家总列表.ContainsKey(zID) == false) {
                        C_玩家 nn = C_Server.ooo.o_数据库.S_用户注册(zID, sss[1], sss[2]);
                        o_通信器.S_发送消息(3, "1&" + sss[3]);//1 &玩家地址-------
                    } else {
                        o_通信器.S_发送消息(3, "-1&" + sss[3]);//-1 &玩家地址-------
                    }
                    Console.WriteLine("玩家注册_____________");
                    #endregion
                    break;
                case 10:
                    #region MyRegion------收到的域名Server消息-----心跳回应-------------
                    //Console.WriteLine("收到心跳包: " + zMsg);
                    o_通信器.S_发送消息(10, "0");
                    o_在线时间 = C_Toot.S_Get时间搓int();
                    #endregion
                    break;
                case 50://z玩家ID &z密码---------------
                    #region MyRegion---------------玩家精确登陆----------
                    Console.WriteLine("玩家精确登陆----------- " + zMsg);
                    sss = zMsg.Split('&');
                    zID = long.Parse(sss[0]);
                    if (C_玩家.o_玩家总列表.ContainsKey(zID) == true) {
                        o_雇主 = C_玩家.o_玩家总列表[zID];
                        if (o_雇主.o_库名.Length > 4) {//是否是正规玩家------
                            ss = C_Server.ooo.o_数据库.S_用户登陆(zID, sss[1]);
                            o_通信器.S_发送消息(50, ss);//&用户名/-5==密码错误/-10==用户不存在
                        } else {
                            o_通信器.S_发送消息(50, "-10");//&用户名/-5==密码错误/-10==用户不存在
                        }
                        o_雇主.o_Tcp通信 = this;
                        Console.WriteLine("玩家________存在_____");
                    } else {
                        o_通信器.S_发送消息(50, "-10");//&用户名/-5==密码错误/-10==用户不存在
                        if (C_玩家.o_临时玩家列表.Contains(zID) == false) {
                            C_玩家 nn = new C_玩家("1", zID, true);
                            o_雇主 = nn;
                            nn.o_Tcp通信 = this;
                            Console.WriteLine("玩家______不存在_____创建临时玩家_______________________");
                        } else {//重复登录--------------------
                            o_雇主 = C_玩家.o_玩家总列表[zID];
                            o_雇主.o_Tcp通信 = this;
                            Console.WriteLine("玩家______不存在_____临时玩家存在__________________");
                        }
                    }
                    o_在线时间 = C_Toot.S_Get时间搓int();
                    #endregion
                    break;
                case 500://----单人匹配-----------------------            
                    o_通信器.S_发送消息(500, "0");//进入匹配界面-----------
                    if (C_游戏房间.o_单人匹配列表.Contains(o_雇主) == false) {
                        C_游戏房间.o_单人匹配列表.Add(o_雇主);
                    }
                    break;
                case 501://zID &zID ........
                    #region MyRegion------组队请求5v5匹配--------//zID &zID ........
                    sss = zMsg.Split('&');
                    List<C_玩家> z列表 = new List<C_玩家>();
                    for (int i = 0; i < sss.Length; i++) {
                        try {
                            zID = int.Parse(sss[i]);
                            if (C_玩家.o_玩家总列表.ContainsKey(zID) == true) {
                                C_玩家 nn = C_玩家.o_玩家总列表[zID];
                                nn.o_Tcp通信.o_通信器.S_发送消息(500, "0");//进入匹配界面-----
                                z列表.Add(nn);
                            } else {
                            }
                        } catch (Exception ex) {
                            Console.WriteLine("组队匹配__玩家ID不正确________" + ex.Message);
                            return;
                        }
                    }
                    C_游戏房间.o_团队匹配列表.Add(z列表);
                    #endregion
                    break;
                case 503://取消匹配等待-------------------
                    #region MyRegion--------取消匹配等待-------------------
                    if (C_游戏房间.o_单人匹配列表.Contains(o_雇主)) {
                        C_游戏房间.o_单人匹配列表.Remove(o_雇主);
                        o_通信器.S_发送消息(503, "00");
                    } else {
                        for (int i = 0; i < C_游戏房间.o_团队匹配列表.Count; i++) {
                            if (C_游戏房间.o_团队匹配列表[i].Contains(o_雇主)) {
                                List<C_玩家> z列表0 = C_游戏房间.o_团队匹配列表[i];
                                for (int x = 0; x < z列表0.Count; x++) {
                                    z列表0[x].o_Tcp通信.o_通信器.S_发送消息(503, "00");
                                }
                                C_游戏房间.o_团队匹配列表.Remove(z列表0);
                                break;
                            }
                        }
                    }
                    #endregion
                    break;
                case 510://z请求玩家加入队伍---------//z房主ID,z房主头像,z房主姓名,z请求ID
                    #region MyRegion---z请求玩家加入队伍---------//z房主ID,z房主头像,z房主姓名,z请求ID
                    sss = zMsg.Split('&');
                    zID = long.Parse(sss[3]);
                    if (C_玩家.o_玩家总列表.ContainsKey(zID) == true) {
                        C_玩家.o_玩家总列表[zID].o_Tcp通信.o_通信器.S_发送消息(510, zMsg);
                    }
                    #endregion
                    break;
                case 512://z玩家同意加入队伍--------//z房主ID,zID,z头像,z姓名
                    #region MyRegion-------z玩家同意加入队伍--------//z房主ID,zID,z头像,z姓名
                    sss = zMsg.Split('&');
                    zID = long.Parse(sss[0]);
                    if (C_玩家.o_玩家总列表.ContainsKey(zID) == true) {
                        C_玩家.o_玩家总列表[zID].o_Tcp通信.o_通信器.S_发送消息(512, zMsg);
                    }
                    #endregion
                    break;
                case 514://z收到队伍消息--------//z位置,z消息 @ zID,z头像,z姓名@zID,z头像,z姓名@ .....
                    #region MyRegion---z收到队伍消息-----//z位置,z消息 @ zID,z头像,z姓名@zID,z头像,z姓名@ .....
                    sss = zMsg.Split('@');
                    for (int i = 1; i < sss.Length; i++) {//消息群发--------------
                        string[] sss2 = sss[i].Split('&');
                        zID = int.Parse(sss2[0]);
                        if (C_玩家.o_玩家总列表.ContainsKey(zID) == true) {
                            C_玩家.o_玩家总列表[zID].o_Tcp通信.o_通信器.S_发送消息(514, zMsg);
                        }
                    }
                    #endregion
                    break;
                case 516://退出队伍-------------//z位置,z原因索引 @ zID,z头像,z姓名@zID,z头像,z姓名@ .....
                    #region MyRegion----退出队伍----//z位置,z原因索引 @ zID,z头像,z姓名@zID,z头像,z姓名@ .....
                    sss = zMsg.Split('@');
                    for (int i = 1; i < sss.Length; i++) {//消息群发--------------
                        string[] sss2 = sss[i].Split('&');
                        zID = int.Parse(sss2[0]);
                        if (C_玩家.o_玩家总列表.ContainsKey(zID) == true) {
                            C_玩家.o_玩家总列表[zID].o_Tcp通信.o_通信器.S_发送消息(516, zMsg);
                        }
                    }
                    #endregion
                    break;
                case 518://队伍被解散-------------
                    #region MyRegion---队伍被解散-------------
                    sss = zMsg.Split('&');
                    for (int i = 0; i < sss.Length; i++) {//消息群发--------------
                        zID = int.Parse(sss[i]);
                        if (C_玩家.o_玩家总列表.ContainsKey(zID) == true) {
                            C_玩家.o_玩家总列表[zID].o_Tcp通信.o_通信器.S_发送消息(518, zMsg);
                        }
                    }
                    #endregion
                    break;
                case 526://解散房间-----------------
                    #region MyRegion-------解散房间---房间号----
                    int z房间号 = int.Parse(zMsg);
                    if (C_游戏房间.o_房间集合.ContainsKey(z房间号) == true) {
                        Console.WriteLine("解散房间__" + zMsg);
                        C_游戏房间 mm = C_游戏房间.o_房间集合[z房间号];
                        for (int i = 0; i < mm.o_玩家列表.Count; i++) {
                            if (mm.o_玩家列表[i].o_Tcp通信.o_通信器 != null) {
                                o_通信器.S_发送消息(526, zMsg);
                                mm.o_玩家列表[i].o_游戏房间 = null;
                            }
                        }
                        C_游戏房间.o_房间集合.Remove(z房间号);
                    } else {
                        Console.WriteLine("房间__" + zMsg + "_____不存在，无法解散");
                    }
                    #endregion
                    break;
                case 610:
                    S_游戏房间_群发(z消息类型, zMsg, zData);
                    break;
                case 630:
                    S_游戏房间_群发(z消息类型, zMsg, zData);
                    break;
                case 631:
                    S_游戏房间_群发(z消息类型, zMsg, zData);
                    break;
                case 633:
                    S_游戏房间_群发(z消息类型, zMsg, zData);
                    break;
                case 660:
                    S_游戏房间_群发(z消息类型, zMsg, zData);
                    break;
                case 810://添加好友------------------
                    #region MyRegion------添加好友----------
                    sss = zMsg.Split('&');
                    zID = int.Parse(sss[0]);
                    if (C_玩家.o_玩家总列表.ContainsKey(zID) == true) {
                        C_玩家.o_玩家总列表[zID].o_Tcp通信.o_通信器.S_发送消息(811, zMsg);
                        o_通信器.S_发送消息(810, sss[0] + "&1");
                    } else {
                        o_通信器.S_发送消息(810, sss[0] + "&0");
                    }
                    #endregion
                    break;
                case 813://z收到好友消息--------z发送者ID,z接收者ID,z表情编号,z消息内容------
                    #region MyRegion----z收到好友消息--------z发送者ID,z接收者ID,z表情编号,z消息内容------
                    sss = zMsg.Split('&');
                    zID = int.Parse(sss[1]);
                    if (C_玩家.o_玩家总列表.ContainsKey(zID) == true) {
                        C_玩家.o_玩家总列表[zID].o_Tcp通信.o_通信器.S_发送消息(813, zMsg);
                    } else {
                        o_通信器.S_发送消息(815, sss[0] + "&0");//好友不在线---
                    }
                    #endregion
                    break;
                case 815://z好友是否在线------z好友ID,z是否在心@ ...------------
                    #region MyRegion--z好友是否在线------z好友ID,z是否在心@ ...------------
                    sss = zMsg.Split('&');
                    ss = "";
                    for (int i = 0; i < sss.Length; i++) {
                        zID = int.Parse(sss[i]);
                        if (C_玩家.o_玩家总列表.ContainsKey(zID) == true) {
                            ss += sss[i] + "&1@";//在线--
                        } else {
                            ss += sss[i] + "&0@";//不在线--
                        }
                    }
                    if (ss.Length > 3) {
                        ss = ss.Remove(ss.Length - 1, 1);
                    }
                    o_通信器.S_发送消息(815, ss);
                    #endregion
                    break;
            }
        }
        void S_游戏房间_群发(int z消息类型, string msg, byte[] zData) {
            foreach (var n in o_雇主.o_游戏房间.o_玩家列表) {
                if (n.o_ID > 300 && n.o_Tcp通信 != null && n.o_Tcp通信.o_通信器 != null) {
                    n.o_Tcp通信.o_通信器.S_发送消息(zData);
                }
            }
        }
        public override void S_接口_关闭() {
        }
        //public override void S_接口_Debug显示(string msg, int z等级) {
        //    Console.WriteLine(msg);
        //}
    }
    public class C_Udp管理器  {
        public static Dictionary<IPEndPoint, C_玩家> o_地址玩家列表 = new Dictionary<IPEndPoint, C_玩家>();
        public static void S_接口_消息处理(C_UdpClient zUdpClient, IPEndPoint z地址, byte[] zData) {
            switch (zData[0]) {
                case 10:
                    C_Server.ooo.o_Udp通信.S_发送消息(zData, z地址);
                    string ss = Encoding.UTF8.GetString(zData, 1, zData.Length - 1);

                    break;
                case 50://连接---------------玩家目录

                    break;
            }
        }
        //public override void S_接口_Debug显示(string msg, int z等级, IPEndPoint z地址) {
        //    Console.WriteLine(msg);
        //}
    }
    public class C_游戏房间 {
        public int o_房间号;
        public bool o_是否刷新;
        public List<C_玩家> o_玩家列表 = new List<C_玩家>();
        public C_玩家 o_玩家Serv;
        public static List<List<C_玩家>> o_团队匹配列表 = new List<List<C_玩家>>();
        public static List<C_玩家> o_单人匹配列表 = new List<C_玩家>();//玩家地址----
        public static Dictionary<int, C_游戏房间> o_房间集合 = new Dictionary<int, C_游戏房间>();//房间号-房间-
        public C_游戏房间(List<C_玩家> z玩家列表) {
            Random ran = new Random();
            do {
                long ll = DateTime.Now.S_Get时间搓();
                o_房间号 = (int)ll - ran.Next(1000000000);//房间号--10长度数字---   
            } while (C_游戏房间.o_房间集合.ContainsKey(o_房间号) == true);
            o_玩家列表 = z玩家列表;
            o_是否刷新 = true;
            for (int i = 0; i < z玩家列表.Count; i++) {
                z玩家列表[i].o_游戏房间 = this;
                if (z玩家列表[i].o_Tcp通信 != null && z玩家列表[i].o_Tcp通信.o_通信器 != null) {
                    o_玩家Serv = z玩家列表[i];
                }
            }
            C_游戏房间.o_房间集合.Add(o_房间号, this);
            Console.WriteLine(o_房间号 + "_____房间创建完成-----");
        }
        public void S_刷新玩家服务器() {
            int z时间差 = C_Toot.S_Get时间搓int() - o_玩家Serv.o_Tcp通信.o_在线时间;
            if (z时间差 > 10) {
                Console.WriteLine(o_房间号 + "___" + C_Toot.S_Get时间搓int() + "____时间______" + o_玩家Serv.o_Tcp通信.o_在线时间 + "_____" + z时间差 + "_____" + o_是否刷新);
                o_玩家列表 = o_玩家列表.OrderByDescending(n => n.o_Tcp通信.o_在线时间).ToList();
                if (o_玩家列表[0] != o_玩家Serv) {
                    o_玩家Serv = o_玩家列表[0];
                } else if (o_玩家列表[1].o_Tcp通信.o_通信器 != null) {
                    o_玩家Serv = o_玩家列表[1];
                } else {
                    return;
                }
                Console.WriteLine("房间_" + o_房间号 + "__更新玩家服务器位置_" + o_玩家列表.IndexOf(o_玩家Serv));
                S_发送房间消息(660, o_玩家列表.IndexOf(o_玩家Serv) + "");
            }
        }
        public void S_发送房间消息(int z消息类型, string msg) {
            //byte[] z包体=Encoding.UTF8.GetBytes(msg);
            //byte[] z包类型=z消息类型.S_10进制到256进制(2);
            //byte[] z房间=Encoding.UTF8.GetBytes(o_房间号.ToString("0000000000"));
            //int z消息长度=16+z包体.Length;
            //byte[] z包长=z消息长度.S_10进制到256进制(4);
            //byte[] zData=z包类型.S_相加(z包长).S_相加(z房间).S_相加(z包体);
            ////C_业务.ooo.S_接口_消息转发(z消息类型, z房间号, zData, null);
            //foreach(var n in o_玩家列表) {
            //    if(n.o_是否电脑==false) {
            //        try {
            //            C_Udp业务.o_Udp通信.S_发送消息(zData, n.o_地址);
            //        } catch(Exception ex) {
            //            Console.WriteLine("转发消息出错:"+ex.Message);
            //        }
            //    }
            //}
        }
        public static void S_30秒_匹配刷新() {
            S_组队匹配检测();
            while (o_单人匹配列表.Count > 0) {
                S_匹配组(new List<C_玩家>(), new List<C_玩家>());
            }
        }
        static void S_组队匹配检测() {
            if (o_团队匹配列表.Count == 0) {
                return;
            }
            o_团队匹配列表 = o_团队匹配列表.OrderByDescending(n => n.Count).ToList();
            if (o_团队匹配列表.Count > 1) {
                List<C_玩家> z列表1 = o_团队匹配列表[0];
                o_团队匹配列表.RemoveAt(0);
                List<C_玩家> z列表2 = o_团队匹配列表[0];
                o_团队匹配列表.RemoveAt(0);
                S_匹配组(z列表1, z列表2);
                if (o_团队匹配列表.Count > 1) {
                    S_组队匹配检测();
                }
            } else if (o_团队匹配列表.Count == 1) {
                List<C_玩家> z列表1 = o_团队匹配列表[0];
                o_团队匹配列表.RemoveAt(0);
                S_匹配组(z列表1, new List<C_玩家>());
            }
        }
        static void S_匹配组(List<C_玩家> z列表1, List<C_玩家> z列表2) {
            Console.WriteLine("开始匹配________S_匹配组____");
            int z人员数量 = 2;
            while (z列表2.Count < z人员数量) {
                if (o_单人匹配列表.Count == 0) {
                    break;
                }
                if (z列表1.Count < z人员数量) {
                    z列表1.Add(o_单人匹配列表[0]);
                    o_单人匹配列表.RemoveAt(0);
                    if (o_单人匹配列表.Count == 0) {
                        break;
                    }
                }
                z列表2.Add(o_单人匹配列表[0]);
                o_单人匹配列表.RemoveAt(0);
            }
            Console.WriteLine("z列表1.Count___" + z列表1.Count + "________z列表2.Count__" + z列表2.Count);
            //List<C_玩家> z电脑玩家列表 = new List<C_玩家>();
            Random ran = new Random();
            //while (z电脑玩家列表.Count <10) {
            //    int z英雄编号 = ran.Next(C_玩家.O_电脑玩家列表.Count);
            //    C_玩家 nn=  C_玩家.O_电脑玩家列表[z英雄编号];
            //    if (C_玩家.O_电脑玩家列表.Contains(nn) == false) {
            //        z电脑玩家列表.Add(nn);
            //    }
            //}
            //for (int i = z列表1.Count; i < z人员数量; i++) {
            //    z列表1.Add(z电脑玩家列表[0]);
            //    z电脑玩家列表.RemoveAt(0);
            //}
            //for (int i = z列表2.Count; i < z人员数量; i++) {
            //    z列表2.Add(z电脑玩家列表[0]);
            //    z电脑玩家列表.RemoveAt(0);
            //}
            while (z列表2.Count < z人员数量) {
                int z英雄编号 = ran.Next(C_玩家.O_电脑玩家列表.Count);
                C_玩家 nn = C_玩家.O_电脑玩家列表[z英雄编号];
                if (z列表1.Contains(nn) == false && z列表2.Contains(nn) == false) {
                    if (z列表1.Count < z人员数量) {
                        z列表1.Add(nn);
                    } else {
                        z列表2.Add(nn);
                    }
                }
            }
            Console.WriteLine("整理完成______z列表1.Count___" + z列表1.Count + "________z列表2.Count__" + z列表2.Count);
            Random random = new Random();
            int z随机数 = random.Next(10);
            if (z随机数 < 5) {
                S_创建竞技房间(z列表1, z列表2);
            } else {
                S_创建竞技房间(z列表2, z列表1);
            }
        }
        static void S_创建竞技房间(List<C_玩家> z列表1, List<C_玩家> z列表2) {
            Console.WriteLine("__________S_创建竞技房间___________________");
            int z人员数量 = 2;
            z列表1 = z列表1.S_随机排序();
            z列表2 = z列表2.S_随机排序();
            List<C_玩家> z新列表 = new List<C_玩家>();
            for (int i = 0; i < z人员数量; i++) {
                z新列表.Add(z列表1[i]);
                z新列表.Add(z列表2[i]);
            }
            z列表1.Clear();
            z列表2.Clear();
            C_游戏房间 mm = new C_游戏房间(z新列表);
            string ss = mm.o_房间号.ToString("0000000000") + "@";
            for (int i = 0; i < z新列表.Count; i++) {
                ss += z新列表[i].o_ID + "&";
            }
            ss = ss.Remove(ss.Length - 1);
            for (int i = 0; i < z新列表.Count; i++) {
                try {
                    z新列表[i].o_Tcp通信.o_通信器.S_发送消息(505, ss + "@" + i);//房间号 @玩家ID&玩家ID... @z自身位置
                } catch {
                    //发送出错-----------
                }
            }
        }
    }
}
