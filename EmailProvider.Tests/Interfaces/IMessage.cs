using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailProvider.Tests.Interfaces;

public interface IMessage
{
    string Body { get; }
}
