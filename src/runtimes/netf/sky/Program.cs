﻿/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Collections.Generic;
using System.Linq;

using Azos;
using Azos.Platform.Process;
using Azos.Serialization.JSON;

namespace sky
{
  class Program
  {
    static void Main(string[] args)
    {
      new Azos.Platform.Abstraction.NetFramework.DotNetFrameworkRuntime();
      try
      {
        var activator = new ProgramBodyActivator(args);
        var allPrograms = activator.GetAllPrograms().ToArray();

        var wasRun = activator.Run();

        var toRun = allPrograms.FirstOrDefault(p => activator.ProcessName.IsOneOf(p.bodyAttr.Names));
        if (toRun == null)
        {
          printAllPrograms();
        }

      }
      catch(Exception error)
      {
        var doc = new WrappedExceptionData(error);
        Console.WriteLine(error.ToMessageWithType());
        Console.WriteLine();
        Console.WriteLine(doc.ToJson(JsonWritingOptions.PrettyPrintRowsAsMapASCII));
        Environment.ExitCode = -1;
      }
    }
  }
}
