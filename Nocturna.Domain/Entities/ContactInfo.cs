namespace Nocturna.Domain.Entities;

public class ContactInfo(string number, string name)
{
    public string Number { get; private set; } = number;
    public string Name { get; private set; } = name;
}