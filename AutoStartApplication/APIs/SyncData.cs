using AutoStartApplication.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoStartApplication.APIs
{
    public class SyncData
    {
        public HttpClient _httpClient;
        public SyncData()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://crm.creativebuffer.com/api/essl/") };
        }

        private async Task<List<string>> GetDataFromInAndOutAPI(string fromDateTime, string toDateTime, string serialNumber)
        {
            var extractedData= new List<string>();
            string userName = ConfigurationManager.AppSettings["UserName"];
            string userPassword = ConfigurationManager.AppSettings["UserPassword"];
            string url = "http://172.16.16.10:82/iclock/webapiservice.asmx?op=GetTransactionsLog";
            string soapBody = $@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
            <soap:Body>
            <GetTransactionsLog xmlns=""http://tempuri.org/"">
             <FromDateTime>{fromDateTime}</FromDateTime>
            <ToDateTime>{toDateTime}</ToDateTime>
            <SerialNumber>{serialNumber}</SerialNumber>
            <UserName>{userName}</UserName>
            <UserPassword>{userPassword}</UserPassword>
             <strDataList></strDataList>
             </GetTransactionsLog>
            </soap:Body>
            </soap:Envelope>";
            using (HttpClient client = new HttpClient())
            {
                // Set the headers
                client.DefaultRequestHeaders.Add("op", "GetTransactionsLog");

                // Create the request content
                StringContent content = new StringContent(soapBody, Encoding.UTF8, "text/xml");

                // Send the request
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Ensure the response was successful
                response.EnsureSuccessStatusCode();

                // Read the response content
                string responseBody = await response.Content.ReadAsStringAsync();
                if (responseBody != null && responseBody != "")
                {
                    extractedData = await ExtractData(responseBody);
                    return extractedData;
                }
                return extractedData;
            }

        }


        public async Task<string> GetData(string fromDateTime, string toDateTime)
        {
            string inSerialNumber = ConfigurationManager.AppSettings["InSerialNumber"];
            string outSerialNumber = ConfigurationManager.AppSettings["OutSerialNumber"];
            List<AttendanceRecordModel> attendanceLogs = new List<AttendanceRecordModel>();
            try
            {
                var inAttandenceRecords = await GetDataFromInAndOutAPI(fromDateTime, toDateTime, inSerialNumber);
                var outAttandenceRecords = await GetDataFromInAndOutAPI(fromDateTime, toDateTime, outSerialNumber);
                inAttandenceRecords.AddRange(outAttandenceRecords);
                if (inAttandenceRecords != null)
                {
                    attendanceLogs = await GetPunchRecords(inAttandenceRecords);
                    if (attendanceLogs != null && attendanceLogs.Count > 0)
                    {
                        var responseMessage = await SendFormDataAsync(attendanceLogs);
                        if (responseMessage != "")
                        {
                            return responseMessage;
                        }
                    }
                }
                //var date = ExtractData(response);

                return "Error in sync data";
                //MessageBox.Show($"Response:\n{response}", "API Response", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                return "";
                //MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        /// <summary>
        /// Extract data from xml response. 
        /// </summary>
        /// <param name="xmlResponse"></param>
        /// <returns></returns>
        public async Task<List<string>> ExtractData(string xmlResponse)
        {

            // Load the XML string
            XDocument xmlDoc = XDocument.Parse(xmlResponse);

            // Extract the strDataList element from the XML
            var strDataList = xmlDoc.Descendants(XName.Get("strDataList", "http://tempuri.org/")).FirstOrDefault()?.Value;

            // Split the string into lines
            var lines = strDataList?.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Create a list to store the parsed data
            List<string> punchListData = new List<string>();

            // Loop through each line and add it to the list
            foreach (var line in lines)
            {
                // Trim and clean up extra spaces, then add the line to the list
                punchListData.Add(line.Trim());
            }
            return punchListData;

        }

        public async Task<List<AttendanceRecordModel>> GetPunchRecords(List<string> data)
        {
            List<PunchRecordModel> punchRecords = new List<PunchRecordModel>();
            List<AttendanceRecordModel> attendanceLogs = new List<AttendanceRecordModel>();

            // Parse the input data into PunchRecordModel objects
            foreach (var entry in data)
            {
                string[] parts = entry.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                punchRecords.Add(new PunchRecordModel
                {
                    EmployeeId = parts[0],
                    Date = parts[1],
                    Time = parts[2].Substring(0, 5), // Extract hour and minute (HH:mm)
                    Status = parts[3]
                });
            }
            punchRecords = punchRecords.OrderBy(record => record.EmployeeId).ToList();

            // Group by EmployeeId and Date
            var groupedRecords = punchRecords
                .GroupBy(r => new { r.EmployeeId, r.Date })
                .ToList();


            // Process each group separately
            foreach (var group in groupedRecords)
            {
                var inTimes = group
                    .Where(r => r.Status == "in")
                    .OrderBy(r => r.Time)
                    .Select(r => r.Time)
                    .ToList();

                var outTimes = group
                    .Where(r => r.Status == "out")
                    .OrderBy(r => r.Time)
                    .Select(r => r.Time)
                    .ToList();

                int inIndex = 0;
                int outIndex = 0;

                while (inIndex < inTimes.Count || outIndex < outTimes.Count)
                {
                    string inTime = "00:00";
                    string outTime = "00:00";

                    // Get current InTime
                    if (inIndex < inTimes.Count)
                    {
                        inTime = inTimes[inIndex];
                        string nextInTime = (inIndex + 1 < inTimes.Count) ? inTimes[inIndex + 1] : "23:59";

                        // Find OutTime between current InTime and next InTime
                        while (outIndex < outTimes.Count && string.Compare(outTimes[outIndex], inTime) >= 0
                               && string.Compare(outTimes[outIndex], nextInTime) < 0)
                        {
                            outTime = outTimes[outIndex];
                            outIndex++;
                            break; // Stop once paired
                        }

                        inIndex++;
                    }
                    else if (outIndex < outTimes.Count)
                    {
                        // Remaining unmatched OutTime
                        outTime = outTimes[outIndex];
                        outIndex++;
                    }

                    attendanceLogs.Add(new AttendanceRecordModel
                    {
                        EmployeeId = group.Key.EmployeeId,
                        Date = group.Key.Date ?? "1900-01-01",
                        InTime = inTime,
                        OutTime = outTime
                    });
                }
            }

            // Sort attendanceLogs as per the required order
            attendanceLogs = attendanceLogs
                .OrderBy(log => log.EmployeeId)
                .ThenBy(log => log.InTime == "00:00" ? log.OutTime : log.InTime)
                .ThenBy(log => log.OutTime == "00:00" ? log.InTime : log.OutTime)
                .ToList();


            // Return the logs ordered by EmployeeId
            return attendanceLogs;
        }




        /// <summary>
        /// Post Fetched Data 
        /// </summary>
        /// <param name="attendanceRecords"></param>
        /// <returns></returns>
        public async Task<string> SendFormDataAsync(List<AttendanceRecordModel> attendanceRecords)
        {

            //// Set the API endpoint
            //var url = "https://crm.creativebuffer.com/api/essl/store-attandance-log";

            var punchRecord = new PunchRecordRequestModel();
            punchRecord.attandanceLogs = attendanceRecords;
            try
            {
                var serializePunchedData = JsonConvert.SerializeObject(punchRecord);

                HttpContent content = new StringContent(serializePunchedData, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // Send POST request
                var response = await _httpClient.PostAsync("store-attandance-log", content);
                response.EnsureSuccessStatusCode();

                // Read the response
                var responseString = await response.Content.ReadAsStringAsync();
                var apiRespnse = JsonConvert.DeserializeObject<PostAttendanceRecordResponseModel>(responseString);
                if (apiRespnse != null && apiRespnse.success)
                {
                    return apiRespnse.message;
                }
                return apiRespnse.message;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        /// <summary>
        /// Get History Of Synced Record
        /// </summary>
        /// <returns></returns>
        public async Task<List<Histoy>> GetAttendanceLogHistory()
        {
            List<Histoy> historyList = null;
            try
            {
                var httpResponse = (_httpClient.GetAsync("attandance-log-status")).Result;

                if (httpResponse.IsSuccessStatusCode)
                {
                    var httpContent = await httpResponse.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(httpContent))
                    {
                        var apiRespnse = JsonConvert.DeserializeObject<HistoryResponseModel>(httpContent);

                        if (apiRespnse.status == (int)HttpStatusCode.OK)
                        {
                            historyList = apiRespnse.data;
                            //MessageBox.Show(apiRespnse.status.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return historyList = null;
            }
            return historyList;
        }
    }

}

