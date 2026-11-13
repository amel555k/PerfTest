from fastapi import FastAPI

from app.exceptions import register_exception_handlers
from app.routers import health, orders, products, stats

app = FastAPI(title="PerfTest Python server")

register_exception_handlers(app)

app.include_router(health.router)
app.include_router(products.router)
app.include_router(stats.router)
app.include_router(orders.router)