using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using NLog;

namespace Makedonsky.MapLogic.SpreadSheets
{
    public class SpreadsheetService
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static readonly string ApplicationName = "Makedonsky Strategy planner";

        private static SheetsService _service;

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static SheetsService GetService()
        {
            if (_service == null)
            {
                UserCredential credential;

                using (var stream =
                    new FileStream(AssemblyDirectory + @"\SpreadSheets\client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.makedonsky.json");

                    try
                    {
                        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            Scopes,
                            "user",
                            CancellationToken.None,
                            new FileDataStore(credPath, true)).Result;
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                    //Console.WriteLine("Credential file saved to: " + credPath);
                }

                // Create Google Sheets API service.
                _service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
            }
            return _service;
        }

        public static IList<IList<Object>> GetSpreadsheetRange(String spreadsheetId, String range)
        {

            var gsheetApi = GetService();
            SpreadsheetsResource.ValuesResource.GetRequest request = gsheetApi.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;

            return values;
        }

        public static IList<IList<Object>> GetNotEmptySpreadsheetRange(String spreadsheetId, String range, String description)
        {
            IList<IList<Object>> data;

            try
            {
                data = GetSpreadsheetRange(spreadsheetId, range);
            }
            catch (Exception e)
            {
                String message = string.Format("Error quering {0} spreadsheet {1}/{2}: exception occured: {3}.", description, spreadsheetId, range, e.Message);
                throw new Exception(message, e);
            }

            if (null == data)
            {
                String message = string.Format("Error quering {0} spreadsheet {1}/{2}: data is null.", description, spreadsheetId, range);
                throw new NullReferenceException(message);
            }

            if (!(data.Count > 0))
            {
                String message = string.Format("Error quering {0} spreadsheet {1}/{2}: data is empty.", description, spreadsheetId, range);
                throw new InvalidDataException(message);
            }

            return data;
        }

        public class RowDataHandling
        {
            
            public static string GetColumn(IList<object> row, int index)
            {
                if (row.Count < index + 1)
                    return string.Empty;
                else
                    return null == row[index] ? string.Empty : row[index].ToString().Trim();
            }

            private static bool TryGetInt(IList<object> row, int index, out int value)
            {
                string strValue = GetColumn(row, index);

                return int.TryParse(strValue, out value);
            }

            private static int GetInt(IList<object> row, int index)
            {
                int res;

                if (TryGetInt(row, index, out res))
                {
                    return res;
                }

                string strValue = GetColumn(row, index);

                throw new ArgumentException(string.Format("Can't parse value '{0}' to int.", strValue));
            }

            private static int? GetIntNull(IList<object> row, int index)
            {
                int resInt;
                int? res = null;

                if (TryGetInt(row, index, out resInt))
                {
                    res = resInt;
                }

                return res;
            }
        }


    }
}
