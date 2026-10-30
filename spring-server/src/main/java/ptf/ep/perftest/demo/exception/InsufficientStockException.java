package ptf.ep.perftest.demo.exception;

public class InsufficientStockException extends RuntimeException {
    public InsufficientStockException(String message){
        super(message);
    }
}
