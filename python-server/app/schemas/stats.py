from app.schemas.common import CamelModel, Money


class CategoryStatResponse(CamelModel):
    """Ekvivalent dto/CategoryStatDto.java."""

    category: str
    product_count: int
    avg_price: Money
    total_stock: int