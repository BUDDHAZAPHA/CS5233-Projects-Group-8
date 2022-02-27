using System.Collections.Generic;

namespace O2DESNet
{
    public class FutureEventList : SortedSet<Event>
    {
        public FutureEventList() : base(EventComparer.Instance) { }
    }
}
