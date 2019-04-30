using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.Business.Samples
{
    public sealed class TestEntity
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public bool AreYouOK { get; set; }

        public DateTime CurrentTime { get; set; }
    }
}