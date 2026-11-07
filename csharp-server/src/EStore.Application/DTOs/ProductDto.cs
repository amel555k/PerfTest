namespace EStore.Application.Dtos;

public record ProductDto(long Id, string Name, string? Description, string Category, decimal Price, int Stock);



/*
[WHAT IS APPLICATION LAYER]

- thats a layer that contains use-cases (create order, send the product page...)
Depends only on Domain layer


[RECORDS]

- record generates 
    -- construtor, 
    -- init properties
    -- value equality
    -- ToString
    -- 'with' statements

- always use:
    -- decimal for prices
    -- long for BIGINT columns (int is too small)
    -- String? for eventually empty values when customer sends blank placeholder (without ? , API will break before even coming to step where it sends exception)

    RESUME:
        -- take the appropriate type for columns in DB and use cases
        -- records are born to be DTOs  


*/