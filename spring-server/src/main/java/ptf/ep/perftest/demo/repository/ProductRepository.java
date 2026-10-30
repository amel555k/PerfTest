package ptf.ep.perftest.demo.repository;

import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import ptf.ep.perftest.demo.dto.CategoryStatDto;
import ptf.ep.perftest.demo.entity.Product;

import java.util.List;

public interface ProductRepository extends JpaRepository<Product,Long> {

    Page<Product> findByCategoryOrderByIdAsc(String category, Pageable pageable);

    @Query("SELECT p FROM Product p WHERE LOWER(p.name) LIKE LOWER(CONCAT('%', :q, '%')) OR LOWER(p.description) LIKE LOWER(CONCAT('%', :q, '%')) ORDER BY p.id ASC")
    Page<Product> searchByNameOrDescription(@Param("q") String q, Pageable pageable);

    @Query("SELECT new ptf.ep.perftest.demo.dto.CategoryStatDto(p.category, COUNT(p), AVG(p.price), SUM(p.stock)) FROM Product p GROUP BY p.category ORDER BY p.category ASC")
    List<CategoryStatDto> getCategoryStats();

    @Modifying
    @Query("UPDATE Product p SET p.stock = p.stock - :qty WHERE p.id = :id AND p.stock >= :qty")
    int decreaseStock(@Param("id") Long id, @Param("qty") int qty);
}
