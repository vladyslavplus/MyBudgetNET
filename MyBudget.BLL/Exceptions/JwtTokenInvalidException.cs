namespace MyBudget.BLL.Exceptions;

public class JwtTokenInvalidException() : JwtUnauthorizedException("Invalid refresh token");