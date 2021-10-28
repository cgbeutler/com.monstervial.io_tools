using Godot;

namespace MonsterVial.Results
{
  [Tool]
  public class JsonParseResult : DataResult<object>
  {
    public JsonParseResult FromJsonResult( JSONParseResult jsonResult )
    {
      SetError( Error.Failed, $"Line { jsonResult.ErrorLine }:  { jsonResult.ErrorString }" );
      if (jsonResult.Error == Error.Ok) { Data = jsonResult.Result; }
      return this;
    }

    protected JsonParseResult() { } //Default constructor for Godot
    // See 'from_json_result'
    public JsonParseResult( JSONParseResult jsonResult = null! ) : base("Parse JSON string")
    {
      if (jsonResult != null) { FromJsonResult(jsonResult); }
    }
  };
}
