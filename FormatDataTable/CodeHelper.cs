using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatDataTable
{
    public class CodeHelper
    {
        public static void ExportDataTableToXlsx(DataTable dt, string fileName)
        {
            var workbook = new Workbook();
            Worksheet sheet = null;
            Action<int, int, string> Cell = (rowIndex, colIndex, value) =>
            {
                if (colIndex == 0 || rowIndex == 0) return;

                sheet.Range[rowIndex, colIndex].Value = value;
            };
            sheet = workbook.CreateEmptySheet();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var colIndex = 1;
                DataRow dr = dt.Rows[i];

                Cell(i + 1, colIndex, dr["qymc"].ToString());
            }
            workbook.SaveToFile(fileName);

        }

    }
}
