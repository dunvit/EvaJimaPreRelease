using EveJimaCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EJTests
{
    /// <summary>
    /// Summary description for InfrastructureTests
    /// </summary>
    [TestClass]
    public class InfrastructureTests
    {

        [TestMethod]
        public void TestMethod1()
        {
            var CrestData = new CrestAuthorization("1r8BCKYl8yJsVi3gbs-aXLYK8FINerSuyvZFOqfuk5zmLYAz_WiGYhq-irMf2j030",
                "8f1e2ac9d4aa467c88b12674926dc5e6",
                "GZyvG71OxmfHzcDrTMreHw6CV7sDUwiBMiPSpbPn");

            CrestData.Refresh("U1lAcCcmsI3cqbKjQ8nnnaoiN0KwyeSgzR_NTcATa2w1");

            var data = CrestData.GetCharacterInfo(95476725);
        }
    }
}
