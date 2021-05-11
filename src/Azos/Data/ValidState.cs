/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;

namespace Azos.Data
{
  /// <summary>
  /// Defines mode of validation error generation: whether the system should stop validation on the
  /// very first error (if any), or should continue validating generating more errors (which can take longer)
  /// </summary>
  public enum ValidErrorMode : byte
  {
    /// <summary>
    /// Validations trips on a first validation error found. This is the fastest method but
    /// the caller would only get one error at a time
    /// </summary>
    Single = 0,

    /// <summary>
    /// The system would try to validate more items (such as fields) at once even if validation errors
    /// have been triggered, as long as these validation calls do not slow the system down significantly (e.g. no external calls).
    /// The definition of 'fast' is up to a concrete implementation as this flag is just a hint, for example: in a
    /// complex custom validation routine this flag should prevent making long IO calls
    /// </summary>
    FastBatch = 1,

    /// <summary>
    /// The system will try to validate as many items as it can in the presence of validation failures,
    /// regardless of possible performance issues
    /// </summary>
    Batch = 2
  }


  /// <summary>
  /// Encapsulates validation state passed around between `Validate(state)` calls.
  /// This is an immutable struct by design and one changes the state by allocating a new instance, deriving
  /// its current state from an existing one in a functional style: `state = new ValidState(state, error)`.
  /// This is done for performance (avoids extra object allocation) and simplicity.
  /// The Error property contains either a single validation error or an error batch which contains multiple error instances
  /// produced while validating different parts of data document graph.
  /// </summary>
  /// <remarks>
  /// The system is purposely built not to create extra object instances for every validate call, as sometimes
  /// we need to validate hundreds of thousands of doc instances a second in high load applications.
  /// The errors are created and returned, but not thrown as throw is a very expensive operation.
  /// Under normal conditions, validation does not find any errors and does not cause any extra allocation just for
  /// validation that would have been required had a class been used instead
  /// </remarks>
  public struct ValidState
  {
    public const int DEFAULT_ERROR_LIMIT = 50;
    public const int MAX_ERROR_LIMIT = 7_000;

    /// <summary>
    /// Allocates new validation state without errors.
    /// If validation succeeds the Validate method succession shall return a copy of this state
    /// </summary>
    /// <param name="targetName">The target name to perform validation under. Controls attribute/metadata selection</param>
    /// <param name="mode">A hint specifying whether the system shall break on the first error or continue with error batch</param>
    /// <param name="errorLimit">Sets an approximate limit for total error count generated by validation in batch mode</param>
    /// <param name="context">Passes an optional business-centric context into validation chain</param>
    public ValidState(string targetName, ValidErrorMode mode, int errorLimit = DEFAULT_ERROR_LIMIT, object context = null)
    {
      TargetName = targetName ?? TargetedAttribute.ANY_TARGET;
      Mode = mode;
      Error = null;
      ErrorLimit = IntUtils.MinMax(1, errorLimit, MAX_ERROR_LIMIT);
      Context = context;//may be null
    }

    /// <summary>
    /// Allocates a new validation state with the specified error added to the existing state
    /// </summary>
    /// <param name="existing">Existing context state that the new state will be based on</param>
    /// <param name="error">An error to return</param>
    public ValidState(ValidState existing, Exception error)
    {
      if (!existing.IsAssigned)
        throw new CallGuardException(nameof(ValidState)+".ctor", nameof(existing), "Unassigned");

      TargetName = existing.TargetName;
      Mode = existing.Mode;
      Error = ValidationBatchException.Concatenate(existing.Error, error);
      ErrorLimit = existing.ErrorLimit;
      Context = existing.Context;
    }

    /// <summary>
    /// Allocates a new validation state with a different context value
    /// </summary>
    /// <param name="existing">Existing context state that the new state will be based on</param>
    /// <param name="context">Passes an optional business-centric context into validation chain</param>
    public ValidState(ValidState existing, object context)
    {
      if (!existing.IsAssigned)
        throw new CallGuardException(nameof(ValidState) + ".ctor", nameof(existing), "Unassigned");

      TargetName = existing.TargetName;
      Mode = existing.Mode;
      Error = existing.Error;
      ErrorLimit = existing.ErrorLimit;
      Context = context;
    }

    /// <summary>
    /// The target name of the validation to be performed against. This controls metadata applicable to specific
    /// validation
    /// </summary>
    public readonly string TargetName;

    /// <summary>
    /// Describes the mode of validation: Single error vs Error Batch
    /// </summary>
    public readonly ValidErrorMode Mode;

