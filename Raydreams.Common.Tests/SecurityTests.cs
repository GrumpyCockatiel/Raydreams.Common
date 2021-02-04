using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Security;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class SecurityTests
    {
        [TestMethod]
        public void NullTokenTest()
        {
            ITokenManager mgr = new NullTokenManager();

            string token = mgr.Encode( null );

            TokenPayload p = mgr.Decode( token );

            Assert.IsTrue( p != null && p.ID == "1" );
        }

        [TestMethod]
        public void SignTest()
        {
            RSAKeyGenerator gen = new RSAKeyGenerator();
            int size = gen.GenerateKeys( RSAKeySize.Key2048, false );

            var data = new byte[] { 0x00, 0x34, 0x56, 0x19 };

            byte[] sig = AsymmetricEncryptor.SignWithRSA256( data, gen.PrivateKey );

            bool verified = AsymmetricEncryptor.VerifyRSA256( data, sig, gen.PublicKey );

            Assert.IsTrue(verified);
        }
    }
}
