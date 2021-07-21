using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace DofusMarket.Services
{
    internal class CryptoService
    {
        public byte[] AesDecrypt(byte[] value, AesManaged aes)
        {
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.Key[..16]);
            using MemoryStream inStream = new(value);
            using CryptoStream cryptoStream = new(inStream, decryptor, CryptoStreamMode.Read);
            using MemoryStream outStream = new();
            cryptoStream.CopyTo(outStream);
            return outStream.ToArray();
        }

        public byte[] RsaEncrypt(byte[] value, string key)
        {
            return GetRsaCipher(key, true).ProcessBlock(value, 0, value.Length);
        }

        public byte[] RsaDecrypt(byte[] value, string key)
        {
            return GetRsaCipher(key, false).ProcessBlock(value, 0, value.Length);
        }

        private static IAsymmetricBlockCipher GetRsaCipher(string key, bool forEncryption)
        {
            ICipherParameters rsaKeyParameters = ReadPublicKey(key);
            IAsymmetricBlockCipher cipher = new Pkcs1Encoding(new RsaEngine());
            cipher.Init(forEncryption, rsaKeyParameters);
            return cipher;
        }

        private static RsaKeyParameters ReadPublicKey(string key)
        {
            PemReader pemReader = new(new StringReader(key));
            return (RsaKeyParameters)pemReader.ReadObject();
        }
    }
}
