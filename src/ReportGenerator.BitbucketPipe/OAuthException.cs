using System;
using IdentityModel.Client;
using JetBrains.Annotations;

namespace ReportGenerator.BitbucketPipe
{
    [PublicAPI]
    public class OAuthException : Exception
    {
        public TokenResponse TokenResponse { get; }

        public OAuthException(TokenResponse tokenResponse) : base(
            $"Bitbucket Authentication error: {tokenResponse.ErrorDescription}") =>
            TokenResponse = tokenResponse;
    }
}
