# IO Tools

Adds support for relative paths! Also adds (hopefully temporary) custom Resource fixes for Godot. This is a currently a static addon, so it should cause less errors than a full C# addon would.

## Installing

Create the folder `res://addons/` if it doesn't exist.

Download this repo as a zip and unzip it to `res://addons/`

Make sure the file `IO.cs` is located at `res://addons/com.monstervial.io_tools/io_tools/IO.cs`, or relative paths won't work!

If relative paths are giving an error, make sure the zip didn't add an extra folder in the middle. If you want to rename stuff or move `IO.cs`, follow the error's instructions.

There is no need to add the addon in the project settings, as this is currently a static-only addon.


## CSharpScript Fixes

Custom resources in C# currently cannot be saved to disk if created from C# using regular constructors. The provided `CSharpScriptAttribute` and its helpers are a temporary solution to that problem.

Example
```csharp
  // Declare a class with the attribute
  [Tool, CSharpScript]
  public class CustomResource : Resource { ... }

  // Later, create new resources with
  CSharpScript<CustomResource>.New()
```

Lets hope [#48828](https://github.com/godotengine/godot/issues/48828) gets fixed soon. (Course, even if it does get fixed, using the style above calls any `_Ready` methods for post-load resource operations.)


## WeakRef

Also provided is a typed WeakRef class. C#'s `WeakReference<>` does not currently work with Godot objects. Similarly, Godot's WeakRef doesn't work with things that don't derive from `Godot.Object`. The generic `WeakRef<>` included in this addon will work for Godot objects and C# objects! It also provides `TryGetTarget` and `GetTargetOrNull`, so either workflow/ideology can be used.


## File IO

The static class `IO` located in the namespace `MonsterVial` contains loads of helpful file IO operations. Most of these support relative paths starting with `./` or `../`. The relative paths rely on the C# feature `[CallerFilePath]` to auto-fill a path using the method-caller's location.

Most of `IO`'s file operations return a `Result` object. This is a wrapper for errors, error messages, and sometimes data.

Most of `IO`'s file operations will auto-create parent directories, unlike stock Godot file operations.

Example
```csharp
var result = IO.LoadResource<TItem>( "./sword.tres" );
if (result.HasError) { GD.PushError( result.ToString() ); }
else { __items.Add( result.Data ); }
```

For resources `LoadResourceOrNull` is also available for quick-use
```csharp
var item = IO.LoadResourceOrNull<TItem>( "./sword.tres" );
```

There are also loads of other helpful file and directory operations including:
```
GlobalizePath
LocalizePath
ProjectPath
LocalizeProjectPath
LocalizeRelativeProjectPath
DirExists
FileExists
Exists
OpenDir
EnsureDir
EnsureBaseDir
DeleteFile
DeleteDir
CopyFile
CopyDir
ReadText
WriteText
LoadResource<TResource>
LoadResourceOrNull<TResource>
SaveResource
```
All fully commented with Doc comments to make auto-complete go more smoothly!
