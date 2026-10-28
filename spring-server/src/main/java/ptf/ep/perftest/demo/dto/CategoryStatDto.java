package ptf.ep.perftest.demo.dto;


import lombok.Getter;
import lombok.Setter;

import java.math.BigDecimal;
import java.math.RoundingMode;

@Getter
@Setter
public class CategoryStatDto {

    private String category;
    private Long productCount;
    private BigDecimal avgPrice;
    private Long totalStock;

    public CategoryStatDto(String category, Long productCount, Double avgPrice, Long totalStock) {
        this.category = category;
        this.productCount = productCount;
        this.avgPrice = avgPrice != null ? BigDecimal.valueOf(avgPrice).setScale(2, RoundingMode.HALF_UP): BigDecimal.ZERO;
        this.totalStock = totalStock;
    }
}
