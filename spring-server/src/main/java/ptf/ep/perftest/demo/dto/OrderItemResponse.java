package ptf.ep.perftest.demo.dto;

import ptf.ep.perftest.demo.entity.OrderItem;

import java.math.BigDecimal;

public record OrderItemResponse(
        Long productId,
        Integer quantity,
        BigDecimal unitPrice
) {

    public static OrderItemResponse from(OrderItem item){
        return new OrderItemResponse(
                item.getProduct().getId(),
                item.getQuantity(),
                item.getUnitPrice()
        );
    }
}
