using Domain.ValueObjects;

namespace Infrastructure.User;

public class Customer : ApplicationUser
{
    private Customer() {}
    private Customer(string userName, string email, string fullName, string phoneNumber, Address address, int? age)
    {
        UserName = userName;
        Email = email;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Address = address;
        Age = age;
    }

    public string FullName { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public int? Age { get; private set; }

    public static Customer Create(
        string userName,
        string email,
        string fullName,
        string phoneNumber,
        Address address,
        int? age)
    {
        return new(userName, email, fullName, phoneNumber, address, age);
    }

    public void Update(string fullName, Address address, string phoneNumber, int? age)
    {
        FullName = fullName;
        Address = address;
        PhoneNumber = phoneNumber;
        Age = age;
    }
}
