namespace MyBudget.BLL.Exceptions;

public class JwtTokenMissingException() : JwtUnauthorizedException("Refresh token is missing");