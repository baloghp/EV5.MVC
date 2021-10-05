using System.Collections.Generic;

namespace EV5.Mvc.ViewEngine
{
    public interface ISection
    {
        IList<string> Contents { get; }
        string Name { get; set; }
        bool RenderInstead { get; set; }
    }
}