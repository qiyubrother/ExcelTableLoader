using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Spire.Xls;
using qiyubrother;
using System.IO;

namespace ExcelTableLoader
{
    class Program
    {
        static string dbFileName = @"ExcelData";
        static void Main(string[] args)
        {
            LogHelper.StartService();

            if (args.Length != 1)
            {
                ShowErrorMessage();
                return;
            }

            if (args[0].StartsWith("-excel:"))
            {
                var fn = args[0].Substring(7);
                LoadExcelFile(fn);
            }
            else if (args[0].StartsWith("-exceldir:"))
            {
                var dir = args[0].Substring(10);
                var fileNames = Directory.GetFiles(dir, "*.xls");
                foreach (var fn in fileNames)
                {
                    LoadExcelFile(fn);
                }
                var fileNames2 = Directory.GetFiles(dir, "*.xlsx");
                foreach (var fn in fileNames2)
                {
                    LoadExcelFile(fn);
                }
            }
            else
            {
                ShowErrorMessage();
                return;
            }

            LogHelper.Stop();
        }

        private static void ShowErrorMessage()
        {
            LogHelper.Trace($"ExcelTableLoader.exe -excel:source-data.xls");
            LogHelper.Trace($"ExcelTableLoader.exe -exceldir:sourcedir");
        }

