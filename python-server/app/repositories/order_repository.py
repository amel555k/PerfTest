from sqlalchemy import select
from sqlalchemy.orm import Session, selectinload

from app.models import Order


def save(db: Session, order: Order) -> Order:
    """Ekvivalent save: ubacuje order i njegove stavke (cascade).

    flush() šalje INSERT u bazu da dobijemo id, ali još ne potvrđuje transakciju.
    """
    db.add(order)
    db.flush()
    return order


def find_by_id_with_items(db: Session, order_id: int) -> Order | None:
    """Ekvivalent findByIdWithItems (LEFT JOIN FETCH o.items)."""
    stmt = (
        select(Order).where(Order.id == order_id).options(selectinload(Order.items))
    )
    return db.scalars(stmt).first()