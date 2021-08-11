using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace System.App.Utility.Helpers
{
    public static class HttpFormSimulator
    {
        /// <summary>
        /// Simulate a form submit action.
        /// i.e. Get response from a POST method
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="fc">form collection of parameters</param>
        /// <returns>response from server side</returns>
        /// <sample>
        /// var dict = new Dictionary&lt;string,string&gt;();
        /// dict.Add("drug_name",textBoxDrugName.Text);
        /// dict.Add("drug_id",textBoxDrugId.Text);
        /// this.textBoxResponse.Text = HttpFormSimulator.Submit(textBoxUrl.Text, dict);
        /// </sample>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static string Submit(string url, Dictionary<string,string> fc)
        {
            // Create a request using a URL that can receive a post. 
            var request = WebRequest.Create(url);
            // Set the Method property of the request to POST.
            request.Method = "POST";

            // Create POST data and convert it to a byte array.
            //string postData = ("drug_name=" + textBoxDrugName.Text + "&drug_id=" + textBoxDrugId.Text); //HttpUtility.UrlEncode
            StringBuilder postData = new StringBuilder();
            if (fc != null && fc.Count > 0)
            {
                postData.Append(fc.Select(x => x.Key + "=" + HttpUtility.UrlEncode(x.Value)).Aggregate((x, y) => x + "&" + y));
            }
    
            byte[] byteArray = Encoding.UTF8.GetBytes(postData.ToString());
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            
            // Clean up the streams.
            reader.Close();
            dataStream.Close(); // CA2202: Dispose is called more than once. The StreamReader class will take 'ownership' of the Stream so disposing StreamReader will also dispose Stream.
            response.Close();

            return responseFromServer;
        }
    }
}
