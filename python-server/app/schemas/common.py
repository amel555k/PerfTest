from decimal import Decimal
from typing import Annotated, Generic, TypeVar

from pydantic import BaseModel, ConfigDict, PlainSerializer
from pydantic.alias_generators import to_camel


class CamelModel(BaseModel):
    """Bazna klasa za sve DTO-ove.

    U Pythonu pišemo snake_case (unit_price), a u JSON-u mora biti camelCase
    (unitPrice), kako traži api.md. alias_generator to radi automatski.
    from_attributes=True dozvoljava pravljenje DTO-a direktno iz SQLAlchemy objekta.
    """

    model_config = ConfigDict(
        alias_generator=to_camel,
        populate_by_name=True,
        from_attributes=True,
    )

Money = Annotated[
    Decimal,
    PlainSerializer(lambda value: float(value), return_type=float, when_used="json"),
]

T = TypeVar("T")


class PageResponse(CamelModel, Generic[T]):
    """Ekvivalent dto/PageResponse.java."""

    content: list[T]
    page: int
    size: int
    total_elements: int
    total_pages: int