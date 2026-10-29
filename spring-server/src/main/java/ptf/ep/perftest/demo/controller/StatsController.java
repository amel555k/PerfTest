package ptf.ep.perftest.demo.controller;


import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;
import ptf.ep.perftest.demo.dto.CategoryStatDto;
import ptf.ep.perftest.demo.service.ProductService;

import java.util.List;

@RestController
public class StatsController {

    private final ProductService productService;

    public StatsController(ProductService productService){
        this.productService=productService;
    }

    @GetMapping("/stats/categories")
    public List<CategoryStatDto> categories(){
        return productService.categoryStats();
    }
}
