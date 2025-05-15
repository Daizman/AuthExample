namespace AuthExample.Exceptions;

public class PostNotFoundException(int id, int userId) : Exception($"Post with id={id} not found for userId={userId}");
