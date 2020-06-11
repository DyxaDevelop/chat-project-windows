using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ServerChatTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AllUsersMessageCountTest()
        {

            List<ChatMessage> listOfMessages = new List<ChatMessage>();

            ChatMessage chatMessageAdmin1 = new ChatMessage
            {
                eventName = "null",
                timeStamp = "null",
                userName = "Admin",
                userMessage = "null"
            };

            ChatMessage chatMessageNazar = new ChatMessage
            {
                eventName = "null",
                timeStamp = "null",
                userName = "Nazar",
                userMessage = "null"
            };

            ChatMessage chatMessageAndrii = new ChatMessage
            {
                eventName = "null",
                timeStamp = "null",
                userName = "Andrii",
                userMessage = "null"
            };

            ChatMessage chatMessageAdmin2 = new ChatMessage
            {
                eventName = "null",
                timeStamp = "null",
                userName = "Admin",
                userMessage = "null"
            };

            listOfMessages.Add(chatMessageAdmin1);
            listOfMessages.Add(chatMessageNazar);
            listOfMessages.Add(chatMessageAndrii);
            listOfMessages.Add(chatMessageAdmin2);


            string responseExpected = "Admin: 2 Messages; Nazar: 1 Messages; Andrii: 1 Messages; ";

            string actualResponse = AsynchronousSocketListener.displayAllUserMessageCount(listOfMessages);
            Assert.AreEqual(responseExpected, actualResponse, "Error");
        }
    }
}
