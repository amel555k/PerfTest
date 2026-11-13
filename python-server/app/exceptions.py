import logging

from fastapi import FastAPI, Request
from fastapi.exceptions import RequestValidationError
from fastapi.responses import JSONResponse
from starlette.exceptions import HTTPException as StarletteHTTPException

log = logging.getLogger(__name__)


class ResourceNotFoundException(Exception):
    """404: ekvivalent exception/ResourceNotFoundException.java."""


class InsufficientStockException(Exception):
    """409: ekvivalent exception/InsufficientStockException.java."""


class BadRequestException(Exception):
    """400: ekvivalent exception/BadRequestException.java."""


def _error(status: int, message: str) -> JSONResponse:
    # Format greške je uvijek {"error": "poruka"} (api.md)
    return JSONResponse(status_code=status, content={"error": message})


def _field_path(loc: tuple) -> str:
    """("body", "items", 0, "quantity") -> "items[0].quantity" """
    path = ""
    for part in loc[1:]:  # preskačemo "body"
        if isinstance(part, int):
            path += f"[{part}]"
        else:
            path += f".{part}" if path else str(part)
    return path


def _validation_message(err: dict) -> str:
    """Pretvara Pydantic poruku u poruku kakvu vraća Spring."""
    kind = err["type"]
    field = _field_path(err["loc"])
    if kind == "missing":
        if field in ("customer", "items"):
            return f"{field} must not be empty"
        return f"{field} must not be null"
    if kind == "too_short":
        return f"{field} must not be empty"
    if kind == "greater_than_equal":
        return f"{field} must be at least 1"
    if kind == "value_error":
        return f"{field} {err['msg'].removeprefix('Value error, ')}"
    return f"{field} {err['msg']}"


def register_exception_handlers(app: FastAPI) -> None:
    """Ekvivalent GlobalExceptionHandler.java."""

    @app.exception_handler(ResourceNotFoundException)
    async def handle_not_found(request: Request, exc: ResourceNotFoundException):
        return _error(404, str(exc))

    @app.exception_handler(InsufficientStockException)
    async def handle_insufficient_stock(
        request: Request, exc: InsufficientStockException
    ):
        return _error(409, str(exc))

    @app.exception_handler(BadRequestException)
    async def handle_bad_request(request: Request, exc: BadRequestException):
        return _error(400, str(exc))

    @app.exception_handler(RequestValidationError)
    async def handle_validation(request: Request, exc: RequestValidationError):
        errors = exc.errors()
        first = errors[0]
        loc = first["loc"]

        if loc and loc[0] == "body":
            # Tijelo zahtjeva (npr. POST /orders)
            if len(loc) == 1 or first["type"] in ("json_invalid", "model_attributes_type"):
                return _error(400, "Validation failed: malformed request body")
            return _error(400, "Validation failed: " + _validation_message(first))

        # Query ili path parametar (npr. ?q=...&page=abc)
        name = loc[-1] if loc else "unknown"
        if first["type"] == "missing":
            return _error(400, f"Missing required parameter: {name}")
        return _error(400, f"Invalid value for parameter: {name}")

    @app.exception_handler(StarletteHTTPException)
    async def handle_http(request: Request, exc: StarletteHTTPException):
        if exc.status_code == 404:
            return _error(404, "Resource not found")
        if exc.status_code == 405:
            return _error(405, "Method not allowed")
        return _error(exc.status_code, str(exc.detail))

    @app.exception_handler(Exception)
    async def handle_generic(request: Request, exc: Exception):
        log.error("Unexpected error", exc_info=exc)
        return _error(500, "Internal server error")