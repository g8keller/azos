﻿/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azos.Collections;
using Azos.Conf;
using Azos.Serialization.JSON;

namespace Azos.Scripting.Steps
{
  /// <summary>
  /// Facilitates invocation of C# Steps from a script file in sequence.
  /// You can extend this class to supply extra use-case context-specific fields/props.
  /// This class is not thread-safe by design
  /// </summary>
  public class StepRunner
  {
    public const string CONFIG_STEP_SECTION = "step";

    public StepRunner(IApplication app, IConfigSectionNode rootSource)
    {
      m_App = app.NonNull(nameof(app));
      m_RootSource = rootSource.NonEmpty(nameof(rootSource));
      ConfigAttribute.Apply(this, m_RootSource);
    }

    private IApplication m_App;
    private JsonDataMap m_GlobalState = new JsonDataMap(true);
    private IConfigSectionNode m_RootSource;

    /// <summary>
    /// Application context that this runner operates under
    /// </summary>
    public IApplication App => m_App;

    /// <summary>
    /// Defines log Level
    /// </summary>
    [Config]
    public virtual Azos.Log.MessageType LogLevel {  get; set; }

    /// <summary>
    /// Runner global state, does not get reset between runs (unless you re-set it by step)
    /// </summary>
    public JsonDataMap GlobalState => m_GlobalState;

    /// <summary>
    /// Executes the whole script. The <see cref="GlobalState"/> is NOT cleared automatically
    /// </summary>
    public virtual void Run()
    {
      Exception error = null;
      JsonDataMap state = null;
      try
      {
        state = DoBeforeRun();

        OrderedRegistry<Step> script = new OrderedRegistry<Step>();

        Steps.ForEach(s => script.Register(s).IsTrue($"Duplicate step `{s.Name}`"));

        for(var ip=0; ip < script.Count;)
        {
          var step = script[ip];
          var nextName = step.Run(state);
          if (nextName.IsNullOrWhiteSpace())
          {
            ip++;
          }
          else
          {
            var next = script[nextName];
            next.NonNull($"Step not found: `{nextName}`");
            ip = next.Order;
          }
        }

      }
      catch(Exception cause)
      {
        error = cause;
      }

      var handled = DoAfterRun(error, state);
      if (!handled && error!=null) throw error;
    }

    /// <summary>
    /// Invoked before steps, makes run state instance.
    /// Default implementation makes case-sensitive state bag
    /// </summary>
    protected virtual JsonDataMap DoBeforeRun()
    {
      return new JsonDataMap(true);
    }

    /// <summary>
    /// Invoked after all steps are run, if error is present it is set and return true if
    /// you handle the error yourself, otherwise return false for default processing
    /// </summary>
    protected virtual bool DoAfterRun(Exception error, JsonDataMap state)
    {
      return false;
    }

    /// <summary>
    /// Returns all runnable steps, default implementation returns all sections named "STEP"
    /// in their declaration syntax
    /// </summary>
    public virtual IEnumerable<IConfigSectionNode> StepSections
      => m_RootSource.ChildrenNamed(CONFIG_STEP_SECTION);

    /// <summary>
    /// Returns materialized steps of <see cref="StepSections"/>
    /// </summary>
    public virtual IEnumerable<Step> Steps
    {
      get
      {
        var i = 0;
        foreach(var nstep in StepSections)
        {
          var step = FactoryUtils.Make<Step>(nstep, null, new object[] { this, nstep, i });
          yield return step;
          i++;
        }
      }
    }

    /// <summary>
    /// Writes a log message for this runner; returns the new log msg GDID for correlation, or GDID.Empty if no message was logged.
    /// The file/src are only used if `from` is null/blank
    /// </summary>
    protected internal virtual Guid WriteLog(Azos.Log.MessageType type,
                                               string from,
                                               string text,
                                               Exception error = null,
                                               Guid? related = null,
                                               string pars = null,
                                               [System.Runtime.CompilerServices.CallerFilePath]string file = null,
                                               [System.Runtime.CompilerServices.CallerLineNumber]int src = 0)
    {
      if (type < LogLevel) return Guid.Empty;

      if (from.IsNullOrWhiteSpace())
        from = "{0}:{1}".Args(file.IsNotNullOrWhiteSpace() ? System.IO.Path.GetFileName(file) : "?", src);

      var msg = new Azos.Log.Message
      {
        App = App.AppId,
        Topic = CoreConsts.RUN_TOPIC,
        From = "{0}.{1}".Args(GetType().DisplayNameWithExpandedGenericArgs(), from),
        Type = type,
        Text = text,
        Exception = error,
        Parameters = pars,
        Source = src
      };

      msg.InitDefaultFields(App);

      if (related.HasValue) msg.RelatedTo = related.Value;

      App.Log.Write(msg);

      return msg.Guid;
    }

  }
}