namespace RccgHopeHouse.Core.Exceptions
{
    public class ContactUsException:DomainException
    {
        public ContactUsException(string message) : base(message) { }

        public static ContactUsException InvalidEmail(string email) =>
            new($"Invalid email address: {email}");

        public static ContactUsException InvalidPhoneNumber(string? phone) =>
            new($"Invalid phone number: {phone ?? "empty"}");
    }
}
