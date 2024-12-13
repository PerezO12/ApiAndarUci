namespace ApiUCI.Utilities
{
    public enum ErrorType
    {
        None, // Para casos exitosos
        BadRequest,
        NotFound,
        InternalServerError,
        Unauthorized,
        Forbidden
    }
}