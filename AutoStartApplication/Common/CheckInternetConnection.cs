using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AutoStartApplication.Common
{
    public class CheckInternetConnection
    {
        /// <summary>
        /// Check Internet Connection
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedToInternet()
        {
            string host = "www.google.com"; // Use a valid hostname
            int timeout = 3000; // Timeout in milliseconds
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, timeout);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
            }
            catch
            {
                // Log exception if needed
            }
            return false;
        }
    }
}
