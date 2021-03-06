using System;
using System.Runtime.CompilerServices;
using Godot.Collections;
using MonsterVial;
using Array = Godot.Collections.Array;

/* Example Usage:
  // Declare a class with the attribute
  [CSharpScript]
  public class CustomResource : Resource { ... }

  // Later, create new resources with
  CSharpScript<CustomResource>.New()

  // Report issues to the gist at: https://gist.github.com/cgbeutler/c4f00b98d744ac438b84e8840bbe1740
*/

#pragma warning disable IDE1006 // I use '__' prefix for private methods. Can cause warning.
#nullable enable

namespace Godot
{
  [AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = false )]
  public sealed class CSharpScriptAttribute : Attribute
  {
    public CSharpScriptAttribute( [CallerFilePath] string path = "" )
    {
      FilePath = IO.LocalizeProjectPath( path );
    }

    public string FilePath { get; set; }
  };


  public static class CSharpScript<T>
    where T : Object
  {
    public static string FilePath;
    public static string Filename;

    public static string ResourcePath => FilePath;

    static CSharpScript()
    {
      if (Attribute.GetCustomAttribute( typeof( T ), typeof( CSharpScriptAttribute ) ) is CSharpScriptAttribute attr)
      {
        if (attr.FilePath.Empty() || !attr.FilePath.EndsWith( ".cs" ))
        {
          GD.PushError( $"Can't get CShaprScript resource path:  Raw path was empty or didn't end with '.cs'" );
          FilePath = Filename = "";
          return;
        }
        FilePath = attr.FilePath;
        Filename = FilePath.GetFile();
        if (Filename.BaseName() != typeof( T ).Name)
        {
          GD.PushError( $"Class name '{ typeof( T ).Name }' doesn't match filename '{ Filename }'" );
        }
      }
      else
      {
        FilePath = Filename = typeof(T).Name;
        GD.PushError( $"Class '{typeof( T ).Name}' is missing '{nameof( CSharpScriptAttribute )}'." );
      }
    }

    private static WeakRef? __csharpScript; //<CSharpScript>
    /// Get the CSharpScript Godot Resource
    public static CSharpScript GetCSharpScript()
    {
      if (__csharpScript?.GetRef() is CSharpScript scr) { return scr; }
      scr = GD.Load<CSharpScript>( ResourcePath );
      if (scr != null) { __csharpScript = Godot.Object.WeakRef( scr ); }
      else { throw new Exception( "Can't load CSharp Script" ); }
      return scr;
    }

    private static readonly object[] __emptyObj = new object[0];
    /// Returns a new instance of the script.
    public static T New()
    {
      var script = GetCSharpScript();
      try
      {
        var o = script.New();
        var t = (T) o;
        if (IOToolsSettings.CALL_READY_ON_RESOURCES && typeof(Resource).IsAssignableFrom(typeof(T)))
        {
          var readyMethod = typeof(T).GetMethod( "_Ready", Type.EmptyTypes );
          if (readyMethod != null) { readyMethod.Invoke( t, __emptyObj ); }
        }
        return t;
      }
      catch (Exception e)
      {
        GD.PrintErr( "Exception in New(): " + e.ToString() );
        GD.PrintStack();
      }
      return null!;
    }

    /// Returns the default value of the specified property.
    public static object GetPropertyDefaultValue( string property )
    {
      var script = GetCSharpScript();
      return script.GetPropertyDefaultValue( property );
    }

    /// Returns a dictionary containing constant names and their values.
    public static Dictionary GetScriptConstantMap()
    {
      var script = GetCSharpScript();
      return script.GetScriptConstantMap();
    }

    /// Returns the list of methods in this Godot.Script.
    public static Array GetScriptMethodList()
    {
      var script = GetCSharpScript();
      return script.GetScriptMethodList();
    }

    /// Returns the list of properties in this Godot.Script.
    public static Array GetScriptPropertyList()
    {
      var script = GetCSharpScript();
      return script.GetScriptPropertyList();
    }

    /// Returns the list of user signals defined in this Godot.Script.
    public static Array GetScriptSignalList()
    {
      var script = GetCSharpScript();
      return script.GetScriptSignalList();
    }

    /// Returns true if the script, or a base class, defines a signal with the given
    /// name.
    public static bool HasScriptSignal( string signalName )
    {
      var script = GetCSharpScript();
      return script?.HasScriptSignal( signalName ) ?? false;
    }

    /// Returns true if the script is a tool script. A tool script can run in the editor.
    public static bool IsTool()
    {
      var script = GetCSharpScript();
      return script?.IsTool() ?? false;
    }
  };


  public static class CSharpScriptExt
  {
    public static string ResourcePath( this Type t )
    {
      var sourceInfo = (CSharpScriptAttribute) Attribute.GetCustomAttribute( t, typeof( CSharpScriptAttribute ) );
      if (sourceInfo == null)
      {
        GD.PushError( $"Could not file script info. Did you add '{nameof( CSharpScriptAttribute )}' to the class '{t.Name}'?" );
        return "";
      }
      if (sourceInfo?.FilePath.GetFile().BaseName() != t.Name)
      {
        GD.PushError( $"Class and script name mismatch. Class name is '{ t.Name }' for script '{ sourceInfo?.FilePath }'" );
        return "";
      }
      return sourceInfo?.FilePath ?? "";
    }

    public static CSharpScript AsCSharpScript( this Type t )
    {
      var scriptPath = ResourcePath( t );
      if (scriptPath.Empty()) { throw new Exception( "Can't load CSharp Script" ); }
      // Don't worry, it will usually be a cached load
      // Also, in tool mode it can get scrapped randomly, so we kinda need to use load each time
      return GD.Load<CSharpScript>( scriptPath );
    }

    /// Returns a new instance of the script.
    public static object New( this Type t )
    {
      var script = AsCSharpScript( t );
      // if ( Engine.EditorHint && ! script.IsTool() )
      // { GD.PushWarning($"Script is not in tool mode: '{ typeof( T ).Name }'"); }
      return script.New();
    }

    /// Returns the default value of the specified property.
    public static object GetPropertyDefaultValue( this Type t, string property )
    {
      var script = AsCSharpScript( t );
      return script.GetPropertyDefaultValue( property );
    }

    /// Returns a dictionary containing constant names and their values.
    public static Dictionary GetScriptConstantMap( this Type t )
    {
      var script = AsCSharpScript( t );
      return script.GetScriptConstantMap();
    }

    /// Returns the list of methods in this Godot.Script.
    public static Array GetScriptMethodList( this Type t )
    {
      var script = AsCSharpScript( t );
      return script.GetScriptMethodList();
    }

    /// Returns the list of properties in this Godot.Script.
    public static Array GetScriptPropertyList( this Type t )
    {
      var script = AsCSharpScript( t );
      return script.GetScriptPropertyList();
    }

    /// Returns the list of user signals defined in this Godot.Script.
    public static Array GetScriptSignalList( this Type t )
    {
      var script = AsCSharpScript( t );
      return script.GetScriptSignalList();
    }

    /// Returns true if the script, or a base class, defines a signal with the given
    /// name.
    public static bool HasScriptSignal( this Type t, string signalName )
    {
      var script = AsCSharpScript( t );
      return script?.HasScriptSignal( signalName ) ?? false;
    }

    /// Returns true if the script is a tool script. A tool script can run in the editor.
    public static bool IsTool( this Type t )
    {
      var script = AsCSharpScript( t );
      return script?.IsTool() ?? false;
    }
  };
}
