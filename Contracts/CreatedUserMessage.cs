namespace Contracts;

public record CreatedUserMessage(int Id,string Fullname,string Email,DocumentInfo Document);
public record DocumentInfo(int Id,DateTime GivenDate,string SerialNumber);