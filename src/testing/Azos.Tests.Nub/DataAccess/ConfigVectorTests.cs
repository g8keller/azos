﻿/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Collections.Generic;
using System.Text;
using Azos.Conf;
using Azos.Data;
using Azos.Scripting;
using Azos.Serialization.JSON;

namespace Azos.Tests.Nub.DataAccess
{
  [Runnable]
  public class ConfigVectorTests
  {
    [Run]
    public void Test_Implicit()
    {
      ConfigVector v = "{r: {a:1, b:2}}";
      Aver.AreEqual(1, v.Node.Of("a").ValueAsInt());
      Aver.AreEqual(2, v.Node.Of("b").ValueAsInt());
    }

    public class Tezt : TypedDoc
    {
      [Field(required: true)]
      public ConfigVector C1{ get; set;}

      [Field(maxLength: 1024)]
      public ConfigVector C2 { get; set; }
    }

    [Run]
    public void Test_Roundtrip()
    {
      var d = new Tezt
      {
        C1 = "{r: {a:1, b:2}}",
        C2 = null
      };

      Aver.IsNull(d.Validate());

      var json = JsonWriter.Write(d, JsonWritingOptions.PrettyPrintRowsAsMap);

      json.See();

      var got = JsonReader.ToDoc<Tezt>(json);

      Aver.AreEqual(1, got.C1.Node.Of("a").ValueAsInt());
      Aver.AreEqual(2, got.C1.Node.Of("b").ValueAsInt());
    }

    [Run]
    [Aver.Throws(typeof(ConfigException))]
    public void Test_Malformed()
    {
      var json = "{C1: 'crap'}";

      var got = JsonReader.ToDoc<Tezt>(json);

      Aver.AreEqual("crap", got.C1.Content);

      got.C1.Node.Of("a");
    }

  }
}