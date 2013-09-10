using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pulsus
{
    public class Ignore
    {
        public virtual LoggingEventLevel MinLevel { get; set; }
        public virtual LoggingEventLevel MaxLevel { get; set; }
        public virtual LoggingEventLevel Level { get; set; }
        public virtual string LogKeyContains { get; set; }
        public virtual string LogKeyStartsWith { get; set; }
        public virtual string TextContains { get; set; }
        public virtual string TextStartsWith { get; set; }
        public virtual string TagsContains { get; set; }
        public virtual double? MinValue { get; set; }
        public virtual double? MaxValue { get; set; }
    }
}
