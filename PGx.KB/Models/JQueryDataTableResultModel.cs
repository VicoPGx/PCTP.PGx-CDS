using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PGx.KB.Models
{
    /// <summary>
    /// Class that encapsulates json result for DataTables plugin
    /// </summary>
    public class JQueryDataTableResultModel
    {
         public JQueryDataTableResultModel()
        {
            iTotalDisplayRecords = 0;
            iTotalRecords = 0;
            aaData = null;
        }

        /// <summary>
        /// Request sequence number sent by DataTable, same value must be returned in response
        /// </summary>
        public string sEcho{ get; set; }

        /// <summary>
        /// Number of total records
        /// </summary>
        public int iTotalRecords { get; set; }

        /// <summary>
        /// Number of total records filtered by search keyword
        /// </summary>
        public int iTotalDisplayRecords { get; set; }

        /// <summary>
        /// Array of array 
        /// </summary>
        public object aaData { get; set; } //IEnumerable<string[]>
    }
}