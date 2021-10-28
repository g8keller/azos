﻿/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using Azos.Data;

namespace Azos.Conf.Forest
{
  /// <summary>
  /// Defines constraints for config forest such as data min/max length etc.
  /// </summary>
  public static class Constraints
  {
    public const string ID_NS_CONFIG_FOREST_PREFIX = "az-cforest-";

    public static readonly Atom SCH_PATH = Atom.Encode("path");
    public static readonly Atom SCH_GNODE = Atom.Encode("gnode");
    public static readonly Atom SCH_GVER = Atom.Encode("gver");

    public const int PATH_SEGMENT_MAX_COUNT = 0xff;
    public const int SEGMENT_MIN_LEN = 1;
    public const int SEGMENT_MAX_LEN = 64;

    public const int CONFIG_MIN_LEN = 6; // {r:{}}
    public const int CONFIG_MAX_LEN = 512 * 1024;

#warning THIS IS TENTATIVE NEED DESIGN!!!!!!!!!!!!!!!!!!!!!!!!  Why not require at least one path segment??? Why is this needed?
#warning Reserve GDID for root node 0:0:1 (may not be Gdid.ZERO) as special GDID for very root node and treat it as a special case
    /// <summary>
    /// The name of the very root path segment.
    /// Example: a path `us/oh` or equivalent `/us/oh` really has 3 levels: [`/`, `us`, `oh`] where the
    /// very first level is implicit un-named root.
    /// Note: the only time when TreeNode instances with G_Parent = null are allowed is  when their path segment is `/`.
    /// There can be only one
    /// </summary>
    public const string VERY_ROOT_PATH_SEGMENT = "/";

    /// <summary>
    /// The very root node GDID is hard-coded
    /// </summary>
    public static readonly GDID G_VERY_ROOT_NODE = new GDID(0, 1);//0:0:1

    /// <summary>
    /// Returns true if the id is of `gnode` address schema
    /// </summary>
    public static bool IsGNode(this EntityId id) => id.Schema == SCH_GNODE;

    /// <summary>
    /// Returns true if the id is of `gver` address schema
    /// </summary>
    public static bool IsGVersion(this EntityId id) => id.Schema == SCH_GVER;

    /// <summary>
    /// Returns true if the id is of `path` address schema
    /// having absence of schema specification treated as being equivalent to path
    /// </summary>
    public static bool IsPath(this EntityId id) => id.Schema == SCH_PATH || id.Schema.IsZero;

  }
}
