using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.UseCases.Exceptions;
using RollerCoaster.Account.API.UseCases.Exceptions.InvaildRequestsException;
using System.Collections.Generic;

namespace RollerCoaster.Account.API.UseCases.Tests.Exceptions
{
    [TestClass]
    public class InvaildRequestExceptionTests 
    {
        #region  Constructor

        [TestMethod]
        public void Constructor_UserEntityData_SetsValues()
        {
            //Setup
            var invaildRequestDataItems = new List<InvaildRequestData>
            {

            };

            //Act
            var observed = new InvaildRequestException(invaildRequestDataItems);

            //Assert
            Assert.AreEqual(invaildRequestDataItems, observed.InvaildRequestDatas);
        }


        #endregion

        #region  GetInvaildRequestDatas

        [TestMethod]
        public void GetInvaildRequestDatas_UserEntityData_SetsValues()
        {
            //Setup
            var invaildRequestDataItems = new List<InvaildRequestData>
            {

            };

            var invaildRequestException = new InvaildRequestException(invaildRequestDataItems);

            //Act
            var observed = invaildRequestException.InvaildRequestDatas;

            //Assert
            Assert.AreEqual(invaildRequestDataItems, observed);
        }

        #endregion
    }
}
