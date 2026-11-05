namespace EStore.Application.Exceptions;

public class NotFoundException(string message) : Exception(message);    // 404