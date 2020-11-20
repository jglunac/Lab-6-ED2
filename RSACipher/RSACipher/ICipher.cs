using System;
using System.Collections.Generic;
using System.Text;

namespace RSACipher
{
    public interface ICipher
    {
        public bool Cipher(string route, out byte[] cipheredMsg, RSAkey PublicKey);
        public bool Decipher(string route, out byte[] Message, RSAkey PrivateKey);
    }
}