        private static void LoadExcelFile(string fileName)
        {
            var cols = new[]
            {
                new ColMap { Name = "异常日期" },
                new ColMap { Name = "企业名称" },
                new ColMap { Name = "经营状态" },
                new ColMap { Name = "法定代表人" },
                new ColMap { Name = "注册资本" },
                new ColMap { Name = "成立日期" },
                new ColMap { Name = "所属省份" },
                new ColMap { Name = "所属城市" },
                new ColMap { Name = "所属区县" },
                new ColMap { Name = "电话" },
                new ColMap { Name = "更多电话" },
                new ColMap { Name = "邮箱" },
                new ColMap { Name = "更多邮箱" },
                new ColMap { Name = "统一社会信用代码" },
                new ColMap { Name = "纳税人识别号" },
                new ColMap { Name = "注册号" },
                new ColMap { Name = "组织机构代码" },
                new ColMap { Name = "参保人数" },
                new ColMap { Name = "企业类型" },
                new ColMap { Name = "所属行业" },
                new ColMap { Name = "曾用名" },
                new ColMap { Name = "官网" },
                new ColMap { Name = "企业地址" },
                new ColMap { Name = "经营范围" },
            };
            var titleRowIndex = 1; //标题所在行
            Workbook workbook = new Workbook();

            workbook.LoadFromFile(fileName);

            var sheet = workbook.Worksheets[0];
            var max_columns = sheet.Columns.Length;

            Func<int, int, string> Cell = (rowIndex, colIndex) =>
            {
                return sheet.Range[rowIndex, colIndex].Value;
            };

            for (var colIndex = 1; colIndex <= max_columns; colIndex++)
            {
                foreach (var c in cols)
                {
                    if (Cell(titleRowIndex, colIndex) == c.Name)
                    {
                        c.Index = colIndex;
                        c.IsValid = true;
                    }
                }
            }
            //foreach (var c in cols)
            //{
            //    LogHelper.Trace($"{c.Name}, {c.Index}, {c.IsValid}");
            //}

            var dataLineNumber = titleRowIndex + 1;
            var insertCount = 0;
            var updateCount = 0;
            var ignoreCount = 0;
            using (SQLiteConnection cn = new SQLiteConnection("data source=" + dbFileName))
            {
                cn.Open();
                var trans = cn.BeginTransaction();
                var rows = sheet.Rows.Count() - 1;
                while (dataLineNumber < rows)
                {
                    SQLiteCommand cmdInsert = new SQLiteCommand("INSERT INTO EnterpriseInfo(ycrq, qymc, jyzt, fddbr, zczb, clrq, sssf, sscs, ssqx, dh1, dh2, dh3, dh4, dh5, dh6, dh7, dh8, dh9, dh10, dh11, dh12, email1, email2,email3, email4, email5, email6, tyshxydm, nsrsbm, zch, zzjgdm, cbrs, qylx, sshy, cym1, cym2, cym3, cym4, cym5, gw, qydz, jyfw) values (@ycrq, @qymc, @jyzt, @fddbr, @zczb, @clrq, @sssf, @sscs, @ssqx, @dh1, @dh2, @dh3, @dh4, @dh5, @dh6, @dh7, @dh8, @dh9, @dh10, @dh11, @dh12, @email1, @email2, @email3, @email4, @email5, @email6, @tyshxydm, @nsrsbm, @zch, @zzjgdm, @cbrs, @qylx, @sshy, @cym1, @cym2, @cym3, @cym4, @cym5, @gw, @qydz, @jyfw)", cn);
                    SQLiteCommand cmdUpdate = new SQLiteCommand("UPDATE EnterpriseInfo SET ycrq=@ycrq, qymc=@qymc, jyzt=@jyzt, fddbr=@fddbr, zczb=@zczb, clrq=@clrq, sssf=@sssf, sscs=@sscs, ssqx=@ssqx, dh1=@dh1, dh2=@dh2, dh3=@dh3, dh4=@dh4, dh5=@dh5, dh6=@dh6, dh7=@dh7, dh8=@dh8, dh9=@dh9, dh10=@dh10, dh11=@dh11, dh12=@dh12, email1=@email1, email2=@email2, email3=@email3, email4=@email4, email5=@email5, email6=@email6, nsrsbm=@nsrsbm, zch=@zch, zzjgdm=@zzjgdm, cbrs=@cbrs, qylx=@qylx, sshy=@sshy, cym1=@cym1, cym2=@cym2, cym3=@cym3, cym4=@cym4, cym5=@cym5, gw=@gw, qydz=@qydz, jyfw=@jyfw WHERE tyshxydm=@tyshxydm", cn);

                    var item = GetColItem(cols, "异常日期");
                    var ycrq = Cell(dataLineNumber, item.Index);
                    SQLiteParameter param = new SQLiteParameter("@ycrq", ycrq);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "企业名称");
                    var qymc = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@qymc", qymc);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "经营状态");
                    var jyzt = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@jyzt", jyzt);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "法定代表人");
                    var fddbr = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@fddbr", fddbr);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "注册资本");
                    var zczb = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@zczb", zczb);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "成立日期");
                    var clrq = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@clrq", clrq);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "所属省份");
                    var sssf = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@sssf", sssf);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "所属城市");
                    var sscs = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@sscs", sscs);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "所属区县");
                    var ssqx = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@ssqx", ssqx);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "电话");
                    var dh1 = Cell(dataLineNumber, item.Index);
                    var lstDh1 = dh1.Split(';'); //电话清单1
                    item = GetColItem(cols, "更多电话");
                    var dh2 = Cell(dataLineNumber, item.Index);
                    var lstDh2 = dh2.Split(';'); //电话清单2
                    var dhs = new List<string>();
                    dhs.AddRange(lstDh1);
                    dhs.AddRange(lstDh2);
                    var dh_arr = new string[12];
                    for (var i = 0; i < dhs.Count && i < 12; dh_arr[i] = dhs[i], i++) ;
                    for (var i = 0; i < 12; i++)
                    {
                        param = new SQLiteParameter("@dh" + (i + 1).ToString(), dh_arr[i]);
                        param.DbType = DbType.String;
                        cmdInsert.Parameters.Add(param);
                        cmdUpdate.Parameters.Add(param);
                    }

                    item = GetColItem(cols, "邮箱");
                    var email1 = Cell(dataLineNumber, item.Index);
                    var lstEmail1 = email1.Split(';'); //邮箱清单1
                    item = GetColItem(cols, "更多邮箱");
                    var email2 = Cell(dataLineNumber, item.Index);
                    var lstEmail2 = email2.Split(';'); //邮箱清单2
                    var emails = new List<string>();
                    dhs.AddRange(lstEmail1);
                    dhs.AddRange(lstEmail2);
                    var email_arr = new string[6];
                    for (var i = 0; i < emails.Count && i < 6; email_arr[i] = emails[i], i++) ;
                    for (var i = 0; i < 6; i++)
                    {
                        param = new SQLiteParameter("@email" + (i + 1).ToString(), email_arr[i]);
                        param.DbType = DbType.String;
                        cmdInsert.Parameters.Add(param);
                        cmdUpdate.Parameters.Add(param);
                    }

                    item = GetColItem(cols, "统一社会信用代码"); // PK
                    var tyshxydm = Cell(dataLineNumber, item.Index).Trim();
                    param = new SQLiteParameter("@tyshxydm", tyshxydm);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    if (tyshxydm == string.Empty)
                    {
                        LogHelper.Trace($"无效的“统一社会信用代码”。忽略。DataLineNumber:{dataLineNumber}");
                        ignoreCount++;
                        dataLineNumber++;
                        continue;
                    }

                    item = GetColItem(cols, "纳税人识别号");
                    var nsrsbm = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@nsrsbm", nsrsbm);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "注册号");
                    var zch = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@zch", zch);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "组织机构代码");
                    var zzjgdm = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@zzjgdm", zzjgdm);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "参保人数");
                    var cbrs = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@cbrs", cbrs);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "企业类型");
                    var qylx = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@qylx", qylx);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "所属行业");
                    var sshy = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@sshy", sshy);
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "曾用名");
                    var cym = Cell(dataLineNumber, item.Index);
                    var lstCym = email1.Split(';'); //曾用名清单
                    var cym_arr = new string[5];
                    for (var i = 0; i < lstCym.Length && i < 5; cym_arr[i] = lstCym[i], i++) ;
                    for (var i = 0; i < 5; i++)
                    {
                        param = new SQLiteParameter("@cym" + (i + 1).ToString(), cym_arr[i]);
                        param.DbType = DbType.String;
                        cmdInsert.Parameters.Add(param);
                        cmdUpdate.Parameters.Add(param);
                    }

                    item = GetColItem(cols, "官网");
                    var gw = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@gw", gw);
                    param.DbType = DbType.String;
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "企业地址");
                    var qydz = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@qydz", qydz);
                    param.DbType = DbType.String;
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    item = GetColItem(cols, "经营范围");
                    var jyfw = Cell(dataLineNumber, item.Index);
                    param = new SQLiteParameter("@jyfw", jyfw);
                    param.DbType = DbType.String;
                    cmdInsert.Parameters.Add(param);
                    cmdUpdate.Parameters.Add(param);

