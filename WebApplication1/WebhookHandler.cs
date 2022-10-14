using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Web;

namespace WebApplication1
{
    public class WebhookHandler : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the Web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var body = string.Empty;
            context.Request.InputStream.Position = 0;

            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                body = inputStream.ReadToEnd();
            }

            JObject jsonObj = JObject.Parse(body);
            dynamic dynJsonObj = (dynamic)jsonObj;

            var recordType = dynJsonObj.RecordType;
            
            switch (recordType.ToString())
            {
                case "Delivery":
                    var messageId = dynJsonObj.MessageID;
                    var path = Path.GetTempPath() + "postmark\\" + messageId + ".json";
                    File.WriteAllText(path, jsonObj.ToString());
                    break;
            }

            context.Response.StatusCode = 200;
            context.Response.End();
        }

        #endregion
    }
}
