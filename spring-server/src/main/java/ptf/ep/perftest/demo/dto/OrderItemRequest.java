package ptf.ep.perftest.demo.dto;

import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;

public record OrderItemRequest(
        @NotNull(message ="must not be null") Long productId,
        @NotNull(message="must not be null") @Min(value=1, message="must be at least 1") Integer quantity
) {
}
