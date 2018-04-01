using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CC_生成下载配置文件 {
    class C_生成配置文件 {
        static void Main(string[] args) {
            string z当前路径=System.Environment.CurrentDirectory;
            List<FileInfo> zmm = S_获取目录下_所有文件(z当前路径, "*.*");
            string zmm3 = "";
            Dictionary<int, List<int>> z列表 = new Dictionary<int, List<int>>();
            for (int i = 0; i < zmm.Count; i++) {
                string[] sss = zmm[i].Name.Split('_');
                if (sss.Length > 2) {
                    try {
                        int ii0 = int.Parse(sss[0]);
                        int ii2 = int.Parse(sss[1]);
                        bool z是否可用添加 = false;
                        if (z列表.ContainsKey(ii0)) {
                            if (z列表[ii0].Contains(ii2)==false) {
                                z是否可用添加 = true;
                            }
                        } else {
                            z是否可用添加 = true;
                        }
                        if (z是否可用添加) {
                            if (z列表.ContainsKey(ii0)==false) {
                                z列表[ii0] = new List<int>();
                                z列表[ii0].Add(ii2);
                            }
                            string ss = zmm[i].FullName.Remove(0, z当前路径.Length + 1);
                            zmm3 += ss + "\t" + GetMD5HashFromFile(zmm[i].FullName) + "\r\n";
                            Console.WriteLine(ss);
                        }
                    } catch (Exception) {
                    }
                }
            }
            S_Txt写入(zmm3, z当前路径 + "/AssetBundleInfo.txt");
            //Console.WriteLine("\n\n生成完成___AssetBundleInfo.txt");
            //while (true) {
            //    Console.ReadKey();
            //}
        }
        public static List<FileInfo> S_获取目录下_所有文件(string path, string type) {
            List<FileInfo> list = new List<FileInfo>();
            //遍历文件夹
            DirectoryInfo theFolder = new DirectoryInfo(path);
            FileInfo[] z子文件集 = theFolder.GetFiles(type, SearchOption.AllDirectories);
            for (int i = 0; i < z子文件集.Length; i++) {
                list.Add(z子文件集[i]);
            }
            return list;
        }
        //public static string S_MD5(this string ss) {
        //    using (MD5 m5 = MD5.Create()) {
        //        byte[] zData = Encoding.UTF8.GetBytes(ss);
        //        zData = m5.ComputeHash(zData);
        //        m5.Clear();
        //        StringBuilder bb5 = new StringBuilder();
        //        for (int i = 0; i < zData.Length; i++) {
        //            bb5.Append(zData[i].ToString("x2"));
        //        }
        //        return bb5.ToString();
        //    }
        //}
        public static string GetMD5HashFromFile(string fileName) {
            try {
                FileStream file = new FileStream(fileName, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++) {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            } catch (Exception ex) {
                //Console.WriteLine(ex.Message);
                return "ssss";
                //throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
        public static void S_Txt写入(string z内容, string path) {
            if (File.Exists(path))
                File.Delete(path); //文件存在就删除
            FileStream fs;
            if (!File.Exists(path)) {
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                fs.Close();
            }
            Encoding zUTF8 = new System.Text.UTF8Encoding(false);
            using (StreamWriter sw = new StreamWriter(path, false, zUTF8)) {
                sw.WriteLine(z内容);
            }
        }
    }
}
