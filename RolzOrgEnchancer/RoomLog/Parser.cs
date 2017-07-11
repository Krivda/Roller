using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace RolzOrgEnchancer.RoomLog
{
    //
    // RoomLog parser
    //
    class Parser
    {
        private const string room_log_prefix = "https://rolz.org/api/roomlog?room=";
        private Uri room_log;
        private long session_time = 0;

        public Parser(string room_name)
        {
            this.room_log = new Uri(room_log_prefix + room_name, UriKind.Absolute);
        }

        public void SetSessionTime(long time)
        {
            Program.Log("Parser: session_time is " + time.ToString());
            session_time = time;
        }

        private RootObject GetRoomLog()
        {
            RootObject ret;
            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadString(room_log);
                //Now parse with JSON.Net
                ret = JsonConvert.DeserializeObject<RootObject>(json);
            }
            return ret;
        }

        public Item MatchMessage(string message)
        {
            try
            {
                return GetRoomLog().items.Last(m => (
                    m.type.Equals("txtmsg") &&
                    m.text.Equals(message) &&
                    (m.time >= session_time)
                    ));
            }
            catch (InvalidOperationException)
            {
            }
            return null;
        }

        public Item MatchRoll(int roll_id)
        {
            try
            {
                return GetRoomLog().items.Last(m => (
                    m.type.Equals("dicemsg") &&
                    (null != m.comment) &&
                    m.comment.Equals("roll_id=" + roll_id.ToString()) &&
                    (m.time >= session_time)
                    ));
            }
            catch (InvalidOperationException)
            {
            }
            return null;
        }

        public string GetSessionRoomLogParsed()
        {
            string res = "";
            if (session_time != 0)
            {
                try
                {
                    var items = GetRoomLog().items.Where(m => (
                            (m.time >= session_time)
                            ));
                    foreach(Item item in items)
                    {
                        if (item.type.Equals("txtmsg")) res += ParseRoomLogMessage(item);
                        if (item.type.Equals("dicemsg")) res += ParseRoomLogRoll(item);
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }
            return res;
        }

        public string ParseRoomLogMessage(Item item)
        {
            return item.time.ToString() + ": " + item.text + "\r\n";
        }

        public string ParseRoomLogRoll(Item item)
        {
            string id = (item.comment == null) ? "roll_id=<no>" : item.comment;
            string tag_info = "";
            if (null != item.tags)
            {
                foreach (var tag in item.tags)
                {
                    if (tag.k == "10s") tag_info += "tens=" + Convert.ToInt16(tag.v).ToString() + ";";
                    if (tag.k == "ones") tag_info += "ones=" + Convert.ToInt16(tag.v).ToString() + ";";
                }
            }
            return item.time.ToString() + ": " + id + ", " + item.input + "= " + Convert.ToInt16(item.result).ToString() + " (" + tag_info + ") <<" + item.details + "\r\n";
        }

    }
}
