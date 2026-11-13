from sqlalchemy import func, or_, select, update
from sqlalchemy.orm import Session

from app.models import Product


def find_by_id(db: Session, product_id: int) -> Product | None:
    """Ekvivalent findById."""
    return db.get(Product, product_id)


def find_page(
    db: Session, page: int, size: int, category: str | None
) -> tuple[list[Product], int]:
    """Lista proizvoda sa paginacijom, sortirano po id rastuće.

    Vraća (proizvodi na stranici, ukupan broj proizvoda).
    Ekvivalent findAll(pageable) i findByCategoryOrderByIdAsc.
    """
    filters = []
    if category:
        filters.append(Product.category == category)

    total = db.scalar(select(func.count()).select_from(Product).where(*filters))
    items = db.scalars(
        select(Product)
        .where(*filters)
        .order_by(Product.id.asc())
        .offset(page * size)
        .limit(size)
    ).all()
    return list(items), total


def search(db: Session, q: str, page: int, size: int) -> tuple[list[Product], int]:
    """Pretraga po name i description, bez indeksa (namjerno "težak" upit).

    ilike = LIKE bez razlike velikih i malih slova.
    Ekvivalent searchByNameOrDescription.
    """
    pattern = f"%{q}%"
    condition = or_(Product.name.ilike(pattern), Product.description.ilike(pattern))

    total = db.scalar(select(func.count()).select_from(Product).where(condition))
    items = db.scalars(
        select(Product)
        .where(condition)
        .order_by(Product.id.asc())
        .offset(page * size)
        .limit(size)
    ).all()
    return list(items), total


def category_stats(db: Session):
    """Agregacija po kategorijama. Ekvivalent getCategoryStats."""
    stmt = (
        select(
            Product.category,
            func.count(Product.id),
            func.avg(Product.price),
            func.sum(Product.stock),
        )
        .group_by(Product.category)
        .order_by(Product.category.asc())
    )
    return db.execute(stmt).all()


def decrease_stock(db: Session, product_id: int, qty: int) -> int:
    """Atomično smanjuje zalihu, samo ako ima dovoljno.

    Vraća broj izmijenjenih redova: 0 znači da nema dovoljno zalihe.
    Ekvivalent decreaseStock (UPDATE ... WHERE stock >= qty).
    """
    stmt = (
        update(Product)
        .where(Product.id == product_id, Product.stock >= qty)
        .values(stock=Product.stock - qty)
        .execution_options(synchronize_session=False)
    )
    return db.execute(stmt).rowcount