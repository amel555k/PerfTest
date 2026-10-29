package ptf.ep.perftest.demo.service;

import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import ptf.ep.perftest.demo.dto.CategoryStatDto;
import ptf.ep.perftest.demo.dto.PageResponse;
import ptf.ep.perftest.demo.dto.ProductResponse;
import ptf.ep.perftest.demo.entity.Product;
import ptf.ep.perftest.demo.exception.BadRequestException;
import ptf.ep.perftest.demo.exception.ResourceNotFoundException;
import ptf.ep.perftest.demo.repository.ProductRepository;

import java.util.List;

@Service
public class ProductService {
    private static final int MAX_SIZE = 100;

    private final ProductRepository productRepository;

    public ProductService(ProductRepository productRepository){
        this.productRepository = productRepository;
    }

    @Transactional(readOnly = true)
    public PageResponse<ProductResponse> list(int page, int size, String category) {
        validatePaging(page, size);
        Page<Product> result;
        if (category == null || category.isBlank()) {
            Pageable pageable = PageRequest.of(page, size, Sort.by("id").ascending());
            result = productRepository.findAll(pageable);
        } else {
            result = productRepository.findByCategoryOrderByIdAsc(category, PageRequest.of(page, size));
        }
        return PageResponse.from(result, ProductResponse::from);
    }


    @Transactional(readOnly=true)
        public ProductResponse getById(Long id){
            return productRepository.findById(id)
                    .map(ProductResponse::from)
                    .orElseThrow(() ->new ResourceNotFoundException("Product not found"));
        }


    @Transactional(readOnly = true)
        public PageResponse<ProductResponse> search (String q,int page, int size){
        if (q == null | q.isBlank()) {
            throw new BadRequestException("Missing required parameter: q");
        }

        validatePaging(page, size);
        Page<Product> result = productRepository.searchByNameOrDescription(q, PageRequest.of(page,size));
        return PageResponse.from(result, ProductResponse::from);
    }

    @Transactional(readOnly=true)
    public List<CategoryStatDto> categoryStats() {
        return productRepository.getCategoryStats();
    }

    private void validatePaging(int page, int size){
        if (page < 0){
            throw new BadRequestException("Invalid value for parameter: page");
        }

        if(size<1 || size > MAX_SIZE){
            throw new BadRequestException("Invalid valude for parameter: size");
        }
    }

}
