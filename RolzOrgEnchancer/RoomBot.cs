using System;
using System.Collections.Concurrent;
using System.Threading;
using RollerEngine.Character;
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
        Red,    //Verbosity.Error
        Green,                          //command
        Blue,   //Verbosity.Critical
        Gray,   //Verbosity.Debug
        Maroon,
        Olive,
        Orange, //Roll Description
        Purple, //Session
        Teal,   //Verbosity.Important
        Pink    //Verbosity.Warning
    }

    internal class RoomBootImpl : IRollLogger, IRoller
    {
        private static uint _idRoll = 100;
        //RollerEngine.Roller.IRoller
        public RollData Roll(int diceCount, int dc, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill, string description)
        {
            RoomBot.MakeMessage(Color.Orange, description);

            var specialization = hasSpecialization ? Specialization.UsingSpecialization : Specialization.False;
            var negateBotch = hasWill ? NegateBotch.UsingWillpower : NegateBotch.False;
            var ignoreFailures = removeSuccessOnOnes ? IgnoreFailures.IgnoreFailures : IgnoreFailures.False;

            var input = new RollInput(diceCount, dc, specialization, negateBotch, ignoreFailures);
            var item = RoomBot.MakeRoll((uint)diceCount, (uint)dc, _idRoll++);
            var output = new RollOutput(item, input);   
            return new RollData(output.Result, output.RawDices);
        }

        //RollerEngine.Logger.IRollLogger
        public void Log(Verbosity verbosity, string record)
        {
            if (verbosity > Verbosity.Details) Program.Log(string.Format("RoomBootImpl: {0}: {1}", verbosity, record));
            Color color;
            switch (verbosity)
            {
                case Verbosity.Debug:
                    color = Color.Gray;
                    break;
                case Verbosity.Details:
                    color = Color.Black;
                    break;
                case Verbosity.Important:
                    color = Color.Teal;
                    break;
                case Verbosity.Critical:
                    color = Color.Blue;
                    break;
                case Verbosity.Warning:
                    color = Color.Pink;
                    break;
                case Verbosity.Error:
                    color = Color.Red;
                    break;
                default:
                    color = Color.Green;
                    break;
            }
            RoomBot.MakeMessage(color, record);
        }

    }

  internal static class RoomBot
  {
    private static Thread _thread;
    private static IRolzOrg _browser;
    private static IFormUpdate _updater;
    private static int _ticks;
    private static ConcurrentQueue<uint> _actionQueue;
    private static ConcurrentQueue<string> _messageQueue;

    public static Parser Parser = new Parser(DefaultRoomName);
    private const string DefaultRoomName = "Hatys%20Test7";

        private static void Worker()
        {
          while (!_browser.RoomEntered()) Thread.Sleep(100);
          Program.Log("Worker: Inside room");

          //Establish session
          var sessId = new Random(unchecked((int) DateTime.Now.Ticks));
          var connectionMessage = "establishing session #" + sessId.Next() + "..."; //TODO Format
          var item = MakeSingleMessage(Color.Purple, connectionMessage);
          Program.Log("Worker: session was established");
          Parser.SetSessionTime(item.time);

          MakeCommand("/opt autoexpand=on");
          MakeCommand("/nick HatysBot");

          //InitializeKrivda(IRoomBot)
          //EmulateKrivda()

          var interfaces = new RoomBootImpl();
          var res = HatysParty.LoadFromGoogle(interfaces, interfaces);

          uint action;
          for (;;)
            while (_actionQueue.TryDequeue(out action))
            {
              Thread.Sleep(100);
              Program.Log("Worker: Deque action #" + action); //TODO Format
                res.DoWeek(1);
                res.DoWeek(2);
              MakeMessage(Color.Red, "=== END OF ACTION ===");
            }
        }

        private static readonly char[] LineSeparators = { '\n', '\r' };
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
          foreach (var el in lines)
          {
              //fourth stage: remove starting / ending white chars
              var line = el.Trim();
              //fifth stage: remove starting spaces and special characters for commands and rolls
              line = line.TrimStart(' ', '/', '#', '-');
              if (line != el) Program.Log("Bot: replaced '" + el + "' with '" + line + "'"); //TODO Format
              MakeSingleMessage(color, line);
          }
        }

        private static Item MakeSingleMessage(Color color, string message)
        {
            var attempt = 0;
            for (; ; )
            {
                //repeat message each 5 secs
                if (0 == attempt % 50)
                {
                    QueueColorMessage(color, message);
                }

                var item = Parser.MatchMessage(message);
                if (null != item)
                {
                  Thread.Sleep(100);
                  return item;
                }
                attempt++;
                Thread.Sleep(100);
            }
        }

        public static Item MakeRoll(uint diceCount, uint dc, uint rollId)
        {
            var rollmsg = "#" + diceCount + "d10f" + dc + " #" + Parser.RollIdToComment(rollId); //TODO Format
            var attempt = 0;
            for (; ; )
            {
                //repeat message each 5 secs
                if (0 == attempt % 50)
                {
                    QueueMessage(rollmsg);
                }

                var item = Parser.MatchRoll(rollId);
                if (null != item)
                {
                    Thread.Sleep(100);
                    return item;
                }
                attempt++;
                Thread.Sleep(100);
            }
        }

        private static void MakeCommand(string command)
        {
            QueueMessage(command);
            Thread.Sleep(300);
        }

        private static readonly string[] ColorPrefix = { "", "red:", "green:", "blue:", "gray:", "maroon:", "olive:", "orange:", "purple:", "teal:", "pink:" };
        private static void QueueColorMessage(Color color, string message)
        {
            QueueMessage(ColorPrefix[(int) color] + message);
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

        public static void OnGuiAction(uint action)
        {
            Program.Log("Bot: added to queue action=" + action); //TODO Format
            _actionQueue.Enqueue(action);
        }

        public static void OnGuiStarted(IRolzOrg browser, IFormUpdate updater)
        {
            _thread = new Thread(Worker) {IsBackground = true};
            _browser = browser;
            _updater = updater;
            _actionQueue = new ConcurrentQueue<uint>();
            _messageQueue = new ConcurrentQueue<string>();
            _browser.JoinRoom(DefaultRoomName);
            _thread.Start();
        }

    }
}
