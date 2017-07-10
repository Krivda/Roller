using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
//using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RolzOrgEnchancer
{

    class RoomLogParser
    {
        private const string room_log_prefix = "https://rolz.org/api/roomlog?room=";
        private Uri room_log;

        public RoomLogParser(string room_name)
        {
            this.room_log = new System.Uri(room_log_prefix + room_name, System.UriKind.Absolute);
        }

        public string  GetRawLog()
        {
            string json;
            using (var webClient = new System.Net.WebClient())
            {
                json = webClient.DownloadString(room_log);
            }
            return json;
        }

        public RoomLog.RootObject GetLog()
        {
            var json = GetRawLog();
            //Now parse with JSON.Net
            return JsonConvert.DeserializeObject<RoomLog.RootObject>(json);
        }

        public bool ParseLog(string message)
        {
            RoomLog.RootObject log = GetLog();
            try
            {
                var item = log.items.Last(m => (m.type.Equals("txtmsg") && m.text.Equals(message)));
                Program.Log("ParseLog: time=" + item.time);
                return true;
            }
            catch (System.InvalidOperationException)
            {
            }
            return false;
        }

        public bool ParseRoll(int roll_id)
        {
            RoomLog.RootObject log = GetLog();
            try
            {
                var item = log.items.Last(m => (m.type.Equals("dicemsg") && (null!=m.comment) && m.comment.Equals("roll_id="+roll_id.ToString())));
                Program.Log("ParseRoll: time=" + item.time + " input=" + item.input + " result=" + item.result +
                    " details=" + item.details);
                
                if (null != item.tags)
                {
                    string tag_info = "";
                    foreach (var tag in item.tags)
                    {
                        tag_info += (tag.k.ToString() + "=" + tag.v.ToString() + ";");
                    }
                    Program.Log("tags=" + item.tags.Count + " : " + tag_info);
                }
                
                return true;
            }
            catch (System.InvalidOperationException)
            {
            }
            return false;
        }
    }

    //item.detail; /* "( (6, 4, 3, 4, 9, 5  \u2192 2 successes against 6) )" - need to parse to understand results */

    /*
    class IRoomBot
    {
        void PerformRoll();
        void SendMessage();
        void ChangeRoom();
    }
    */
    //TODO: ask what roll to make (buttons will put high-level requests to action queue
    //                             while GetNextRoll will take action and perform sequence of rolls)
    //bool = GetNextRoll(roll);

    static class RoomBot
    {
        static Thread thread;
        static IRolzOrg browser;
        static RoomLogParser parser = new RoomLogParser(default_room_name);
        const string default_room_name = "Hatys%20Test";

        static int ticks = 0;

        static bool is_system_message;
        static object _lock = new object();
        static string current_message = null;
        static Tuple<string, bool> null_tuple = new Tuple<string, bool>(null, false);

        static ConcurrentQueue<string> action_queue;

        static Tuple<string,bool> UpdateMessage(Tuple<string,bool> tuple)
        {
            Tuple<string, bool> ret = null_tuple;
            lock (_lock)
            {
                if (tuple.Item1 == null)
                {
                    //this is get
                    if (current_message != null)
                    {
                        ret = new Tuple<string, bool>(current_message, is_system_message);
                        current_message = null;
                    }
                }
                else 
                {
                    //this is set
                    current_message = tuple.Item1;
                    is_system_message = tuple.Item2;
                }
            }
            return ret;
        }

        static string GetMessage()
        {
            Tuple<string, bool> tuple = UpdateMessage(null_tuple);
            return tuple.Item1;
        }

        static void SetMessage(string message, bool is_system_message)
        {
            var ret = new Tuple<string, bool>(message, is_system_message);
            UpdateMessage(ret);
        }

        static async void Worker()
        {
            while (!browser.RoomEntered())
                await Task.Delay(100);
            Program.Log("Worker: Inside room");

            

            //Establish session
            int connection_attempt = 0;
            string connection_message;
            do //check for random uniqueness in room
            {
                Random n = new Random(unchecked((int)DateTime.Now.Ticks));
                connection_message = "establishing session #" + n.Next().ToString() + "...";
            } while (parser.ParseLog(connection_message));
            for (;;)
            {
                //repeat message each 5 secs
                if (0 == connection_attempt % 50)
                {
                    SetMessage(connection_message, true);
                }

                if (parser.ParseLog(connection_message))
                {
                    Program.Log("Worker: session was established");
                    break;
                    //return true;
                }
                
                connection_attempt++;
                await Task.Delay(100);
                //if (connection_attempt == 500)
                    //return false;
            }

            SetMessage("/opt autoexpand=on", false);
            await Task.Delay(500);
            SetMessage("/nick HatysBot", false);
            await Task.Delay(500);

            for (int n = 0; n < 5; n++)
            {
                SetMessage("#6d10f5 #roll_id=" + n.ToString(), false);
                for (; ; )
                {
                    if (parser.ParseRoll(n))
                    {
                        Program.Log("Worker: roll was found");
                        break;
                    }
                    await Task.Delay(100);
                }
                await Task.Delay(500);
            }

            //InitializeKrivda(IRoomBot)
            //EmulateKrivda()
        }

        static public void OnGuiTick()
        {            
            Program.ProcessLogQueue();
            if (0 == ticks++ % 20) Program.logger.LogRoomLog(parser.GetRawLog());
            Program.logger.UpdateActionQueueDepth(action_queue.Count);
            //Program.logger.LogRoomLog();
            var message = GetMessage();
            if (null != message)
            {
                if (is_system_message)
                    browser.SendSystemMessage(message);
                else
                    browser.SendMessage(message);
            }
            //Program.Log - process log here
        }

        static public void OnGuiAction(string action)
        {
            Program.Log("added to queue action=" + action);
            action_queue.Enqueue(action);
        }

        static public void OnGuiStarted(IRolzOrg _browser)
        {
            thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Start();
            browser = _browser;
            action_queue = new ConcurrentQueue<string>();
            browser.JoinRoom(default_room_name);
        }

    }
}
