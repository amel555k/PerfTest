from decimal import Decimal

from sqlalchemy.orm import Session

from app.exceptions import (
    BadRequestException,
    InsufficientStockException,
    ResourceNotFoundException,
)
from app.models import Order, OrderItem
from app.repositories import order_repository, product_repository
from app.schemas.order import OrderRequest, OrderResponse


def create(db: Session, request: OrderRequest) -> OrderResponse:
    """Pravi narudžbu u JEDNOJ transakciji (ako išta pukne, ništa se ne upiše)."""

    
    seen: set[int] = set()
    for item in request.items:
        if item.product_id in seen:
            raise BadRequestException("Validation failed: duplicate productId")
        seen.add(item.product_id)


    sorted_items = sorted(request.items, key=lambda i: i.product_id)

    order = Order(customer=request.customer, total=Decimal("0"))
    total = Decimal("0")

    try:
        for item in sorted_items:
            product = product_repository.find_by_id(db, item.product_id)
            if product is None:
                raise ResourceNotFoundException(
                    f"Product not found: {item.product_id}"
                )

            updated = product_repository.decrease_stock(db, product.id, item.quantity)
            if updated == 0:
                raise InsufficientStockException(
                    f"Insufficient stock for product: {product.id}"
                )

            order.items.append(
                OrderItem(
                    product_id=product.id,
                    quantity=item.quantity,
                    unit_price=product.price,
                )
            )
            total += product.price * item.quantity

        order.total = total
        order_repository.save(db, order)
        db.commit()
    except Exception:
        db.rollback()
        raise

    return OrderResponse.model_validate(order)


def get_by_id(db: Session, order_id: int) -> OrderResponse:
    order = order_repository.find_by_id_with_items(db, order_id)
    if order is None:
        raise ResourceNotFoundException("Order not found")
    return OrderResponse.model_validate(order)