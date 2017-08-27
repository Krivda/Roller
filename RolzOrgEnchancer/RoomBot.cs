using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
//using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using RollerEngine.Character;
using RollerEngine.Character.Modifiers;
using RolzOrgEnchancer.Interfaces;
using RolzOrgEnchancer.UI;
using WOD;
using IRollLogger = RollerEngine.Logger.IRollLogger;
using IRoller = RollerEngine.Roller.IRoller;
using Verbosity = RollerEngine.Logger.Verbosity;

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

    class RoomBootImpl : IRollLogger, IRoller
    {
        static int nRoll = 100;
        //RollerEngine.Roller.IRoller
        public int Roll(int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill, string description)
        {
            description = description.Replace('\n', 'x');
            description = description.Replace('\r', 'x');
            description = description.Replace('[', '{');
            description = description.Replace(']', '}');
            description = description.Replace('\'', '`');
            Program.Log("RoomBootImpl: Rolling description: " + description);
            RoomBot.SetMessage(description, true);
            RoomLog.Item item;
            for (; ; )
            {
                item = RoomBot.parser.MatchMessage(description);
                if (null != item) break;
                Task.Delay(100);
            }
            Task.Delay(1000);

            var input = new RollInput();
            input.Initialize(diceCount, DC, removeSuccessOnOnes, hasSpecialization, hasWill);
            RoomBot.SetMessage("#" +diceCount + "d10f" + DC + " #roll_id=" + nRoll.ToString(), false);
            item = null;
            for (; ; )
            {
                item = RoomBot.parser.MatchRoll(nRoll);
                if (null != item)
                {
                    Program.Log("RoomBootImpl: roll was found");
                    break;
                }
                Task.Delay(100);
            }
            Task.Delay(500);
            nRoll++;

            int tens = 0;
            int ones = 0;
            if (null != item.tags)
            {
                throw new Exception("tags were gone");
                foreach (var tag in item.tags)
                {
                    if (tag.k == "10s") tens = Convert.ToInt16(tag.v);
                    if (tag.k == "ones") ones = Convert.ToInt16(tag.v);
                }
            }
            var output = new RollOutput();
            if (!item.details.StartsWith("( (")) throw new Exception("Invalid 1");
            int index = item.details.IndexOf('→', 0);
            if (index == -1) throw new Exception("Invalid 2");
            string res = item.details.Substring(0, index);
            res = res.Substring(3);
            string[] res2 = res.Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            if (res2.Length != diceCount) throw new Exception("Invalid 3");
            output._raw_dices = new List<int>();
            foreach (var r in res2)
            {
                int i;
                if (!int.TryParse(r, out i)) throw new Exception("Invalid 4");
                output._raw_dices.Add(i);
            }

            output._raw_number_of_ones = 0;
            output._raw_number_of_tens = 0;
            output._raw_result = 0;
            foreach (var x in output._raw_dices)
            {
                if ((x < 1) || (x > 10)) throw new Exception("Invalid 5");
                if (x == 1) output._raw_number_of_ones++;
                if (x == 10) output._raw_number_of_tens++;
                if (x >= input.DC) output._raw_result++;
            }
            output._raw_result -= output._raw_number_of_ones;
            var check_r = Convert.ToInt16(item.result);
            if (check_r != output._raw_result)
            {
                throw new Exception("Invalid 6");
            }
            output.CalculateResult(input);
            return output.result;
        }

        //RollerEngine.Logger.IRollLogger
        public void Log(Verbosity verbosity, string record)
        {
            record = record.Replace('\n', 'x');
            record = record.Replace('\r', 'x');
            record = record.Replace('[', '{');
            record = record.Replace(']', '}');
            record = record.Replace('\'', '`');
            Program.Log("RoomBootImpl: Logging message: " + verbosity.ToString() + record);
            RoomBot.SetMessage(record, false);
            for (; ; )
            {
                var item = RoomBot.parser.MatchMessage(record);
                if (null != item) break;
                Task.Delay(100);
            }
            Task.Delay(1000);
        }

    }

    static class RoomBot 
    {
        static Thread thread;
        static IRolzOrg browser;
        static IFormUpdate updater;
        static public RoomLog.Parser parser = new RoomLog.Parser(default_room_name);
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

        public static void SetMessage(string message, bool is_system_message)
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

            for (int n = 0; n < 3; n++)
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

            RoomBootImpl interfaces = new RoomBootImpl();
            var res = HatysPartyLoader.LoadParty(interfaces, interfaces);            

            string action;
            for(;;)
                while (action_queue.TryDequeue(out action))
                {
                    await Task.Delay(100);
                    if (!action.StartsWith("Action")) continue;
                    int n_action;
                    if (int.TryParse(action.Substring(6), out n_action))
                    {
                        Program.Log("Deque action #" + n_action);
                        res.Nameless.WeeklyBoostTeachersEase();
                    }
                }
        }

        static public void OnGuiTick()
        {            
            Program.SafeLog.ProcessLogQueue();
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
