using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.Business.LaGou
{
    public sealed partial class LaGouBll
    {
        private abstract class SchemeBase
        {
            public CollectorBase Collector { get; set; }

            public abstract void Run();
        }
    }
}