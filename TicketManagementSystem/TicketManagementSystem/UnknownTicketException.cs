using System;
namespace TicketManagementSystem
{
    public class UnknownTicketException : Exception
    {
        public UnknownTicketException(string message) : base(message)
        {
        }
    }
}
