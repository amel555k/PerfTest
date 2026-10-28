# API Specification

All servers (Spring Boot, Python, C#) must implement **identical endpoints, field names and HTTP status codes**. The k6 scripts are the same for all three servers, so any difference will break the tests.

## General

- **Base URL:** `http://localhost:<port>`
  - Spring: `8081`
  - Python: `8082`
  - C#: `8083`
- **Format:** JSON (`Content-Type: application/json`), UTF-8
- **Field names:** `camelCase` (e.g. `unitPrice`, `createdAt`)
- **Money:** a number with two decimals (e.g. `199.99`), not a string
- **Date/time:** ISO 8601 (e.g. `2026-10-28T14:30:00`)
- **Authentication:** none
- **IDs:** integers (`long`)

## Data Model

Matches the tables in `db/init.sql`.

**Product**
```json
{
  "id": 1,
  "name": "Product 1",
  "description": "Description for product 1",
  "category": "electronics",
  "price": 199.99,
  "stock": 500
}
```

**Order**
```json
{
  "id": 1,
  "customer": "Amel",
  "total": 399.98,
  "createdAt": "2026-10-28T14:30:00",
  "items": [
    {
      "productId": 1,
      "quantity": 2,
      "unitPrice": 199.99
    }
  ]
}
```

## Endpoints

### 1. Health check

`GET /health`

Used to verify that the server is up (smoke test, Docker healthcheck).

**200 OK**
```json
{ "status": "UP" }
```

### 2. List products (pagination)

`GET /products`

| Query parameter | Type | Default | Description |
|---|---|---|---|
| `page` | int | `0` | Page number (zero-based) |
| `size` | int | `20` | Items per page (max `100`) |
| `category` | string | / | Optional category filter |

Sorting is fixed: by `id` ascending.

**200 OK**
```json
{
  "content": [
    { "id": 1, "name": "Product 1", "description": "...", "category": "books", "price": 12.50, "stock": 100 }
  ],
  "page": 0,
  "size": 20,
  "totalElements": 10000,
  "totalPages": 500
}
```

**400 Bad Request** if `page < 0`, `size < 1` or `size > 100`.

### 3. Get a single product

`GET /products/{id}`

**200 OK**: a `Product` object (see above)

**404 Not Found**
```json
{ "error": "Product not found" }
```

### 4. Search products ("heavy" endpoint)

`GET /products/search`

Text search over name and description (case-insensitive, `LIKE '%q%'`), without an index, intentionally an expensive operation.

| Query parameter | Type | Default | Description |
|---|---|---|---|
| `q` | string | required | Text to search for |
| `page` | int | `0` | Page number |
| `size` | int | `20` | Items per page (max `100`) |

The response has the same format as `GET /products` (paginated object).

**400 Bad Request** if `q` is missing or empty.

### 5. Category statistics (aggregation)

`GET /stats/categories`

For each category, returns the number of products, the average price and the total stock. Sorted by `category` ascending.

**200 OK**
```json
[
  { "category": "books", "productCount": 2000, "avgPrice": 250.43, "totalStock": 999120 },
  { "category": "clothing", "productCount": 2000, "avgPrice": 251.10, "totalStock": 1001230 }
]
```

`avgPrice` is rounded to 2 decimals.

### 6. Create an order

`POST /orders`

Must run in a **single transaction**: check stock, decrease stock, insert into `orders` and `order_items`, calculate `total`. If anything fails, nothing may be written.

**Request**
```json
{
  "customer": "Amel",
  "items": [
    { "productId": 1, "quantity": 2 },
    { "productId": 5, "quantity": 1 }
  ]
}
```

Rules:
- `customer` is required and must not be empty
- `items` is required and must contain at least 1 item
- `quantity` must be `>= 1`
- `unitPrice` is **taken from the database** (the product's current price), not from the request
- `total` = sum of `unitPrice * quantity`
- If the same `productId` appears more than once in the request, it is treated as an error (`400`)

**201 Created**: an `Order` object (see above, with `id`, `total`, `createdAt` and `items` filled in)

**400 Bad Request**: invalid input
```json
{ "error": "Validation failed: items must not be empty" }
```

**404 Not Found**: product does not exist
```json
{ "error": "Product not found: 999" }
```

**409 Conflict**: insufficient stock
```json
{ "error": "Insufficient stock for product: 5" }
```

### 7. Get a single order

`GET /orders/{id}`

**200 OK**: an `Order` object with the `items` list

**404 Not Found**
```json
{ "error": "Order not found" }
```

## Summary

| Method | Path | Purpose | Status codes |
|---|---|---|---|
| GET | `/health` | Health check | 200 |
| GET | `/products` | List with pagination and filter | 200, 400 |
| GET | `/products/{id}` | Single product | 200, 404 |
| GET | `/products/search` | Text search | 200, 400 |
| GET | `/stats/categories` | Aggregation | 200 |
| POST | `/orders` | Create order | 201, 400, 404, 409 |
| GET | `/orders/{id}` | Single order | 200, 404 |

## Rules for All Servers

- The error format is always `{ "error": "message" }`
- All servers use the **same database and the same `db/init.sql`**; the ORM only maps onto the existing tables (it must not generate the schema)
- Connection pool: the same maximum number of connections on all servers (`10`), to keep the comparison fair
- No caching (no in-memory cache of any kind), so that real database work is measured
- Logging at `INFO` level or lower during tests, since excessive logging affects the results
- All servers have the same CPU/RAM limits in Docker (e.g. `cpus: 2`, `memory: 1g`)

## Notes for k6

- Test `GET /products/{id}` with random IDs from 1 to 10000
- For `POST /orders`, use random products and small quantities (1-3) so that stock does not run out too early
- Reset the database before every test (`docker compose down -v && docker compose up -d`) so that all servers start from the same state
- Test one server at a time, never all three simultaneously