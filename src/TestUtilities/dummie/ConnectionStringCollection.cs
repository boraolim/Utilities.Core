using System;
using System.Collections.Generic;
using System.Text;

namespace TestUtilities
{
  [Serializable]
  public class ConnectionStringCollection
  {
    public string ConnectionDbFirst { get; set; }
    public string ConnectionDbSecond { get; set; }
  }
}
