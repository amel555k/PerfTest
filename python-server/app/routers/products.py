from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session

from app.database import get_db
from app.schemas.common import PageResponse
from app.schemas.product import ProductResponse
from app.services import product_service

router = APIRouter()


@router.get("/products", response_model=PageResponse[ProductResponse])
def list_products(
    page: int = 0,
    size: int = 20,
    category: str | None = None,
    db: Session = Depends(get_db),
):
    return product_service.list_products(db, page, size, category)


@router.get("/products/search", response_model=PageResponse[ProductResponse])
def search_products(
    q: str,
    page: int = 0,
    size: int = 20,
    db: Session = Depends(get_db),
):
    return product_service.search(db, q, page, size)


@router.get("/products/{product_id}", response_model=ProductResponse)
def get_product(product_id: int, db: Session = Depends(get_db)):
    return product_service.get_by_id(db, product_id)