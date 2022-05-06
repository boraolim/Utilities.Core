using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

using Utilities;

using TestUtilitiesLibrary;

namespace TestUtilities
{
    public class App
    {
        private readonly IInterfaceSample _interfaceSample1;
        public App(IInterfaceSample interfaceSample) => _interfaceSample1 = interfaceSample;

        public async Task RunAsync(string[] args) => await _interfaceSample1.DoActionSomethingAsync();
    }
}
