using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using qiyubrother;
using Spire.Xls;

namespace QueryData
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string configFileName = "config.json";
        string mysqlConnectionString = string.Empty;
        DataTable dt = new DataTable();
        public MainWindow()
        {
            InitializeComponent();

            if (!File.Exists(configFileName))
            {
                LogHelper.Trace("config.json file losted.");
                return;
            }
            var jo = JObject.Parse(File.ReadAllText(configFileName));
            try { mysqlConnectionString = jo["mysqlConnectionString"].ToString(); }
            catch { LogHelper.Trace("config error. key is [mysqlConnectionString]"); }

            LogHelper.StartService();
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            dt.Clear();               

            var sql = "select ";
            sql += "qymc as '企业名称',";
            sql += "jyzt as '经营状态',";
            sql += "fddbr as '法定代表人',";
            sql += "clrq as '成立日期',";
            sql += "sssf as '所属省份',";
            sql += "sscs as '所属城市',";
            sql += "ssqx as '所属区县',";
            sql += "dh1 as '电话1',";
            sql += "dh2 as '电话2',";
            sql += "dh3 as '电话3',";
            sql += "dh4 as '电话4',";
            sql += "dh5 as '电话5',";
            sql += "dh6 as '电话6',";
            sql += "dh7 as '电话7',";
            sql += "dh8 as '电话8',";
            sql += "dh9 as '电话9',";
            sql += "dh10 as '电话10',";
            sql += "dh11 as '电话11',";
            sql += "dh12 as '电话12',";
            sql += "zj1 as '座机1',";
            sql += "zj2 as '座机2',";
            sql += "zj3 as '座机3',";
            sql += "zj4 as '座机4',";
            sql += "zj5 as '座机5',";
            sql += "email1 as '邮箱1',";
            sql += "email2 as '邮箱2',";
            sql += "email3 as '邮箱3',";
            sql += "email4 as '邮箱4',";
            sql += "email5 as '邮箱5',";
            sql += "email6 as '邮箱6',";
            sql += "tyshxydm as '统一社会信用代码',";
            sql += "nsrsbm as '纳税人识别号',";
            sql += "zch as '注册号',";
            sql += "zzjgdm as '组织机构代码',";
            sql += "cbrs as '参保人数',";
            sql += "qylx as '企业类型',";
            sql += "sshy as '所属行业',";
            sql += "cym1 as '曾用名1',";
            sql += "cym2 as '曾用名2',";
            sql += "cym3 as '曾用名3',";
            sql += "gw as '官网',";
            sql += "qydz as '企业地址',";
            sql += "jyfw as '经营范围',";
            sql += "th as '是否同行'";
            sql += " from enterpriseinfo where 1=1 ";

            using (var conn = new MySqlConnection(mysqlConnectionString))
            {
                #region 查询条件
                // 企业名称
                qymc.Text = qymc.Text.Trim();
                if (!string.IsNullOrEmpty(qymc.Text))
                {
                    sql += $" and qymc like '%{qymc.Text}%'";
                }
                // 经营状态
                jyzt.Text = jyzt.Text.Trim();
                if (!string.IsNullOrEmpty(jyzt.Text))
                {
                    sql += $" and jyzt like '%{jyzt.Text}%'";
                }
                // 法定代表人
                fddbr.Text = fddbr.Text.Trim();
                if (!string.IsNullOrEmpty(fddbr.Text))
                {
                    sql += $" and fddbr like '%{fddbr.Text}%'";
                }
                // 成立日期
                clrq.Text = clrq.Text.Trim();
                if (!string.IsNullOrEmpty(clrq.Text))
                {
                    sql += $" and clrq like '%{clrq.Text}%'";
                }
                // 所属省份
                sssf.Text = sssf.Text.Trim();
                if (!string.IsNullOrEmpty(sssf.Text))
                {
                    sql += $" and sssf like '%{sssf.Text}%'";
                }
                // 所属城市
                sscs.Text = sscs.Text.Trim();
                if (!string.IsNullOrEmpty(sscs.Text))
                {
                    sql += $" and sscs like '%{sscs.Text}%'";
                }
                // 所属区县
                ssqx.Text = ssqx.Text.Trim();
                if (!string.IsNullOrEmpty(ssqx.Text))
                {
                    sql += $" and ssqx like '%{ssqx.Text}%'";
                }
                // 电话1 - 电话12 + 座机1 - 座机5
                dh.Text = dh.Text.Trim();
                if (!string.IsNullOrEmpty(dh.Text))
                {
                    sql += $" and (dh1 like '%{dh.Text}%' or dh2 like '%{dh.Text}%' or dh3 like '%{dh.Text}%' or dh4 like '%{dh.Text}%' or dh5 like '%{dh.Text}%' or dh6 like '%{dh.Text}%' or dh7 like '%{dh.Text}%' or dh8 like '%{dh.Text}%' or dh9 like '%{dh.Text}%' or dh10 like '%{dh.Text}%' or dh11 like '%{dh.Text}%' or dh12 like '%{dh.Text}%' or zj1 like '%{dh.Text}%' or zj2 like '%{dh.Text}%' or zj3 like '%{dh.Text}%' or zj4 like '%{dh.Text}%' or zj5 like '%{dh.Text}%')";
                }
                // 邮箱1 - 邮箱6
                email.Text = email.Text.Trim();
                if (!string.IsNullOrEmpty(email.Text))
                {
                    sql += $" and (email1 like '%{email.Text}%' or email2 like '%{email.Text}%' or email3 like '%{email.Text}%' or email4 like '%{email.Text}%' or email5 like '%{email.Text}%' or email6 like '%{email.Text}%')";
                }
                // 统一社会信用代码
                tyshxydm.Text = tyshxydm.Text.Trim();
                if (!string.IsNullOrEmpty(tyshxydm.Text))
                {
                    sql += $" and tyshxydm like '%{tyshxydm.Text}%'";
                }
                // 纳税人识别号
                nsrsbm.Text = nsrsbm.Text.Trim();
                if (!string.IsNullOrEmpty(nsrsbm.Text))
                {
                    sql += $" and nsrsbm like '%{nsrsbm.Text}%'";
                }
                // 组织机构代码
                zzjgdm.Text = zzjgdm.Text.Trim();
                if (!string.IsNullOrEmpty(zzjgdm.Text))
                {
                    sql += $" and zzjgdm like '%{zzjgdm.Text}%'";
                }
                // 参保人数
                cbrs.Text = cbrs.Text.Trim();
                if (!string.IsNullOrEmpty(cbrs.Text))
                {
                    sql += $" and cbrs like '%{cbrs.Text}%'";
                }
                // 企业类型
                qylx.Text = qylx.Text.Trim();
                if (!string.IsNullOrEmpty(qylx.Text))
                {
                    sql += $" and qylx like '%{qylx.Text}%'";
                }
                // 所属行业
                sshy.Text = sshy.Text.Trim();
                if (!string.IsNullOrEmpty(sshy.Text))
                {
                    sql += $" and sshy like '%{sshy.Text}%'";
                }
                // 曾用名1 - 曾用名3
                cym.Text = cym.Text.Trim();
                if (!string.IsNullOrEmpty(cym.Text))
                {
                    sql += $" and (cym1 like '%{cym.Text}%' or cym2 like '%{cym.Text}%' or cym3 like '%{cym.Text}%')";
                }
                // 企业地址
                qydz.Text = qydz.Text.Trim();
                if (!string.IsNullOrEmpty(qydz.Text))
                {
                    sql += $" and qydz like '%{qydz.Text}%'";
                }
                // 经营范围
                jyfw.Text = jyfw.Text.Trim();
                if (!string.IsNullOrEmpty(jyfw.Text))
                {
                    sql += $" and jyfw like '%{jyfw.Text}%'";
                }
                // 是否同行
                if (th.SelectedIndex > 0)
                {
                    if (th.SelectedIndex == 2)
                    {
                        sql += $" and th = 'Y'";
                    }
                    else
                    {
                        sql += $" and (th = 'N' or th is null)";
                    }
                }
                #endregion

                var ada = new MySqlDataAdapter(sql, conn);
                var rst = ada.Fill(dt);
                if (rst == 0)
                {
                    ;
                }
                else
                {
                    grid.ItemsSource = dt.DefaultView;
                    grid.GridLinesVisibility = DataGridGridLinesVisibility.All;
                }
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            //SaveFileDialog sfd = new SaveFileDialog();
            //sfd.Title = "导出数据到";
            //sfd.Filter = "Excel文件|*.xlsx|All files (*.*)|*.*";

            //Workbook workbook = new Workbook();
            //Worksheet sheet = null;

            //if (dt.Rows.Count == 0)
            //{
            //    MessageBox.Show("没有任何数据！");
            //    return;
            //}


            ////调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮
            //if (sfd.ShowDialog() == true)
            //{
            //    CodeHelper.ExportDataTableToXlsx(dt, sfd.FileName);

            //    MessageBox.Show("导出成功！");
            //}
        }
    }
}
