
#pragma warning disable IDE1006 // I use '__' prefix for private methods. Can cause warning.
#nullable enable

namespace Godot
{
  /// <summary> A WeakRef class that is typed and can take Godot and CSharp objects safely. </summary>
  public class WeakRef<T> // : Reference
    where T : class
  {
    private static readonly bool __useGodotWeakPtr = typeof( Godot.Object ).IsAssignableFrom( typeof( T ) );

    private Godot.WeakRef? __gdWeak;
    private System.WeakReference<T>? __stdWeak;

    /// <summary> Create a new WeakRef with no target. </summary>
    public WeakRef() { }
    /// <summary> Create a new WeakRef pointing to the target provided. </summary>
    public WeakRef( T? target ) { if (target != null) { SetTarget( target ); } }
    /// <summary> Implicit conversion CSharp weak reference to this safer version. </summary>
    public static implicit operator WeakRef<T>( System.WeakReference<T> weak )
      => new WeakRef<T>( weak.TryGetTarget( out var strong ) ? strong : null );
    /// <summary> Implicit conversion from Godot weak reference to this typed version. Throws on type mismatch. </summary>
    public static implicit operator WeakRef<T>( Godot.WeakRef weak )
    {
      var strong = weak.GetRef();
      if (strong == null) { return new WeakRef<T>(); }
      if (strong is T t) { return new WeakRef<T>( t ); }
      throw new System.ArgumentException( $"Unable to cast type '{strong.GetType().Name}' to type '{typeof(T).Name}'", nameof( weak ) );
    }

    /// <summary> Sets the target object that is referenced. </summary>
    /// <param name="target"> The new target object. </param>
    public void SetTarget( T? target )
    {
      if (target == null)
      {
        __gdWeak = null;
        __stdWeak = null;
      }
      else if (__useGodotWeakPtr)
      {
        if (!(target is Godot.Object gdObj)) { Godot.GD.PrintErr( "Failed to convert target to gd object." ); return; }
        __gdWeak = Godot.Object.WeakRef( gdObj );
      }
      else
      {
        __stdWeak = new System.WeakReference<T>( target );
      }
    }

    /// <summary> Tries to get the target reference object. </summary>
    /// <returns> The reference object or null if it is dead or unset. </returns>
    public T? GetTargetOrNull()
    {
      if (__useGodotWeakPtr)
      {
        if (__gdWeak != null) { return __gdWeak.GetRef() as T; }
      }
      else
      {
        if (__stdWeak != null && __stdWeak.TryGetTarget( out var strong )) { return strong; }
      }
      return null;
    }

    /// <summary> Tries to get the target reference object. </summary>
    /// <param name="strong">
    ///   When this method returns, contains the target object, if it is available. This
    ///   parameter is treated as uninitialized.
    /// </param>
    /// <returns> true if the target was retrieved; otherwise, false. </returns>
    public bool TryGetTarget( out T strong )
    {
      strong = null!; // Try pattern, null ok
      if (__useGodotWeakPtr)
      {
        if (__gdWeak != null && __gdWeak.GetRef() is T t) { strong = t; return true; }
      }
      else
      {
        if (__stdWeak != null) { return __stdWeak.TryGetTarget( out strong ); }
      }
      return false;
    }
  };
}
