from app.schemas.common import CamelModel, Money


class ProductResponse(CamelModel):
    """Ekvivalent dto/ProductResponse.java."""

    id: int
    name: str
    description: str | None
    category: str
    price: Money
    stock: int