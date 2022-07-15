using System;
using System.Configuration;
using System.IO;
using System.Text.Json;

namespace TicketManagementSystem
{
    /**
        * interview notes:

        * My way of solving this was: initially trying to read the flow of the program from Program.cs
        * Assuming TicketRepository.cs and Ticket.cs didn't need any refactoring I put my focus in here
        * I tried to find any areas that could be refactored and made them more readable to the extent of my knowledge
        * Some parts had code duplication and I tried to make them more modular, so they could be reused instead.
        *
        */
    public class TicketService
    {
        ITicketRepository ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            this.ticketRepository = ticketRepository;
        }

        public int CreateTicket(string title, Priority prio, string assignedTo, string desc, DateTime date, bool isPayingCustomer)
        {
            // Check if title or desc are null or if they are invalid and throw exception
            bool isTitleAndDescValid = !string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(desc);
            if (!isTitleAndDescValid)
                throw new InvalidTicketException("Title or description were null");

            User user = GetUser(assignedTo);

            // Raise ticket priority if an hour has passed without being resolved OR ticket contains important keywords.
            bool oneHourPassed = date < DateTime.UtcNow - TimeSpan.FromHours(1); // weird
            bool containsImportantKeywords = title.Contains("Crash") || title.Contains("Important") || title.Contains("Failure");
            if (oneHourPassed || containsImportantKeywords)
                RaisePriority(prio);


            /*
                interview notes: Initially I wasn't very fond of the if IsPayingcustomer statement that was there, 
                but I decided to keep it so I wouldn't have to send in isPayingCustomer as an argument twice.
                
                But then I thought about it and realized that it was more modular doing it like this:
            */
            double price = GetPrice(prio, isPayingCustomer);  
            User accountManager = GetAccountManager(isPayingCustomer);

            // Create the ticket
            var ticket = new Ticket()
            {
                Title = title,
                AssignedUser = user,
                Priority = prio,
                Description = desc,
                Created = date,
                PriceDollars = price,
                AccountManager = accountManager
        };

            var id = ticketRepository.CreateTicket(ticket);
            Console.WriteLine("Ticket created with id: " + id);

            // Return the id
            return id;
        }

        public void AssignTicket(int id, string username)
        {
            User user = GetUser(username);
            var ticket = GetTicket(id);
            ticket.AssignedUser = user;
            ticketRepository.UpdateTicket(ticket);
        }

        // Gets the user from the UserRepository, and throws an exception if the user is not found.
        private User GetUser(string username)
        {
            User user = null;
            var ur = new UserRepository();
            if (username != null)
                user = ur.GetUser(username);
 
            if (user == null)
                throw new UnknownUserException("User not found");
            return user;
        }

        // Raises the priority.
        private void RaisePriority(Priority prio)
        {
            if (prio == Priority.Low)
                prio = Priority.Medium;
            else if (prio == Priority.Medium)
                prio = Priority.High;
        }

        // Gets the price for the ticket based on the priority and whether the customer is paying or not.
        private double GetPrice(Priority prio, bool isPayingCustomer)
        {
            if (!isPayingCustomer)
                return 0;
            if (prio == Priority.High)
                return 100;
            else
                return 50;
        }
        
        // Gets the account manager for the ticket based on whether the customer is paying or not.
        private User GetAccountManager(bool isPayingCustomer)
        {
            if(isPayingCustomer)
                return new UserRepository().GetAccountManager();
            else
                return null;
        }

        // Gets the ticket from the TicketRepository, and throws an exception if the ticket is not found.
        private Ticket GetTicket(int id)
        {
            var ticket = ticketRepository.GetTicket(id);

            if (ticket == null)
                throw new UnknownTicketException("No ticket found for id " + id);
            return ticket;
        }

        private void WriteTicketToFile(Ticket ticket)
        {
            var ticketJson = JsonSerializer.Serialize(ticket);
            File.WriteAllText(Path.Combine(Path.GetTempPath(), $"ticket_{ticket.Id}.json"), ticketJson);
        }
    }

    public enum Priority
    {
        High,
        Medium,
        Low
    }
}
