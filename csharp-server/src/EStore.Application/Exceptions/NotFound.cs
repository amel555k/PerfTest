namespace EStore.Application.Exceptions;

public class NotFoundException(string message) : Exception(message);    // 404


/*
[EXCEPTIONS AND THE COST]

- it helps the service when we are talking about returning error from any depth instead of every method do that
- middleware is place for turning them into status codes
- in .NET thats a cost when we are talking about benchmark (especially in case when there are too many 409 responses)

*/