    /// <summary>
    /// Returns validation error or errors. Returns null if there are no validation errors
    /// </summary>
    public readonly Exception Error;

    /// <summary>
    /// Sets an approximate limit for total error count generated by Validate()
    /// </summary>
    public readonly int ErrorLimit;

    /// <summary>
    /// Passes an optional business-centric context into validation chain
    /// </summary>
    public readonly object Context;

    /// <summary>
    /// True if this is NOT a default instance which was created using parameterized .ctor
    /// </summary>
    public bool IsAssigned => TargetName != null;

    /// <summary>
    /// There are validation errors in this state object
    /// </summary>
    public bool HasErrors => Error != null;

    /// <summary>
    /// There are no validation errors in this state object
    /// </summary>
    public bool NoErrors => Error == null;

    /// <summary>
    /// Returns true when validation should continue that is:
    ///  when no errors have been detected OR mode is set to batch and total error count is below the ErrorLimit
    /// </summary>
    public bool ShouldContinue => NoErrors || (Mode > ValidErrorMode.Single && ErrorCount < ErrorLimit);

    /// <summary>
    /// An inverse of ShouldContinue: validation shall stop when there is an error in an single mode, or mode is batch and total
    /// error count exceeds or equal to the ErrorLimit
    /// </summary>
    public bool ShouldStop => !ShouldContinue;

    /// <summary>
    /// Error count batched in the state instance
    /// </summary>
    public int ErrorCount => Error==null ? 0 : Error is ValidationBatchException vbe ? vbe.Batch.Count : 1;

    public override string ToString() => $"ValState(`{TargetName}`)";

    /// <summary>
    /// Performs validation checks one by one returning as soon as the validation should stop.
    /// This is a syntax sugar not to write `if (state.ShouldStop()) return state;` every time
    /// </summary>
    public ValidState Of(Func<ValidState, ValidState> f1,
                         Func<ValidState, ValidState> f2,
                         Func<ValidState, ValidState> f3 = null,
                         Func<ValidState, ValidState> f4 = null,
                         Func<ValidState, ValidState> f5 = null,
                         Func<ValidState, ValidState> f6 = null,
                         Func<ValidState, ValidState> f7 = null,
                         Func<ValidState, ValidState> f8 = null)
    {
      var state = this;

      if (f1 != null) { state = f1(state); if (state.ShouldStop) return state; }
      if (f2 != null) { state = f2(state); if (state.ShouldStop) return state; }
      if (f3 != null) { state = f3(state); if (state.ShouldStop) return state; }
      if (f4 != null) { state = f4(state); if (state.ShouldStop) return state; }
      if (f5 != null) { state = f5(state); if (state.ShouldStop) return state; }
      if (f6 != null) { state = f6(state); if (state.ShouldStop) return state; }
      if (f7 != null) { state = f7(state); if (state.ShouldStop) return state; }
      if (f8 != null) state = f8(state);

      return state;
    }

    /// <summary>
    /// Performs validation checks one by one returning as soon as the validation should stop.
    /// This is a syntax sugar not to write `if (state.ShouldStop()) return state;` every time.
    /// This version of the function passes a TContext value between calls
    /// </summary>
    public ValidState Of<TContext>(TContext ctx,
                                   Func<TContext, ValidState, ValidState> f1,
                                   Func<TContext, ValidState, ValidState> f2,
                                   Func<TContext, ValidState, ValidState> f3 = null,
                                   Func<TContext, ValidState, ValidState> f4 = null,
                                   Func<TContext, ValidState, ValidState> f5 = null,
                                   Func<TContext, ValidState, ValidState> f6 = null,
                                   Func<TContext, ValidState, ValidState> f7 = null,
                                   Func<TContext, ValidState, ValidState> f8 = null)
    {
      var state = this;

      if (f1 != null) { state = f1(ctx, state); if (state.ShouldStop) return state; }
      if (f2 != null) { state = f2(ctx, state); if (state.ShouldStop) return state; }
      if (f3 != null) { state = f3(ctx, state); if (state.ShouldStop) return state; }
      if (f4 != null) { state = f4(ctx, state); if (state.ShouldStop) return state; }
      if (f5 != null) { state = f5(ctx, state); if (state.ShouldStop) return state; }
      if (f6 != null) { state = f6(ctx, state); if (state.ShouldStop) return state; }
      if (f7 != null) { state = f7(ctx, state); if (state.ShouldStop) return state; }
      if (f8 != null) state = f8(ctx, state);

      return state;
    }
  }

}
