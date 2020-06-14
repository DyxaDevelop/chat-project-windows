using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogInForm;

namespace LoginFormTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string jsonData = @"{ 
            'FirstName':'Jignesh',  
            'LastName':'Trivedi'  
            ";

            bool responseExpected = true;

            bool actual = AsyncClientEvent.isValidJSON(jsonData);
            Assert.AreEqual(responseExpected, actual, "Error");
        }
    }
}
