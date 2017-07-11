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
        static IFormUpdate updater;
        static RoomLog.Parser parser = new RoomLog.Parser(default_room_name);
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
            } while (null != parser.MatchMessage(connection_message));
            for (;;)
            {
                //repeat message each 5 secs
                if (0 == connection_attempt % 50)
                {
                    SetMessage(connection_message, true);
                }

                var item = parser.MatchMessage(connection_message);
                if (null != item)
                {
                    Program.Log("Worker: session was established");
                    parser.SetSessionTime(item.time);
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
                SetMessage("#8d10f5 #roll_id=" + n.ToString(), false);
                for (; ; )
                {
                    if (null != parser.MatchRoll(n))
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
            Program.safe_log.ProcessLogQueue();
            if (0 == ticks++ % 30) updater.LogRoomLog(parser.GetSessionRoomLogParsed());
            updater.UpdateActionQueueDepth(action_queue.Count);
            var message = GetMessage();
            if (null != message)
            {
                if (is_system_message)
                    browser.SendSystemMessage(message);
                else
                    browser.SendMessage(message);
            }
        }

        static public void OnGuiAction(string action)
        {
            Program.Log("added to queue action=" + action);
            action_queue.Enqueue(action);
        }

        static public void OnGuiStarted(IRolzOrg _browser, IFormUpdate _updater)
        {
            thread = new Thread(Worker);
            thread.IsBackground = true;
            browser = _browser;
            updater = _updater;
            action_queue = new ConcurrentQueue<string>();
            browser.JoinRoom(default_room_name);
            thread.Start();
        }

    }
}
