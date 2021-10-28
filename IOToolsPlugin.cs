using Godot;

namespace MonsterVial
{
  public class IOToolsPlugin : EditorPlugin
  {
    #region ____ Settings ____
    // Static settings used by Plugin

    // A bit slower, but allows '_Ready' to be called on resources.
    // Called after using CSharpScript<CustRes>.New() and after IO.Load<CustRes>.
    // Default Load will call constructors, then fill all values, then be done.
    // This allows a sort of late-init that triggers post-load.
    public const bool CALL_READY_ON_RESOURCES = true;

    #endregion ==== Settings ====


    // Nothing else to do at this time, as this is a static-only plugin
  };
}
