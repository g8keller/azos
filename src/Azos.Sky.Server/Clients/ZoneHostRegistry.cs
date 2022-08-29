/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/
//Generated by Azos.Sky.Clients.Tools.SkyGluecCompiler

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Azos.Glue;
using Azos.Glue.Protocol;


namespace Azos.Sky.Clients
{
// This implementation needs @Azos.@Sky.@Contracts.@IZoneHostRegistryClient, so
// it can be used with ServiceClientHub class

  ///<summary>
  /// Client for glued contract Azos.Sky.Contracts.IZoneHostRegistry server.
  /// Each contract method has synchronous and asynchronous versions, the later denoted by 'Async_' prefix.
  /// May inject client-level inspectors here like so:
  ///   client.MsgInspectors.Register( new YOUR_CLIENT_INSPECTOR_TYPE());
  ///</summary>
  public class ZoneHostRegistry : ClientEndPoint, @Azos.@Sky.@Contracts.@IZoneHostRegistryClient
  {

  #region Static Members

     private static TypeSpec s_ts_CONTRACT;
     private static MethodSpec @s_ms_GetSubordinateHosts_0;
     private static MethodSpec @s_ms_GetSubordinateHost_1;
     private static MethodSpec @s_ms_RegisterSubordinateHost_2;
     private static MethodSpec @s_ms_Spawn_3;

     //static .ctor
     static ZoneHostRegistry()
     {
         var t = typeof(@Azos.@Sky.@Contracts.@IZoneHostRegistry);
         s_ts_CONTRACT = new TypeSpec(t);
         @s_ms_GetSubordinateHosts_0 = new MethodSpec(t.GetMethod("GetSubordinateHosts", new Type[]{ typeof(@System.@String) }));
         @s_ms_GetSubordinateHost_1 = new MethodSpec(t.GetMethod("GetSubordinateHost", new Type[]{ typeof(@System.@String) }));
         @s_ms_RegisterSubordinateHost_2 = new MethodSpec(t.GetMethod("RegisterSubordinateHost", new Type[]{ typeof(@Azos.@Sky.@Contracts.@HostInfo), typeof(@System.@Nullable<@Azos.@Sky.@Contracts.@DynamicHostID>) }));
         @s_ms_Spawn_3 = new MethodSpec(t.GetMethod("Spawn", new Type[]{ typeof(@System.@String), typeof(@System.@String) }));
     }
  #endregion

  #region .ctor
     public ZoneHostRegistry(IGlue glue, string node, Binding binding = null) : base(glue, node, binding) { ctor(); }
     public ZoneHostRegistry(IGlue glue, Node node, Binding binding = null) : base(glue, node, binding) { ctor(); }

     //common instance .ctor body
     private void ctor()
     {

     }

  #endregion

     public override Type Contract
     {
       get { return typeof(@Azos.@Sky.@Contracts.@IZoneHostRegistry); }
     }



  #region Contract Methods

         ///<summary>
         /// Synchronous invoker for  'Azos.Sky.Contracts.IZoneHostRegistry.GetSubordinateHosts'.
         /// This is a two-way call per contract specification, meaning - the server sends the result back either
         ///  returning '@System.@Collections.@Generic.@IEnumerable<@Azos.@Sky.@Contracts.@HostInfo>' or WrappedExceptionData instance.
         /// ClientCallException is thrown if the call could not be placed in the outgoing queue.
         /// RemoteException is thrown if the server generated exception during method execution.
         ///</summary>
         public @System.@Collections.@Generic.@IEnumerable<@Azos.@Sky.@Contracts.@HostInfo> @GetSubordinateHosts(@System.@String  @hostNameSearchPattern)
         {
            var call = Async_GetSubordinateHosts(@hostNameSearchPattern);
            return call.GetValue<@System.@Collections.@Generic.@IEnumerable<@Azos.@Sky.@Contracts.@HostInfo>>();
         }

         ///<summary>
         /// Asynchronous invoker for  'Azos.Sky.Contracts.IZoneHostRegistry.GetSubordinateHosts'.
         /// This is a two-way call per contract specification, meaning - the server sends the result back either
         ///  returning no exception or WrappedExceptionData instance.
         /// CallSlot is returned that can be queried for CallStatus, ResponseMsg and result.
         ///</summary>
         public CallSlot Async_GetSubordinateHosts(@System.@String  @hostNameSearchPattern)
         {
            var request = new RequestAnyMsg(s_ts_CONTRACT, @s_ms_GetSubordinateHosts_0, false, RemoteInstance, new object[]{@hostNameSearchPattern});
            return DispatchCall(request);
         }



