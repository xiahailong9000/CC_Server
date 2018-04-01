using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_AutoCopy_002 {
    class C_AutoCopy {
        static C_Node58 o_RootNode;
        static List<C_Node58> zNodeList = new List<C_Node58>();
        static void Main(string[] args) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string zConfigPath = System.Environment.CurrentDirectory+"\\Copy.txt";
            Console.WriteLine(zConfigPath);
            if (File.Exists(zConfigPath) == false) {
                File.Create(zConfigPath);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("配置文件不存在---" + zConfigPath + "\n\n已经创建空的 Copy.txt\n");
            } else {
                string[] sss=File.ReadAllText(zConfigPath).Split('\n');
                o_RootNode = new C_Node58(sss[0].Trim());
                for(int i = 1; i < sss.Length; i++) { 
                    //Console.WriteLine("ddddddddddddddd___________" + n);
                    zNodeList.Add(new C_Node58(sss[i].Trim()));
                }
                Console.WriteLine("\n\n");
                foreach (var n in zNodeList) {
                    n.S_FileCompare(o_RootNode);
                    Console.WriteLine("\n\n");
                }
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n\n\n------------------------自动同步---完成------------------\n\n");
            Console.ReadKey();
        }
        public class C_Node58 {
            public string o_Path;
            public Dictionary<string, long> o_FileDic = new Dictionary<string, long>();
            public C_Node58(string zPath) {
                o_Path = zPath;
                FileInfo[] zmm=  new DirectoryInfo(zPath).GetFiles("*.*", SearchOption.AllDirectories);
                foreach(var n in zmm) {
                    string zpp = n.FullName.Remove(0, o_Path.Length + 1);
                    o_FileDic[zpp] = Convert.ToInt64((n.LastWriteTime - new DateTime(2018, 1, 1, 8, 0, 0)).TotalSeconds);
                }
                //foreach (var n in o_FileDic) {
                //    Console.WriteLine(n.Key + "____________" + n.Value);
                //}
            }
            public void S_FileCompare(C_Node58 zRootNode) {
                List<string> zUpdateList = new List<string>();
                foreach (var n in zRootNode.o_FileDic) {
                    if (o_FileDic.ContainsKey(n.Key)) {
                        if (n.Value > o_FileDic[n.Key]) {
                            zUpdateList.Add(n.Key);
                        }
                        o_FileDic[n.Key] = 0;
                    } else {
                        zUpdateList.Add(n.Key);
                    }
                }
                List<string> zDeleteLIst = new List<string>();
                foreach (var n in o_FileDic) {//获取删除列表---------
                    if (n.Value != 0) {
                        zDeleteLIst.Add(n.Key);
                    }
                }
                foreach (var n in zDeleteLIst) {//删除文件-----------
                    string zTargetPath = o_Path + "\\" + n;
                    Console.WriteLine("删除_" + zTargetPath);
                    try {
                        File.Delete(zTargetPath);
                    } catch (Exception ex){
                        Console.WriteLine("文件删除失败_" + zTargetPath + "__"+ ex.Message);
                    }
                }
                foreach (var n in zUpdateList) {//拷贝更新文件----------
                    string zSourcePath = zRootNode.o_Path + "\\" + n;
                    string zTargetPath = o_Path + "\\" + n;
                    if (File.Exists(zTargetPath)) {
                        File.Delete(zTargetPath);
                    }
                    string zDirectory= Path.GetDirectoryName(zTargetPath);
                    if (Directory.Exists(zDirectory)==false) {
                        Directory.CreateDirectory(zDirectory);
                    }
                    Console.WriteLine("拷贝_" + zTargetPath);
                    try {
                        S_大文件流拷贝(zSourcePath, zTargetPath);
                    } catch (Exception ex) {
                        Console.WriteLine("文件拷贝失败_" + zTargetPath + "__" + ex.Message);
                    }
                }
            }
            void S_大文件流拷贝(string zSourcePath,string zTargetPath) {  //--------------------------------------------------------------------
                FileStream zRead = new FileStream(zSourcePath, FileMode.Open);
                FileStream zWrite = new FileStream(zTargetPath, FileMode.Create);
                byte[] zBytes = new byte[1024 * 1024 * 32];
                int zLength = 0;
                do {
                    zLength = zRead.Read(zBytes, 0, zBytes.Length);
                    zWrite.Write(zBytes, 0, zLength);
                    Console.WriteLine("                   拷贝流-------" + zLength);
                } while (zLength == zBytes.Length);
                zWrite.Dispose();
                zRead.Dispose();
                //Console.WriteLine("------拷贝成功--------");
            }
        }
    }
}
