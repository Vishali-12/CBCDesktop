using AutoStartApplication.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
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
        public HttpClient _httpClientCRM;
        public HttpClient _httpClientEssl;
        public readonly string _esslBaseUrl;
        public readonly string _crmBaseUrl;
        public readonly string _userName;
        public readonly string _userPassword;
        public readonly string _inSerialNumber;
        public readonly string _outSerialNumber;
        public SyncData()
        {
            _esslBaseUrl = ConfigurationManager.AppSettings["EsslBaseUrl"];
            _crmBaseUrl = ConfigurationManager.AppSettings["CRMBaseUrl"];
            _userName = ConfigurationManager.AppSettings["UserName"];
            _userPassword = ConfigurationManager.AppSettings["UserPassword"];
            _httpClientCRM = new HttpClient { BaseAddress = new Uri(_crmBaseUrl) };
            _httpClientEssl = new HttpClient { BaseAddress = new Uri(_esslBaseUrl) };
            _inSerialNumber = ConfigurationManager.AppSettings["InSerialNumber"];
            _outSerialNumber = ConfigurationManager.AppSettings["OutSerialNumber"];

        }


        #region Get In and Out attendance records from Essl Api 
        /// <summary>
        /// Get attendance records from biometric machines.
        /// </summary>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        private async Task<List<string>> GetDataFromInAndOutAPI(string fromDateTime, string toDateTime, string serialNumber)
        {
            var extractedData = new List<string>();

            //string url = _httpClientEssl+"?op=GetTransactionsLog";
            string soapBody = $@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
            <soap:Body>
            <GetTransactionsLog xmlns=""http://tempuri.org/"">
            <FromDateTime>{fromDateTime}</FromDateTime>
            <ToDateTime>{toDateTime}</ToDateTime>
            <SerialNumber>{serialNumber}</SerialNumber>
            <UserName>{_userName}</UserName>
            <UserPassword>{_userPassword}</UserPassword>
             <strDataList></strDataList>
             </GetTransactionsLog>
            </soap:Body>
            </soap:Envelope>";
            _httpClientEssl.DefaultRequestHeaders.Add("op", "GetTransactionsLog");

            // Create the request content
            StringContent content = new StringContent(soapBody, Encoding.UTF8, "text/xml");

            // Send the request
            HttpResponseMessage response = await _httpClientEssl.PostAsync("?op=GetTransactionsLog", content);

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


        /// <summary>
        /// Sync data functionality
        /// </summary>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public async Task<string> GetData(string fromDateTime, string toDateTime)
        {
            List<AttendanceRecordModel> attendanceLogs = new List<AttendanceRecordModel>();

            var blockedEmployeeList = await GetBlockdEmployees();
            if (blockedEmployeeList.Count > 0)
            {
                foreach (var employee in blockedEmployeeList)
                {
                    if (employee.block_status_in_machine == 0)
                    {
                        var inMachineDeleteStatus = await DeleteEmployeeFromBiometric(employee.employee_no, _inSerialNumber);
                        if (inMachineDeleteStatus)
                        {
                            employee.block_status_in_machine = 1;
                        }
                        var outMachineDeleteStatus = await DeleteEmployeeFromBiometric(employee.employee_no, _outSerialNumber);
                        if (outMachineDeleteStatus)
                        {
                            employee.block_status_in_machine = 1;
                        }
                        if (inMachineDeleteStatus && outMachineDeleteStatus)
                        {
                            var updateStatus = await UpdateEmployeeDeleteStatus(employee.id, employee.block_status_in_machine);
                        }
                    }
                }
            }
            var inAttandenceRecords = await GetDataFromInAndOutAPI(fromDateTime, toDateTime, _inSerialNumber);
            var outAttandenceRecords = await GetDataFromInAndOutAPI(fromDateTime, toDateTime, _outSerialNumber);
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
            return "Error in sync data";
        }

        public async Task<string> AddEmployeesInBiometric()
        {
            var employees = await GetEmployees();

            if (employees.Count > 0)
            {
                foreach (var user in employees)
                {
                    if (user.in_machine_status == 0)
                    {
                        // Call the API
                        bool result = await AddEmployeeAsync(user.employee_no, user.name, _inSerialNumber);
                        if (result)
                        {
                            user.in_machine_status = 1;
                        }

                    }
                    if (user.out_machine_status == 0)
                    {
                        // Call the API
                        bool result = await AddEmployeeAsync(user.employee_no, user.name, _outSerialNumber);
                        if (result)
                        {
                            user.out_machine_status = 1;
                        }
                    }
                    UpdateEmployeeStatusRequestModel model = new UpdateEmployeeStatusRequestModel
                    {
                        id = user.id,
                        in_machine_status = user.in_machine_status,
                        out_machine_status = user.out_machine_status
                    };

                    UpdateEmployeeStatus(model);
                }
                return "data added successfully.";

            }
            else
            {
                return "";
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


        #endregion


        #region Send In and Out attendance records
        /// <summary>
        /// Post Fetched Data 
        /// </summary>
        /// <param name="attendanceRecords"></param>
        /// <returns></returns>
        public async Task<string> SendFormDataAsync(List<AttendanceRecordModel> attendanceRecords)
        {

            var punchRecord = new PunchRecordRequestModel();
            punchRecord.attandanceLogs = attendanceRecords;

            var serializePunchedData = JsonConvert.SerializeObject(punchRecord);

            HttpContent content = new StringContent(serializePunchedData, Encoding.UTF8, "application/json");

            _httpClientCRM.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Send POST request
            var response = await _httpClientCRM.PostAsync("store-attandance-log", content);
            response.EnsureSuccessStatusCode();

            // Read the response
            var responseString = await response.Content.ReadAsStringAsync();
            var apiRespnse = JsonConvert.DeserializeObject<ApiResponseModel>(responseString);
            if (apiRespnse != null && apiRespnse.success)
            {
                return apiRespnse.message;
            }
            return apiRespnse.message;

        }
        #endregion





        #region Get Sync Data History
        /// <summary>
        /// Get History Of Synced Record
        /// </summary>
        /// <returns></returns>
        public async Task<List<Histoy>> GetAttendanceLogHistory()
        {
            List<Histoy> historyList = null;

            var httpResponse = (_httpClientCRM.GetAsync("attandance-log-status")).Result;

            if (httpResponse.IsSuccessStatusCode)
            {
                var httpContent = await httpResponse.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(httpContent))
                {
                    var apiRespnse = JsonConvert.DeserializeObject<HistoryResponseModel>(httpContent);

                    if (apiRespnse.status == (int)HttpStatusCode.OK)
                    {
                        historyList = apiRespnse.data.OrderByDescending(r => r.date).ToList();
                    }
                }
            }

            return historyList;
        }
        #endregion

        #region Add employees to both the In and Out biometric machines.
        /// <summary>
        /// Add employees to both the In and Out biometric machines.
        /// </summary>
        /// <param name="employeeCode"></param>
        /// <param name="employeeName"></param>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        private async Task<bool> AddEmployeeAsync(string employeeCode, string employeeName, string serialNumber)
        {
            string soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                   xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                   xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                <soap:Body>
                    <AddEmployee xmlns=""http://tempuri.org/"">
                    <APIKey></APIKey>
                    <EmployeeCode>{employeeCode}</EmployeeCode>
                    <EmployeeName>{employeeName}</EmployeeName>
                    <CardNumber></CardNumber>
                    <SerialNumber>{serialNumber}</SerialNumber>
                    <UserName>{_userName}</UserName>
                    <UserPassword>{_userPassword}</UserPassword>
                    <CommandId>1</CommandId>
                    </AddEmployee>
                </soap:Body>
            </soap:Envelope>";

            var content = new StringContent(soapRequest, System.Text.Encoding.UTF8, "text/xml");
            var response = await _httpClientEssl.PostAsync("?op=AddEmployee", content);
            var result = response.IsSuccessStatusCode;
            return result;


        }

        #endregion


        #region Retrieve employee records for enrollment in the biometric machine..
        /// <summary>
        /// Retrieve employee records for enrollment in the biometric machine.
        /// </summary>
        /// <returns></returns>

        public async Task<List<User>> GetEmployees()
        {
            List<User> employees = new List<User>();
            var httpResponse = (_httpClientCRM.GetAsync("get-employee")).Result;

            if (httpResponse.IsSuccessStatusCode)
            {
                var httpContent = await httpResponse.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(httpContent))
                {
                    var apiRespnse = JsonConvert.DeserializeObject<GetEmployeesResponseModel>(httpContent);

                    if (apiRespnse.status == (int)HttpStatusCode.OK)
                    {
                        employees = apiRespnse.data;
                    }
                }
            }

            return employees;
        }
        #endregion



        #region Send employee status

        /// <summary>
        ///   Send the status of the employee being added to both the In and Out machines.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> UpdateEmployeeStatus(UpdateEmployeeStatusRequestModel model)
        {

            var serializePunchedData = JsonConvert.SerializeObject(model);

            HttpContent content = new StringContent(serializePunchedData, Encoding.UTF8, "application/json");

            var response = await _httpClientCRM.PostAsync("update-employee-status", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var apiRespnse = JsonConvert.DeserializeObject<ApiResponseModel>(responseString);
            if (apiRespnse != null && apiRespnse.success)
            {
                return apiRespnse.success;
            }
            return false;
        }
        #endregion


        #region Retrieve employee records to delete in both machines.
        /// <summary>
        /// Retrieve employee records for enrollment in the biometric machine.
        /// </summary>
        /// <returns></returns>

        public async Task<List<User>> GetBlockdEmployees()
        {
            List<User> employees = new List<User>();
            var httpResponse = (_httpClientCRM.GetAsync("get-block-un-block-employee")).Result;

            if (httpResponse.IsSuccessStatusCode)
            {
                var httpContent = await httpResponse.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(httpContent))
                {
                    var apiRespnse = JsonConvert.DeserializeObject<GetEmployeesResponseModel>(httpContent);

                    if (apiRespnse.status == (int)HttpStatusCode.OK)
                    {
                        employees = apiRespnse.data;
                    }
                }
            }
            return employees;
        }
        #endregion


        #region Delete Employee
        /// <summary>
        /// Delete employee from both In or Out machine
        /// </summary>
        /// <param name="employeeCode"></param>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        private async Task<bool> DeleteEmployeeFromBiometric(string employeeCode, string serialNumber)
        {
            string soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                   xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                   xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                <soap:Body>
                    <AddEmployee xmlns=""http://tempuri.org/"">
                    <APIKey></APIKey>
                    <EmployeeCode>{employeeCode}</EmployeeCode>
                    <SerialNumber>{serialNumber}</SerialNumber>
                    <UserName>{_userName}</UserName>
                    <UserPassword>{_userPassword}</UserPassword>
                    <CommandId>1</CommandId>
                    </AddEmployee>
                </soap:Body>
            </soap:Envelope>";

            var content = new StringContent(soapRequest, System.Text.Encoding.UTF8, "text/xml");
            var response = await _httpClientEssl.PostAsync("?op=DeleteUser", content);
            var result = response.IsSuccessStatusCode;
            return result;
        }
        #endregion


        #region Send employee delete status

        /// <summary>
        ///   Send the status of the employee being added to both the In and Out machines.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> UpdateEmployeeDeleteStatus(int employeeId, int status)
        {
            var model = new DeleteStatusRequestModel
            {
                id = employeeId,
                block_status_in_machine = status
            };
            var serializePunchedData = JsonConvert.SerializeObject(model);
           

            HttpContent content = new StringContent(serializePunchedData, Encoding.UTF8, "application/json");

            var response = await _httpClientCRM.PostAsync("update-block-un-block-status", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var apiRespnse = JsonConvert.DeserializeObject<ApiResponseModel>(responseString);
            if (apiRespnse != null && apiRespnse.success)
            {
                return apiRespnse.success;
            }
            return false;
        }
        #endregion

    }

}

