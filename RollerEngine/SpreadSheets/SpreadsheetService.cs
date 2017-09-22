using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace RollerEngine.SpreadSheets
{
    public class SpreadsheetService
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private const string ApplicationName = "Serial Hatys Roller boosted by Rolz.org";
        private static SheetsService _service;

        private static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static SheetsService GetService()
        {
            if (_service != null) return _service;

            UserCredential credential;
            var path = AssemblyDirectory + @"\SpreadSheets\client_secret.json";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);                    
            }

            using (var stream =
                new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.hatys.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Google Sheets API service.
            _service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return _service;
        }

        public static IList<IList<object>> GetSpreadsheetRange(string spreadsheetId, string range)
        {

            var gsheetApi = GetService();
            var request = gsheetApi.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = request.Execute();
            return response.Values;
        }

        public static IList<IList<object>> GetNotEmptySpreadsheetRange(string spreadsheetId, string range, string description)
        {
            IList<IList<object>> data;

            try
            {
                data = GetSpreadsheetRange(spreadsheetId, range);
            }
            catch (Exception e)
            {
                var message = string.Format("Error quering {0} spreadsheet {1}/{2}: exception occured: {3}.", description, spreadsheetId, range, e.Message);
                throw new Exception(message, e);
            }

            if (null == data)
            {
                var message = string.Format("Error quering {0} spreadsheet {1}/{2}: data is null.", description, spreadsheetId, range);
                throw new NullReferenceException(message);
            }

            if (!(data.Count > 0))
            {
                var message = string.Format("Error quering {0} spreadsheet {1}/{2}: data is empty.", description, spreadsheetId, range);
                throw new InvalidDataException(message);
            }

            return data;
        }

        public class RowDataHandling //TODO: parser helpers
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
