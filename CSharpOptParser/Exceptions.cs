using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpOptParser
{
    /// <summary>
    /// @author strnadj 
    /// </summary>
    public class MissingOptions : Exception
    {
        public MissingOptions(string msg)
            : base(msg)
        {
        }
    }

    /// <summary>
    /// @author strnadj
    /// </summary>
    public class MissingOptionsHelp : Exception
    {
        public MissingOptionsHelp(string msg)
            : base(msg)
        {
        }
    }

    /// <summary>
    /// @author strnadj
    /// </summary>
    public class MissingOptionValue : Exception
    {
        public MissingOptionValue(string msg)
            : base(msg)
        {
        }
    }

    /// <summary>
    /// Exception class - overlaping brackets
    /// </summary>
    public class OverlapingBracketsException : Exception
    {
        public OverlapingBracketsException(string error)
            : base(error)
        {
        }
    }

    /// <summary>
    /// @author strnadj
    /// </summary>
    public class UnexpectedOption : Exception
    {
        public UnexpectedOption(string msg)
            : base(msg)
        {
        }
    }

    /// <summary>
    /// @author strnadj
    /// </summary>
    public class UnknownAttribute : Exception
    {
        public UnknownAttribute(string msg)
            : base(msg)
        {
        }
    }
}
