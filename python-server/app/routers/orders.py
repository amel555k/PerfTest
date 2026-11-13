from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session

from app.database import get_db
from app.schemas.order import OrderRequest, OrderResponse
from app.services import order_service

router = APIRouter()


@router.post("/orders", response_model=OrderResponse, status_code=201)
def create_order(request: OrderRequest, db: Session = Depends(get_db)):
    return order_service.create(db, request)


@router.get("/orders/{order_id}", response_model=OrderResponse)
def get_order(order_id: int, db: Session = Depends(get_db)):
    return order_service.get_by_id(db, order_id)