         ///<summary>
         /// Synchronous invoker for  'Azos.Sky.Contracts.IZoneHostRegistry.GetSubordinateHost'.
         /// This is a two-way call per contract specification, meaning - the server sends the result back either
         ///  returning '@Azos.@Sky.@Contracts.@HostInfo' or WrappedExceptionData instance.
         /// ClientCallException is thrown if the call could not be placed in the outgoing queue.
         /// RemoteException is thrown if the server generated exception during method execution.
         ///</summary>
         public @Azos.@Sky.@Contracts.@HostInfo @GetSubordinateHost(@System.@String  @hostName)
         {
            var call = Async_GetSubordinateHost(@hostName);
            return call.GetValue<@Azos.@Sky.@Contracts.@HostInfo>();
         }

         ///<summary>
         /// Asynchronous invoker for  'Azos.Sky.Contracts.IZoneHostRegistry.GetSubordinateHost'.
         /// This is a two-way call per contract specification, meaning - the server sends the result back either
         ///  returning no exception or WrappedExceptionData instance.
         /// CallSlot is returned that can be queried for CallStatus, ResponseMsg and result.
         ///</summary>
         public CallSlot Async_GetSubordinateHost(@System.@String  @hostName)
         {
            var request = new RequestAnyMsg(s_ts_CONTRACT, @s_ms_GetSubordinateHost_1, false, RemoteInstance, new object[]{@hostName});
            return DispatchCall(request);
         }



         ///<summary>
         /// Synchronous invoker for  'Azos.Sky.Contracts.IZoneHostRegistry.RegisterSubordinateHost'.
         /// This is a two-way call per contract specification, meaning - the server sends the result back either
         ///  returning no exception or WrappedExceptionData instance.
         /// ClientCallException is thrown if the call could not be placed in the outgoing queue.
         /// RemoteException is thrown if the server generated exception during method execution.
         ///</summary>
         public void @RegisterSubordinateHost(@Azos.@Sky.@Contracts.@HostInfo  @host, @System.@Nullable<@Azos.@Sky.@Contracts.@DynamicHostID>  @hid)
         {
            var call = Async_RegisterSubordinateHost(@host, @hid);
            call.CheckVoidValue();
         }

         ///<summary>
         /// Asynchronous invoker for  'Azos.Sky.Contracts.IZoneHostRegistry.RegisterSubordinateHost'.
         /// This is a two-way call per contract specification, meaning - the server sends the result back either
         ///  returning no exception or WrappedExceptionData instance.
         /// CallSlot is returned that can be queried for CallStatus, ResponseMsg and result.
         ///</summary>
         public CallSlot Async_RegisterSubordinateHost(@Azos.@Sky.@Contracts.@HostInfo  @host, @System.@Nullable<@Azos.@Sky.@Contracts.@DynamicHostID>  @hid)
         {
            var request = new RequestAnyMsg(s_ts_CONTRACT, @s_ms_RegisterSubordinateHost_2, false, RemoteInstance, new object[]{@host, @hid});
            return DispatchCall(request);
         }



         ///<summary>
         /// Synchronous invoker for  'Azos.Sky.Contracts.IZoneHostRegistry.Spawn'.
         /// This is a two-way call per contract specification, meaning - the server sends the result back either
         ///  returning '@Azos.@Sky.@Contracts.@DynamicHostID' or WrappedExceptionData instance.
         /// ClientCallException is thrown if the call could not be placed in the outgoing queue.
         /// RemoteException is thrown if the server generated exception during method execution.
         ///</summary>
         public @Azos.@Sky.@Contracts.@DynamicHostID @Spawn(@System.@String  @hostPath, @System.@String  @id)
         {
            var call = Async_Spawn(@hostPath, @id);
            return call.GetValue<@Azos.@Sky.@Contracts.@DynamicHostID>();
         }

         ///<summary>
         /// Asynchronous invoker for  'Azos.Sky.Contracts.IZoneHostRegistry.Spawn'.
         /// This is a two-way call per contract specification, meaning - the server sends the result back either
         ///  returning no exception or WrappedExceptionData instance.
         /// CallSlot is returned that can be queried for CallStatus, ResponseMsg and result.
         ///</summary>
         public CallSlot Async_Spawn(@System.@String  @hostPath, @System.@String  @id)
         {
            var request = new RequestAnyMsg(s_ts_CONTRACT, @s_ms_Spawn_3, false, RemoteInstance, new object[]{@hostPath, @id});
            return DispatchCall(request);
         }


  #endregion

  }//class
}//namespace