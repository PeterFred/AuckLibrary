using NUnit.Framework;
using Library;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Library.Controllers;

namespace Library_NUnitTestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestMethodIndex()
        {
            //Arrange
            HomeController controller = new HomeController();

            //Act
            ViewResult result = controller.Index() as ViewResult;

            //Assert
            Assert.IsNotNull(result);
        }
    }
}