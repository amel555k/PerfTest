package ptf.ep.perftest.demo.controller;


import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import ptf.ep.perftest.demo.dto.PageResponse;
import ptf.ep.perftest.demo.dto.ProductResponse;
import ptf.ep.perftest.demo.service.ProductService;

@RestController
public class ProductController {

    private final ProductService productService;

    public ProductController(ProductService productService){
        this.productService=productService;
    }

    @GetMapping("/products")
    public PageResponse<ProductResponse> list(
            @RequestParam(defaultValue = "0") int page,
            @RequestParam(defaultValue = "20") int size,
            @RequestParam(required = false) String category) {
        return productService.list(page, size, category);
    }

    @GetMapping("/products/search")
    public PageResponse<ProductResponse> search(
            @RequestParam String q,
            @RequestParam(defaultValue="0") int page,
            @RequestParam(defaultValue="20") int size){
        return productService.search(q,page,size);
    }

    @GetMapping("/products/{id}")
    public ProductResponse getById(@PathVariable Long id){
        return productService.getById(id);
    }


}
