﻿/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System.Collections.Generic;
using Azos.Serialization.Bix;
using Azos.Serialization.JSON;

namespace Azos.Data.Heap
{
  /// <summary>
  /// Provides a base for writing commands sent into heap area.
  /// A Query is a data document having its type represent an action akin to "stored procedure"
  /// and its fields represent query parameters.
  /// A system may support a queries that take <see cref="AST.Expression"/> which
  /// is a similar concept to GraphQL where a query "shapes" data.
  /// </summary>

  //todo: Need area:collection resolver
  //[BixJsonHandler]
  public abstract class AreaQuery : TypedDoc
  {
    protected override void AddJsonSerializerField(Schema.FieldDef def, JsonWritingOptions options, Dictionary<string, object> jsonMap, string name, object value)
    {
      if (def?.Order == 0)
      {
        //todo: Need area:collection resolver
        //  BixJsonHandler.EmitJsonBixDiscriminator(this, jsonMap);
      }

      base.AddJsonSerializerField(def, options, jsonMap, name, value);
    }
  }

}
