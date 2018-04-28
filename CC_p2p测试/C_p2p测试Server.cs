﻿using CC_ServerDLL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CC_p2p测试 {
    public class C_p2p测试Server {
        public static C_UdpClient o_Udp;
        static void Main(string[] args) {
            C_MySql.S_查询();
            //int z端口类型 = 4009;
            //C_TcpClient.S_启动监听(z端口类型, new C_Serv客户端());
            //o_Udp = C_UdpClient.S_启动监听(z端口类型, C_Udp管理器.S_接口_消息处理);
            while (true) {
                Console.ReadKey();
            }
        }
    }
    public class C_Serv客户端 : C_TcpClient.C_逻辑端 {
        public override void S_接口_消息处理(ushort z消息类型, string msg, byte[] zData) {
            //o_通信器.S_发送消息(z消息类型, o_通信器.o_IP端口.O_String);
            Console.WriteLine("收到消息_" + z消息类型 + "___" + msg);
        }
    }
    public class C_Udp管理器 {
        public static void S_接口_消息处理(byte[] zData, IPEndPoint z地址) {
            C_p2p测试Server.o_Udp.S_发送消息(zData[0], z地址.ToString(), z地址);
        }
    }
    public class C_MySql {
        public static void S_查询() {
            string o_linq = "Data Source=139.196.17.39; port=3306;UId=root; PWD=ASDfgh132; Charset=gb2312";
            string linq = "select * from zz.8hao";
            try {
                MySqlConnection mysql = new MySqlConnection(o_linq);
                mysql.Open();
                MySqlCommand cmd = new MySqlCommand(linq, mysql);
                MySqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows) {
                    while (rd.Read()) {
                        string ss2 = "";
                        for (int i = 0; i < rd.FieldCount; i++) {
                            ss2 += rd[i] + "___";
                        }
                        Console.WriteLine("数据=="+ss2);
                    }
                }
                rd.Close();
                mysql.Close();
            } catch (Exception ex) {
                Console.WriteLine("数据库查询失败5244 " + ex.Message);
            }
        }//=====
    }
}
