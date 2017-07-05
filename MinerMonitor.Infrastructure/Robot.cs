using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MinerMonitor.Infrastructure
{
    public class Robot
    {
        private int timeout = 60000;
        private CookieContainer cookie = new CookieContainer();
        private string userAgent = "";
        
        public Robot()
        {
            Random r = new Random();
            int n = r.Next(5);
            switch (n)
            {
                case 0:
                    userAgent = "MSIE 6.0";
                    break;
                case 1:
                    userAgent = "MSIE 7.0";
                    break;
                case 2:
                    userAgent = "MSIE 8.0";
                    break;
                case 3:
                    userAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215; fqSpider)";
                    break;
                default:
                    userAgent = "MSIE 6.0";
                    break;
            }
        }

        public string GetPage(string url)
        {
            return GetPage(url, null, Encoding.Default);
        }

        public string GetPage(string url, Encoding en)
        {
            return GetPage(url, null, en);
        }

        public string GetPage(string url, string referer)
        {
            return GetPage(url, referer, Encoding.Default);
        }

        public string GetPage(string url,string referer, Encoding en)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = userAgent;
            if (referer != null) request.Referer = referer;
            request.Timeout = timeout;
            if (cookie.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
                cookie = request.CookieContainer;
            }
            else
            {
                request.CookieContainer = cookie;
            }


            string text = null;
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, en);
                text = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            catch(Exception exc)
            {
                return null;
            }
            finally
            {
                if (response != null)
                    response.Close(); 
                    
                
            }

            return text;
        }


        public string PostPage(string url, string postData, string referer, Encoding enc)
        {
            //System.Net.ServicePointManager.Expect100Continue = false;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.UseDefaultCredentials = true;
            //request.Timeout = 1000 * 60 * 10;
            if (referer != null) request.Referer = referer;
            request.Timeout = timeout;
            if (cookie.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
                cookie = request.CookieContainer;
            }
            else
            {
                request.CookieContainer = cookie;
            }

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, enc);
            string text = reader.ReadToEnd();
            reader.Close();
            stream.Close();
            response.Close();
            return text;
        }
        

        public bool GetFile(string url,string path)
        {
            return GetFile(url, path, null);
        }

        public bool GetFile(string url, string path,string referer)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "MSIE 6.0";
            
            if (referer != null) request.Referer = referer;
            request.Timeout = timeout;
            if (cookie.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
                cookie = request.CookieContainer;
            }
            else
            {
                request.CookieContainer = cookie;
            }

            HttpWebResponse response = null;
            Stream stream = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                stream = response.GetResponseStream();
            }
            catch
            {
                if (stream != null)
                    stream.Close();
                if (response != null)
                    response.Close();
                return false;
            }

            byte[] buffer = new byte[1024];
            Stream file = File.OpenWrite(path);
            bool ok = true;

            try
            {
                int byteread = stream.Read(buffer, 0, buffer.Length);

                while (byteread > 0)
                {
                    file.Write(buffer, 0, byteread);
                    byteread = stream.Read(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                ok = false;
            }
            finally
            {
                file.Close();
                stream.Close();
                response.Close();
            }

            return ok;
        }



        public string PostPage(string url, string postData)
        {
            return PostPage(url, postData, null, Encoding.Default);
        }      

       

        public void PostFile(string url, string postData,string path)
        {
            PostFile(url, postData, path, null);
        }

        public void PostFile(string url, string postData, string path,string referer)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (referer != null) request.Referer = referer;
            request.Timeout = timeout;
            if (cookie.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
                cookie = request.CookieContainer;
            }
            else
            {
                request.CookieContainer = cookie;
            }
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            stream = response.GetResponseStream();
            byte[] buffer = new byte[100];
            int byteread = stream.Read(buffer, 0, buffer.Length);
            Stream file = File.OpenWrite(path);
            while (byteread > 0)
            {
                file.Write(buffer, 0, byteread);
                byteread = stream.Read(buffer, 0, buffer.Length);
            }
            file.Close();
            stream.Close();
            response.Close();
        }


        public string[] ParseText(string pat, string text,int cols)
        {
            Regex r = new Regex(pat);
            Match m = r.Match(text);
            List<string> list = new List<string>();
            while (m.Success)
            {
                for (int i = 1; i <= cols && i < m.Groups.Count; i++)
                {
                    string temp = m.Groups[i].ToString();
                    if (!string.IsNullOrEmpty(temp))
                    {
                        list.Add(temp);
                    }
                }
                m = m.NextMatch();
            }
            return list.ToArray();
        }

/*        public int GetSqlResult(string s)
        {
            OracleConnection conn = new OracleConnection(constr);
            OracleCommand cm = new OracleCommand(s,conn);
            conn.Open();
            int result = 0;
            OracleDataReader reader = cm.ExecuteReader();
            if (reader.Read())
            {
                result = reader.GetInt32(0);
            }
            conn.Close();
            return result;
        }

        public void ExecuteSql(string []comms)
        {
            OracleConnection conn = new OracleConnection(constr);
            OracleCommand cm = new OracleCommand("", conn);
            conn.Open();
            foreach (string comm in comms)
            {
                cm.CommandText = comm;
                cm.ExecuteNonQuery();
            }
            conn.Close();
        }
        */
        public string Encode(string s, Encoding e1, Encoding e2)
        {
            return e2.GetString(e1.GetBytes(s));
        }

      

       
    }
}
