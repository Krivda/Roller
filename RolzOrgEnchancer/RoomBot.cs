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

    public static class VerbosityHelper
    {
        public static Color Verbosity2Color(Verbosity verbosity)
        {
            return (Color)verbosity;
        }

    }

    internal class RoomBootImpl : ILogWrapper<object>, IRoller
    {
        public int Week { get; set; }
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

        public object CreateChannelLogger(ActivityChannel channel)
        {
            if (channel != ActivityChannel.Main) return null;
            return new object();
        }

        public void AppendInternalLog(object logger, Verbosity verbosity, string record)
        {
            if (logger != null) Log(verbosity, record);
        }

        public string GetInternalLog(object logger)
        {
            throw new NotImplementedException();
        }

        public void Log(Verbosity verbosity, string record)
        {
            Color color;
            switch (verbosity)
            {
                case Verbosity.Debug:
                    color = Color.Gray;
                    break;
                case Verbosity.Details:
                    color = Color.Black;
                    break;
                case Verbosity.Normal:
                    color = Color.Orange;
                    break;
                case Verbosity.Important:
                    color = Color.Blue;
                    break;
                case Verbosity.Critical:
                    color = Color.Pink;
                    break;
                case Verbosity.Warning:
                    color = Color.Teal;
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
    private const string DefaultRoomName = "Hatys%20Test8";

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

          var myInterface = new RoomBootImpl();
          var logger = new BaseLogger<ILogWrapper<object>, object>(Verbosity.Details, new List<ActivityChannel>(), myInterface);
          logger.TreatSpecialVerbosityAs(Verbosity.Normal);

          var res = HatysParty.LoadFromGoogle(logger, myInterface);

          uint action;
          for (;;)
              while (_actionQueue.TryDequeue(out action))
              {
                  Thread.Sleep(100);
                  Program.Log("Worker: Deque action #" + action); //TODO Format
                  for (int i = 1; i < 20; i++)
                  {
                      res.DoWeek(i);
                  }
                  res.LogTotalProgress();
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
              var line = el;
              //fourth stage: if starts with === replace = from string
              if (line.StartsWith("===")) line = line.Replace("=", "");
              //fifth stage: remove starting spaces and special characters for commands and rolls
              line = line.TrimStart(' ', '/', '#', '-');
              //last stage: remove ending white chars
              line = line.TrimEnd();
              if (line != el) Program.Log("Bot: replaced '" + el + "' with '" + line + "'"); //TODO Format
              if (line != "") MakeSingleMessage(color, line);
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

        private static readonly string[] ColorPrefix = { "gray:", "", "orange:", "blue:", "pink:", "teal:", "red:", "green:", "purple:", "maroon:", "olive:" };
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
