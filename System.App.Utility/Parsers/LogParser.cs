using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Interop.MSUtil;
using System.Linq;
using System.Xml.Serialization;
using System.Xml;

namespace System.App.Utility.Parsers
{
    public class IISW3CLog
    {
        public DateTime datetime;
        public string c_ip, cs_username, s_sitename, s_ip, s_port, cs_method, cs_uri_stem, cs_uri_query, cs_version, cs_host;
        public int sc_status, sc_substatus, sc_win32_status, sc_bytes, cs_bytes, time_taken;
        public string ClientRequestUserAgentHeader; //cs (User-Agent)
        public string ClientRequestCookieHeader; // cs(Cookie)
        public string ClientRequestRefererHeader; // cs(Referer)
    }

    /// <summary>
    /// A simple log object used for serialization
    /// </summary>
    public class LogEntry
    {
        public DateTime datetime;
        public string url;
    }

    /// <summary>
    /// Parses log files
    /// Based on Microsoft Log Parser. 
    /// Supports various log formats, e.g. IIS, IISW3C, CSV, TSV, XML, W3C, NCSA, EVT, REG
    /// </summary>
    public static class LogParser
    {
        /// <remarks>
        /// 
        /// Error	Interop type 'Interop.MSUtil.LogQueryClassClass' cannot be embedded. Use the applicable interface instead.
        /// 
        /// In most cases this error is the result of code which tries to instantiate a COM object e.g. here piece of code starting up Excel: Excel.ApplicationClass xlapp = new Excel.ApplicationClass();
        /// Typically, in .Net 4 you just need to remove the 'Class' suffix and compile the code: Excel.Application xlapp = new Excel.Application();
        /// </remarks>
        public static List<IISW3CLog> ParseIISW3C(string path, bool isFolder = false, List<string> filter = null)
        {
            var list = new List<IISW3CLog>();

            if (isFolder == true && IO.Directory.Exists(path) == false ||
                isFolder == false && IO.File.Exists(path) == false)
            {
                return list;
            }

            var keywords = new List<string>();
            if (filter != null && filter.Count > 0)
            {
                foreach (var x in filter)
                {
                    if (string.IsNullOrWhiteSpace(x) == false)
                    {
                        keywords.Add(x.ToLower());
                    }
                }
                keywords = keywords.Distinct().ToList();
            }
            else
            {
                keywords = null;
            }

            // Instantiate the LogQuery object
            var oLogQuery = new LogQueryClass();

            // Instantiate the Event Log Input Format object
            var ctx = new COMIISW3CInputContextClass();

            // Set Recursive level to 0
            ctx.recurse = 0;

            string[] files = { path };
            if (isFolder)
            {
                files = IO.Directory.GetFiles(path);
            }

            foreach (var file in files)
            {
                // Create the query
                string query = @"SELECT date,time,c-ip,cs-username,s-sitename,s-ip,s-port,cs-method,cs-uri-stem,cs-uri-query,sc-status,sc-substatus,sc-win32-status,sc-bytes,cs-bytes,time-taken,cs-version,cs-host,cs(User-Agent),cs(Cookie),cs(Referer) FROM " + file;

                // Execute the query
                ILogRecordset oRecordSet = oLogQuery.Execute(query, ctx);

                // Browse the recordset
                for (; !oRecordSet.atEnd(); oRecordSet.moveNext())
                {
                    var record = oRecordSet.getRecord();
                    var cs_uri_stem = record.getValue("cs-uri-stem");

                    //
                    // judge whether url stem contains any keyword
                    if (keywords != null)
                    {
                        if (cs_uri_stem is DBNull)
                        {
                            continue;
                        }

                        string s = cs_uri_stem;
                        if (string.IsNullOrWhiteSpace(s))
                        {
                            continue;
                        }

                        s = s.Trim().ToLower();
                        if (keywords.Any(x => s.Contains(x)) == false)
                        {
                            continue;
                        }
                    }

                    var log = new IISW3CLog();
                    DateTime date = record.getValue("date");
                    DateTime time = record.getValue("time");
                    log.datetime = date.Date + time.TimeOfDay;
                    var c_ip = record.getValue("c-ip");
                    var cs_username = record.getValue("cs-username");
                    var cs_method = record.getValue("cs-method");
                    var cs_uri_query = record.getValue("cs-uri-query");
                    var sc_status = record.getValue("sc-status");
                    var sc_substatus = record.getValue("sc-substatus");
                    var ClientRequestUserAgentHeader = record.getValue("cs(User-Agent)");
                    var ClientRequestCookieHeader = record.getValue("cs(Cookie)");
                    var ClientRequestRefererHeader = record.getValue("cs(Referer)");

                    if (c_ip is DBNull == false)
                    {
                        log.c_ip = c_ip;
                    }
                    if (cs_username is DBNull == false)
                    {
                        log.cs_username = cs_username;
                    }
                    if (cs_method is DBNull == false)
                    {
                        log.cs_method = cs_method;
                    }
                    if (cs_uri_stem is DBNull == false)
                    {
                        log.cs_uri_stem = cs_uri_stem;
                    }
                    if (cs_uri_query is DBNull == false)
                    {
                        log.cs_uri_query = cs_uri_query;
                    }
                    if (sc_status is DBNull == false)
                    {
                        log.sc_status = sc_status;
                    }
                    if (sc_substatus is DBNull == false)
                    {
                        log.sc_substatus = sc_substatus;
                    }
                    if (ClientRequestUserAgentHeader is DBNull == false)
                    {
                        log.ClientRequestUserAgentHeader = ClientRequestUserAgentHeader;
                    }
                    if (ClientRequestCookieHeader is DBNull == false)
                    {
                        log.ClientRequestCookieHeader = ClientRequestCookieHeader;
                    }
                    if (ClientRequestRefererHeader is DBNull == false)
                    {
                        log.ClientRequestRefererHeader = ClientRequestRefererHeader;
                    }

                    list.Add(log);
                }

                // Close the recordset
                oRecordSet.close();
            }


            return list;
        }

#if false
        /// <summary>        
        /// Process IISW3C log folders to retrive relevant log entries of infobutton requests.        
        /// </summary>
        private void ProcessIISW3CLogFolderForInfobutton()
        {
            //
            // Get pre-filtered log entries
            var entries = LogParser.ParseIISW3C(@"C:\Users\elevenjohns\Desktop\W3SVC5\W3SVC3", true, new List<string>() { "/display/", "/upload/" });
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer ser = new XmlSerializer(entries.GetType());
                ser.Serialize(sw, entries);
                var s = (sw.ToString());

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(s);
                xdoc.Save(@"d:\PreProcessed.xml");
            }

