namespace DiplomaApi.Services
{
    public interface IUserService
    {
        // Token
        string CreateToken(ClaimSettings claimSettings);
        string GetClaimValue(ClaimType claimType);
        string GetClaimValue(string token, ClaimType claimType);

        // User
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }

    public enum ClaimType
    {
        UserId
    }
}
