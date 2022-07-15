using System;

namespace TicketManagementSystem
{
    /*
        final interview notes:
        Most of my time spent refactoring was in TicketService.cs as well as trying to add new tests in TicketServiceTests.
        I also added a UnknownTicketException as I thought that it was appropiate when trying to get a ticket that doesn't exist.

        I'm not sure if I managed to get all tests, but I definitely wanted to add some more. However, due to time constraints and my limited
        knowledge of testing, I didn't manage to.
        Maybe a test for when we get an account manager that does not exist? Right now the code doesn't make use of the account manager,
        but for when it does 

        I also wanted to add new folders for each module, e.g exceptions being in TicketManagementSystem.Exceptions,
        and users being in TicketManagementSystem.Users, etc.

        -- Pouya Shirin

    */  

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ticket Service Test Harness");

            var service = new TicketService(new TicketRepository());

            Console.WriteLine("Creating new ticket.");
            var ticketId = service.CreateTicket(
                "System Crash",
                Priority.Medium,
                "jsmith",
                "The system crashed when user performed a search",
                DateTime.UtcNow,
                true);

            service.AssignTicket(ticketId, "sberg");


            Console.WriteLine("Done");
        }
    }
}
