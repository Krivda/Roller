using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RollerEngine.Character;
using RollerEngine.Character.Common;
using RolzOrgEnchancer.Interfaces;
using RolzOrgEnchancer.RoomLog;
using RolzOrgEnchancer.UI;
using IRollLogger = RollerEngine.Logger.IRollLogger;
using IRoller = RollerEngine.Roller.IRoller;
using RollData = RollerEngine.Roller.RollData;
using Verbosity = RollerEngine.Logger.Verbosity;

namespace RolzOrgEnchancer
{
    internal enum Color
    {
        Black,  //Verbosity.Details
        Red,    //Roll Description
        Green,                          //command
        Blue,   //Verbosity.Important
        Gray,   //Verbosity.Debug
        Maroon,
        Olive,  
        Orange,
        Purple, //Session
        Teal,   
        Pink    //Verbosity.Warning
    }

    internal class Message
    {
        

        string SetString(string message, Color color, bool isRoll)
        {
            //string[] messageStrings = message.Split()
            return null;
        }
    }

    internal class RoomBootImpl : IRollLogger, IRoller
    {
        static uint nRoll = 100;
        //RollerEngine.Roller.IRoller
        public RollData Roll(int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill, string description)
        {
            description = description.Replace('\n', 'x');
            description = description.Replace('\r', 'x');
            description = description.Replace('[', '{');
            description = description.Replace(']', '}');
            description = description.Replace('\'', '`');
            Program.Log("RoomBootImpl: Rolling description: " + description);
            RoomBot.QueueMessage(description);
            RoomLog.Item item;
            for (; ; )
            {
                item = RoomBot.Parser.MatchMessage(description);
                if (null != item) break;
                Task.Delay(100);
            }
            Task.Delay(1000);

            var input = new RollInput();
            input.Initialize(diceCount, DC, removeSuccessOnOnes, hasSpecialization, hasWill);
            item = RoomBot.MakeRoll((uint)diceCount, (uint)DC, nRoll);
            nRoll++;

            var output = new RollOutput();
            if (!item.details.StartsWith("( (")) throw new Exception("Invalid 1");
            var index = item.details.IndexOf('→', 0);
            if (index == -1) throw new Exception("Invalid 2");
            var res = item.details.Substring(0, index);
            res = res.Substring(3);
            var res2 = res.Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            if (res2.Length != diceCount) throw new Exception("Invalid 3");
            output.RawDices = new List<int>();
            foreach (var r in res2)
            {
                int i;
                if (!int.TryParse(r, out i)) throw new Exception("Invalid 4");
                output.RawDices.Add(i);
            }

            output.RawNumberOfOnes = 0;
            output.RawNumberOfTens = 0;
            output.RawResult = 0;
            foreach (var x in output.RawDices)
            {
                if ((x < 1) || (x > 10)) throw new Exception("Invalid 5");
                if (x == 1) output.RawNumberOfOnes++;
                if (x == 10) output.RawNumberOfTens++;
                if (x >= input.DC) output.RawResult++;
            }
            output.RawResult -= output.RawNumberOfOnes;
            var checkR = Convert.ToInt16(item.result);
            if (checkR != output.RawResult)
            {
                throw new Exception("Invalid 6");
            }
            output.CalculateResult(input);
            return new RollData(output.Result, output.RawDices);
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
            RoomBot.QueueMessage(record);
            for (; ; )
            {
                var item = RoomBot.Parser.MatchMessage(record);
                if (null != item) break;
                Task.Delay(100);
            }
            Task.Delay(1000);
        }

    }

    internal static class RoomBot 
    {
        private static Thread _thread;
        private static IRolzOrg _browser;
        private static IFormUpdate _updater;
        private static int _ticks;
        private static ConcurrentQueue<string> _actionQueue;
        private static ConcurrentQueue<string> _messageQueue;

        public static Parser Parser = new Parser(DefaultRoomName);
        private const string DefaultRoomName = "Hatys%20Test";

