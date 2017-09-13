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
        Red,    
        Green,                          //command
        Blue,   //Verbosity.Important
        Gray,   //Verbosity.Debug
        Maroon,
        Olive,
        Orange, //Roll Description
        Purple, //Session
        Teal,
        Pink    //Verbosity.Warning
    }

    internal class RoomBootImpl : IRollLogger, IRoller
    {
        static uint nRoll = 100;
        //RollerEngine.Roller.IRoller
        public RollData Roll(int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill, string description)
        {
            Program.Log("RoomBootImpl: Rolling description: " + description);
            RoomBot.MakeMessage(Color.Orange, description);

            var input = new RollInput();
            input.Initialize(diceCount, DC, hasSpecialization, hasWill, !removeSuccessOnOnes);
            var item = RoomBot.MakeRoll((uint)diceCount, (uint)DC, nRoll);
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
            var checkR = Convert.ToInt16(item.result); //TODO exception
            if (checkR != output.RawResult)
            {
                throw new Exception("Invalid 6");
            }
            output.CalculateResult(input);
            Program.Log("Result = " + output.Result);
            return new RollData(output.Result, output.RawDices);
        }

        //RollerEngine.Logger.IRollLogger
        public void Log(Verbosity verbosity, string record)
        {
            Program.Log("RoomBootImpl: Logging message: " + verbosity + record);
            Color color;
            switch (verbosity)
            {
                case Verbosity.Details:
                    color = Color.Black;
                    break;
                case Verbosity.Debug:
                    color = Color.Gray;
                    break;
                case Verbosity.Warning:
                    color = Color.Pink;
                    break;
                case Verbosity.Important:
                    color = Color.Blue;
                    break;
                default:
                    color = Color.Red;
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
    private const string DefaultRoomName = "Hatys%20Test2";

    private static void Worker()
    {
      while (!_browser.RoomEntered()) Thread.Sleep(100);
      Program.Log("Worker: Inside room");

      //Establish session
      var sessId = new Random(unchecked((int) DateTime.Now.Ticks));
      var connectionMessage = "establishing session #" + sessId.Next() + "...";
      var item = MakeSingleMessage(Color.Purple, connectionMessage);
      Program.Log("Worker: session was established");
      Parser.SetSessionTime(item.time);

      MakeCommand("/opt autoexpand=on");
      MakeCommand("/nick HatysBot");

      //InitializeKrivda(IRoomBot)
      //EmulateKrivda()

      var interfaces = new RoomBootImpl();
      var res = HatysParty.LoadFromGoogle(interfaces, interfaces);

        var plan = new List<TeachPlan>
        {
            new TeachPlan(res.Nameless, res.Yoki, Build.Abilities.Brawl),
            new TeachPlan(res.Yoki, res.Kurt, Build.Abilities.Rituals),
            new TeachPlan(res.Kinfolk1, res.Kinfolk1, Build.Abilities.Science)
        };

      uint action;
      for (;;)
        while (_actionQueue.TryDequeue(out action))
        {
          Thread.Sleep(100);
          Program.Log("Worker: Deque action #" + action);
            res.TeachingWeek(plan);
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
              if (line != el) Program.Log("Bot: replaced '" + el + "' with '" + line + "'");
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
            var rollmsg = "#" + diceCount + "d10f" + dc + " #" + Parser.RollIdToComment(rollId);
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
            Program.Log("Bot: added to queue action=" + action);
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
