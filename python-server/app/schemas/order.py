from datetime import datetime

from pydantic import Field, field_validator

from app.schemas.common import CamelModel, Money


class OrderItemRequest(CamelModel):
    """Ekvivalent dto/OrderItemRequest.java (@NotNull, @Min(1))."""

    product_id: int
    quantity: int = Field(ge=1)


class OrderRequest(CamelModel):
    """Ekvivalent dto/OrderRequest.java (@NotBlank, @NotEmpty, @Valid)."""

    customer: str
    items: list[OrderItemRequest] = Field(min_length=1)

    @field_validator("customer")
    @classmethod
    def customer_not_blank(cls, value: str) -> str:
        if not value.strip():
            raise ValueError("must not be empty")
        return value


class OrderItemResponse(CamelModel):
    """Ekvivalent dto/OrderItemResponse.java."""

    product_id: int
    quantity: int
    unit_price: Money


class OrderResponse(CamelModel):
    """Ekvivalent dto/OrderResponse.java."""

    id: int
    customer: str
    total: Money
    created_at: datetime
    items: list[OrderItemResponse]