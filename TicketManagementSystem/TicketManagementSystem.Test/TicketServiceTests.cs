using System;
using Moq;
using NUnit.Framework;

namespace TicketManagementSystem.Test
{
    public class Tests
    {
        private TicketService ticketService;
        private Mock<ITicketRepository> ticketRepositoryMock;
        
        [SetUp]
        public void Setup()
        {
            ticketRepositoryMock = new Mock<ITicketRepository>();
            ticketService = new TicketService(ticketRepositoryMock.Object);
        }

        [Test]
        public void ShallThrowExceptionIfTitleIsNull()
        {
            Assert.That(() => ticketService.CreateTicket(null, Priority.High, "jim", "high prio ticket", DateTime.Now, false), Throws.InstanceOf<InvalidTicketException>().With.Message.EqualTo("Title or description were null"));
        }

        [Test]
        public void ShallCreateTicket()
        {
            const string title = "MyTicket";
            const Priority prio = Priority.High;
            const string assignedTo = "jsmith";
            const string description = "This is a high ticket"; 
            DateTime when = DateTime.Now;

            ticketService.CreateTicket(title, prio, assignedTo, description, when, false);

            ticketRepositoryMock.Verify(a => a.CreateTicket(It.Is<Ticket>(t =>
                t.Title == title && 
                t.Priority == Priority.High && 
                t.Description == description &&
                t.AssignedUser.Username == assignedTo && 
                t.Created == when)));
        }

        [Test]
        public void ShallThrowExceptionIfUserIsNull()
        {
            Assert.That(() => ticketService.CreateTicket("MyTicket", Priority.High, "jnotsmith", "high prio ticket", DateTime.Now, false), Throws.InstanceOf<UnknownUserException>().With.Message.EqualTo("User not found"));
        }

        [Test]
        public void ShallThrowExceptionIfTicketIsNull()
        {
            int ticketId = 0;
            Assert.That(() => ticketService.AssignTicket(ticketId, "jsmith"), Throws.InstanceOf<UnknownTicketException>().With.Message.EqualTo("No ticket found for id " + ticketId));
        }


        /*
            interview notes:
            Last 20 minutes were spent trying to figure out why this test isn't passing.
            If I had to guess, it seems like the ticketrepository does not add the ticket in its repository due to it being a Mock? type,
            therefore the ticketservice cannot get the ticket from the repository.

            I believe it's mostly because of my limited knowledge of testing.
        */
        [Test]
        public void ShallAssignTicket()
        {
            const string title = "MyTicket";
            const Priority prio = Priority.High;
            const string assignedTo = "jsmith";
            const string description = "This is a high ticket"; 
            DateTime when = DateTime.Now;

            int ticketId = ticketService.CreateTicket(title, prio, assignedTo, description, when, false);
            Console.WriteLine("Ticket id is " + ticketId);
            Assert.That(() => ticketService.AssignTicket(ticketId, "sberg"), Throws.Nothing);
        }
    }
}