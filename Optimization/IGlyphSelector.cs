using System.Collections.Generic;
using Mmeg.Model;

namespace Mmeg.Optimization
{
    public interface IGlyphSelector
    {
        IEnumerable<IGlyphSelection> GetGlyphCombination();

        void PropagateValue(IGlyphSelection selection, decimal value);
    }
}