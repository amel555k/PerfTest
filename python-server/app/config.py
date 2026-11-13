import os

DB_HOST = os.getenv("DB_HOST", "localhost")
DB_PORT = os.getenv("DB_PORT", "5432")
DB_NAME = os.getenv("DB_NAME", "shop")
DB_USER = os.getenv("DB_USER", "shop")
DB_PASSWORD = os.getenv("DB_PASSWORD", "shop")

DB_URL = f"postgresql+psycopg://{DB_USER}:{DB_PASSWORD}@{DB_HOST}:{DB_PORT}/{DB_NAME}"

POOL_SIZE = int(os.getenv("DB_POOL_SIZE", "10"))

MAX_PAGE_SIZE = 100