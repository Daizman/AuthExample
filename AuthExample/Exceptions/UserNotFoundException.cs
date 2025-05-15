namespace AuthExample.Exceptions;

public class UserNotFoundException(int id) : Exception($"User with id={id} not found.");