                    var ada = new SQLiteDataAdapter($"select count(tyshxydm) from EnterpriseInfo where tyshxydm='{tyshxydm}'", cn);
                    var dt = new DataTable();
                    ada.Fill(dt);
                    if (Convert.ToInt32(dt.Rows[0][0]) > 0)
                    {
                        // Update
                        try
                        {
                            cmdUpdate.ExecuteNonQuery();
                            updateCount++;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Trace($"UpdateError::DataLineNumber:{dataLineNumber},{ex.Message}");
                        }
                    }
                    else
                    {
                        // Insert
                        try
                        {
                            cmdInsert.ExecuteNonQuery();
                            insertCount++;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Trace($"InsertError::DataLineNumber:{dataLineNumber},{ex.Message}");
                        }
                    }

                    dataLineNumber++;
                    cmdInsert.Dispose();
                    cmdUpdate.Dispose();
                    ada.Dispose();
                    dt.Dispose();
                }
                trans.Commit();
            }

            LogHelper.Trace($"FileName:{fileName}, Insert:{insertCount}, Updated:{updateCount}, Ignored:{ignoreCount}");
        }

        private static ColMap GetColItem(IEnumerable<ColMap> lst, string name)
        {
            return lst.First(x => x.Name == name);
        }
    }

    class ColMap
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public bool IsValid { get; set; }
    }
}
