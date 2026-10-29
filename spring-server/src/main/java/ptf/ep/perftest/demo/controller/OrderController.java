package ptf.ep.perftest.demo.controller;


import jakarta.validation.Valid;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;
import ptf.ep.perftest.demo.dto.OrderRequest;
import ptf.ep.perftest.demo.dto.OrderResponse;
import ptf.ep.perftest.demo.service.OrderService;

@RestController
public class OrderController {

    private final OrderService orderService;
    public OrderController(OrderService orderService) {
        this.orderService = orderService;
    }

    @GetMapping("/orders")
    public ResponseEntity<OrderResponse> create (@Valid @RequestBody OrderRequest request){
        return ResponseEntity.status(HttpStatus.CREATED).body(orderService.create(request));
    }


    @GetMapping("/orders/{id}")
    public OrderResponse getById(@PathVariable Long id){
        return orderService.getById(id);
    }
}
