using AgeSharp.Scripting.Language.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public abstract class Statement : Validated
    {
        public abstract IEnumerable<Block> GetContainedBlocks();
    }
}
