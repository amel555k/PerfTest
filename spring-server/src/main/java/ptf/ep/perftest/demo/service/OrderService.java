package ptf.ep.perftest.demo.service;


import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import ptf.ep.perftest.demo.dto.OrderItemRequest;
import ptf.ep.perftest.demo.dto.OrderRequest;
import ptf.ep.perftest.demo.dto.OrderResponse;
import ptf.ep.perftest.demo.entity.Order;
import ptf.ep.perftest.demo.entity.OrderItem;
import ptf.ep.perftest.demo.entity.Product;
import ptf.ep.perftest.demo.exception.BadRequestException;
import ptf.ep.perftest.demo.exception.InsufficientStockException;
import ptf.ep.perftest.demo.exception.ResourceNotFoundException;
import ptf.ep.perftest.demo.repository.OrderRepository;
import ptf.ep.perftest.demo.repository.ProductRepository;

import java.math.BigDecimal;
import java.util.Comparator;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

@Service
public class OrderService {

    private final OrderRepository orderRepository;
    private final ProductRepository productRepository;


    public OrderService(OrderRepository orderRepository, ProductRepository productRepository){
        this.orderRepository=orderRepository;
        this.productRepository=productRepository;
    }

    @Transactional
    public OrderResponse create(OrderRequest request){
        Set<Long> seen = new HashSet<>();
        for(OrderItemRequest item : request.items()){
            if(!seen.add(item.productId())){
                throw new BadRequestException("Validation failed: duplicate productId");
            }
        }

        List<OrderItemRequest> sorted = request.items().stream()
                .sorted(Comparator.comparing(OrderItemRequest::productId))
                .toList();


        Order order = new Order();
        order.setCustomer(request.customer());
        BigDecimal total = BigDecimal.ZERO;

        for(OrderItemRequest itemRequest : sorted){
            Product product = productRepository.findById(itemRequest.productId())
                    .orElseThrow(()-> new ResourceNotFoundException("Product not found: " + itemRequest.productId()));

            int updated = productRepository.decreaseStock(product.getId(), itemRequest.quantity());
            if(updated == 0){
                throw new InsufficientStockException("Insufficient stock for product: " + product.getId());
            }


            OrderItem item = new OrderItem();
            item.setProduct(product);
            item.setQuantity(itemRequest.quantity());
            item.setUnitPrice(product.getPrice());
            order.addItem(item);


            total = total.add(product.getPrice().multiply(BigDecimal.valueOf(itemRequest.quantity())));
        }


        order.setTotal(total);
        Order saved = orderRepository.save(order);
        return OrderResponse.from(saved);

    }


    @Transactional(readOnly = true)
    public OrderResponse getById(Long id){
        return orderRepository.findByIdWithItems(id)
                .map(OrderResponse::from)
                .orElseThrow(()-> new ResourceNotFoundException("Order not found"));
    }
}
