using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.OleDb;

namespace System.App.Utility.Parsers
{
    /// <summary>
    /// An excel file parser
    /// Originally written by Wu Fan, modified by zys later.
    /// </summary>
    public class ExcelOperator
    {
        public DataSet DataSetFromFile { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sheet">Index starts from 1. Get 1st sheet by default</param>
        /// <param name="firstRowAsHeader">For the case of FirstRowAsHeader</param>
        public ExcelOperator(string path, int sheet = 1, bool firstRowAsHeader = true)
        {
            this.DataSetFromFile = new DataSet();
            OpenExcel(path, sheet, firstRowAsHeader);
        }

        private void OpenExcel(string path, int sheet, bool firstRowAsHeader)
        {
            if (System.IO.File.Exists(path) == false)
                throw new System.IO.FileNotFoundException("Specified Excel file doesn't exist");

            var dt = new DataTable();
            this.DataSetFromFile.Tables.Add(dt);

            object missing = System.Reflection.Missing.Value;
            var excel = new Excel.Application();

            if (excel == null)
            {                
                excel.DisplayAlerts = false;
                excel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                excel = null;
                throw new SystemException("Excel Application ojbect cannot be created!");
            }
            else
            {
                excel.Visible = false;
                excel.UserControl = true;
                var wb = excel.Application.Workbooks.Open(path, missing, true, missing, missing, missing, missing, missing, missing, true, missing, missing, missing, missing, missing);

                if (sheet < 1 || sheet > wb.Worksheets.Count)
                    throw new ArgumentException("sheet parameter invalid");

                var ws = wb.Worksheets.get_Item(sheet) as Excel.Worksheet;

                int rowCount = ws.UsedRange.Rows.Count;

                if (firstRowAsHeader == true)
                {
                    if (rowCount < 2)
                        throw new SystemException("Excel is empty");
                }
                else
                {
                    if (rowCount < 1)
                        throw new SystemException("Excel is empty");
                }               

                int columnCount = ws.UsedRange.Columns.Count;

                //添加行
                for (int row = 0; row < rowCount - (firstRowAsHeader ? 1 : 0); row++)
                {
                    DataRow dr = this.DataSetFromFile.Tables[0].NewRow();
                    this.DataSetFromFile.Tables[0].Rows.Add(dr);
                }

                //添加列
                for (int column = 0; column < columnCount; column++)
                {
                    DataColumn dc = new DataColumn();
                    this.DataSetFromFile.Tables[0].Columns.Add(dc);
                    Excel.Range ran = (Excel.Range)(ws.Cells[1, column + 1]);
                    ran.EntireColumn.Hidden = false;//取消隐藏列，否则无法读取这些列的数据
                    ran.EntireColumn.AutoFit();//自动调整列宽
                }

                //Read Headers
                if (firstRowAsHeader == true)
                {
                    for (int c = 0; c < columnCount; c++)
                    {
                        Excel.Range ran = (Excel.Range)ws.Cells[1, c + 1];//Worksheet索引从1开始              
                        if (string.IsNullOrEmpty(ran.Text) == false)
                        {
                            this.DataSetFromFile.Tables[0].Columns[c].ColumnName = ran.Text;
                        }
                    }
                }

                //Read cells
                for (int r = (firstRowAsHeader ? 1 : 0); r < rowCount; r++)
                {
                    for (int c = 0; c < columnCount; c++)
                    {
                        Excel.Range ran = (Excel.Range)ws.Cells[r + 1, c + 1];//Worksheet索引从1开始
                        this.DataSetFromFile.Tables[0].Rows[r - (firstRowAsHeader ? 1 : 0)][c] = ran.Text;//datatable行列索引从0开始                                     
                    }
                }

                excel.DisplayAlerts = false;
                excel.Quit();
            }
        }

#if false
        private void GetExcelConnect()
        {
            String dataPath = FilePath.Trim();
            if (dataPath == "")
            {
                System.Windows.MessageBox.Show("请选择要导入的数据文件!");
                return;
            }

            String strCon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dataPath + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1;'";

            OleDbConnection myConnect = new OleDbConnection(strCon);
            myConnect.Open();

            DataTable dtSheetName = myConnect.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
            String[] strTableNames = new String[dtSheetName.Rows.Count];
            for (int k = 0; k < dtSheetName.Rows.Count; k++)
            {
                strTableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
            }

            String strCommand = "select * from [" + strTableNames[0] + "]";//默认表名,e.g.“Sheet1” 
            OleDbDataAdapter myCommand = new OleDbDataAdapter(strCommand, myConnect);
            myCommand.Fill(DataSetFromFile, "[" + strTableNames[0] + "]");
            myConnect.Close();
        }
#endif

    }
}
