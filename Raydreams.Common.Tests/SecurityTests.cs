using System;
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
    }
}
