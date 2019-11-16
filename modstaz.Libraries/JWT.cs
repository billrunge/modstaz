using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    public class JWT
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public int TokenLifeInMinutes { get; set; } = 60;
        public string Key { get; set; }

        public async Task<string> GenerateJwtAsync()
        {
            SigningCredentials credentials =
                new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Key)), SecurityAlgorithms.HmacSha256);

            JwtHeader header = new JwtHeader(credentials);

            JwtPayload payload = new JwtPayload
           {
                { "userId", UserId },
                { "emailAddress", EmailAddress },
                { "iat", GetCurrentEpoch() },
                { "exp", GetCurrentEpoch() + TokenLifeInMinutes * 60 }
           };

            JwtSecurityToken secToken = new JwtSecurityToken(header, payload);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            return await Task.Run(() => handler.WriteToken(secToken));
        }

        public bool IsJwtValid(string jwt)
        {
            string[] parts = jwt.Split(".".ToCharArray());
            string header = parts[0];
            string payload = parts[1];
            string signature = parts[2];//Base64UrlEncoded signature from the token

            byte[] bytesToSign = Encoding.UTF8.GetBytes(string.Join(".", header, payload));

            byte[] secret = Encoding.UTF8.GetBytes(Key);

            HMACSHA256 alg = new HMACSHA256(secret);
            byte[] hash = alg.ComputeHash(bytesToSign);

            string computedSignature = Base64UrlEncode(hash);

            return signature == computedSignature;
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input).Split('=')[0].Replace('+', '-').Replace('/', '_');
        }

        public int GetCurrentEpoch()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public string ParseJWT(string jwt)
        {
            if (IsJwtValid(jwt))
            {
                JwtSecurityToken token = new JwtSecurityToken(jwt);
                JWTobject jwtObject = new JWTobject()
                {
                    emailAddress = token.Claims.First(c => c.Type == "emailAddress").Value,
                    userId = int.Parse(token.Claims.First(c => c.Type == "userId").Value),
                    exp = int.Parse(token.Claims.First(c => c.Type == "exp").Value),
                    iat = int.Parse(token.Claims.First(c => c.Type == "iat").Value)
                };
                if (jwtObject.exp > GetCurrentEpoch())
                {
                    return JsonConvert.SerializeObject(jwtObject);
                }
                else
                {
                    return "false";
                }
                
            }
            else
            {
                return "false";
            }
        }

        public class JWTobject
        {
            public int userId { get; set; }
            public string emailAddress { get; set; }
            public int iat { get; set; }
            public int exp { get; set; }
        }
    }
}