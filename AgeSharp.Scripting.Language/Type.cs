using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public abstract class Type(string name) : Validated
    {
        public string Name { get; } = name;
        public abstract int Size { get; }

        public void ValidateAssignmentFrom(Type from)
        {
            if (this == from)
            {
                return;
            }
            else if (this == PrimitiveType.Int && from == PrimitiveType.Bool)
            {
                return;
            }

            if (this is RefType rtt)
            {
                if (rtt.ReferencedType == from)
                {
                    return;
                }

                if (rtt.ReferencedType is ArrayType at)
                {
                    if (from is ArrayType af)
                    {
                        if (at.ElementType == af.ElementType)
                        {
                            return;
                        }
                    }

                    if (from is RefType rtf)
                    {
                        if (rtf.ReferencedType is ArrayType raf)
                        {
                            if (at.ElementType == raf.ElementType)
                            {
                                return;
                            }
                        }
                    }
                }
            }
            else if (from is RefType rtf)
            {
                if (this == rtf.ReferencedType)
                {
                    return;
                }
            }

            throw new NotSupportedException($"Can not assign type {from.Name} to type {Name}.");
        }
    }
}
