using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Payment
{
    public class WireCardController : Controller
    {
        /// <summary>
        /// 创建表单支付
        /// 使用 WireCard 的HPP(Host Payment Page)支付方式 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var reqFormData = new NameValueCollection();
            //表单数据
            reqFormData.Add("request_time_stamp", DateTime.UtcNow.ToString("yyyyMMddHHmmss")); //支付时间：人家指定要UtcNow
            reqFormData.Add("request_id", Guid.NewGuid().ToString()); //请求编号
            reqFormData.Add("merchant_account_id", "your_account_id"); //商户编号：WireCard帐号，等同支付宝帐号，用来打款
            reqFormData.Add("transaction_type", "authorization");
            reqFormData.Add("requested_amount", "100.00"); //付款金额
            reqFormData.Add("requested_amount_currency", "CNY");
            reqFormData.Add("redirect_url", "http://localhost/Payment/WireCard/Result"); //支付结果
            reqFormData.Add("ip_address", "127.0.0.1"); // 本机外网IP：
            reqFormData.Add("secret_key", "secret_key");
            //数字签名，防止传输过程中数据被篡改
            reqFormData.Add("request_signature",
                getSHA256(
                reqFormData["request_time_stamp"] +
                reqFormData["request_id"] +
                reqFormData["merchant_account_id"] +
                reqFormData["transaction_type"] +
                reqFormData["requested_amount"] +
                reqFormData["requested_amount_currency"] +
                reqFormData["redirect_url"] +
                reqFormData["ip_address"] +
                reqFormData["secret_key"]
                )
            );
            reqFormData.Add("attempt_three_d", "true"); // With the 3D Secure
            reqFormData.Add("card_type", "mastercard");
            reqFormData.Add("notification_url_1", "http://127.0.0.1/Payment/WireCard/Notification"); //付款事务通知
            reqFormData.Add("notification_transaction_state_1", "success");

            //生成支付表单，自动并提交到付款平台入口
            Response.Write(generalForm(reqFormData, "UTF-8", "https://testapi.ep2-global.com/engine/hpp/"));

            return null;
        }

        /// <summary>
        /// 收到支付事务通知
        /// </summary>
        /// <returns></returns>
        public ActionResult Notification()
        {
            /*
             * 服务端Java做的：
             *
             * 支付平台会包含三个事务步骤，
             * 走完三个步骤才算完成支付，
             * 期间会有三次 Notification
             * 注：返回的状态在Header中，参考三次Headers[request_id]的值的变化，Form里没数据的
             * 
             */
            return View();
        }

        /// <summary>
        /// 最终返回支付结果
        /// </summary>
        /// <returns></returns>
        public ActionResult Result()
        {
            /*
             * 最终返回的付款结果，相关支付详细信息都在Form里
             */

            return View();
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string getSHA256(string text)
        {
            byte[] hasValue;
            byte[] message = Encoding.UTF8.GetBytes(text);

            var hashString = new SHA256Managed();
            string hex = "";

            hasValue = hashString.ComputeHash(message);
            foreach (byte x in hasValue)
            {
                hex += String.Format("{0:x2}", x);
            }

            return hex.Trim();
        }

        /// <summary>
        /// 生成Form表单
        /// </summary>
        /// <param name="collections">Form数据，NameValueCollection</param>
        /// <param name="charset">字符类型</param>
        /// <param name="PostUrl">付款平台的入口</param>
        /// <returns></returns>
        private string generalForm(NameValueCollection collections, string charset, string PostUrl)
        {
            var values = collections;
            var html = new StringBuilder();
            html.AppendLine("<html>").AppendLine("<head>");
            html.AppendFormat("<meta http-equiv=\"Content-Type\" content=\"application/x-www-form-urlencoded; charset={0}\" />", charset).AppendLine();
            html.AppendLine("<style type=\"text/css\">#pay_form { margin: 30px auto 0 auto; text-align: left; } label{ width:250px; display:block;} input{ width: 220px;}</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body onload=\"\">"); //javascript:document.pay_form.submit();
            //html.AppendLine("<div id=\"search_flight_loading\"><img src=\"/Content/img/oval.svg\" /></div>");
            html.AppendFormat("<form id=\"pay_form\" name=\"pay_form\" action=\"{0}\" method=\"POST\">", PostUrl).AppendLine();
            foreach (string k in values.AllKeys)
            {
                html.AppendFormat("<label for=\"{0}\">{0}</label>: <input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\" readOnly=\"true\" /> <br/>", k, values[k]).AppendLine();
            }
            html.AppendLine("<input type=\"submit\" style=\"display:block;\" value=\"提交\" />");
            html.AppendLine("</form>").AppendLine("</body>").AppendLine("</html>");
            return html.ToString();
        }

    }
}