            //
            // Further filter by more restirctions
            var filtered = new List<LogEntry>();
            entries.ForEach(x =>
            {
                if (x.cs_uri_stem.Contains("/Upload") && x.cs_uri_stem != "/Upload/EBM/DrugFactSheet/drugfactsheet.css" && x.cs_uri_stem != "/Display/DrugFactSheetPost" && x.cs_uri_stem != "/Display/DrugPartial" && x.cs_uri_stem.Contains("/Upload/Plugin/") == false && x.cs_uri_stem.EndsWith("/") == false && x.cs_uri_stem.Contains("/Upload/Drug/TCM/Images") == false)
                {
                    filtered.Add(new LogEntry() { datetime = x.datetime, url = x.cs_uri_stem });
                }
            });
            File.Delete(@"d:\Filtered.xml");
            using (StringWriter sw = new StringWriter())
            {
                var ser = new XmlSerializer(filtered.GetType());
                ser.Serialize(sw, filtered);
                var s = (sw.ToString());

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(s);
                xdoc.Save(@"d:\Filtered.xml");
            }

            //
            // Group by url and output a html table for Excel
            var table = "<table style=\"font-family:Trebuchet MS, Arial, Helvetica, sans-serif; width:100%; border-collapse:collapse; \">";
            filtered.GroupBy(x => x.url, (k, v) => new { key = k, group = v.Count() }).OrderByDescending(x => x.group).ToList().ForEach(x =>
            {
                var index1 = x.key.LastIndexOf('/');
                var index2 = x.key.LastIndexOf('.');
                var name = x.key.Substring(index1 + 1, index2 - index1 - 1);
                var tr = "<tr><td>" + x.key + "</td><td>" + name + "</td><td>" + x.group + "</td></tr>";
                table += tr;
            });
            table += "</table>";

            File.Delete(@"d:\output.html");
            File.WriteAllText(@"d:\output.html", table, Encoding.Unicode);
        }
#endif
    }
}