        private static void Worker()
        {
            while (!_browser.RoomEntered()) Task.Delay(100);
            Program.Log("Worker: Inside room");

            //Establish session
            var sessId = new Random(unchecked((int)DateTime.Now.Ticks));
            var connectionMessage = "establishing session #" + sessId.Next() + "...";
            var item = MakeSingleMessage(Color.Purple, connectionMessage);
            Program.Log("Worker: session was established");
            Parser.SetSessionTime(item.time);

            MakeCommand("/opt autoexpand=on");
            MakeCommand("/nick HatysBot");

            for (var n = 0; n < 11; n++)
            {
                MakeSingleMessage((Color)n, "color message");
            }

            //MakeMessage(Color.Red, "String 1 \r\nString 2\n\rString 3\r\nString 4\rString 5\n\n\r\r");

            for (uint n = 0; n < 3; n++)
            {
                MakeRoll(8, 5, 10+n);
                Program.Log("Worker: roll was found");
            }

            //InitializeKrivda(IRoomBot)
            //EmulateKrivda()

            var interfaces = new RoomBootImpl();
            var res = HatysPartyLoader.LoadParty(interfaces, interfaces);            

            string action;
            for(;;)
                while (_actionQueue.TryDequeue(out action))
                {
                    Task.Delay(100);
                    if (!action.StartsWith("Action")) continue;
                    int nAction;
                    if (int.TryParse(action.Substring(6), out nAction))
                    {
                        Program.Log("Worker: Deque action #" + nAction);
                        res.Nameless.WeeklyBoostSkill(Build.Abilities.Instruction);
                    }
                }
        }

        public static void MakeMessage(Color color, string message)
        {
            //record = record.Replace('\n', 'x');
            //record = record.Replace('\r', 'x');
            //record = record.Replace('[', '{');
            //record = record.Replace(']', '}');
            //record = record.Replace('\'', '`');
            // starts with #
            // starts with /
            // starts with spaces
            var line = message;
            Program.Log(line);
            MakeSingleMessage(color, line);
        }

        public static Item MakeSingleMessage(Color color, string message)
        {
            var attempt = 0;
            for (; ; )
            {
                //repeat message each 10 secs
                if (0 == attempt % 100)
                {
                    QueueColorMessage(color, message);
                }
                
                var item = Parser.MatchMessage(message);
                if (null != item) return item;
                attempt++;
                Task.Delay(100);
            }
        }

        public static Item MakeRoll(uint diceCount, uint dc, uint rollId)
        {
            var rollmsg = "#" + diceCount + "d10f" + dc + " #roll_id=" + rollId;
            var attempt = 0;
            for (; ; )
            {
                //repeat message each 10 secs
                if (0 == attempt % 100)
                {
                    QueueMessage(rollmsg);
                }

                var item = Parser.MatchRoll(rollId);
                if (null != item) return item;
                attempt++;
                Task.Delay(100);
            }
        }

        private static void MakeCommand(string command)
        {
            QueueMessage(command);
            Task.Delay(1000);
        }

        private static readonly string[] ColorPrefix = { "", "red:", "green:", "blue:", "gray:", "maroon:", "olive:", "orange:", "purple:", "teal:", "pink:" };
        private static void QueueColorMessage(Color color, string message)
        {
            QueueMessage(ColorPrefix[(int) color] + message);
        }

        //TODO private
        public static void QueueMessage(string message)
        {
            _messageQueue.Enqueue(message);
        }

        public static void OnGuiTick()
        {            
            Program.SafeLog.ProcessLogQueue();
            if (0 == _ticks++ % 30) _updater.UpdateRoomLog(Parser.GetSessionRoomLogParsed());
            _updater.UpdateActionQueueDepth(_actionQueue.Count);
            string message;
            if (_messageQueue.TryDequeue(out message))
            {
                _browser.SendMessage(message);
            }
        }

        public static void OnGuiAction(string action)
        {
            Program.Log("Bot: added to queue action=" + action);
            _actionQueue.Enqueue(action);
        }

        public static void OnGuiStarted(IRolzOrg browser, IFormUpdate updater)
        {
            _thread = new Thread(Worker) {IsBackground = true};
            _browser = browser;
            _updater = updater;
            _actionQueue = new ConcurrentQueue<string>();
            _messageQueue = new ConcurrentQueue<string>();
            _browser.JoinRoom(DefaultRoomName);
            _thread.Start();
        }

    }
}
