using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Xls;

namespace ExcelTableLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = @"d:\data.xlsx";
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(fileName);
            //处理Excel数据，更多请参考官方Demo
            Spire.Xls.Worksheet sheet = workbook.Worksheets[0];
            Console.WriteLine(sheet.Range[2, 2].Text);
            Console.WriteLine(sheet.Range[3, 3].Text);
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
