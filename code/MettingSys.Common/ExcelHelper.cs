using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MettingSys.Common
{
    public class ExcelHelper
    {
        private static MemoryStream WriteToStream(HSSFWorkbook hssfworkbook)
        {
            //Write the stream data of workbook to the root directory
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);
            return file;
        }

        //Export(DataTable table, string headerText, string sheetName, string[] columnName, string[] columnTitle)
        /// <summary>
        /// 向客户端输出文件。
        /// </summary>
        /// <param name="table">数据表。</param>
        /// <param name="headerText">头部文本。</param>
        /// <param name="sheetName"></param>
        /// <param name="columnName">数据列名称。</param>
        /// <param name="columnTitle">表标题。</param>
        /// <param name="fileName">文件名称。</param>
        public static void Write(HttpContext context, DataTable table, string headerText, string sheetName, string[] columnName, string[] columnTitle, string fileName)
        {
            context.Response.Clear();

            context.Response.AddHeader("Content-Disposition",
                        "attachment; filename=" + fileName); //HttpUtility.UrlEncode(fileName));
            context.Response.ContentType = "application/vnd.ms-excel";
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            GenerateData(hssfworkbook, table, headerText, sheetName, columnName, columnTitle);
            context.Response.BinaryWrite(WriteToStream(hssfworkbook).GetBuffer());
            context.Response.End();
        }

        /// <summary>
        /// 向客户端输出文件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="list">数据表列表</param>
        /// <param name="headerText">头部文本</param>
        /// <param name="sheetNameList">数据列名称集合</param>
        /// <param name="columnName">表标题集合</param>
        /// <param name="columnTitle">表标题集合</param>
        /// <param name="fileName">文件名称</param>
        //public static void Write(HttpContext context, List<DataTable> list, string headerText, string[] sheetNameList, List<string[]> columnName, List<string[]> columnTitle, string fileName)
        //{
        //    context.Response.Clear();

        //    context.Response.AddHeader("Content-Disposition",
        //                "attachment; filename=" + fileName); //HttpUtility.UrlEncode(fileName));
        //    context.Response.ContentType = "application/vnd.ms-excel";
        //    HSSFWorkbook hssfworkbook = new HSSFWorkbook();
        //    for (int i = 0; i < sheetNameList.Length; i++)
        //    {
        //        GenerateData(hssfworkbook, list[i], headerText, sheetNameList[i], columnName[i], columnTitle[i]);
        //    }
        //    context.Response.BinaryWrite(WriteToStream(hssfworkbook).GetBuffer());
        //    context.Response.End();
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="table"></param>
        /// <param name="headerText"></param>
        /// <param name="sheetName"></param>
        /// <param name="columnName"></param>
        /// <param name="columnTitle"></param>
        /// <returns></returns>
        public static HSSFWorkbook GenerateData(HSSFWorkbook hssfworkbook, DataTable table, string headerText, string sheetName, string[] columnName, string[] columnTitle)
        {
            //HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet(sheetName);
                       
            ICellStyle dateStyle = hssfworkbook.CreateCellStyle();

            IDataFormat format = hssfworkbook.CreateDataFormat();

            dateStyle.DataFormat = format.GetFormat("@");

            #region 取得列宽

            int[] colWidth = new int[columnName.Length];
            for (int i = 0; i < columnName.Length; i++)
            {
                colWidth[i] = Encoding.GetEncoding(936).GetBytes(columnTitle[i]).Length;
            }
            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < columnName.Length; j++)
                {
                    if (columnName[j].IndexOf("/") > -1)
                    {
                        string[] list = columnName[j].Split('/');
                        int intTemp = 0;
                        foreach (string item in list)
                        {
                            intTemp += Encoding.GetEncoding(936).GetBytes(table.Rows[i][item].ToString()).Length;
                        }                        
                        if (intTemp > colWidth[j])
                        {
                            colWidth[j] = intTemp;
                        }
                    }
                    else
                    {
                        int intTemp = Encoding.GetEncoding(936).GetBytes(table.Rows[i][columnName[j]].ToString()).Length;
                        if (intTemp > colWidth[j])
                        {
                            colWidth[j] = intTemp;
                        }
                    }
                }
            }

            #endregion 取得列宽

            int rowIndex = 0;
            foreach (DataRow row in table.Rows)
            {
                #region 新建表，填充表头，填充列头，样式

                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = hssfworkbook.CreateSheet(sheetName + ((int)rowIndex / 65535).ToString());
                    }

                    #region 表头及样式

                    //if (!string.IsNullOrEmpty(headerText))
                    //{
                    //    IRow headerRow = sheet.CreateRow(0);
                    //    headerRow.HeightInPoints = 25;
                    //    headerRow.CreateCell(0).SetCellValue(headerText);

                    //    ICellStyle headStyle = hssfworkbook.CreateCellStyle();
                    //    headStyle.Alignment = HorizontalAlignment.Center;
                    //    IFont font = hssfworkbook.CreateFont();
                    //    font.FontHeightInPoints = 20;
                    //    font.Boldweight = 700;
                    //    headStyle.SetFont(font);

                    //    headerRow.GetCell(0).CellStyle = headStyle;
                    //    //sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                    //    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, table.Columns.Count - 1));
                    //}

                    #endregion 表头及样式

                    #region 列头及样式

                    {
                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.Height = 340;
                        ICellStyle headStyle = hssfworkbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = hssfworkbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        
                        headStyle.SetFont(font);
                        headStyle.FillBackgroundColor = HSSFColor.Red.Index;
                        for (int i = 0; i < columnName.Length; i++)
                        {
                            headerRow.CreateCell(i).SetCellValue(columnTitle[i]);
                            headerRow.GetCell(i).CellStyle = headStyle;
                            //设置列宽
                            if ((colWidth[i] + 1) * 256 > 30000)
                            {
                                sheet.SetColumnWidth(i, 10000);
                            }
                            else
                            {
                                sheet.SetColumnWidth(i, (colWidth[i] + 1) * 256);
                            }
                        }
                        /*
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }
                         * */
                    }

                    #endregion 列头及样式

                    if (!string.IsNullOrEmpty(headerText))
                    {
                        rowIndex = 1;
                    }
                    else
                    {
                        rowIndex = 2;
                    }
                }

                #endregion 新建表，填充表头，填充列头，样式

                #region 填充数据

                IRow dataRow = sheet.CreateRow(rowIndex);
                dataRow.Height = 300;
                for (int i = 0; i < columnName.Length; i++)
                {
                    ICell newCell = dataRow.CreateCell(i);
                    string drValue = "";
                    if (columnName[i].IndexOf("/") > -1)
                    {
                        string[] list = columnName[i].Split('/');                        
                        foreach (string item in list)
                        {
                            drValue += row[item].ToString() + "/";
                        }
                        drValue = drValue.TrimEnd('/');
                        newCell.SetCellValue(drValue);
                    }
                    else
                    {
                        drValue = row[columnName[i]].ToString();

                        switch (table.Columns[columnName[i]].DataType.ToString())
                        {
                            case "System.String"://字符串类型
                                if (drValue.ToUpper() == "TRUE")
                                    newCell.SetCellValue("是");
                                else if (drValue.ToUpper() == "FALSE")
                                    newCell.SetCellValue("否");
                                newCell.SetCellValue(drValue);
                                break;

                            case "System.DateTime"://日期类型
                                                   //DateTime dateV;
                                                   //DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(drValue);

                                newCell.CellStyle = dateStyle;//格式化显示
                                break;

                            case "System.Boolean"://布尔型
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                if (boolV)
                                    newCell.SetCellValue("是");
                                else
                                    newCell.SetCellValue("否");
                                break;

                            case "System.Int16"://整型
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;

                            case "System.Decimal"://浮点型
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(doubV);
                                break;

                            case "System.DBNull"://空值处理
                                newCell.SetCellValue("");
                                break;

                            default:
                                newCell.SetCellValue("");
                                break;
                        }
                    }
                }

                #endregion 填充数据

                rowIndex++;
            }

            return hssfworkbook;
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="filePath"></param>
        public static DataTable Import(string filePath)
        {
            HSSFWorkbook workbook = OpenWorkbook(filePath);
            HSSFSheet sheet = (HSSFSheet)workbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            DataTable dt = new DataTable();

            if (sheet.LastRowNum <= 0)
            {
                return dt;
            }

            int lastCellNum = sheet.GetRow(0).LastCellNum;
            //添加列
            if (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                for (int j = 0; j < lastCellNum; j++)
                {
                    HSSFCell cell = (HSSFCell)row.GetCell(j);
                    if (cell == null)
                    {
                        continue;
                    }
                    dt.Columns.Add(cell.ToString());
                }
            }
            //添加行
            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                DataRow dr = dt.NewRow();
                bool isChecked = false;

                for (int i = 0; i < row.LastCellNum; i++)
                {
                    if (i >= dt.Columns.Count)
                    {
                        break;
                    }

                    HSSFCell cell = (HSSFCell)row.GetCell(i);
                    if (cell == null || string.IsNullOrEmpty(cell.ToString()))
                    {
                        dr[i] = null;
                    }
                    else
                    {
                        isChecked = true;
                        //数字型
                        if (cell.CellType == CellType.Numeric)
                        {
                            dr[i] = cell.NumericCellValue;
                        }
                        //字符串型
                        else
                        {
                            dr[i] = cell.ToString();
                        }
                    }
                }
                if (isChecked)
                {
                    dt.Rows.Add(dr);
                }
                else
                {
                    dr = null;
                }
            }
            return dt;
        }

        public static HSSFWorkbook OpenWorkbook(String fileName)
        {
            try
            {
                return new HSSFWorkbook(new FileStream(fileName, FileMode.Open));
            }
            catch (IOException)
            {
                throw;
            }
        }

        public static HSSFWorkbook ExportDT(DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = workbook.CreateSheet() as HSSFSheet;
            sheet.DefaultColumnWidth = 2;
            sheet.DefaultRowHeight = 15 * 20;

            #region 右击文件 属性信息

            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "aaa";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();

                si.Author = "licao"; //填加xls文件作者信息
                si.ApplicationName = "网银"; //填加xls文件创建程序信息
                si.LastAuthor = "licao"; //填加xls文件最后保存者信息
                si.Subject = "本文档由 JINS OA 系统生成"; //填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }

            #endregion 右击文件 属性信息

            HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd HH:mm:ss");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式

                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet() as HSSFSheet;
                    }

                    #region 表头及样式

                    {
                        HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;

                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 20;
                        font.FontName = "宋体";
                        font.Boldweight = 700;

                        headStyle.SetFont(font);

                        headerRow.GetCell(0).CellStyle = headStyle;

                        sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        //headerRow.Dispose();
                    }

                    #endregion 表头及样式

                    #region 列头及样式

                    {
                        HSSFRow headerRow = sheet.CreateRow(1) as HSSFRow;
                        headerRow.Height = 15 * 20;
                        HSSFCellStyle cellStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        cellStyle.VerticalAlignment = VerticalAlignment.Center;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 9;
                        font.FontName = "宋体";
                        cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                        cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                        cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                        cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                        cellStyle.FillBackgroundColor = HSSFColor.Blue.Index2;

                        font.Boldweight = 700;
                        cellStyle.SetFont(font);

                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = cellStyle;

                            if ((arrColWidth[column.Ordinal] + 1) * 256 > 30000)
                            {
                                sheet.SetColumnWidth(column.Ordinal, 10000);
                            }
                            else
                            {
                                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                            }
                            //设置列宽
                        }
                        //headerRow.Dispose();
                    }

                    #endregion 列头及样式

                    rowIndex = 2;
                }

                #endregion 新建表，填充表头，填充列头，样式

                #region 填充内容

                HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        //case "System.String": //字符串类型
                        //    double result;
                        //    if (isNumeric(drValue, out result))
                        //    {
                        //        double.TryParse(drValue, out result);
                        //        newCell.SetCellValue(result);
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        newCell.SetCellValue(drValue);
                        //        break;
                        //    }

                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);

                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle; //格式化显示
                            break;

                        case "System.Boolean": //布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;

                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;

                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;

                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            break;

                        default:
                            newCell.SetCellValue(drValue);
                            break;
                    }
                }

                #endregion 填充内容

                rowIndex++;
            }

            return workbook;
        }

        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置</param>
        //public static void Write(HttpContext context, DataTable dtSource, string strHeaderText, string strFileName)
        //{
        //    context.Response.Clear();
        //    context.Response.AddHeader("Content-Disposition", "attachment; filename=" + strFileName); //HttpUtility.UrlEncode(fileName));
        //    context.Response.ContentType = "application/vnd.ms-excel";
        //    //HSSFWorkbook hssfworkbook = GenerateData(table, headerText, sheetName, columnName, columnTitle);
        //    context.Response.BinaryWrite(ExportDT(dtSource, strHeaderText).GetBuffer());
        //    context.Response.End();
        //}

        public static void Write(HttpContext context, DataTable dtSource, string strHeaderText, string strFileName)
        {
            context.Response.Clear();

            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + strFileName); //HttpUtility.UrlEncode(fileName));
            context.Response.ContentType = "application/vnd.ms-excel";

            HSSFWorkbook hssfworkbook = ExportDT(dtSource, strHeaderText);
            context.Response.BinaryWrite(WriteToStream(hssfworkbook).GetBuffer());
            context.Response.End();
        }

        public static bool isNumeric(String message, out double result)
        {
            Regex rex = new Regex(@"^[-]?\d+[.]?\d*$");
            result = -1;
            if (rex.IsMatch(message))
            {
                result = double.Parse(message);
                return true;
            }
            else
                return false;
        }
    }
}
