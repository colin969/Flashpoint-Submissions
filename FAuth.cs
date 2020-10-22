using System.Diagnostics;
using System;
using io.fusionauth;
using io.fusionauth.domain;
using io.fusionauth.domain.api;
using io.fusionauth.domain.oauth2;
using io.fusionauth.jwt.domain;
using System.Text.Json;
using io.fusionauth.domain.api.user;

namespace website
{
    public static class FAuth
    {
        private static readonly string apiKey = Environment.GetEnvironmentVariable("FUSIONAUTH_API_KEY");
        private static readonly string tenantId = Environment.GetEnvironmentVariable("FUSIONAUTH_TENANT_ID");
        public static readonly string redirectUri = Environment.GetEnvironmentVariable("FUSIONAUTH_REDIRECT_URI");
        public static readonly string logoutRedirectUri = Environment.GetEnvironmentVariable("FUSIONAUTH_LOGOUT_REDIRECT_URI");

        public static readonly string clientId = Environment.GetEnvironmentVariable("FUSIONAUTH_CLIENT_ID");
        private static readonly string clientSecret = Environment.GetEnvironmentVariable("FUSIONAUTH_CLIENT_SECRET");

        private static readonly string fusionauthURL = "http://localhost:9011";

        public static AccessToken GetToken(string code) {
            var client = new FusionAuthSyncClient(apiKey, fusionauthURL, tenantId);
            var res = client.ExchangeOAuthCodeForAccessToken(code, clientId, clientSecret, redirectUri);
            if (res.WasSuccessful()) {
                return res.successResponse;
            } else {
                throw res.exception;
            }
        }

        public static Boolean ValidateToken(string token) {
            var client = new FusionAuthSyncClient(apiKey, fusionauthURL, tenantId);
            var res = client.ValidateJWT(token);
            return res.WasSuccessful();
        }

        public static User GetUserByToken(string token) {
            var client = new FusionAuthSyncClient(apiKey, fusionauthURL, tenantId);
            var res = client.RetrieveUserUsingJWT(token);
            if (res.WasSuccessful()) {
                return res.successResponse.user;
            } else {
                throw res.exception;
            }
        }

        public static User GetUserById(Guid id) {
            var client = new FusionAuthSyncClient(apiKey, fusionauthURL, tenantId);
            var res = client.RetrieveUser(id);
            if (res.WasSuccessful()) {
                return res.successResponse.user;
            } else {
                throw res.exception;
            }
        }
    }
}
