from sqlalchemy import create_engine
from sqlalchemy.orm import DeclarativeBase, sessionmaker

from app.config import DB_URL, POOL_SIZE


engine = create_engine(DB_URL, pool_size=POOL_SIZE, max_overflow=0)

SessionLocal = sessionmaker(bind=engine, autoflush=False, expire_on_commit=False)

class Base(DeclarativeBase):
    """Bazna klasa za sve entitete (ekvivalent @Entity)."""

def get_db():
    """Daje jednu sesiju po zahtjevu i zatvara je kad zahtjev završi.

    Ekvivalent spring.jpa.open-in-view=false: sesija živi samo za jedan zahtjev.
    NAMJERNO nema Base.metadata.create_all(): šemu pravi samo db/init.sql.
    """
    with SessionLocal() as db:
        yield db