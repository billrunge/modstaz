using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modstaz.Libraries
{

    public class JWT
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public TimeSpan TokenLifeSpan { get; set; } = TimeSpan.FromMinutes(60);
        public string Key { get; set; }
        public async Task<string> GenerateJwtAsync()
        {

            SymmetricSecurityKey securityKey =
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Key));

            SigningCredentials credentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtHeader header = new JwtHeader(credentials);

            JwtPayload payload = new JwtPayload
           {
                {"userId", UserId},
                {"emailAddress", EmailAddress },
                {"iat", DateTime.Now.Ticks }
           };

            JwtSecurityToken secToken = new JwtSecurityToken(header, payload);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            return await Task.Run(() => handler.WriteToken(secToken));

        }


        //public int GetUserFromJWT(string jwt)
        //{
        //    var token = new JwtSecurityToken(jwtEncodedString: jwt);
        //    return int.Parse(token.Claims.First(c => c.Type == "User").Value);
        //}

        //public int GetGameFromJWT(string jwt)
        //{
        //    var token = new JwtSecurityToken(jwtEncodedString: jwt);
        //    return int.Parse(token.Claims.First(c => c.Type == "Game").Value);
        //}

    }
}