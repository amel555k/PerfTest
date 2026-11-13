import math
from decimal import ROUND_HALF_UP, Decimal

from sqlalchemy.orm import Session

from app.config import MAX_PAGE_SIZE
from app.exceptions import BadRequestException, ResourceNotFoundException
from app.repositories import product_repository
from app.schemas.common import PageResponse
from app.schemas.product import ProductResponse
from app.schemas.stats import CategoryStatResponse


def _validate_paging(page: int, size: int) -> None:
    if page < 0:
        raise BadRequestException("Invalid value for parameter: page")
    if size < 1 or size > MAX_PAGE_SIZE:
        raise BadRequestException("Invalid value for parameter: size")


def _to_page(
    products: list, total: int, page: int, size: int
) -> PageResponse[ProductResponse]:
    return PageResponse[ProductResponse](
        content=[ProductResponse.model_validate(p) for p in products],
        page=page,
        size=size,
        total_elements=total,
        total_pages=math.ceil(total / size),
    )


def list_products(
    db: Session, page: int, size: int, category: str | None
) -> PageResponse[ProductResponse]:
    _validate_paging(page, size)
    products, total = product_repository.find_page(db, page, size, category)
    return _to_page(products, total, page, size)


def get_by_id(db: Session, product_id: int) -> ProductResponse:
    product = product_repository.find_by_id(db, product_id)
    if product is None:
        raise ResourceNotFoundException("Product not found")
    return ProductResponse.model_validate(product)


def search(db: Session, q: str, page: int, size: int) -> PageResponse[ProductResponse]:
    if q is None or not q.strip():
        raise BadRequestException("Missing required parameter: q")
    _validate_paging(page, size)
    products, total = product_repository.search(db, q, page, size)
    return _to_page(products, total, page, size)


def category_stats(db: Session) -> list[CategoryStatResponse]:
    rows = product_repository.category_stats(db)
    cent = Decimal("0.01")
    return [
        CategoryStatResponse(
            category=category,
            product_count=count,
            avg_price=(avg or Decimal("0")).quantize(cent, rounding=ROUND_HALF_UP),
            total_stock=int(total_stock or 0),
        )
        for category, count, avg, total_stock in rows
    ]