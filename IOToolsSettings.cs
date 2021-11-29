using Godot;

namespace MonsterVial
{
  /// <summary> Static settings used by the IO Tools addon </summary>
  public static class IOToolsSettings
  {
    /// Allows '_Ready' to be called on resources automatically.
    /// Called after using CSharpScript<CustRes>.New() and after IO.Load<CustRes>.
    /// Default Load will call constructors, then fill all values, then call _Ready.
    /// This allows a sort of late-init that triggers post-load.
    public const bool CALL_READY_ON_RESOURCES = true;

    /// Location of the IO.cs file relative to the project folder.
    public const string IO_SCRIPT_PROJ_PATH = "addons/com.monstervial.io_tools/io_tools/IO.cs";
  };
}
