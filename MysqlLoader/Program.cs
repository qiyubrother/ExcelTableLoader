using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using qiyubrother;
using System.Data.SQLite;
using System.Data;
using MySql.Data.MySqlClient;

namespace MysqlLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            var configFileName = "config.json";
            if (!File.Exists(configFileName))
            {
                LogHelper.Trace("config.json file losted.");
                return;
            }
            string sqliteDBFileName = @"\ExcelData"; // sqlite 数据库文件名
            var mysqlConnectionString = string.Empty;
            var jo = JObject.Parse(File.ReadAllText(configFileName));
            try { sqliteDBFileName = jo["sqliteDatabaseFile"].ToString(); }
            catch { LogHelper.Trace("config error. key is [sqliteDatabaseFile]."); }
            try { mysqlConnectionString = jo["mysqlConnectionString"].ToString(); }
            catch { LogHelper.Trace("config error. key is [mysqlConnectionString]"); }
            var tongHangQuantity = 5;
            try { tongHangQuantity = Convert.ToInt32(jo["tongHangQuantity"].ToString()); }
            catch { LogHelper.Trace("config error. key is [tongHangQuantity]"); }
            LogHelper.StartService();
            LoadSqliteFile(sqliteDBFileName, mysqlConnectionString, tongHangQuantity);
            LogHelper.Trace("Finished.");
        }

        private static void LoadSqliteFile(string sqliteDBFileName, string mysqlConnectionString, int tongHangQuantity)
        {
            var dtSqlite = new DataTable();
            var telQuantityDict = new Dictionary<string, int>(); // 电话号码->数量
            var telQymcDict = new Dictionary<string, List<string>>(); // 电话号码->企业名称
            Action<string> addTelQuantity = (dh) =>
            {
                if (!telQuantityDict.ContainsKey(dh))
                {
                    telQuantityDict[dh] = 1;
                }
                else
                {
                    telQuantityDict[dh]++;
                }
            };

            Action<string, string> addTelQymc = (dh, _qymc) =>{
                if (!telQymcDict.ContainsKey(dh))
                {
                    telQymcDict[dh] = new List<string>();
                }
                if (!telQymcDict[dh].Contains(_qymc))
                {
                    telQymcDict[dh].Add(_qymc);
                }
            };
            #region 读取sqlite数据库中的所有数据
            LogHelper.Trace("读取sqlite数据库中的所有数据...");
            using (SQLiteConnection cn = new SQLiteConnection("data source=" + sqliteDBFileName))
            {
                cn.Open();
                var ada = new SQLiteDataAdapter("select * from enterpriseinfo", cn);
                ada.Fill(dtSqlite);
                ada.Dispose();
            }
            #endregion
            #region 将新数据插入|更新到mysql数据库
            LogHelper.Trace("开始同步数据到mysql...");
            using (MySqlConnection cn = new MySqlConnection(mysqlConnectionString))
            {
                cn.Open();
                var qymcLst = new List<string>();
                {
                    var ada = new MySqlDataAdapter($"select qymc from EnterpriseInfo", cn);
                    var dt = new DataTable();
                    ada.Fill(dt);
                    foreach(DataRow dr in dt.Rows)
                    {
                        qymcLst.Add(dr["qymc"].ToString());
                    }
                    qymcLst.Sort();
                    ada.Dispose();
                    dt.Dispose();
                }
                Int64 pos = 0;
                foreach (DataRow dr in dtSqlite.Rows)
                {
                    #region 定义数据变量
                    var ycrq = dr["ycrq"].ToString();
                    var qymc = dr["qymc"].ToString();
                    var jyzt = dr["jyzt"].ToString();
                    var fddbr = dr["fddbr"].ToString();
                    var zczb = dr["zczb"].ToString();
                    var clrq = dr["clrq"].ToString();
                    var sssf = dr["sssf"].ToString();
                    var sscs = dr["sscs"].ToString();
                    var ssqx = dr["ssqx"].ToString();
                    var dh1 = dr["dh1"].ToString();  // Search From 
                    var dh2 = dr["dh2"].ToString();
                    var dh3 = dr["dh3"].ToString();
                    var dh4 = dr["dh4"].ToString();
                    var dh5 = dr["dh5"].ToString();
                    var dh6 = dr["dh6"].ToString();
                    var dh7 = dr["dh7"].ToString();
                    var dh8 = dr["dh8"].ToString();
                    var dh9 = dr["dh9"].ToString();
                    var dh10 = dr["dh10"].ToString();
                    var dh11 = dr["dh11"].ToString();
                    var dh12 = dr["dh12"].ToString();
                    var zj1 = dr["zj1"].ToString();
                    var zj2 = dr["zj2"].ToString();
                    var zj3 = dr["zj3"].ToString();
                    var zj4 = dr["zj4"].ToString();
                    var zj5 = dr["zj5"].ToString();  // Search To
                    var email1 = dr["email1"].ToString();
                    var email2 = dr["email2"].ToString();
                    var email3 = dr["email3"].ToString();
                    var email4 = dr["email4"].ToString();
                    var email5 = dr["email5"].ToString();
                    var email6 = dr["email6"].ToString();
                    var tyshxydm = dr["tyshxydm"].ToString(); // PK
                    var nsrsbm = dr["nsrsbm"].ToString();
                    var zch = dr["zch"].ToString();
                    var zzjgdm = dr["zzjgdm"].ToString();
                    var cbrs = dr["cbrs"].ToString();
                    var qylx = dr["qylx"].ToString();
                    var sshy = dr["sshy"].ToString();
                    var cym1 = dr["cym1"].ToString();
                    var cym2 = dr["cym2"].ToString();
                    var cym3 = dr["cym3"].ToString();
                    var cym4 = dr["cym4"].ToString();
                    var cym5 = dr["cym5"].ToString();
                    var gw = dr["gw"].ToString();
                    var qydz = dr["qydz"].ToString();
                    var jyfw = dr["jyfw"].ToString();
                    #endregion
                    #region 定义sql语句并配置参数
                    MySqlCommand cmdInsert = new MySqlCommand("INSERT INTO EnterpriseInfo(ycrq, qymc, jyzt, fddbr, zczb, clrq, sssf, sscs, ssqx, dh1, dh2, dh3, dh4, dh5, dh6, dh7, dh8, dh9, dh10, dh11, dh12, zj1, zj2, zj3, zj4, zj5, email1, email2,email3, email4, email5, email6, tyshxydm, nsrsbm, zch, zzjgdm, cbrs, qylx, sshy, cym1, cym2, cym3, cym4, cym5, gw, qydz, jyfw, th) values (@ycrq, @qymc, @jyzt, @fddbr, @zczb, @clrq, @sssf, @sscs, @ssqx, @dh1, @dh2, @dh3, @dh4, @dh5, @dh6, @dh7, @dh8, @dh9, @dh10, @dh11, @dh12, @zj1, @zj2, @zj3, @zj4, @zj5, @email1, @email2, @email3, @email4, @email5, @email6, @tyshxydm, @nsrsbm, @zch, @zzjgdm, @cbrs, @qylx, @sshy, @cym1, @cym2, @cym3, @cym4, @cym5, @gw, @qydz, @jyfw, 'N')", cn);
                    MySqlCommand cmdUpdate = new MySqlCommand("UPDATE EnterpriseInfo SET jyzt=@jyzt, fddbr=@fddbr, zczb=@zczb, clrq=@clrq, sssf=@sssf, sscs=@sscs, ssqx=@ssqx, dh1=@dh1, dh2=@dh2, dh3=@dh3, dh4=@dh4, dh5=@dh5, dh6=@dh6, dh7=@dh7, dh8=@dh8, dh9=@dh9, dh10=@dh10, dh11=@dh11, dh12=@dh12, zj1=@zj1, zj2=@zj2, zj3=@zj3, zj4=@zj4, zj5=@zj5, email1=@email1, email2=@email2, email3=@email3, email4=@email4, email5=@email5, email6=@email6, tyshxydm=@tyshxydm, nsrsbm=@nsrsbm, zch=@zch, zzjgdm=@zzjgdm, cbrs=@cbrs, qylx=@qylx, sshy=@sshy, cym1=@cym1, cym2=@cym2, cym3=@cym3, cym4=@cym4, cym5=@cym5, gw=@gw, qydz=@qydz, jyfw=@jyfw WHERE qymc=@qymc", cn);
                    MySqlParameter param = null;

                    param = new MySqlParameter("@ycrq", ycrq);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@qymc", qymc);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@jyzt", jyzt);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@fddbr", fddbr);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    var _zczb = zczb.Replace("万元人民币", string.Empty).Replace("人民币", string.Empty).Replace("万元", string.Empty);
                    int.TryParse(_zczb, out int iZczb);
                    param = new MySqlParameter("@zczb", iZczb); // 注册资本
                    param.DbType = DbType.Int32;
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@clrq", clrq); // 成立日期
                    param.DbType = DbType.Date;
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@sssf", sssf);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@sscs", sscs);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@ssqx", ssqx);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@dh1", dh1);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@dh2", dh2);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@dh3", dh3);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@dh4", dh4);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@dh5", dh5);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@dh6", dh6);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@dh7", dh7);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@dh8", dh8);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@dh9", dh9);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@dh10", dh10);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@dh11", dh11);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@dh12", dh12);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@zj1", zj1);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@zj2", zj2);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@zj3", zj3);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@zj4", zj4);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@zj5", zj5);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@email1", email1);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@email2", email2);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@email3", email3);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@email4", email4);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@email5", email5);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@email6", email6);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@tyshxydm", tyshxydm);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@nsrsbm", nsrsbm);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@zch", zch);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@zzjgdm", zzjgdm);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@cbrs", cbrs);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@qylx", qylx);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@sshy", sshy);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@cym1", cym1);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@cym2", cym2);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@cym3", cym3);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@cym4", cym4);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@cym5", cym5);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@gw", gw);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@qydz", qydz);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    if (jyfw.Length > 200)
                    {
                        jyfw = jyfw.Substring(0, 200);
                    }
                    param = new MySqlParameter("@jyfw", jyfw);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);
                    #endregion

                    #region Update
                    if (qymcLst.Contains(qymc))
                    {
                        // Update
                        try
                        {
                            cmdUpdate.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Trace($"UpdateError::qymc:{qymc},{ex.Message}");
                        }
                    }
                    #endregion
                    #region Insert
                    else
                    {
                        // Insert
                        try
                        {
                            cmdInsert.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Trace($"InsertError::qymc:{qymc},{ex.Message}");
                        }
                    }
                    #endregion
                    if (++pos % 1000 == 0)
                    {
                        LogHelper.Trace($"已更新（{pos}/{dtSqlite.Rows.Count}）...");
                    }
                }

                LogHelper.Trace($"已更新（{pos}/{dtSqlite.Rows.Count}）...");
            }
            #endregion
            #region 从mysql数据库生成电话号码字典
            LogHelper.Trace("从mysql数据库生成电话号码字典...");
            using (MySqlConnection cn = new MySqlConnection(mysqlConnectionString))
            {
                cn.Open();
                MySqlDataAdapter ada = new MySqlDataAdapter($"select qymc, dh1, dh2, dh3, dh4, dh5, dh6, dh7, dh8, dh9, dh10, dh11, dh12, zj1, zj2, zj3, zj4, zj5 from enterpriseinfo", cn);
                var dt = new DataTable();
                ada.Fill(dt);
                foreach(DataRow dr in dt.Rows)
                {
                    var qymc = dr["qymc"].ToString();
                    var dh1 = dr["dh1"].ToString();  // Search From 
                    var dh2 = dr["dh2"].ToString();
                    var dh3 = dr["dh3"].ToString();
                    var dh4 = dr["dh4"].ToString();
                    var dh5 = dr["dh5"].ToString();
                    var dh6 = dr["dh6"].ToString();
                    var dh7 = dr["dh7"].ToString();
                    var dh8 = dr["dh8"].ToString();
                    var dh9 = dr["dh9"].ToString();
                    var dh10 = dr["dh10"].ToString();
                    var dh11 = dr["dh11"].ToString();
                    var dh12 = dr["dh12"].ToString();
                    var zj1 = dr["zj1"].ToString();
                    var zj2 = dr["zj2"].ToString();
                    var zj3 = dr["zj3"].ToString();
                    var zj4 = dr["zj4"].ToString();
                    var zj5 = dr["zj5"].ToString();  // Search To
                    if (dh1 != string.Empty) { addTelQuantity(dh1); addTelQymc(dh1, qymc); }
                    if (dh2 != string.Empty) { addTelQuantity(dh2); addTelQymc(dh2, qymc); }
                    if (dh3 != string.Empty) { addTelQuantity(dh3); addTelQymc(dh3, qymc); }
                    if (dh4 != string.Empty) { addTelQuantity(dh4); addTelQymc(dh4, qymc); }
                    if (dh5 != string.Empty) { addTelQuantity(dh5); addTelQymc(dh5, qymc); }
                    if (dh6 != string.Empty) { addTelQuantity(dh6); addTelQymc(dh6, qymc); }
                    if (dh7 != string.Empty) { addTelQuantity(dh7); addTelQymc(dh7, qymc); }     
                    if (dh8 != string.Empty) { addTelQuantity(dh8); addTelQymc(dh8, qymc); }     
                    if (dh9 != string.Empty) { addTelQuantity(dh9); addTelQymc(dh9, qymc); }
                    if (dh10 != string.Empty) { addTelQuantity(dh10); addTelQymc(dh10, qymc); }
                    if (dh11 != string.Empty) { addTelQuantity(dh11); addTelQymc(dh11, qymc); }
                    if (dh12 != string.Empty) { addTelQuantity(dh12); addTelQymc(dh12, qymc); } 
                    if (zj1 != string.Empty) { addTelQuantity(zj1); addTelQymc(zj1, qymc); }     
                    if (zj2 != string.Empty) { addTelQuantity(zj2); addTelQymc(zj2, qymc); }     
                    if (zj3 != string.Empty) { addTelQuantity(zj3); addTelQymc(zj3, qymc); }     
                    if (zj4 != string.Empty) { addTelQuantity(zj4); addTelQymc(zj4, qymc); }     
                    if (zj5 != string.Empty) { addTelQuantity(zj5); addTelQymc(zj5, qymc); }
                }
            }
            #endregion
            #region 统计电话号码个数，并标记enterpriseinfo表是否为同行，更新同行电话号码表
            LogHelper.Trace("统计电话号码个数，并标记enterpriseinfo表是否为同行，更新同行电话号码表...");
            using (MySqlConnection cn = new MySqlConnection(mysqlConnectionString))
            {
                cn.Open();
                // 清空同行电话号码表
                MySqlCommand cmdClearTongHang = new MySqlCommand($"Delete from tonghang", cn);
                cmdClearTongHang.ExecuteNonQuery();

                foreach (var dh in telQuantityDict.Keys)
                {
                    if (telQuantityDict[dh] > tongHangQuantity)
                    {
                        // 标记为同行
                        var _qymcList = telQymcDict[dh]; // 多个同行的统一社会信用代码
                        foreach (var _qymc in _qymcList)
                        {
                            MySqlCommand cmdUpdate = new MySqlCommand($"UPDATE EnterpriseInfo SET th='Y' WHERE qymc='{_qymc}'", cn);
                            cmdUpdate.ExecuteNonQuery();
                        }
                        // 更新同行电话号码表
                        MySqlCommand cmdInsert = new MySqlCommand($"Insert into tonghang(Tel, Quantity) values('{dh}', {telQuantityDict[dh]})", cn);
                        cmdInsert.ExecuteNonQuery();
                    }
                }
            }
            #endregion
        }
    }
}
