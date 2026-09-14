namespace RccgHopeHouse.Core.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" with ID ({key}) was not found.") { }
    }
}
