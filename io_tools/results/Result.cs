using System.Text;
using Godot;

namespace MonsterVial.Results
{
  public class Result : Reference
  {
    public Error Error { get; private set; } = Error.Ok;
    public bool HasError => Error != Error.Ok;
    public bool IsOk => Error == Error.Ok;


    public string OperationDescription { get; private set; } = "Operation";
    public string ErrorMessage { get; private set; } = "";


    public virtual bool HasData => false;


    /// Returns "<Operation> succeeded" when error is OK
    /// Returns "<Operation> failed:  <ErrorMessage>:  <Error repr>" when not Ok
    ///   Leaves off the <ErrorMessage> if it is empty.
    ///   Leaves off the <Error repr> if Error is FAILED
    public override string ToString()
    {
      if (IsOk) { return OperationDescription + " succeeded"; }

      var msg = new StringBuilder( OperationDescription + " failed" );
      if (ErrorMessage.Length > 0)
      {
        msg.Append( ":  " ).Append( ErrorMessage );
      }
      if (Error != Error.Failed)
      {
        msg.Append( ":  " ).Append( System.Enum.GetName( typeof(Error), Error ) ?? "" ).Append( " (Err: " ).Append( Error.ToString("D") ).Append( ")" );
      }
      return msg.ToString();
    }



    //////// Builder methods and destructor ////////

    protected Result() { } //Default constructor for Godot

    // Provide an operation description that fits the form "<Operation> succeeded"
    public Result( string operationDescription )
    {
      OperationDescription = operationDescription.Empty() ? "Operation" : operationDescription;
    }
    // Provide an operation description that fits the form "<Operation> succeeded"
    public Result( string operationDescription, Error error, string errorMessage = "" )
      : this( operationDescription )
    {
      SetError( error, errorMessage );
    }
    public Result( Result other, string errorMessage = "" )
      : this( other.OperationDescription )
    {
      SetError( other, errorMessage );
    }
    // Provide an operation description that fits the form "<Operation> succeeded"
    public Result( string operationDescription, Result other, string errorMessage = "" )
      : this( operationDescription )
    {
      SetError( other, errorMessage );
    }

    /// Fill this result using an error code
    /// error - Error code
    /// errorMessage - Optional, additional error message to use for this result.
    public virtual void SetError( Error error, string errorMessage = "" )
    {
      Error = error;
      ErrorMessage = error == Error.Ok ? "" : errorMessage;
    }


    /// Fill this result using another result
    /// other - Other Result object
    /// errorMessage - Optional, additional error message to prepend to other result's
    ///                Sets ErrorMessage to "<errorMessage>:\<indented other.ErrorMessage>" when other was not Ok
    public virtual void SetError( Result other, string errorMessage = "" )
    {
      Error = other.Error;
      ErrorMessage = other.Error == Error.Ok ? "" : $"{ errorMessage }:\n{ _Indent( other.ErrorMessage ) }";
    }
    
    protected static string _Indent( string s, int indentLvl = 1 )
    {
      if (indentLvl == 0) { return s; }
      var indent = new string( ' ', indentLvl*2 );
      var lines = s.Split( '\n' );

      StringBuilder result = new StringBuilder(
        lines[0],
        indent.Length * lines.Length + s.Length
      );
      result.Append( lines[0] );
      for (var i = 1; i < lines.Length; i++)
      {
        result.Append( '\n' ).Append( indent ).Append( lines[i] );
      }
      return result.ToString();
    }
  };



  // Adds builder-style calls like 'WithError' that return 'this' without losing derived type
  public static class ResultExt
  {
    /// Fill this result using an error code
    /// error - Error code
    /// errorMessage - Optional, additional error message to use for this result.
    public static T WithError<T>( this T self, Error error, string errorMessage = "" )
      where T : Result
    {
      self.SetError( error, errorMessage );
      return self;
    }

    /// Fill this result using another result
    /// other - Other Result object
    /// errorMessage - Optional, additional error message to prepend to other result's
    ///                Sets ErrorMessage to "<errorMessage>:\<indented other.ErrorMessage>" when other was not Ok
    public static T WithError<T>( this T self, Result other, string errorMessage = "" )
      where T : Result
    {
      self.SetError(other, errorMessage);
      return self;
    }
    
    public static TResult WithData<TResult,TData>( this TResult self, TData value )
      where TResult : DataResult<TData>
    {
      self.Data = value;
      return self;
    }
  };
}
