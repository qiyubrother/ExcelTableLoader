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
            var telTyshxydmDict = new Dictionary<string, List<string>>(); // 电话号码->统一社会信用代码列表
            Action<string, string> addTelTyshxydm = (dh, tyshxydm) =>{
                if (telTyshxydmDict[dh].Contains(tyshxydm))
                {
                    telTyshxydmDict[dh].Add(tyshxydm);
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
                    MySqlCommand cmdInsert = new MySqlCommand("INSERT INTO EnterpriseInfo(ycrq, qymc, jyzt, fddbr, zczb, clrq, sssf, sscs, ssqx, dh1, dh2, dh3, dh4, dh5, dh6, dh7, dh8, dh9, dh10, dh11, dh12, zj1, zj2, zj3, zj4, zj5, email1, email2,email3, email4, email5, email6, tyshxydm, nsrsbm, zch, zzjgdm, cbrs, qylx, sshy, cym1, cym2, cym3, cym4, cym5, gw, qydz, jyfw) values (@ycrq, @qymc, @jyzt, @fddbr, @zczb, @clrq, @sssf, @sscs, @ssqx, @dh1, @dh2, @dh3, @dh4, @dh5, @dh6, @dh7, @dh8, @dh9, @dh10, @dh11, @dh12, @zj1, @zj2, @zj3, @zj4, @zj5, @email1, @email2, @email3, @email4, @email5, @email6, @tyshxydm, @nsrsbm, @zch, @zzjgdm, @cbrs, @qylx, @sshy, @cym1, @cym2, @cym3, @cym4, @cym5, @gw, @qydz, @jyfw)", cn);
                    MySqlCommand cmdUpdate = new MySqlCommand("UPDATE EnterpriseInfo SET ycrq=@ycrq, qymc=@qymc, jyzt=@jyzt, fddbr=@fddbr, zczb=@zczb, clrq=@clrq, sssf=@sssf, sscs=@sscs, ssqx=@ssqx, dh1=@dh1, dh2=@dh2, dh3=@dh3, dh4=@dh4, dh5=@dh5, dh6=@dh6, dh7=@dh7, dh8=@dh8, dh9=@dh9, dh10=@dh10, dh11=@dh11, dh12=@dh12, zj1=@zj1, zj2=@zj2, zj3=@zj3, zj4=@zj4, zj5=@zj5, email1=@email1, email2=@email2, email3=@email3, email4=@email4, email5=@email5, email6=@email6, nsrsbm=@nsrsbm, zch=@zch, zzjgdm=@zzjgdm, cbrs=@cbrs, qylx=@qylx, sshy=@sshy, cym1=@cym1, cym2=@cym2, cym3=@cym3, cym4=@cym4, cym5=@cym5, gw=@gw, qydz=@qydz, jyfw=@jyfw WHERE tyshxydm=@tyshxydm", cn);
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

                    param = new MySqlParameter("@zczb", zczb);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    param = new MySqlParameter("@clrq", clrq);
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

                    param = new MySqlParameter("@jyfw", jyfw);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);
                    #endregion
                    var ada = new MySqlDataAdapter($"select count(tyshxydm) from EnterpriseInfo where tyshxydm='{tyshxydm}'", cn);
                    var dt = new DataTable();
                    ada.Fill(dt);
                    #region Update
                    if (Convert.ToInt32(dt.Rows[0][0]) > 0)
                    {
                        // Update
                        try
                        {
                            cmdUpdate.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Trace($"UpdateError::tyshxydm:{tyshxydm},{ex.Message}");
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
                            LogHelper.Trace($"InsertError::tyshxydm:{tyshxydm},{ex.Message}");
                        }
                    }
                    #endregion
                    ada.Dispose();
                    dt.Dispose();
                }
            }
            #endregion
            #region 从mysql数据库生成电话号码字典
            LogHelper.Trace("从mysql数据库生成电话号码字典...");
            using (MySqlConnection cn = new MySqlConnection(mysqlConnectionString))
            {
                cn.Open();
                MySqlDataAdapter ada = new MySqlDataAdapter($"select tyshxydm, dh1, dh2, dh3, dh4, dh5, dh6, dh7, dh8, dh9, dh10, dh11, dh12, zj1, zj2, zj3, zj4, zj5 from enterpriseinfo", cn);
                var dt = new DataTable();
                ada.Fill(dt);
                foreach(DataRow dr in dt.Rows)
                {
                    var tyshxydm = dr["tyshxydm"].ToString();
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
                    if (dh1 != string.Empty)  { telQuantityDict[dh1]++; addTelTyshxydm(dh1, tyshxydm); }
                    if (dh2 != string.Empty)  { telQuantityDict[dh2]++; addTelTyshxydm(dh2, tyshxydm); }
                    if (dh3 != string.Empty)  { telQuantityDict[dh3]++; addTelTyshxydm(dh3, tyshxydm); }
                    if (dh4 != string.Empty)  { telQuantityDict[dh4]++; addTelTyshxydm(dh4, tyshxydm); }
                    if (dh5 != string.Empty)  { telQuantityDict[dh5]++; addTelTyshxydm(dh5, tyshxydm); }
                    if (dh6 != string.Empty)  { telQuantityDict[dh6]++; addTelTyshxydm(dh6, tyshxydm); }
                    if (dh7 != string.Empty)  { telQuantityDict[dh7]++; addTelTyshxydm(dh7, tyshxydm); }
                    if (dh8 != string.Empty)  { telQuantityDict[dh8]++; addTelTyshxydm(dh8, tyshxydm); }
                    if (dh9 != string.Empty)  { telQuantityDict[dh9]++; addTelTyshxydm(dh9, tyshxydm); }
                    if (dh10 != string.Empty) { telQuantityDict[dh10]++; addTelTyshxydm(dh10, tyshxydm); }
                    if (dh11 != string.Empty) { telQuantityDict[dh11]++; addTelTyshxydm(dh11, tyshxydm); }
                    if (dh12 != string.Empty) { telQuantityDict[dh12]++; addTelTyshxydm(dh12, tyshxydm); }
                    if (zj1 != string.Empty)  { telQuantityDict[zj1]++; addTelTyshxydm(zj1, tyshxydm); }
                    if (zj2 != string.Empty)  { telQuantityDict[zj2]++; addTelTyshxydm(zj2, tyshxydm); }
                    if (zj3 != string.Empty)  { telQuantityDict[zj3]++; addTelTyshxydm(zj3, tyshxydm); }
                    if (zj4 != string.Empty)  { telQuantityDict[zj4]++; addTelTyshxydm(zj4, tyshxydm); }
                    if (zj5 != string.Empty)  { telQuantityDict[zj5]++; addTelTyshxydm(zj5, tyshxydm); }
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
                        var tyshxydmList = telTyshxydmDict[dh]; // 多个同行的统一社会信用代码
                        foreach (var tyshxydm in tyshxydmList)
                        {
                            MySqlCommand cmdUpdate = new MySqlCommand($"UPDATE EnterpriseInfo SET th='Y' WHERE tyshxydm={tyshxydm}", cn);
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
