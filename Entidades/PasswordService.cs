using System.Security.Cryptography;

namespace Entidades
{
    // Servicio de hashing de contraseñas usando PBKDF2 (nativo de .NET, sin librerías externas).
    // El campo Usuario.Password va a guardar el resultado de HashPassword (no texto plano).
    public class PasswordService
    {
        private const int SaltSize = 16; // 128 bits
        private const int HashSize = 32; // 256 bits
        private const int Iterations = 100_000;

        public string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            // Formato guardado: iteraciones.salt.hash (todo en Base64)
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            var partes = storedHash.Split('.');
            if (partes.Length != 3) return false;

            int iterations = int.Parse(partes[0]);
            byte[] salt = Convert.FromBase64String(partes[1]);
            byte[] hash = Convert.FromBase64String(partes[2]);

            byte[] hashIntento = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                hash.Length);

            return CryptographicOperations.FixedTimeEquals(hash, hashIntento);
        }
    }
}
