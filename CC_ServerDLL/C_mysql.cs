using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
namespace CC_ServerDLL {
   public class C_mysql {
        public static string o_linq = "Data Source=127.0.0.1; port=3306;UId=root; PWD=Abc132; Charset=gb2312";
        //public static string o_linq = "Data Source=192.168.1.46; port=3306;UId=root; PWD=Abc132; Charset=gb2312";
        public static void S_链接信息设置(string zIP,string z端口,string z用户名,string z密码) {
            o_linq = string.Format("Data Source={0}; port={1};UId={2}; PWD={3}; Charset=utf8", zIP,z端口,z用户名,z密码);
        }
        public static List<string> S_查询mysql(string x表,string x主键) {
            string[] 表字段s = S_获取mysql表字段(x表).Split(',');
            string linq = "select * from "+x表+" where "+表字段s[0]+"='"+x主键+"'";
            Console.WriteLine(linq);
            return S_查询(linq);
        }
        public static List<string> S_查询mysql表(string x表) {
            string linq = "select * from "+x表;
            return S_查询(linq);
        }
        public static List<string> S_查询(string linq) {
            //Console.WriteLine("查询："+linq);
            try {
                MySqlConnection mysql = new MySqlConnection(o_linq);
                mysql.Open();
                MySqlCommand cmd = new MySqlCommand(linq,mysql);
                MySqlDataReader rd = cmd.ExecuteReader();
                //cmd.ExecuteNonQuery();//0000000000000000
                List<string> z数据列表=new List<string>();
                if(rd.HasRows) {
                    while(rd.Read()) {
                        string ss2 = "";
                        for(int i=0;i<rd.FieldCount;i++) {
                            try {
                                ss2+=rd.GetString(i)+"&";
                            } catch(Exception ex) {
                                ss2+="&";
                                Console.WriteLine("读取mysql字符：{0} 为空,错误:{1}",i,ex.Message);
                            }
                        }
                        if(ss2.Length>1) {
                            ss2=ss2.Remove(ss2.Length-1,1);
                        }
                        z数据列表.Add(ss2);
                    }
                }
                rd.Close();
                mysql.Close();
                return z数据列表;
            } catch(Exception ex) {
                Console.WriteLine("数据库查询失败5244 "+ex.Message); 
                return null;
            }
        }//====================================================================================================
        public static Dictionary<long, string> S_玩家数据_键值对查询表(string z玩家数据库地址) {
            string linq = "select * from " + z玩家数据库地址;
            return S_玩家数据_键值对查询(linq);
        }
       public static Dictionary<long, string> S_玩家数据_键值对查询(string linq) {
            try {
                MySqlConnection mysql = new MySqlConnection(o_linq);
                mysql.Open();
                MySqlCommand cmd = new MySqlCommand(linq, mysql);
                MySqlDataReader rd = cmd.ExecuteReader();
                //cmd.ExecuteNonQuery();//0000000000000000
                Dictionary<long, string> z数据列表 = new Dictionary<long, string>();
                if (rd.HasRows) {
                    while (rd.Read()) {
                        //string ss2 = "";
                        //for (int i = 0; i < rd.FieldCount; i++) {
                        //    try {
                        //        ss2 += rd.GetString(i) + "&";
                        //    } catch (Exception ex) {
                        //        ss2 += "&";
                        //        Console.WriteLine("读取mysql字符：{0} 为空,错误:{1}", i, ex.Message);
                        //    }
                        //}
                        //if (ss2.Length > 1) {
                        //    ss2 = ss2.Remove(ss2.Length - 1, 1);
                        //}
                        z数据列表.Add(rd.GetInt64(0), rd.GetString(1));
                    }
                }
                rd.Close();
                mysql.Close();
                return z数据列表;
            } catch (Exception ex) {
                Console.WriteLine("数据库查询失败5244 " + ex.Message);
                return null;
            }
        }//====================================================================================================     
       public static bool S_单条插入mysql(string x表,string ss) {
            return S_插入mysql2(x表,ss.Split("&"[0]));
        }
        public static bool S_单条插入mysql(string v表名,string[] sss) {
            string ss = "insert into "+v表名+" values (";
            for(int i = 0;i<sss.Length;i++) {
                ss+="'"+sss[i]+"',";
            }
            ss=ss.Remove(ss.Length-1,1);
            ss+=")";
            return S_操作(ss);
        }
        public static bool S_插入mysql2(string v表名,string[] sss) {
            string[] ss2=S_获取mysql表字段(v表名).Split(',');
            string linq = "insert into "+v表名+" values (";
            for(int i = 0;i<ss2.Length;i++) {
                if(sss.Length>i) {
                    linq+="'"+sss[i]+"',";
                } else {
                    linq+="'',";
                }
            }
            linq=linq.Remove(linq.Length-1,1);
            linq+=")";
            return S_操作(linq);
        }
        public static bool S_操作(string linq) {
            try {
                MySqlConnection mysql = new MySqlConnection(o_linq);
                mysql.Open();
                MySqlCommand cmd = new MySqlCommand(linq,mysql);
                cmd.ExecuteNonQuery();
                mysql.Close();//0000000000000000000000000000
                return true;
            } catch(Exception ex) {
                Console.WriteLine("失败 :"+ex.Message);
                return true;
            }
        }//======================================================================================================================
        /// <summary>
        /// <param> ID或name = false是Id ,true是name </param>
        /// </summary>
        public static bool S_修改mysql(string x表,string sss) {
            return S_修改mysql(x表,sss.Split("&"[0]));
        }
        public static bool S_修改mysql(string x表,string[] sss) {
            string ss = "";
            string[] 表字段s = S_获取mysql表字段(x表).Split(',');
            if(表字段s.Length>=sss.Length) {
                for(int i = 1;i<sss.Length;i++) {
                    ss+=表字段s[i]+"='"+sss[i]+"',";
                }
            } else {
                for(int i = 1;i<表字段s.Length;i++) {
                    ss+=表字段s[i]+"='"+sss[i]+"',";
                }
            }
            ss=ss.Remove(ss.Length-1,1);
            string linq = "update "+x表+" set "+ss+" where "+表字段s[0]+"='"+sss[0]+"'";
            Console.WriteLine("修改：{0}",linq);
            // UPDATE user SET name="ddd",密码="ddd",ip="23232",签名="323232",好友="32323",头像="23212" where id=2
            return S_操作(linq);
        }
        /// <summary>
        /// S00_ID,s01_姓名,S02_密码,s03_签名,s04_自定义问题,s05_自定义答案
        /// </summary>
        /// <param name="x表"></param>
        /// <returns></returns>
        public static string S_获取mysql表字段(string v表名) {
            try {
                MySqlConnection mysql = new MySqlConnection(o_linq);
                mysql.Open();
                MySqlCommand cmd = new MySqlCommand("describe "+v表名,mysql);
                MySqlDataReader rd = cmd.ExecuteReader();
                //cmd.ExecuteNonQuery();//0000000000000000
                string ss = "";
                if(rd.HasRows) {
                    while(rd.Read()) {
                        ss+=rd.GetString(0)+",";
                    }
                    ss=ss.Remove(ss.Length-1,1);
                }
                rd.Close();
                mysql.Close();
                return ss;
            } catch(Exception ex) { Console.WriteLine("数据库查询失败 "+ex.Message); return ""; }
        }//============================================================================================
        public static void S_插入多条数据(string[] sss,string v表名) {
            string ss2=S_获取mysql表字段(v表名);
            string ss3="insert "+v表名+" ("+ss2+") values";
            for(int i=0;i<sss.Length;i++) {
                ss3+="("+sss[i]+"),";
            }
            ss3=ss3.Remove(ss3.Length-1,1);
            Console.WriteLine(ss3);
            S_操作(ss3);
        }
        public static void S_单条数据更新(string v表名,string[] sss) {
            string[] z字段数组=C_mysql.S_获取mysql表字段(v表名).Split(',');
            // "update  tb_资源列表  set  s04_X='333',s05_Z='554'   where s00_id='30017676uuu'";
            string ss="update  "+v表名+"  set  ";
            for(int i=1;i<sss.Length;i++) {
                if(i<z字段数组.Length) {
                    ss+=z字段数组[i]+"='"+sss[i]+"',";
                }
            }
            ss=ss.Remove(ss.Length-1,1);
            ss+=" where "+z字段数组[0]+" = '"+sss[0]+"'";
            Console.WriteLine("更新: "+ss);
            C_mysql.S_操作(ss);
        }
    }
}
