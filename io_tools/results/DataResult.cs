using Godot;

namespace MonsterVial.Results
{
  [Tool]
  public class DataResult<T> : Result
  {
    private bool __hasData = false;
    public override bool HasData => __hasData;

    private T __data = default!;
    public void ClearData() { __data = default!; }

    public T Data
    {
      get => __hasData ? __data : throw new System.Exception( "Result does not contain data. Check HasData first." );
      set
      {
        __hasData = true;
        __data = value;
      }
    }

    public bool TryGetData( out T data ) { data = Data; return __hasData; }
    public bool TryGetData<U>( out U data )
    {
      if (__hasData && Data is U u) { data = u; return true; }
      else { data = default!; return false; }
    }


    protected DataResult() { } //Default constructor for Godot

    /// Provide an operation description that fits the form "<Operation> succeeded"
    public DataResult( string operationDescription )
      : base( operationDescription )
    { }
    /// Provide an operation description that fits the form "<Operation> succeeded"
    public DataResult( string operationDescription, Error error, string errorMessage = "" )
      : base( operationDescription, error, errorMessage )
    { }
    public DataResult( Result other, string errorMessage = "" )
      : base( other, errorMessage )
    { }
    /// Provide an operation description that fits the form "<Operation> succeeded"
    public DataResult( string operationDescription, Result other, string errorMessage = "" )
      : base( operationDescription, other, errorMessage )
    { }
    /// Provide an operation description that fits the form "<Operation> succeeded"
    public DataResult( string operationDescription, T data )
      : base( operationDescription )
    {
      Data = data;
    }
  };
}
