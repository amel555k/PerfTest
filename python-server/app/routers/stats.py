from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session

from app.database import get_db
from app.schemas.stats import CategoryStatResponse
from app.services import product_service

router = APIRouter()


@router.get("/stats/categories", response_model=list[CategoryStatResponse])
def category_stats(db: Session = Depends(get_db)):
    return product_service.category_stats(db)