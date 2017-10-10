using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using RollerEngine.Character;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RolzOrgEnchancer.Interfaces;
using RolzOrgEnchancer.RoomLog;
using RolzOrgEnchancer.UI;

namespace RolzOrgEnchancer
{

    public enum Color
    {
        Gray = 0,   //Verbosity.Debug
        Black = 1,  //Verbosity.Details
        Orange = 2, //Verbosity.Normal
        Blue = 3,   //Verbosity.Important
        Pink = 4,   //Verbosity.Critical
        Teal = 5,   //Verbosity.Warning
        Red = 6,    //Verbosity.Error

        Green,  //UI command
        Purple, //UI session
        Maroon, //free
        Olive   //free
    }

    public static class Helper
    {
        public static Color Verbosity2Color(Verbosity verbosity)
        {
            switch (verbosity)
            {
                case Verbosity.Debug:
                    return Color.Gray;
                case Verbosity.Details:
                    return Color.Black;
                case Verbosity.Normal:
                    return Color.Orange;
                case Verbosity.Important:
                    return Color.Blue;
                case Verbosity.Critical:
                    return Color.Pink;
                case Verbosity.Warning:
                    return Color.Teal;
                case Verbosity.Error:
                    return Color.Red;
                default:
                    throw new Exception("Unknown verbosity");
            }
        }

        public static string Color2Prefix(Color color)
        {
            switch (color)
            {
                case Color.Black:
                    return "";
                case Color.Gray:
                    return "gray:";
                case Color.Orange:
                    return "orange:";
                case Color.Blue:
                    return "blue:";
                case Color.Pink:
                    return "pink:";
                case Color.Teal:
                    return "teal:";
                case Color.Red:
                    return "red:";
                case Color.Green:
                    return "green:";
                case Color.Purple:
                    return "purple:";
                case Color.Maroon:
                    return "maroon:";
                case Color.Olive:
                    return "olive:";
                default:
                    throw new Exception("Unknown color");
            }
        }

    }

    internal class RoomBootImpl : BaseLogger, IRoller
    {
        private const int DICE_FACET = 10;
        private static int _idRoll = 100;

        public RoomBootImpl(Verbosity minVerbosity) : base(minVerbosity)
        {
        }

        public List<int> Roll(int diceCount, int dc)
        {
            var item = RoomBot.MakeRoll(diceCount, dc, _idRoll++);
            var rawResult = new List<int>(DICE_FACET);
            rawResult.AddRange(new int[DICE_FACET]);

            if (!item.details.StartsWith("( (")) throw new Exception("Invalid details 1");
            var index = item.details.IndexOf('→', 0);
            if (index == -1) throw new Exception("Invalid details 2");
            var res = item.details.Substring(0, index);
            res = res.Substring(3);
            var res2 = res.Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            if (res2.Length != diceCount) throw new Exception("Invalid dice count");

            foreach (var r in res2)
            {
                int i;
                if (!int.TryParse(r, out i)) throw new Exception("Not a number dice value");
                if (i < 1 || i > DICE_FACET) throw new Exception("Invalid dice value");
                rawResult[i - 1]++;
            }

            return rawResult;
        }

        public override void Log(Verbosity verbosity, string record)
        {
            RoomBot.MakeMessage(Helper.Verbosity2Color(verbosity), ApplyFormat(record));
        }
    }

    internal static class RoomBot
    {
        private static Thread _thread;
        private static IRolzOrg _browser;
        private static IFormUpdate _updater;
        private static int _ticks;
        private static ConcurrentQueue<int> _actionQueue;
        private static ConcurrentQueue<string> _messageQueue;

        public static Parser Parser = new Parser(DEFAULT_ROOM_NAME);
        private const string DEFAULT_ROOM_NAME = "Hatys%20Test8";

