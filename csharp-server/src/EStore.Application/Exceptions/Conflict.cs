namespace EStore.Application.Exceptions;

public class ConflictException(string message) : Exception(message);    // 409