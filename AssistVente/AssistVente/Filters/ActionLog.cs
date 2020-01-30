using System;

namespace AssistVente.Filters
{
    public class ActionLog
    {
        public string Controller { get; internal set; }
        public string Action { get; internal set; }
        public string IP { get; internal set; }
        public DateTime DateTime { get; internal set; }
        public string User { get; internal set; }
        public Guid ID { get; internal set; }
    }
}