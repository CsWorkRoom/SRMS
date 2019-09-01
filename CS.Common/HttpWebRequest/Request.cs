using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CS.Common.ApiRequest
{
    public static class Request
    {
        public static string PostHttp2(string url,string body)
        {
            byte[] bs = Encoding.UTF8.GetBytes(body);
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = bs.Length;

            //Console.WriteLine("完成准备工作");
            using (Stream reqStream = myRequest.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }

            using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse())
            {
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                var rs = sr.ReadToEnd();
                return rs;
                //Console.WriteLine("反馈结果" + responseString);
            }
            //Console.WriteLine("完成调用接口");
        }


        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url">请求url(不含参数)</param>
        /// <param name="body">请求body. 如果是soap"text/xml; charset=utf-8"则为xml字符串；post的cotentType为"application/x-www-form-urlencoded"则格式为"roleId=1&uid=2"</param>
        /// <param name="timeout">等待时长(毫秒)</param>
        /// <param name="contentType">Content-type http标头的值. post默认为"text/xml;charset=UTF-8"</param>
        /// <returns></returns>
        public static string PostHttp(string url, string body,string contentType= "text/xml;charset=utf-8")
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = "POST";
            //httpWebRequest.Timeout = timeout;//设置超时
            if (contentType.Contains("text/xml"))
            {
                httpWebRequest.Headers.Add("SOAPAction", "http://tempuri.org/mediate");
            }

            byte[] btBodys = Encoding.UTF8.GetBytes(body);
            httpWebRequest.ContentLength = btBodys.Length;
            httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

            HttpWebResponse httpWebResponse;
            try
            {
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpWebResponse = (HttpWebResponse)ex.Response;
            }

            //HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();
            httpWebRequest.Abort();
            httpWebResponse.Close();

            return responseContent;
        }
        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="url">请求url(不含参数)</param>
        /// <param name="postDataStr">参数部分：roleId=1&uid=2</param>
        /// <param name="timeout">等待时长(毫秒)</param>
        /// <returns></returns>
        public static string GetHttp(string url, string postDataStr,int timeout=2000)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.Timeout = timeout;//等待

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

    }
}
