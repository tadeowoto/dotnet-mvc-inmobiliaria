
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System;

namespace inmobiliaria.lib
{
    public class HashPasswordService
    {
        private readonly IConfiguration _configuration;

        public HashPasswordService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string HashPassword(string password)
        {
            try
            {
                string salt = _configuration["Salt"];
                if (string.IsNullOrEmpty(salt)) throw new Exception("Salt no configurado");


                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: System.Text.Encoding.ASCII.GetBytes(salt),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                return hashed;
            }
            catch (Exception ex)
            {
                throw new Exception("Error hashing password", ex);
            }
        }





    }
}