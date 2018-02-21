using System;
using GeoLib.Contracts;
using GeoLib.Data.Entities;
using GeoLib.Data.Repository_Interfaces;
using GeoLib.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GeoLib.Tests
{
    // Unit test class
    [TestClass]
    public class MangerTest
    {
        [TestMethod]
        public void ZipCodeRetrival()
        {
            //Framework of mocking for .Net 
            // it create a fake representation of the repositories
            Mock<IZipCodeRepository> mockZipRep = new Mock<IZipCodeRepository>();

            ZipCode zip = new ZipCode
            {
                City = "New Orleans",
                State = new State { Abbreviation = "LA"},
                Zip = "70112"
            };

            // each time the GetByZip with this parameter returns 
            //a copy of the ZipCode instance hard-coded above
            mockZipRep.Setup(o => o.GetByZip("70112")).Returns(zip);

            IGeoService service = new GeoManager(null, mockZipRep.Object);

            ZipCodeData data = service.GetZipInfo("70112");

            Assert.IsTrue(data.City == "New Orleans");
            Assert.IsTrue(data.State == "LA" );
        }
    }
}
