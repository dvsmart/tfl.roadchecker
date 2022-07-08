namespace Tfl.JourneyChecker.ConsoleApp.Models
{
    public enum ResultStatus
    {
        Success = 0,
        NotFound = 1, 
        HttpResponseError =2,
        GeneralError =3,
        ValidationError=4
    }
}
