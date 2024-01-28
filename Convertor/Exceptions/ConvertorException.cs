 using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Convertor.Exceptions;

public class ConvertorException : Exception
{
    public ConvertorException()
    {
    }

    public ConvertorException(string? message) : base(message)
    {
    }

    public ConvertorException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
 
}
