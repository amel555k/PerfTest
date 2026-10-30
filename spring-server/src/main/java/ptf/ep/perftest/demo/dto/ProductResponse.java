package ptf.ep.perftest.demo.dto;

import ptf.ep.perftest.demo.entity.Product;

import java.math.BigDecimal;

public record ProductResponse (
    Long id,
    String name,
    String description,
    String category,
    BigDecimal price,
    Integer stock
){
    public static ProductResponse from(Product p) {
        return new ProductResponse(
                p.getId(),
                p.getName(),
                p.getDescription(),
                p.getCategory(),
                p.getPrice(),
                p.getStock()

        );
    }
}


