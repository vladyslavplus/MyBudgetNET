namespace MyBudget.BLL.Exceptions;

public class JwtTokenExpiredException() : JwtUnauthorizedException("Token is expired or revoked");