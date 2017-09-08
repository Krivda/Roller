using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using RolzOrgEnchancer.UI;

namespace RolzOrgEnchancer.RoomLog
{
    //
    // RoomLog parser
    //
    internal class Parser
    {
        private const string RoomLogPrefix = "https://rolz.org/api/roomlog?room=";
        private readonly Uri _roomLog;
        private long _sessionTime;

        public Parser(string roomName)
        {
            _roomLog = new Uri(RoomLogPrefix + roomName, UriKind.Absolute);
        }

        public void SetSessionTime(long time)
        {
            Program.Log("Parser: session_time is " + time);
            _sessionTime = time;
        }

        private RootObject GetRoomLog()
        {
            RootObject ret;
            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadString(_roomLog);
                //Now parse with JSON.Net
                ret = JsonConvert.DeserializeObject<RootObject>(json);
            }
            return ret;
        }

        public Item MatchMessage(string message)
        {
            try
            {
                var x = GetRoomLog().items;
                if (x == null) return null;
                return x.LastOrDefault(m => m != null &&
                                            m.type != null &&
                                            m.type.Equals("txtmsg") &&
                                            m.text != null &&
                                            m.text.Equals(message) &&
                                            m.time >= _sessionTime);
            }
            catch (InvalidOperationException)
            {
            }
            return null;
        }

        public Item MatchRoll(uint rollId)
        {
            try
            {
                var x = GetRoomLog().items;
                if (x == null) return null;
                return x.LastOrDefault(m => m != null &&
                                            m.type != null &&
                                            m.type.Equals("dicemsg") &&
                                            m.comment != null &&
                                            m.comment.Equals("roll_id=" + rollId.ToString()) &&
                                            m.time >= _sessionTime);
            }
            catch (InvalidOperationException)
            {
            }
            return null;
        }

        public string GetSessionRoomLogParsed()
        {
            var res = "";
            if (_sessionTime == 0) return res;
            try
            {
                var items = GetRoomLog().items.Where(m => m.time >= _sessionTime);
                foreach(var item in items)
                {
                    if (item.type == null) continue;
                    if (item.type.Equals("txtmsg")) res += ParseRoomLogMessage(item);
                    if (item.type.Equals("dicemsg")) res += ParseRoomLogRoll(item);
                }
            }
            catch (InvalidOperationException)
            {
            }
            return res;
        }

        public string ParseRoomLogMessage(Item item)
        {
            return item.time + ": " + item.text + "\r\n";
        }

        public string ParseRoomLogRoll(Item item)
        {
            string id = item.comment ?? "roll_id=<no>";
            return item.time + ": " + id + ", " + item.input + "= " + Convert.ToInt16(item.result) + " : " + item.details + "\r\n";
        }

    }
}
