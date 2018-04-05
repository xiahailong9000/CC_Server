using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Xml;
namespace CC_ServerDLL {
    //public delegate void Del_事件();
    //public delegate void Del_Tcp事件(C_TcpClient.C_逻辑端 zmm);
    //public delegate void Del_Tcp事件2(C_TcpClient2.C_逻辑端 zmm);
    public class C_TcpClient {
        public TcpClient o_通信对象;
        public m_IP o_IP端口;
        public byte[] o_消息数据 = new byte[1024];
        public C_逻辑端 o_逻辑端;
        public static void S_启动监听(int z端口,C_逻辑端 z逻辑代码) {
            Thread o_hh线程=new Thread(delegate() {
                try {
                    TcpListener  z监听器=new TcpListener(IPAddress.Any,z端口);
                    Console.WriteLine("  ---TCP服务器:{0} 已启动",z端口);
                    z监听器.Start();
                    while(true) {
                        TcpClient z实体=z监听器.AcceptTcpClient();
                        Thread hh2=new Thread(delegate() {
                            C_TcpClient o_用户=new C_TcpClient(z实体,z逻辑代码);
                        });
                        hh2.IsBackground=true;
                        hh2.Start();
                    }
                } catch(System.Exception ex) {
                    Console.WriteLine("端口出错uuuuuuuuuuuuuuuuu："+ex.Message);
                }
            });
            o_hh线程.IsBackground=true;
            o_hh线程.Start();
        }
        public C_TcpClient(TcpClient z实体,C_逻辑端 z逻辑代码) {
           // Console.WriteLine("客户端连接成功 _");
            o_通信对象=z实体;
            o_IP端口 = new m_IP(o_通信对象.Client.RemoteEndPoint.ToString());
            z是否主动链接 = false;
            z通信是否正常 = true;
            o_消息数据 = new byte[o_通信对象.ReceiveBufferSize];
            o_逻辑端 = z逻辑代码.S_克隆对象(this);
            Console.WriteLine(o_IP端口.O_String + " --------------------------------建立新的链接---------------------------------------------------------");
            try {
                o_通信对象.GetStream().BeginRead(o_消息数据, 0, System.Convert.ToInt32(o_通信对象.ReceiveBufferSize), S_接收消息, null);
            } catch (Exception ex) {
                Console.WriteLine("消息出错__" + ex.Message + "___" + ex.StackTrace);
            }
            //C_Toot.S_Debug输出("\nTcp客户端: " + o_IP端口.O_String + " 请求建立_连接_______\n",null, 0);
        }
        byte o_重连次数 = 5;
        byte o_当前次数 = 0;
        bool z通信是否正常,z是否主动链接;
        Action<C_逻辑端> o_链接成功事件;
        public C_TcpClient(string zIP, ushort z端口, byte z重连次数, C_逻辑端 z逻辑代码, Action<C_逻辑端> z成功事件) {
            o_逻辑端 = z逻辑代码.S_克隆对象(this);        
            o_IP端口 = new m_IP(zIP, z端口);
            o_链接成功事件 = z成功事件;
            z是否主动链接 = true;
            o_重连次数 = z重连次数;
            o_当前次数 = 0;
            o_通信对象 = new TcpClient();
            o_消息数据 = new byte[o_通信对象.ReceiveBufferSize];
            S_主动链接_初始化();
            S_循环链接();
        }
        void S_循环链接() {
            Thread thread = new Thread(() => {
                while (true) {
                    if (z是否主动链接 && z通信是否正常 == false) {
                        o_当前次数++;
                        if (o_重连次数 == 0) {
                            o_当前次数 = 0;
                        }
                        Console.WriteLine("断线重连___" + o_当前次数 + "___________" + o_IP端口.O_String);
                        if (o_当前次数 <= o_重连次数) {
                            o_通信对象 = null;
                            o_通信对象 = new TcpClient();
                            Thread.Sleep(3000);
                            S_主动链接_初始化();
                        }
                    }
                    Thread.Sleep(1);
                }
            });
            thread.Start();
        }
        void S_主动链接_初始化() {        
            if (z是否主动链接 == false) {
                return;
            }
            Console.WriteLine("发起主动链接___" + o_IP端口.O_String);
            z通信是否正常 = false;
            AsyncCallback mm = new AsyncCallback(S_接收消息);
            IAsyncResult z连接结果 = o_通信对象.BeginConnect(o_IP端口.O_IP, o_IP端口.port, mm,"dsdsdsd");
            //o_通信对象.EndConnect(z连接结果);
            z连接结果.AsyncWaitHandle.WaitOne(2000, true);  //z等待2秒---1
            if (z连接结果.IsCompleted == false) {
                S_失败重连(0,"链接失败了____",null);
            } else {
          
                z通信是否正常 = true;
                if (o_链接成功事件 != null) {
                    o_链接成功事件(o_逻辑端);
                }
                Console.WriteLine("链接成功___" + o_IP端口.O_String);
            }
        }
        void S_接收消息(IAsyncResult ar) {
            int z消息长度 = 0;
            try {
                lock(o_通信对象) {
                    z消息长度=o_通信对象.GetStream().EndRead(ar);
                }
                if(z消息长度<2) {
                    S_失败重连(5, "信息长度没有超过2", null);  
                    return;
                } else {
                    try {
                        byte[] z新数据 = new byte[z消息长度];
                        Array.Copy(o_消息数据,0,z新数据,0,z消息长度);
                        S_分包(z新数据);
                    } catch(Exception ex) {
                        C_Toot.S_Debug输出("消息处理出错4244: ",ex, 3);
                    }
                }
                lock(o_通信对象) {
                    o_通信对象.GetStream().BeginRead(o_消息数据,0,System.Convert.ToInt32(o_通信对象.ReceiveBufferSize),S_接收消息,null);
                }
            } catch(Exception ex) {
                S_失败重连(0,"接收链接异常",ex);          
            }
        }
        void S_失败重连(byte z当前次数,string z失败消息, Exception ex) {
            if (z是否主动链接 == false) {
                return;
            }
            if (o_通信对象 != null) {
                o_通信对象.Close();
            }
            o_当前次数 = z当前次数;
            z通信是否正常 = false;
            if (ex != null) {
                Console.WriteLine("主动链接出错_" + z失败消息 + "____" + ex.Message + "___" + ex.StackTrace);
            } else {
                Console.WriteLine("主动链接出错_" + z失败消息);
            }
        }
        public void S_发送消息(int z消息类型,string msg) {
            Console.WriteLine("==>>Tcp发送__" + z消息类型 + "____"+o_IP端口.O_IP+"___" + msg);
            byte[] z包体=Encoding.UTF8.GetBytes(msg);
            S_发送消息(z消息类型,z包体);
        }
        public void S_发送消息(int z消息类型,byte[] z包体) {
            byte[] z包类型 = z消息类型.S_10进制到256进制(2);
            int z消息长度 = 6 + z包体.Length;
            byte[] z包长 = z消息长度.S_10进制到256进制(4);
            byte[] zData = z包类型.S_相加(z包长).S_相加(z包体);
            S_发送消息(zData);
        }
        public void S_发送消息(byte[] data) {
            if (z通信是否正常 == false) {
                Console.WriteLine("---------------------------------z通信是否正常==faels-----------------");
                return;
            }
            try {
                NetworkStream ns = o_通信对象.GetStream();
                ns.Write(data, 0, data.Length);
                ns.Flush();
            } catch (Exception ex) {
                S_失败重连(0, "发送出错", ex);
            }
        }
        byte[] o_长数据;
        bool o_起始开关 = true;
        int o_数据累加长度;
        ushort o_消息类型;
        void S_分包(byte[] z新数据) {
            if(o_起始开关) {
                try {
                    o_消息类型=(ushort)z新数据.S_截取(0,2).S_256进制到10进制();
                    o_长数据=new byte[z新数据.S_截取(2,4).S_256进制到10进制()];
                } catch(Exception ex) {
                    try {
                        string ss = Encoding.UTF8.GetString(z新数据, 0, z新数据.Length);
                        Console.WriteLine("TCP消息接收出错_消息长度:{0}_", ss);
                    } catch (Exception) {}
                    Console.WriteLine("TCP消息接收出错_消息长度:{0}_消息类型:{0}", z新数据.Length, o_消息类型);
                    for (int i = 0; i < z新数据.Length; i++) {
                        Console.WriteLine(i + "_____错误_____" + z新数据[i]);
                    }
                    C_Toot.S_Debug输出("sa334; ",ex, 3);
                    return;
                }
                o_起始开关=false;
                o_数据累加长度=0;
                S_分包(z新数据);
            } else {
                if(o_数据累加长度+z新数据.Length>=o_长数据.Length) {
                    Array.Copy(z新数据,0,o_长数据,o_数据累加长度,o_长数据.Length-o_数据累加长度);
                    try {
                        if(o_逻辑端!=null) {
                            string msg="";
                            if(o_消息类型<20000) {
                                msg=Encoding.UTF8.GetString(o_长数据,6,o_长数据.Length-6);
                            } else {
                                C_Toot.S_Debug输出("Tcp收到20000:_" + o_IP端口.O_String + "__" + o_消息类型 + "__" + o_长数据.Length,null, 1);
                            }
                            try {
                                //Console.WriteLine("<<==Tcp收到___" + o_消息类型 + "___" + msg);
                                o_逻辑端.S_接口_消息处理(o_消息类型, msg, o_长数据);
                            } catch(Exception ex) {
                                C_Toot.S_Debug输出("消息处理出错58692:___" + msg, ex, 3);
                            }
                        }
                    } catch(Exception ex) {
                        C_Toot.S_Debug输出("解包出错4525:",ex, 3);//解包出错4525:索引超出了数组界限。
                    }
                    byte[] x数据 = new byte[z新数据.Length-(o_长数据.Length-o_数据累加长度)];
                    o_起始开关=true;
                    o_数据累加长度=x数据.Length;
                    if(x数据.Length!=0) {
                        Array.Copy(z新数据,z新数据.Length-x数据.Length,x数据,0,x数据.Length);
                        //C_Toot.S_Debug输出("粘包了______________",2);
                        S_分包(x数据);
                    }
                } else {
                    Array.Copy(z新数据,0,o_长数据,o_数据累加长度,z新数据.Length);
                    o_数据累加长度+=z新数据.Length;
                }
            }
        }
        public void S_关闭() {
            Console.WriteLine("ddddd========================================o_通信对象.Close();================================\n"+new StackTrace(true).ToString());
            o_通信对象.Close();
        }
        public class C_逻辑端 : ICloneable {
            public C_TcpClient o_通信器;
            //public virtual C_业务逻辑 S_接口_初始化(C_TcpClient nn) {
            //    if(o_通信器!=null) {
            //        C_业务逻辑 mm= new C_业务逻辑();
            //        return mm.S_接口_初始化(nn);
            //    } else {
            //        o_通信器=nn;
            //        return this;
            //    }
            //}
            public C_逻辑端 S_克隆对象(C_TcpClient nn) {
                C_逻辑端 mm = this.Clone() as C_逻辑端;
                mm.o_通信器 = nn;
                return mm;
            }
            public object Clone() {
                return this.MemberwiseClone();
            }
            public virtual void S_接口_消息处理(ushort z消息类型, string msg, byte[] zData) {
                Console.WriteLine("拷贝失败____TCP——收到--" + z消息类型 + "_____" + msg);
            }
            public virtual void S_接口_关闭() {
                if (o_通信器 != null && o_通信器.o_通信对象 != null) {
                    o_通信器.o_通信对象.Close();
                }
                o_通信器 = null;      
            }
        }
    }
    public class C_TcpClient2 {
        public TcpClient o_通信对象;
        public m_IP o_IP端口;
        public byte[] o_消息数据 = new byte[8192];
        public C_逻辑端 o_逻辑端;
        byte o_重连次数 = 5;
        byte o_当前次数 = 0;
        bool z通信是否正常, z是否主动链接;
        Action<C_逻辑端> o_链接成功事件;
        public static void S_启动监听(int z端口, C_逻辑端 z逻辑代码) {
            Thread o_hh线程 = new Thread(delegate() {
                try {
                    TcpListener z监听器 = new TcpListener(IPAddress.Any, z端口);
                    Console.WriteLine("  ---TCP服务器:{0} 已启动", z端口);
                    z监听器.Start();
                    while (true) {
                        TcpClient z实体 = z监听器.AcceptTcpClient();
                        Thread hh2 = new Thread(delegate() {
                            C_TcpClient2 o_用户 = new C_TcpClient2(z实体, z逻辑代码);
                        });
                        hh2.IsBackground = true;
                        hh2.Start();
                    }
                } catch (System.Exception ex) {
                    Console.WriteLine("端口出错uuuuuuuuuuuuuuuuu：" + ex.Message);
                }
            });
            o_hh线程.IsBackground = true;
            o_hh线程.Start();
        }
        public C_TcpClient2(TcpClient z实体, C_逻辑端 z逻辑代码) {
            // Console.WriteLine("客户端连接成功 _");
            o_通信对象 = z实体;
            o_IP端口 = new m_IP(o_通信对象.Client.RemoteEndPoint.ToString());
            o_消息数据 = new byte[o_通信对象.ReceiveBufferSize];
            o_通信对象.GetStream().BeginRead(o_消息数据, 0, System.Convert.ToInt32(o_通信对象.ReceiveBufferSize), S_接收消息, null);
            o_逻辑端 = z逻辑代码.S_克隆对象(this);
            z是否主动链接 = false;
            z通信是否正常 = true;
            //C_Toot.S_Debug输出("\n\n\nTcp客户端: " + o_IP端口.O_String + " 已经连接到服务器_______",null, 0);
        }   
        public C_TcpClient2(string zIP, ushort z端口, byte z重连次数, C_逻辑端 z逻辑代码, Action<C_逻辑端> z成功事件) {
            o_逻辑端 = z逻辑代码.S_克隆对象(this);
            o_IP端口 = new m_IP(zIP, z端口);
            o_链接成功事件 = z成功事件;
            z是否主动链接 = true;
            o_重连次数 = z重连次数;
            o_通信对象 = new TcpClient();
            o_当前次数 = 0;
            S_主动链接_初始化();
            S_循环链接();
        }
        void S_循环链接() {
            Thread thread = new Thread(() => {
                while (true) {
                    if (z通信是否正常 == false && z是否主动链接) {
                        o_当前次数++;
                        if (o_重连次数 == 0) {
                            o_当前次数 = 0;
                        }
                        Console.WriteLine("断线重连___" + o_当前次数+"_____"+o_重连次数+"______" + o_IP端口.O_String);
                        if (o_当前次数 <= o_重连次数) {
                            o_通信对象 = null;
                            o_通信对象 = new TcpClient();
                            Thread.Sleep(3000);
                            S_主动链接_初始化();
                        } else {
                            z通信是否正常 = true;
                        }
                    }
                    Thread.Sleep(1);
                }
            });
            thread.Start();
        }
        void S_主动链接_初始化() {
            if (z是否主动链接 == false) {
                return;
            }
            //Console.WriteLine("主动发起链接___" + o_IP端口.O_String);
            try {
                AsyncCallback mm = new AsyncCallback(S_接收消息2);
                IAsyncResult z连接结果 = o_通信对象.BeginConnect(o_IP端口.O_IP, o_IP端口.port, mm, null);
                o_通信对象.EndConnect(z连接结果);
                z通信是否正常 = true;
                Console.WriteLine("链接成功___" + o_IP端口.O_String);
                o_当前次数 = 0;
                if (o_链接成功事件 != null) {
                    o_链接成功事件(o_逻辑端);
                }
            } catch (Exception ex) {
                S_失败初始化(o_当前次数, "链接出错", ex);
            }
        }
        void S_接收消息2(IAsyncResult ar) {
            int z消息长度 = 0;
            //o_消息数据 = new byte[o_通信对象.ReceiveBufferSize];
            do {
                try {
                    if (o_通信对象.Client == null) {
                        S_失败初始化(o_当前次数, "o_通信对象.Client == null", null);
                        return;
                    }
                    z消息长度 = o_通信对象.Client.Receive(o_消息数据);
                    if (z消息长度 > 2) {
                        try {
                            byte[] z新数据 = new byte[z消息长度];
                            Array.Copy(o_消息数据, 0, z新数据, 0, z消息长度);
                            S_分包(z新数据);
                        } catch (Exception ex) {
                            Console.WriteLine("消息处理出错_______dddddddddddddd__"+ex.Message+"___"+ex.StackTrace);
                        }
                    } else {
                        S_失败初始化(o_当前次数, "消息长度<2", null);
                    }
                } catch (Exception ex) {
                    S_失败初始化(o_当前次数, "断开链接", ex);
                }
            } while (z消息长度 > 0);
        }
        void S_失败初始化(byte z当前次数, string z失败消息, Exception ex) {
            if (o_通信对象 != null) {
                //o_通信对象.Close();
            }
            o_当前次数 = z当前次数;
            z通信是否正常 = false;
            if (ex != null) {
                Console.WriteLine("主动链接出错_" + z失败消息 + "____" + ex.Message + "___" + ex.StackTrace);
            } else {
                Console.WriteLine("主动链接出错_" + z失败消息);
            }
        }
        void S_接收消息(IAsyncResult ar) {
            int z消息长度 = 0;
            try {
                lock (o_通信对象) {
                    z消息长度 = o_通信对象.GetStream().EndRead(ar);
                }
                if (z消息长度 < 2) {
                    C_Toot.S_Debug输出("玩家>" + o_IP端口.O_String + "  离开--- 信息长度没有超过2", null, 3);
                    //o_通信对象.Close();
                    o_当前次数 = 0;
                    return;
                } else {
                    try {
                        byte[] z新数据 = new byte[z消息长度];
                        Array.Copy(o_消息数据, 0, z新数据, 0, z消息长度);
                        S_分包(z新数据);
                    } catch (Exception ex) {
                        C_Toot.S_Debug输出("消息处理出错4244: ", ex, 3);
                    }
                }
                lock (o_通信对象) {
                    o_通信对象.GetStream().BeginRead(o_消息数据, 0, System.Convert.ToInt32(o_通信对象.ReceiveBufferSize), S_接收消息, null);
                }
            } catch (Exception ex) {
                C_Toot.S_Debug输出("玩家>" + o_IP端口.O_String + "  离开:---", ex, 3);
                //o_通信对象.Client.Close();
                o_当前次数 = 0;
                z通信是否正常 = false;
            }
        }
        public void S_发送消息(int z消息类型, string msg) {
            Console.WriteLine("==>>Tcp发送__" + z消息类型 + "____"+o_IP端口.O_IP+"____"+ msg);
            byte[] z包体 = Encoding.UTF8.GetBytes(msg);
            S_发送消息(z消息类型, z包体);
        }
        public void S_发送消息(int z消息类型, byte[] z包体) {
            byte[] z包类型 = z消息类型.S_10进制到256进制(2);
            int z消息长度 = 6 + z包体.Length;
            byte[] z包长 = z消息长度.S_10进制到256进制(4);
            byte[] zData = z包类型.S_相加(z包长).S_相加(z包体);
            S_发送消息(zData);
        }
        public void S_发送消息(byte[] data) {
            if (z通信是否正常 == false) {
                Console.WriteLine("消息不能发送_____" + o_IP端口.O_String+"__"+new StackTrace(true).ToString());
                return;
            }
            try {
                NetworkStream ns = o_通信对象.GetStream();
                ns.Write(data, 0, data.Length);
                ns.Flush();
            } catch (Exception ex) {
                S_失败初始化(o_当前次数, "发送出错", ex);
            }
        }
        byte[] o_长数据;
        bool o_起始开关 = true;
        int o_数据累加长度;
        ushort o_消息类型;
        void S_分包(byte[] z新数据) {
            if (o_起始开关) {
                try {
                    o_消息类型 = (ushort)z新数据.S_截取(0, 2).S_256进制到10进制();
                    o_长数据 = new byte[z新数据.S_截取(2, 4).S_256进制到10进制()];
                } catch (Exception ex) {
                    try {
                        string ss = Encoding.UTF8.GetString(z新数据, 0, z新数据.Length);
                        Console.WriteLine("TCP消息接收出错_消息长度:{0}_", ss);
                    } catch (Exception ex2) {
                        Console.WriteLine("出错___" + ex2.Message + "___" + ex2.StackTrace);
                    }
                    Console.WriteLine("TCP消息接收出错_消息长度:{0}_消息类型:{0}", z新数据.Length, o_消息类型);
                    for (int i = 0; i < z新数据.Length; i++) {
                        Console.WriteLine(i + "_____错误_____" + z新数据[i]);
                    }
                    C_Toot.S_Debug输出("sa334; ", ex, 3);
                    return;
                }
                o_起始开关 = false;
                o_数据累加长度 = 0;
                S_分包(z新数据);
            } else {
                if (o_数据累加长度 + z新数据.Length >= o_长数据.Length) {
                    Array.Copy(z新数据, 0, o_长数据, o_数据累加长度, o_长数据.Length - o_数据累加长度);
                    try {
                        if (o_逻辑端 != null) {
                            string msg = "";
                            if (o_消息类型 < 20000) {
                                if (o_长数据.Length < 6) {
                                    return;
                                }
                                msg = Encoding.UTF8.GetString(o_长数据, 6, o_长数据.Length - 6);
                            } else {
                                C_Toot.S_Debug输出("Tcp收到20000:_" + o_IP端口.O_String + "__" + o_消息类型 + "__" + o_长数据.Length, null, 1);
                            }
                            try {
                                //Console.WriteLine("<<==Tcp收到___" + o_消息类型 + "___" + msg);
                                o_逻辑端.S_接口_消息处理(o_消息类型, msg, o_长数据);
                            } catch (Exception ex) {
                                C_Toot.S_Debug输出("消息处理出错58692:___" + msg, ex, 3);
                            }
                        }
                    } catch (Exception ex) {
                        C_Toot.S_Debug输出("解包出错4525:", ex, 3);//解包出错4525:索引超出了数组界限。
                    }
                    byte[] x数据 = new byte[z新数据.Length - (o_长数据.Length - o_数据累加长度)];
                    o_起始开关 = true;
                    o_数据累加长度 = x数据.Length;
                    if (x数据.Length != 0) {
                        Array.Copy(z新数据, z新数据.Length - x数据.Length, x数据, 0, x数据.Length);
                        //C_Toot.S_Debug输出("粘包了______________",2);
                        S_分包(x数据);
                    }
                } else {
                    Array.Copy(z新数据, 0, o_长数据, o_数据累加长度, z新数据.Length);
                    o_数据累加长度 += z新数据.Length;
                }
            }
        }
        public void S_关闭() {
            Console.WriteLine("ddddd========================================o_通信对象.Close();================================\n" + new StackTrace(true).ToString());
            if (o_通信对象 != null) {
                o_通信对象.Close();
            }
        }
        public class C_逻辑端 : ICloneable {
            public C_TcpClient2 o_通信器;
            //public virtual C_业务逻辑 S_接口_初始化(C_TcpClient nn) {
            //    if(o_通信器!=null) {
            //        C_业务逻辑 mm= new C_业务逻辑();
            //        return mm.S_接口_初始化(nn);
            //    } else {
            //        o_通信器=nn;
            //        return this;
            //    }
            //}
            public C_逻辑端 S_克隆对象(C_TcpClient2 nn) {
                C_逻辑端 mm = this.Clone() as C_逻辑端;
                mm.o_通信器 = nn;
                return mm;
            }
            public object Clone() {
                return this.MemberwiseClone();
            }
            public virtual void S_接口_消息处理(ushort z消息类型, string msg, byte[] zData) {
                Console.WriteLine("拷贝失败____TCP——收到--" + z消息类型 + "_____" + msg);
            }
        }
        void S_mmm() {
            int  i = 123;
            byte [] intBuff = BitConverter.GetBytes(i);     // 将 int 转换成字节数组
            //lob.Write(intBuff, 0, 4);
            i = BitConverter.ToInt32(intBuff, 0);           // 从字节数组转换成 int

            double x = 123.456;
            byte [] doubleBuff = BitConverter.GetBytes(x);  // 将 double 转换成字节数组
            //lob.Write(doubleBuff, 0, 8);
            x = BitConverter.ToDouble(doubleBuff, 0);       // 从字节数组转换成 double
        }
    }
    public class C_UdpClient {
        public UdpClient o_通信对象;
        Thread o_线程;
        Action<C_UdpClient,IPEndPoint,byte[]> o_消息处理;
        public static C_UdpClient S_启动监听(int z端口, Action<C_UdpClient, IPEndPoint, byte[]> z消息处理) {
            C_UdpClient ooo =new C_UdpClient(z端口,z消息处理);
            return ooo;
        }
        C_UdpClient(int z端口, Action<C_UdpClient, IPEndPoint, byte[]> z消息处理) {
            o_消息处理=z消息处理;
            o_通信对象=new UdpClient(z端口);
            Console.WriteLine("  ---Udp服务器:{0} 已启动",z端口);                                    
            o_线程=new Thread(delegate() {
                S_接收消息();
            });
            o_线程.IsBackground=true;
            o_线程.Start();
        }
        void S_接收消息() {
            IPEndPoint z地址 = new IPEndPoint(IPAddress.Any,0);
            while(true) {
                try {
                    byte[] data = o_通信对象.Receive(ref z地址);
                    if (o_消息处理 != null) {
                        try {
                            o_消息处理(this, z地址,data);
                        } catch (Exception ex) {
                            Console.WriteLine("Udp消息处理出错0: " + ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                } catch (Exception ex) {
                    Console.WriteLine("Udp消息处理出错1: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
        public void S_发送消息(byte z消息类型,string msg,string s地址) {
            string[] sss=s地址.Split(':');
            S_发送消息(z消息类型,msg,sss[0],int.Parse(sss[1]));
        }
        public void S_发送消息(byte z消息类型, string msg, string IP, int z端口) {
            IPEndPoint z地址=new IPEndPoint(IPAddress.Parse(IP),z端口);
            S_发送消息(z消息类型,msg,z地址);
        }
        public void S_发送消息(byte z消息类型, string msg, IPEndPoint z地址) {
            byte[] z包类型 = new byte[] { z消息类型 };
            byte[] z包体 = Encoding.UTF8.GetBytes(msg);
            byte[] zData = z包类型.S_相加(z包体);
            S_发送消息(zData, z地址);
        }
        public void S_发送消息(byte[] zData, string z地址0) {
            string[] sss = z地址0.Split(':');
            IPEndPoint z地址 = new IPEndPoint(IPAddress.Parse(sss[0]), int.Parse(sss[1]));
            S_发送消息(zData, z地址);
        }
        public void S_发送消息(byte[] data, IPEndPoint z地址) {
            try {
                o_通信对象.Send(data,data.Length,z地址);
            } catch (Exception ex) {
                Console.WriteLine("Udp发送消息出错55: " + ex.Message + "\n" + ex.StackTrace);
            }
        }
        public void S_关闭监听() {
            o_线程.Abort();
            o_通信对象.Close();
        }
    }
    public class C_Toot {
        public static void S_自启动设置() {
            Thread.Sleep(1000);
            C_Toot.S_Debug输出("\n帮助提示：\n   输入 1 开启自启动;\n   输入 2 关闭自启动;\n",null,2);
            string ss=Console.ReadLine();
            SetAutoRun(System.Reflection.Assembly.GetExecutingAssembly().Location,ss);
            S_自启动设置();
        }
        /// <summary>     
        /// 设置应用程序开机自动运行  
        /// <para>SetAutoRun(@"D:\CSharpStart.exe",true);</para>
        /// </summary>     
        static void SetAutoRun(string fileName,string ss) {
            RegistryKey reg=null;
            try {
                if(!System.IO.File.Exists(fileName))
                    C_Toot.S_Debug输出("错误：该文件不存在!",null,3);
                String name=fileName.Substring(fileName.LastIndexOf(@"\")+1);
                reg=Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",true);
                if(reg==null)
                    reg=Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                C_Toot.S_Debug输出(name+"     "+fileName,null,1);
                if(ss=="1") {
                    reg.SetValue(name,fileName);
                    C_Toot.S_Debug输出("设置开机自启动————成功",null,0);
                } else if(ss=="2") {
                    reg.SetValue(name,false);
                    C_Toot.S_Debug输出("关闭开机自启动————成功",null,0);
                } else {
                    C_Toot.S_Debug输出("输入的内容无法识别----------424-----------",null,0);
                }
            } catch(Exception ex) {
                C_Toot.S_Debug输出("设置失败 ： ({0})",ex,3);
            } finally {
                if(reg!=null) {
                    reg.Close();
                }
            }
        }
        /// <summary>
        /// <para>0:绿色，1:白色，2:黄色，3:红色</para>
        /// </summary>
        public static void S_Debug输出(string ss,Exception ex,int ii) {
            if(ii==0) {
                Console.ForegroundColor=ConsoleColor.Cyan;//绿色 --成功
            } else if(ii==1) {
                Console.ForegroundColor=ConsoleColor.White;//白色---正常
            } else if(ii==2) {
                Console.ForegroundColor=ConsoleColor.Yellow;//黄色---警告
            } else if(ii==3) {
                Console.ForegroundColor=ConsoleColor.Red;//红色---错误
            }
            if (ex == null) {
                Console.WriteLine(ss);
            } else {
                Console.WriteLine(ss+"_____________"+ex.Message+"\n"+ex.StackTrace);
            }         
        }
        public static long S_Get时间搓long() {
            TimeSpan ts = DateTime.UtcNow-new DateTime(1970,1,1,0,0,0,0);
            long ll=Convert.ToInt64(ts.TotalSeconds);
            return ll;
        }
        public static int S_Get时间搓int() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(2017, 1, 1, 0, 0, 0, 0);
            int ll = Convert.ToInt32(ts.TotalSeconds);
            return ll;
        }
        public static IEnumerable<string> S_模糊查询(IEnumerable<string> z集合,string z关键词) {
            IEnumerable<string> z数组 = from ss in z集合
                                      where ss.Contains(z关键词)
                                      select ss;
            return z数组;
        }
        public static string S_获取本机IP() {
            string name = Dns.GetHostName();
            string ss = "";
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            foreach (IPAddress ipa in ipadrlist) {
                Console.WriteLine(ipa.ToString() + "_____________" + ipa.AddressFamily);
                if (ipa.AddressFamily == AddressFamily.InterNetwork) {
                    //Console.Writeline(ipa.ToString());
                    ss= ipa.ToString();
                    //return ss;
                }
            }
            return ss;
            //IPHostEntry fromHE = Dns.GetHostEntry(Dns.GetHostName());
            //IPEndPoint ipEndPointFrom = new IPEndPoint(fromHE.AddressList[0], 80);
            //EndPoint EndPointFrom = (ipEndPointFrom);
            //return EndPointFrom.ToString();
        }
        public static string S_GetIP() {    
              Process cmd = new Process();    
              cmd.StartInfo.FileName = "ipconfig.exe";//设置程序名     
              cmd.StartInfo.Arguments = "/all";  //参数     
       //重定向标准输出     
              cmd.StartInfo.RedirectStandardOutput = true;    
              cmd.StartInfo.RedirectStandardInput = true;    
              cmd.StartInfo.UseShellExecute = false;    
             cmd.StartInfo.CreateNoWindow = true;//不显示窗口（控制台程序是黑屏）     
      //cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//暂时不明白什么意思     
             /*  
    13  收集一下 有备无患  
    14         关于:ProcessWindowStyle.Hidden隐藏后如何再显示？  
    15         hwndWin32Host = Win32Native.FindWindow(null, win32Exinfo.windowsName);  
    16         Win32Native.ShowWindow(hwndWin32Host, 1);     //先FindWindow找到窗口后再ShowWindow  
    17         */    
             cmd.Start();    
             string info = cmd.StandardOutput.ReadToEnd();    
             cmd.WaitForExit();    
             cmd.Close();
             return info;
         }
        public class C_xml {
            static C_xml ccc;
            public static Dictionary<string,List<C_节点>> S_获取配置信息(string[] sss) {
                if(ccc==null) {
                    ccc=new C_xml();
                }
                string path=System.Environment.CurrentDirectory+"\\配置文件.txt";
                if(File.Exists(path)==false) {
                    Console.WriteLine("配置文件.txt 信息错误,请尽快进行配置 \n"+path);
                    ccc.S_创建xml文件(path,sss);
                    return null;
                } else {
                    ccc.S_读取xml(path);
                    Console.WriteLine("配置文件-------------------");
                    foreach(string ss2 in ccc.o_数据列表.Keys) {
                        Console.WriteLine(ss2);
                        foreach(C_xml.C_节点 nn in ccc.o_数据列表[ss2]) {
                            Console.WriteLine("      {0}    {1}   {2}   {3}",nn.m01,nn.m02,nn.m03,nn.m值);
                        }
                    }
                    return ccc.o_数据列表;
                }
            }
            public Dictionary<string,List<C_节点>> o_数据列表=new Dictionary<string,List<C_节点>>();
            void S_创建xml文件(string path,string[] sss) {
                XmlDocument xmlDoc = new XmlDocument(); //创建类型声明节点
                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0","utf-8","");
                xmlDoc.AppendChild(node);//创建根节点
                XmlNode root = xmlDoc.CreateElement("root");
                xmlDoc.AppendChild(root);
                for(int i=0;i<sss.Length;i++) {
                    S_创建节点组(xmlDoc,root,sss[i]);
                }
                try {
                    xmlDoc.Save(path);
                } catch (Exception ex) {
                    Console.WriteLine("出错___" + ex.Message + "___" + ex.StackTrace);
                }
            }
            void S_创建节点组(XmlDocument xmlDoc,XmlNode root,string z名字) {
                XmlNode nn = xmlDoc.CreateElement(z名字);
                root.AppendChild(nn);
                for(int i=0;i<5;i++) {
                    XmlElement node = xmlDoc.CreateElement("mm");
                    node.SetAttribute("m1","0");
                    node.SetAttribute("m2","0");
                    node.SetAttribute("m3","0");
                    node.InnerText=i+"m";
                    nn.AppendChild(node);
                }
            }
            void S_读取xml(string path) {
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                XmlNode nn = xml.SelectSingleNode("root");
                XmlNodeList nns = nn.ChildNodes;
                foreach(XmlNode n in nns) {
                    XmlNodeList nns2 = n.ChildNodes;
                    o_数据列表[n.Name]=new List<C_节点>();
                    foreach(XmlNode n2 in nns2) {
                        o_数据列表[n.Name].Add(new C_节点(n2));
                    }
                }
            }
            public class C_节点 {
                public string m名字,m01,m02,m03,m值;
                public C_节点(XmlNode nn) {
                    m名字=nn.Name;
                    m01=nn.Attributes["m1"].Value;
                    m02=nn.Attributes["m2"].Value;
                    m03=nn.Attributes["m3"].Value;
                    m值=nn.InnerText;
                }
            }
        }
        public class C_定时任务 {
            public delegate void Del_定时事件();
            Thread o_线程;
            System.Timers.Timer o_Timer;
            public C_定时任务(int z延时时间,bool z是否循环,Del_定时事件 z定时事件) {
                if(C_CPU数量.S_CPU数量()>2) {
                    o_线程=new Thread(delegate() {
                        S_多线程_循环事件(z延时时间,z是否循环,z定时事件);
                    });
                    o_线程.IsBackground=true;
                    o_线程.Start();
                } else {
                    o_Timer = new System.Timers.Timer(z延时时间*1000);   //实例化Timer类，设置间隔时间毫秒；   
                    //o_Timer.Elapsed+=new ElapsedEventHandler(theout); //到达时间的时候执行事件；  
                    o_Timer.Elapsed+=delegate(object source,ElapsedEventArgs e) {
                        if(z定时事件!=null) {
                            z定时事件();
                            if(z是否循环==false) {
                                S_Stop();
                            }
                        }
                    }; //到达时间的时候执行事件；
                    o_Timer.AutoReset=z是否循环;   //设置是执行一次（false）还是一直执行(true)；   
                    o_Timer.Enabled=true;     //是否执行System.Timers.Timer.Elapsed事件； 
                }
            }
            void S_多线程_循环事件(int z延时时间,bool z是否循环, Del_定时事件 z定时事件) {
                Thread.Sleep(z延时时间*1000);
                if(z定时事件!=null) {
                    z定时事件();
                }
                if(z是否循环==true) {
                    S_多线程_循环事件(z延时时间,z是否循环,z定时事件);
                } else {
                    S_Stop();
                }
            }
            public void S_Stop() {
                if(o_线程!=null) {
                    o_线程.Abort();
                }
                if(o_Timer!=null) {
                    o_Timer.Stop();
                }
            }
            ~C_定时任务() {
                S_Stop();
            }
        }
        class C_CPU数量 {
            //引入32库，kernel32.dll内封装了WIN32 API          
            [DllImport("kernel32")]
            public static extern void GetSystemInfo(ref CPU_INFO cpuinfo);
            [StructLayout(LayoutKind.Sequential)]
            public struct CPU_INFO {
                public uint dwOemId;
                public uint dwPageSize;
                public uint lpMinimumApplicationAddress;
                public uint lpMaximumApplicationAddress;
                public uint dwActiveProcessorMask;
                public uint dwNumberOfProcessors;
                public uint dwProcessorType;
                public uint dwAllocationGranularity;
                public uint dwProcessorLevel;
                public uint dwProcessorRevision;
            }
            static uint o_数量=0;
            public static uint S_CPU数量() {
                if(o_数量==0) {
                    CPU_INFO CpuInfo;
                    CpuInfo=new CPU_INFO();
                    //设置为引用类型，可以让CpuInfo的值可以被修改  
                    GetSystemInfo(ref CpuInfo);
                    o_数量= CpuInfo.dwNumberOfProcessors;
                }
                return o_数量;
            }
        }
        public static int S_当前行号() {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
            return st.GetFrame(0).GetFileLineNumber();
        }
    }  
    public static class C_扩展方法_string {
        public static bool S_是否合格(this string ss) {
            if(ss.Contains("@")) {
                return false;
            }
            if(ss.Contains("&")) {
                return false;
            }
            if(ss.Contains("*")) {
                return false;
            }
            if(ss.Contains("_")) {
                return false;
            }
            return true;
        }
        public static string S_10进制到62进制(this int z数字) {
            return S_10进制到62进制((long)z数字);
        }
        public static string S_10进制到62进制(this long z数字) {
            int z进制=62;
            byte[] bb=new byte[20];
            int i=0;
            while(z数字!=0) {
                i++;
                bb[bb.Length-i]=byte.Parse((z数字%z进制).ToString());
                z数字/=z进制;
            }
            //0=0,A=36,a=10
            //0=48,A=65,a=97
            string ss="";
            for(int i2=bb.Length-i;i2<bb.Length;i2++) {
                ss+=" "+bb[i2];
                if(bb[i2]<10) {
                    bb[i2]=byte.Parse((bb[i2]+48).ToString());
                } else if(bb[i2]<36) {
                    bb[i2]=byte.Parse((bb[i2]+87).ToString());
                } else {
                    bb[i2]=byte.Parse((bb[i2]+29).ToString());
                }
            }
            return System.Text.Encoding.UTF8.GetString(bb,bb.Length-i,i);
        }
        public static long S_62进制到10进制(this string ss) {
            int z进制=62;
            long ll=0;
            byte[] bb=Encoding.UTF8.GetBytes(ss);
            for(int i=0;i<bb.Length;i++) {
                if(bb[i]>96) {
                    bb[i]=byte.Parse((bb[i]-87).ToString());
                } else if(bb[i]>64) {
                    bb[i]=byte.Parse((bb[i]-29).ToString());
                } else {
                    bb[i]=byte.Parse((bb[i]-48).ToString());
                }
                ll+=bb[i]*(long)Math.Pow(z进制,bb.Length-i-1);
            }
            return ll;
        }
        public static List<string> S_模糊查询(this IEnumerable<string> z集合,string z关键词) {     
            IEnumerable<string> z列表 = from ss in z集合
                                      where ss.Contains(z关键词)
                                      select ss;
            return z列表.ToList();//没有查询到结果  z列表.Count==0
        }
    }
    public static class C_扩展方法_byte {
        public static byte[] S_10进制到256进制(this int ii,int z位数) {
            byte[] zData = new byte[z位数];
            for(int i=0;i<z位数;i++) {
                zData[i]=(byte)(ii>>8*i&0xff);
                if (ii >> 8 * i < 256) {
                    break;
                }
            }
            return zData;
        }
        public static int S_256进制到10进制(this byte[] zData) {
            int ii = 0;
            for (int i = 0; i < zData.Length; i++) {
                ii += (int)(zData[i] << 8 * i);
            }
            return ii;
        }
        public static int S_256进制到10进制(this byte[] zData, int z开始位置, int z长度) {
            int ii = 0;
            for (int i = 0; i < z长度; i++) {
                ii += (int)(zData[i + z开始位置] << 8 * i);
            }
            return ii;
        }
        public static byte[] S_相加(this byte[] zData,byte[] bb) {
            byte[] b3 = new byte[zData.Length+bb.Length];
            Array.Copy(zData,0,b3,0,zData.Length);
            Array.Copy(bb,0,b3,zData.Length,bb.Length);
            return b3;
        }
        public static byte[] S_截取(this byte[] zData,int z开始位置,int z截取长度) {
            byte[] bb=new byte[z截取长度];
            Array.Copy(zData,z开始位置,bb,0,bb.Length);
            return bb;
        }
    }
    public static class C_扩展方法_DateTime {
        /// <summary>
        /// 时间转换为标准字符窜-------
        /// </summary>
        public static string S_String(this DateTime v时间) {  //
            if(v时间.S_是否为空()==false) {
                return v时间.ToString("yyyy.M.d.H.m.s");
            } else {
                return "0";
            }
        }
        /// <summary>
        /// 两位数时间 2天21小时
        /// </summary>
        public static string S_时间字符(this float v秒) {
            return S_时间字符((int)v秒);
        }
        /// <summary>
        /// 两位数时间 2天21小时
        /// </summary>
        public static string S_时间字符(this int v秒) {
            string ss="";
            int o_天=v秒/86400;
            int o_时=(v秒%86400)/3600;
            int o_分=(v秒%3600)/60;
            int o_秒=(v秒%60);
            if(o_天>0) {
                ss="<color=#00ff00>"+o_天+"</color>天";
                if(o_时>0) {
                    ss+="<color=#00ff00>"+o_时+"</color>小时";
                }
            } else {
                if(o_时>0) {
                    ss+="<color=#00ff00>"+o_时+"</color>小时";
                    if(o_分>0) {
                        ss+="<color=#00ff00>"+o_分+"</color>分钟";
                    }
                } else {
                    if(o_分>0) {
                        ss+="<color=#00ff00>"+o_分+"</color>分钟";
                        if(o_秒>0) {
                            ss+="<color=#00ff00>"+o_秒+"</color>秒";
                        }
                    } else {
                        if(o_秒>0) {
                            ss+="<color=#00ff00>"+o_秒+"</color>秒";
                        } else {
                            ss+="<color=#00ff00>"+0+"</color>秒";
                        }
                    }
                }
            }
            return ss;
        }
        /// <summary>
        /// 时间字符串转换为时间
        /// </summary>
        public static DateTime S_DateTime(this string ss) {
            if(ss.Length>6) {
                string[] sa=ss.Split("."[0]);
                return new DateTime(int.Parse(sa[0]),int.Parse(sa[1]),
                    int.Parse(sa[2]),int.Parse(sa[3]),int.Parse(sa[4]),int.Parse(sa[5]));
            } else {
                return new DateTime(1990,2,2,2,2,2);
            }
        }
        public static bool S_是否为空(this DateTime dt) {
            if(DateTime.Compare(dt,new DateTime(1995,2,2,2,2,2))<0) {
                return true;
            } else {
                return false;
            }
        }
        public static int S_时间差值(this DateTime v开始时间,DateTime v结束时间) {
            if(v开始时间.S_是否为空()==true) {
                v开始时间=new DateTime(1990,2,2,2,2,2);
            }
            if(v结束时间.S_是否为空()==true) {
                v结束时间=new DateTime(1990,2,2,2,2,2);
            }
            TimeSpan v开始 = new TimeSpan(v开始时间.Ticks);
            TimeSpan v结束 = new TimeSpan(v结束时间.Ticks);
            TimeSpan ts = v结束.Subtract(v开始);//Subtract(相减)//Add(相加)   
            ts=ts.Duration();
            return ts.Days*24*3600+ts.Hours*3600+ts.Minutes*60+ts.Seconds;
        }
        public static int S_Get时间搓(this DateTime z时间) {  //1970年 的秒数---------------
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            int ll = Convert.ToInt32(ts.TotalSeconds);
            return ll;
        }
    }
    public struct m_IP {
        public byte o_IP0, o_IP1, o_IP2, o_IP3;
        public ushort port;
        //public m_IP(int ii) {
        //    Random ran = new Random();
        //    o_IP组 = new byte[4];
        //    o_IP组[0] = (byte)ran.Next(256);
        //    o_IP组[1] = (byte)ran.Next(256);
        //    o_IP组[2] = (byte)ran.Next(256);
        //    o_IP组[3] = (byte)ran.Next(256);
        //    port = (ushort)ran.Next(65535);
        //}
        public m_IP(string zIP_Pot) {
            o_IP0 = 0;
            o_IP1 = 0;
            o_IP2 = 0;
            o_IP3 = 0;
            port = 0;
            string[] sss = zIP_Pot.Split(':');
            S_mm(sss[0], ushort.Parse(sss[1]));
        }
        public m_IP(string ip, ushort z端口) {
            o_IP0 = 0;
            o_IP1 = 0;
            o_IP2 = 0;
            o_IP3 = 0;
            port = 0;
            S_mm(ip, z端口);
        }
        void S_mm(string ip, ushort z端口) {
            string[] sss = ip.Split('.');
            o_IP0 = byte.Parse(sss[0]);
            o_IP1 = byte.Parse(sss[1]);
            o_IP2 = byte.Parse(sss[2]);
            o_IP3 = byte.Parse(sss[3]);
            port = z端口;
        }
        public string O_IP {
            get {
                return string.Format("{0}.{1}.{2}.{3}", o_IP0, o_IP1, o_IP2, o_IP3);
            }
        }
        public string O_String {
            get {
                return string.Format("{0}.{1}.{2}.{3}:{4}", o_IP0, o_IP1, o_IP2, o_IP3, port);
            }
            set {
                string[] sss = value.Split(':');
                S_mm(sss[0], ushort.Parse(sss[1]));
            }
        }
        public static bool operator ==(m_IP a, m_IP b) {
            if (a.port != b.port) {
                return false;
            } else if(a.o_IP0 != b.o_IP0){
                return false;
            } else if (a.o_IP1 != b.o_IP1) {
                return false;
            } else if (a.o_IP2 != b.o_IP2) {
                return false;
            } else if (a.o_IP3 != b.o_IP3) {
                return false;
            } else {
                return true;
            }
            //return (a.port == b.port&&a.o_IP组[0] == b.o_IP组[0] && a.o_IP组[1] == b.o_IP组[1] && a.o_IP组[2] == b.o_IP组[2] && a.o_IP组[3] == b.o_IP组[3]);
        }
        public static bool operator !=(m_IP a, m_IP b) {
            return (a.o_IP0!= b.o_IP0 || a.o_IP1 != b.o_IP1 || a.o_IP2 != b.o_IP2 || a.o_IP3 != b.o_IP3 || a.port != b.port);
            //if (a == b) {
            //    return false;
            //} else {
            //    return true;
            //}        
        }
    }
    public static class C_扩展方法_List {
        public static List<T> S_随机排序<T>(this List<T> z列表0) {
            Random random = new Random();
            List<T> z列表 = new List<T>();
            foreach (T n in z列表0) {
                z列表.Insert(random.Next(z列表.Count + 1), n);
            }
            return z列表;
        }
    }
    public class C_Debug {
        static C_Debug ooo;
        StreamWriter o_写文件; //写文件    
        static string o_日志路径;
        string z保存路径 = "";
        public static C_Debug Ooo {
            get {
                if (ooo == null) {
                    ooo = new C_Debug();
                    o_日志路径 = System.Environment.CurrentDirectory + "\\" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_日志";
                    Console.WriteLine(o_日志路径);
                }
                return ooo;
            }
        }
        StringBuilder bbs = new StringBuilder();
        int o_消息条数 = 0;
        C_Debug() {
        }
        public static void S_Add(string ss) {
            ooo.bbs.Append(ss + "\n\r");
            ooo.o_消息条数++;
            if (ooo.o_消息条数 > 2000) {
                ooo.S_保存日志();
            }
        }
        public static void S_Add(string z消息, Exception ex) {
            S_Add(z消息 + "__" + ex.Message + "___" + ex.StackTrace);
        }
        void S_保存日志() {
            if (o_消息条数 > 1800 || z保存路径.Length < 2) {
                Random ran = new Random();
                int z随机数 = ran.Next(99999);
                z保存路径 = o_日志路径 + string.Format(@"\{0}_{1}.log", DateTime.Now.ToString("yyyy-MM-dd,HH-mm-ss-fff"), z随机数);
                //z保存路径 = @"D:\XiangMu\CC_Server\CC_好友地图Server_12000\bin\Debug\CC_好友地图Server_12000_日志\mmm.txt";
                o_消息条数 = 0;
            }
            S_Write(z保存路径, bbs.ToString());
            bbs.Clear();
        }
        public void S_命令保存() {
            Console.WriteLine("输入 ‘1’  保存日志");
            string ss = Console.ReadLine();
            if (ss.Trim() == "1") {
                S_保存日志();
                Console.WriteLine("保存日志 成功......");
            } else {
                Console.WriteLine("输入错误————保存日志 失败....");
            }
        }
        public void S_10分钟定时清除多余日志() {
            //遍历文件夹
            DirectoryInfo theFolder = new DirectoryInfo(o_日志路径);
            FileInfo[] z子文件集 = theFolder.GetFiles("*.*", SearchOption.AllDirectories);
            //Console.WriteLine(z子文件集.Length);
            if (z子文件集.Length > 10000) {
                for (int i = 0; i < z子文件集.Length - 10000; i++) {
                    try {
                        if (File.Exists(z子文件集[i].FullName)) {
                            File.Delete(z子文件集[i].FullName); //文件存在就删除
                        }
                    } catch (Exception ex) {
                        //Debug.LogError("删除目录出错:_" + path + "__" + ex.Message);
                    }
                }
            }
        }
        void S_Write(string z保存路径, string message) {
            try {
                //DateTime dt = new DateTime();  
                //string directPath = System.Environment.CurrentDirectory+"/dddddd.txt";    //在获得文件夹路径  
                if (!Directory.Exists(o_日志路径)) {   //判断文件夹是否存在，如果不存在则创建  
                    Directory.CreateDirectory(o_日志路径);
                }

                if (o_写文件 == null) {
                    o_写文件 = !File.Exists(z保存路径) ? File.CreateText(z保存路径) : File.AppendText(z保存路径);    //判断文件是否存在如果不存在则创建，如果存在则添加。  
                }
                o_写文件.WriteLine("输出信息***********************************************************************");
                o_写文件.WriteLine(DateTime.Now.ToString("HH:mm:ss:fff"));
                if (message != null) {
                    o_写文件.WriteLine("异常信息：\r\n" + message);
                }
            } finally {
                if (o_写文件 != null) {
                    o_写文件.Flush();
                    o_写文件.Dispose();
                    o_写文件 = null;
                }
            }
        }
    }  
}

