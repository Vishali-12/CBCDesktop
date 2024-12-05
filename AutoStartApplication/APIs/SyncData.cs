﻿using AutoStartApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoStartApplication.APIs
{
    public class SyncData
    {
        public async Task<string> GetData(string fromDateTime, string toDateTime)
        {

            string url = "http://172.16.16.10:82/iclock/webapiservice.asmx?op=GetTransactionsLog";
            string soapBody = $@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
            <soap:Body>
            <GetTransactionsLog xmlns=""http://tempuri.org/"">
             <FromDateTime>{fromDateTime}</FromDateTime>
            <ToDateTime>{toDateTime}</ToDateTime>
            <SerialNumber>BOCK201560671</SerialNumber>
            <UserName>creativebuffer</UserName>
            <UserPassword>Buffer@2020</UserPassword>
             <strDataList></strDataList>
             </GetTransactionsLog>
            </soap:Body>
            </soap:Envelope>";

            try
            {
                string response = await CallSoapApiAsync(url, soapBody);
                var date = ExtractData(response);

                return response;
                //MessageBox.Show($"Response:\n{response}", "API Response", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                return "sd";
                //MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async Task<string> CallSoapApiAsync(string url, string soapBody)
        {
            var data = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n    <soap:Body>\r\n        <GetTransactionsLogResponse xmlns=\"http://tempuri.org/\">\r\n            <GetTransactionsLogResult>Logs Count:213</GetTransactionsLogResult>\r\n            <strDataList>012\t2024-12-02 12:01:43\tin\r\n012\t2024-12-02 14:04:26\tout\r\n012\t2024-12-02 15:02:23\tin\r\n012\t2024-12-02 18:34:45\tout\r\n012\t2024-12-02 20:57:45\tout\r\nCBC-107\t2024-12-02 10:36:51\tin\r\nCBC-107\t2024-12-02 19:38:50\tout\r\nCBC-108\t2024-12-02 10:15:15\tin\r\nCBC-108\t2024-12-02 10:57:07\tout\r\nCBC-108\t2024-12-02 10:58:50\tin\r\nCBC-108\t2024-12-02 11:37:18\tout\r\nCBC-108\t2024-12-02 11:39:09\tin\r\nCBC-108\t2024-12-02 12:01:12\tout\r\nCBC-108\t2024-12-02 12:09:24\tin\r\nCBC-108\t2024-12-02 12:47:29\tout\r\nCBC-108\t2024-12-02 12:49:31\tin\r\nCBC-108\t2024-12-02 13:33:35\tout\r\nCBC-108\t2024-12-02 13:35:33\tin\r\nCBC-108\t2024-12-02 13:41:13\tout\r\nCBC-108\t2024-12-02 13:43:31\tin\r\nCBC-108\t2024-12-02 16:09:15\tout\r\nCBC-108\t2024-12-02 16:11:40\tin\r\nCBC-108\t2024-12-02 17:47:31\tout\r\nCBC-108\t2024-12-02 17:50:07\tin\r\nCBC-108\t2024-12-02 18:40:06\tout\r\nCBC-108\t2024-12-02 18:42:50\tin\r\nCBC-108\t2024-12-02 19:26:17\tout\r\nCBC-114\t2024-12-02 17:10:45\tin\r\nCBC-114\t2024-12-02 19:05:56\tout\r\nCBC-114\t2024-12-02 19:14:12\tin\r\nCBC-114\t2024-12-02 19:33:35\tout\r\nCBC-115\t2024-12-02 10:50:32\tout\r\nCBC-115\t2024-12-02 15:11:24\tin\r\nCBC-115\t2024-12-02 20:09:20\tout\r\nCBC-122\t2024-12-02 10:35:23\tin\r\nCBC-122\t2024-12-02 17:32:43\tout\r\nCBC-122\t2024-12-02 19:50:13\tout\r\nCBC-127\t2024-12-02 17:20:19\tout\r\nCBC-127\t2024-12-02 17:21:59\tin\r\nCBC-127\t2024-12-02 19:25:55\tout\r\nCBC-128\t2024-12-02 19:38:16\tout\r\nCBC-133\t2024-12-02 10:10:14\tin\r\nCBC-133\t2024-12-02 12:22:35\tout\r\nCBC-133\t2024-12-02 12:26:38\tin\r\nCBC-133\t2024-12-02 12:44:15\tout\r\nCBC-133\t2024-12-02 12:49:29\tin\r\nCBC-133\t2024-12-02 16:48:19\tout\r\nCBC-133\t2024-12-02 19:38:34\tout\r\nCBC-135\t2024-12-02 11:56:47\tout\r\nCBC-135\t2024-12-02 17:05:56\tin\r\nCBC-135\t2024-12-02 18:06:30\tout\r\nCBC-135\t2024-12-02 18:08:42\tin\r\nCBC-135\t2024-12-02 19:39:22\tout\r\nCBC-136\t2024-12-02 17:15:41\tin\r\nCBC-136\t2024-12-02 17:16:01\tout\r\nCBC-136\t2024-12-02 19:29:33\tout\r\nCBC-137\t2024-12-02 13:53:38\tout\r\nCBC-137\t2024-12-02 16:58:43\tout\r\nCBC-137\t2024-12-02 18:37:35\tout\r\nCBC-137\t2024-12-02 18:39:40\tin\r\nCBC-137\t2024-12-02 19:17:35\tout\r\nCBC-140\t2024-12-02 12:45:21\tout\r\nCBC-140\t2024-12-02 12:59:22\tin\r\nCBC-140\t2024-12-02 15:11:38\tout\r\nCBC-140\t2024-12-02 15:11:43\tin\r\nCBC-140\t2024-12-02 15:12:50\tout\r\nCBC-140\t2024-12-02 15:12:54\tin\r\nCBC-140\t2024-12-02 17:04:18\tout\r\nCBC-140\t2024-12-02 17:12:41\tin\r\nCBC-140\t2024-12-02 19:25:27\tout\r\nCBC-144\t2024-12-02 09:59:30\tout\r\nCBC-144\t2024-12-02 13:53:41\tout\r\nCBC-145\t2024-12-02 10:08:58\tin\r\nCBC-145\t2024-12-02 12:01:04\tout\r\nCBC-145\t2024-12-02 12:03:20\tin\r\nCBC-145\t2024-12-02 13:11:34\tout\r\nCBC-145\t2024-12-02 13:15:14\tin\r\nCBC-145\t2024-12-02 15:49:06\tout\r\nCBC-145\t2024-12-02 15:52:09\tin\r\nCBC-145\t2024-12-02 17:08:38\tout\r\nCBC-145\t2024-12-02 17:11:51\tin\r\nCBC-145\t2024-12-02 17:12:07\tout\r\nCBC-145\t2024-12-02 17:12:18\tin\r\nCBC-145\t2024-12-02 19:50:20\tout\r\nCBC-147\t2024-12-02 09:58:33\tin\r\nCBC-147\t2024-12-02 12:46:58\tout\r\nCBC-147\t2024-12-02 12:48:21\tin\r\nCBC-147\t2024-12-02 14:04:02\tout\r\nCBC-147\t2024-12-02 15:05:32\tin\r\nCBC-147\t2024-12-02 17:02:12\tout\r\nCBC-147\t2024-12-02 17:04:08\tin\r\nCBC-147\t2024-12-02 18:14:08\tout\r\nCBC-147\t2024-12-02 18:19:06\tin\r\nCBC-147\t2024-12-02 20:57:31\tout\r\nCBC-148\t2024-12-02 10:14:26\tin\r\nCBC-148\t2024-12-02 11:18:11\tout\r\nCBC-148\t2024-12-02 11:33:20\tin\r\nCBC-148\t2024-12-02 13:27:01\tout\r\nCBC-148\t2024-12-02 13:36:10\tin\r\nCBC-148\t2024-12-02 15:10:34\tout\r\nCBC-148\t2024-12-02 15:18:06\tin\r\nCBC-148\t2024-12-02 16:35:48\tout\r\nCBC-148\t2024-12-02 16:41:53\tin\r\nCBC-148\t2024-12-02 17:47:28\tout\r\nCBC-148\t2024-12-02 17:55:02\tin\r\nCBC-148\t2024-12-02 19:09:03\tout\r\nCBC-148\t2024-12-02 19:21:33\tin\r\nCBC-148\t2024-12-02 19:42:05\tout\r\nCBC-149\t2024-12-02 11:01:48\tin\r\nCBC-149\t2024-12-02 17:02:34\tout\r\nCBC-149\t2024-12-02 17:04:11\tin\r\nCBC-149\t2024-12-02 17:46:02\tout\r\nCBC-149\t2024-12-02 18:07:58\tin\r\nCBC-149\t2024-12-02 19:26:21\tout\r\nCBC-149\t2024-12-02 19:27:44\tin\r\nCBC-149\t2024-12-02 20:44:41\tout\r\nCBC-151\t2024-12-02 11:13:45\tin\r\nCBC-151\t2024-12-02 14:04:08\tout\r\nCBC-151\t2024-12-02 15:05:05\tin\r\nCBC-151\t2024-12-02 17:02:10\tout\r\nCBC-151\t2024-12-02 17:04:07\tin\r\nCBC-151\t2024-12-02 20:12:43\tout\r\nCBC-151\t2024-12-02 20:14:55\tin\r\nCBC-151\t2024-12-02 20:57:59\tout\r\nCBC-154\t2024-12-02 09:58:54\tout\r\nCBC-154\t2024-12-02 13:31:10\tout\r\nCBC-154\t2024-12-02 16:31:30\tout\r\nCBC-154\t2024-12-02 19:00:49\tout\r\nCBC-155\t2024-12-02 10:54:17\tin\r\nCBC-155\t2024-12-02 11:26:51\tout\r\nCBC-155\t2024-12-02 11:28:45\tin\r\nCBC-155\t2024-12-02 14:00:50\tout\r\nCBC-155\t2024-12-02 14:59:50\tin\r\nCBC-155\t2024-12-02 18:02:51\tout\r\nCBC-155\t2024-12-02 18:04:45\tin\r\nCBC-155\t2024-12-02 19:42:34\tout\r\nCBC-159\t2024-12-02 11:03:48\tin\r\nCBC-159\t2024-12-02 11:43:47\tout\r\nCBC-159\t2024-12-02 11:46:42\tin\r\nCBC-159\t2024-12-02 12:57:03\tout\r\nCBC-159\t2024-12-02 13:00:42\tin\r\nCBC-159\t2024-12-02 14:26:04\tout\r\nCBC-159\t2024-12-02 15:04:43\tin\r\nCBC-159\t2024-12-02 16:51:57\tout\r\nCBC-159\t2024-12-02 16:54:24\tin\r\nCBC-159\t2024-12-02 19:19:19\tout\r\nCBC-159\t2024-12-02 19:26:09\tin\r\nCBC-159\t2024-12-02 20:57:27\tout\r\nCBC-160\t2024-12-02 09:58:52\tin\r\nCBC-160\t2024-12-02 12:04:45\tout\r\nCBC-160\t2024-12-02 12:06:28\tin\r\nCBC-160\t2024-12-02 14:18:53\tout\r\nCBC-160\t2024-12-02 14:56:36\tin\r\nCBC-160\t2024-12-02 17:29:16\tout\r\nCBC-160\t2024-12-02 17:31:07\tin\r\nCBC-160\t2024-12-02 19:00:54\tout\r\nCBC-162\t2024-12-02 10:38:11\tin\r\nCBC-162\t2024-12-02 10:51:01\tout\r\nCBC-162\t2024-12-02 10:54:48\tin\r\nCBC-162\t2024-12-02 12:37:14\tout\r\nCBC-162\t2024-12-02 12:45:11\tin\r\nCBC-162\t2024-12-02 13:52:26\tout\r\nCBC-162\t2024-12-02 13:54:18\tin\r\nCBC-162\t2024-12-02 14:13:19\tout\r\nCBC-162\t2024-12-02 14:59:04\tin\r\nCBC-162\t2024-12-02 16:17:08\tout\r\nCBC-162\t2024-12-02 16:20:22\tin\r\nCBC-162\t2024-12-02 17:27:48\tout\r\nCBC-162\t2024-12-02 17:29:31\tin\r\nCBC-162\t2024-12-02 18:10:27\tout\r\nCBC-162\t2024-12-02 18:12:27\tin\r\nCBC-162\t2024-12-02 18:18:58\tout\r\nCBC-162\t2024-12-02 18:19:01\tin\r\nCBC-162\t2024-12-02 18:52:23\tout\r\nCBC-162\t2024-12-02 18:56:37\tin\r\nCBC-162\t2024-12-02 19:29:35\tout\r\nCBC-162\t2024-12-02 19:32:57\tin\r\nCBC-162\t2024-12-02 19:48:49\tout\r\nCBC-164\t2024-12-02 10:02:38\tin\r\nCBC-164\t2024-12-02 10:42:26\tout\r\nCBC-164\t2024-12-02 10:44:49\tin\r\nCBC-164\t2024-12-02 12:03:59\tout\r\nCBC-164\t2024-12-02 12:06:37\tin\r\nCBC-164\t2024-12-02 14:01:55\tout\r\nCBC-164\t2024-12-02 14:03:13\tin\r\nCBC-164\t2024-12-02 14:09:36\tout\r\nCBC-164\t2024-12-02 14:52:10\tin\r\nCBC-164\t2024-12-02 17:56:20\tout\r\nCBC-164\t2024-12-02 17:57:57\tin\r\nCBC-164\t2024-12-02 18:44:55\tout\r\nCBC-164\t2024-12-02 18:46:44\tin\r\nCBC-164\t2024-12-02 19:34:57\tout\r\nCBC-166\t2024-12-02 13:38:52\tin\r\nCBC-166\t2024-12-02 14:58:52\tout\r\nCBC-166\t2024-12-02 15:00:52\tin\r\nCBC-166\t2024-12-02 17:33:54\tout\r\nCBC-166\t2024-12-02 17:36:36\tin\r\nCBC-166\t2024-12-02 19:01:38\tout\r\nCBC-167\t2024-12-02 10:25:17\tin\r\nCBC-167\t2024-12-02 12:51:40\tout\r\nCBC-167\t2024-12-02 12:53:15\tin\r\nCBC-167\t2024-12-02 14:03:58\tout\r\nCBC-167\t2024-12-02 15:09:19\tin\r\nCBC-167\t2024-12-02 19:26:34\tout\r\nCBC-168\t2024-12-02 09:59:02\tout\r\nCBC-168\t2024-12-02 09:59:05\tout\r\nCBC-168\t2024-12-02 09:59:07\tout\r\nCBC-168\t2024-12-02 09:59:14\tin\r\nCBC-168\t2024-12-02 12:11:17\tout\r\nCBC-168\t2024-12-02 12:13:43\tin\r\nCBC-168\t2024-12-02 14:05:16\tout\r\nCBC-168\t2024-12-02 15:09:17\tin\r\nCBC-168\t2024-12-02 19:09:26\tout\r\n</strDataList>\r\n        </GetTransactionsLogResponse>\r\n    </soap:Body>\r\n</soap:Envelope>";

            //try
            //{
            //    using (HttpClient client = new HttpClient())
            //    {
            //        // Set the headers
            //        client.DefaultRequestHeaders.Add("op", "GetTransactionsLog");

            //        // Create the request content
            //        StringContent content = new StringContent(soapBody, Encoding.UTF8, "text/xml");

            //        // Send the request
            //        HttpResponseMessage response = await client.PostAsync(url, content);

            //        // Ensure the response was successful
            //        response.EnsureSuccessStatusCode();

            //        // Read the response content
            //        string responseBody = await response.Content.ReadAsStringAsync();
            //        return responseBody;
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            return data;
        }

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

            var punchRecordList = GetPunchRecords(punchListData).OrderBy(r => r.EmployeeId);
            return punchListData;

        }

        public List<PunchRecordModel> GetPunchRecords(List<string> data)
        {
            List<PunchRecordModel> punchRecords = new List<PunchRecordModel>();

            foreach (var entry in data)
            {
                // Split the string using tab and space as delimiters
                string[] parts = entry.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Add the record to the list
                punchRecords.Add(new PunchRecordModel
                {
                    EmployeeId = parts[0],
                    Date = parts[1],
                    Time = parts[2].Substring(0, 5), // Extract hour and minute (HH:mm)
                    Status = parts[3]
                });
            }

            // Step 2: Transform into AttendanceLogModel
            List<PunchRecordRequestModel> attendanceLogs = new List<PunchRecordRequestModel>();

            var groupedRecords = punchRecords
                .GroupBy(r => new { r.EmployeeId, r.Date }) // Group by EmployeeId and Date
                .ToList();

            foreach (var group in groupedRecords)
            {
                // Get all IN and OUT records for the current group
                var inRecords = group.Where(r => r.Status == "in").OrderBy(r => r.Time).ToList();
                var outRecords = group.Where(r => r.Status == "out").OrderBy(r => r.Time).ToList();

                int inIndex = 0, outIndex = 0;

                while (inIndex < inRecords.Count || outIndex < outRecords.Count)
                {
                    string inTime = null;
                    string outTime = null;

                    // Check if an "Out" record exists without a preceding "In" record
                    if (outIndex < outRecords.Count && (inIndex >= inRecords.Count || string.Compare(outRecords[outIndex].Time, inRecords[inIndex].Time) < 0))
                    {
                        outTime = outRecords[outIndex].Time;
                        outIndex++;
                    }
                    else if (inIndex < inRecords.Count)
                    {
                        inTime = inRecords[inIndex].Time;
                        if (outIndex < outRecords.Count)
                        {
                            outTime = outRecords[outIndex].Time;
                            outIndex++;
                        }
                        inIndex++;
                    }

                    // Add the paired or unpaired record
                    attendanceLogs.Add(new PunchRecordRequestModel
                    {
                        EmployeeId = group.Key.EmployeeId,
                        Date = group.Key.Date,
                        InTime = inTime,
                        OutTime = outTime
                    });
                }
            }
            var gg = attendanceLogs;
            return punchRecords;
        }


        //public List<PunchRecordModel> GetPunchRecords(List<string> data)
        //{
        //    List<PunchRecordModel> punchRecords = new List<PunchRecordModel>();

        //    foreach (var entry in data)
        //    {

        //        // Split the string using tab and space as delimiters
        //        string[] parts = entry.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //        // Add the record to the list
        //        punchRecords.Add(new PunchRecordModel
        //        {
        //            EmployeeId = parts[0],
        //            Date = parts[1],
        //            Time = parts[2].Substring(0, 5), // Extract hour and minute (HH:mm)
        //            Status = parts[3]
        //        });
        //    }
        //    // Step 2: Transform into AttendanceLogModel
        //    List<AttendanceLogModel> attendanceLogs = new List<AttendanceLogModel>();

        //    var groupedRecords = punchRecords
        //        .GroupBy(r => new { r.EmployeeId, r.Date }) // Group by EmployeeId and Date
        //        .ToList();

        //    foreach (var group in groupedRecords)
        //    {
        //        // Get all IN and OUT records for the current group
        //        var inRecords = group.Where(r => r.Status == "in").OrderBy(r => r.Time).ToList();
        //        var outRecords = group.Where(r => r.Status == "out").OrderBy(r => r.Time).ToList();

        //        // Pair IN and OUT records
        //        int count = Math.Max(inRecords.Count, outRecords.Count);

        //        for (int i = 0; i < count; i++)
        //        {
        //            attendanceLogs.Add(new AttendanceLogModel
        //            {
        //                EmployeeId = group.Key.EmployeeId,
        //                Date = group.Key.Date,
        //                InTime = i < inRecords.Count ? inRecords[i].Time : null,
        //                OutTime = i < outRecords.Count ? outRecords[i].Time : null
        //            });
        //        }
        //    }
        //    var dd = attendanceLogs;
        //    return punchRecords;

        //}
    }
}