        private static void Worker()
        {
            while (!_browser.RoomEntered()) Thread.Sleep(100);
            Program.Log("Worker: Inside room");

            //Establish session
            var sessId = new Random((int) DateTime.Now.Ticks);
            var connectionMessage = string.Format("establishing session #{0} ...", sessId.Next());
            var item = MakeSingleMessage(Color.Purple, connectionMessage);
            Program.Log("Worker: session was established");
            Parser.SetSessionTime(item.time);

            MakeCommand("/opt autoexpand=on");
            MakeCommand("/nick HatysBot");

            var myInterface = new RoomBootImpl(Verbosity.Critical);
            var res = HatysParty.LoadFromGoogle(myInterface, myInterface);
            MakeMessage(Color.Purple, "*** PARTY LOADED ***");

            for (;;)
            {
                Thread.Sleep(100);
                int action;
                while (_actionQueue.TryDequeue(out action))
                {
                    Thread.Sleep(100);
                    Program.Log(string.Format("Worker: Deque action #{0}", action));
                    //TODO switch on actions!
                    for (var i = 1; i < 2; i++)
                    {
                        res.DoWeek(i);
                    }
                    res.LogTotalProgress();
                    MakeMessage(Color.Purple, "*** END OF ACTION ***");
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static readonly char[] LineSeparators = {'\n', '\r'};
        public static void MakeMessage(Color color, string message)
        {
            /*
             * The Dice Room chat works like any other chat room.
             * It features special commands that start with a / (slash),
             * and it facilitates dice rolls using the prefix # (number sign), or - (minus),
             * or inline codes within [ ] (square brackets).
             */

            //first stage: replace [] with {} due to inline code
            message = message.Replace('[', '{');
            message = message.Replace(']', '}');
            //second stage: replace ' with ` due to jscript
            message = message.Replace('\'', '`');
            //third stage: split string to lines
            var lines = message.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in lines)
            {
                var line = item;
                //fourth stage: if starts with === replace = from string
                if (line.StartsWith("===")) line = line.Replace("=", "");
                //fifth stage: remove starting spaces and special characters for commands and rolls
                line = line.TrimStart(' ', '/', '#', '-');
                //last stage: remove ending white chars
                line = line.TrimEnd();
                if (line != item) Program.Log(string.Format("Bot: replaced '{0}'  with '{1}'", item, line));
                if (line != "") MakeSingleMessage(color, line);
            }
        }

        private static Item MakeSingleMessage(Color color, string message)
        {
            for (var attempt = 0;;attempt++)
            {
                //repeat message each 5 secs
                if (0 == attempt % 50)
                {
                    QueueColorMessage(color, message);
                }
                Thread.Sleep(100);

                var item = Parser.MatchMessage(message);
                if (null != item)
                {
                    return item;
                }
            }
        }

        public static Item MakeRoll(int diceCount, int dc, int rollId)
        {
            var rollmsg = string.Format("#{0}d10f{1} #{2}", diceCount, dc, Parser.RollIdToComment(rollId));

            for (var attempt = 0; ; attempt++)
            {
                //repeat message each 5 secs
                if (0 == attempt % 50)
                {
                    QueueMessage(rollmsg);
                }
                Thread.Sleep(100);

                var item = Parser.MatchRoll(rollId);
                if (null != item)
                {
                    return item;
                }
            }
        }

        private static void MakeCommand(string command)
        {
            QueueMessage(command);
            Thread.Sleep(500);
        }

        private static void QueueColorMessage(Color color, string message)
        {
            QueueMessage(Helper.Color2Prefix(color) + message);
        }

        private static void QueueMessage(string message)
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

        public static void OnGuiAction(int action)
        {
            Program.Log("Bot: added to queue action=" + action);
            _actionQueue.Enqueue(action);
        }

        public static void OnGuiStarted(IRolzOrg browser, IFormUpdate updater)
        {
            _thread = new Thread(Worker) {IsBackground = true};
            _browser = browser;
            _updater = updater;
            _actionQueue = new ConcurrentQueue<int>();
            _messageQueue = new ConcurrentQueue<string>();
            _browser.JoinRoom(DEFAULT_ROOM_NAME);
            _thread.Start();
        }
    }
}
