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

        public static string RollIdToComment(uint rollId)
        {
          return "roll_id=" + rollId;
        }

        public Parser(string roomName)
        {
            _roomLog = new Uri(RoomLogPrefix + roomName, UriKind.Absolute);
        }

        public void SetSessionTime(long time)
        {
            Program.Log(string.Format("Parser: session_time is {0}", time));
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

        private Item MatchMessageInternal(string type, string text, string comment)
        {
            try
            {
                var x = GetRoomLog().items;
                if (x == null) return null;
                return x.LastOrDefault(m => m != null &&
                                            m.type != null &&
                                            m.type.Equals(type) &&
                                            (text == null || m.text != null && m.text == text) &&
                                            (comment == null || m.comment != null && m.comment == comment) &&
                                            m.time >= _sessionTime);
            }
            catch (InvalidOperationException)
            {
            }
            return null;
        }

        public Item MatchMessage(string message)
        {
            return MatchMessageInternal("txtmsg", message, null);
        }

        public Item MatchRoll(uint rollId)
        {
            return MatchMessageInternal("dicemsg", null, RollIdToComment(rollId));
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

        private static string ParseRoomLogMessage(Item item)
        {
            return string.Format("{0}: {1}\r\n", item.time, item.text);
        }

        private static string ParseRoomLogRoll(Item item)
        {
            var id = item.comment ?? "roll_id=<no>";
            return string.Format("{0}: {1}, {2}= {3} : {4}\r\n", item.time, id, item.input, item.result, item.details);
        }

    }
}
