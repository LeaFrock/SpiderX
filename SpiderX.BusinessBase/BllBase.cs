namespace SpiderX.BusinessBase
{
    public abstract class BllBase
    {
        private string _className;

        public string ClassName
        {
            get
            {
                if (_className == null)
                {
                    _className = GetType().Name;
                }
                return _className;
            }
        }

        public abstract void Run(params string[] args);

        public virtual void Run()
        {
        }
    }
}