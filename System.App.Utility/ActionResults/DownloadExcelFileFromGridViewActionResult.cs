using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Diagnostics.CodeAnalysis;

namespace System.App.Utility.ActionResults
{
    public class DownloadExcelFileFromGridViewActionResult : ActionResult
    {
        public GridView ExcelGridView { get; set; }
        public string fileName { get; set; }

        public DownloadExcelFileFromGridViewActionResult(GridView gv, string pFileName)
        {
            ExcelGridView = gv;
            fileName = pFileName;
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public override void ExecuteResult(ControllerContext context)
        {
            HttpContext curContext = HttpContext.Current;
            curContext.Response.Clear();
            curContext.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            curContext.Response.Charset = "";
            curContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            curContext.Response.ContentType = "application/vnd.ms-excel";
            using(StringWriter sw = new StringWriter())
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                ExcelGridView.RenderControl(htw);
                byte[] byteArray = Encoding.Unicode.GetBytes(sw.ToString());
                using (MemoryStream s = new MemoryStream(byteArray))
                using (StreamReader sr = new StreamReader(s, Encoding.Unicode))
                {
                    curContext.Response.Write(sr.ReadToEnd());
                    curContext.Response.End();
                }
            }
        }
    }
}