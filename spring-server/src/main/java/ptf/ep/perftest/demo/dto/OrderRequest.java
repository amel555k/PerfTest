package ptf.ep.perftest.demo.dto;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotEmpty;

import java.util.List;

public record OrderRequest(
        @NotBlank(message = "must not be empty") String customer,
        @NotEmpty(message = "must not be empty") @Valid List<OrderItemRequest> items
) {
}