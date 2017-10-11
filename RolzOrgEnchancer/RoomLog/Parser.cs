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
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        const int MAX_ROOM_LOG_ITEMS = 25;
        private const string ROOM_LOG_PREFIX = "https://rolz.org/api/roomlog?room=";
        private readonly Uri _roomLog;
        private long _sessionTime;

        public static string RollIdToComment(int rollId)
        {
          return "roll_id=" + rollId;
        }

        public Parser(string roomName)
        {
            _roomLog = new Uri(ROOM_LOG_PREFIX + roomName, UriKind.Absolute);
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
                logger.Warn("Getting json...");
                var json = webClient.DownloadString(_roomLog);
                logger.Warn("Parsing json...");
                //Now parse with JSON.Net
                ret = JsonConvert.DeserializeObject<RootObject>(json);

                logger.Warn("Got json, last line =  {0}:{1}:{2}", 
                    ret.items[ret.items.Count - 1].type,
                    ret.items[ret.items.Count - 1].text,
                    ret.items[ret.items.Count - 1].comment);
            }
            return ret;
        }

        private Item MatchMessageInternal(string type, string text, string comment)
        {
            try
            {
                var x = GetRoomLog().items;

                if (x != null)
                {

                    var item = x.LastOrDefault(m => m != null &&
                                                    m.time >= _sessionTime &&
                                                    m.type != null &&
                                                    m.type.Equals(type) &&
                                                    (text == null || m.text != null && m.text == text) &&
                                                    (comment == null || m.comment != null && m.comment == comment)
                                                );

                    if (item == null)
                    {
                        if (text != null)
                        {
                            logger.Warn("Cant find string {0}, last item is {1}", text, x[x.Count - 1].text);
                        }
                        if (comment != null)
                        {
                            logger.Warn("Cant find string {0},  last item is {1}", comment, x[x.Count - 1].comment);
                        }

                        logger.Warn("anyway item == null");
                    }

                    return item;
                }
            }
            catch (InvalidOperationException e)
            {
                logger.Warn("Got exception {0}", e.Message);
            }
            return null;
        }

        public Item MatchMessage(string message)
        {
            return MatchMessageInternal("txtmsg", message, null);
        }

        public Item MatchRoll(int rollId)
        {
            return MatchMessageInternal("dicemsg", null, RollIdToComment(rollId));
        }

        public string GetSessionRoomLogParsed()
        {            
            var res = "";
            if (_sessionTime == 0) return res;
            try
            {
                var items = GetRoomLog().items; //TODO take last cached value
                if (items.Count > MAX_ROOM_LOG_ITEMS)
                {
                    items = items.SkipWhile((val, index) => index < items.Count - MAX_ROOM_LOG_ITEMS).ToList();                    
                }
                items = items.Where(m => m.time >= _sessionTime).ToList();
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
