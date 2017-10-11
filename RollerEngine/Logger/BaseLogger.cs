namespace RollerEngine.Logger
{
    public abstract class BaseLogger : IRollLogger
    {
        private Verbosity _minVerbosity;
        private int _week;
        public abstract void Log(Verbosity verbosity, string record);

        public virtual Verbosity MinVerbosity
        {
            get { return _minVerbosity; }
            set { _minVerbosity = value; }
        }

        public virtual int Week
        {
            get { return _week; }
            set { _week = value; }
        }

        protected BaseLogger(Verbosity minVerbosity)
        {
            _minVerbosity = minVerbosity;
            _week = -1;
        }

        public string ApplyFormat(string record)
        {
            var msg = record;
            if (Week != -1)
            {
                msg = string.Format("W{0}: {1}", Week, record).TrimEnd();
            }
            return msg;
        }
    }
}