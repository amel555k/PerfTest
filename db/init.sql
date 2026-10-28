-- Šema
CREATE TABLE products (
    id          BIGSERIAL PRIMARY KEY,
    name        VARCHAR(255) NOT NULL,
    description TEXT,
    category    VARCHAR(100) NOT NULL,
    price       NUMERIC(10,2) NOT NULL CHECK (price >= 0),
    stock       INTEGER NOT NULL CHECK (stock >= 0)
);

CREATE TABLE orders (
    id          BIGSERIAL PRIMARY KEY,
    customer    VARCHAR(255) NOT NULL,
    total       NUMERIC(10,2) NOT NULL,
    created_at  TIMESTAMP NOT NULL DEFAULT now()
);

CREATE TABLE order_items (
    id          BIGSERIAL PRIMARY KEY,
    order_id    BIGINT NOT NULL REFERENCES orders(id),
    product_id  BIGINT NOT NULL REFERENCES products(id),
    quantity    INTEGER NOT NULL CHECK (quantity > 0),
    unit_price  NUMERIC(10,2) NOT NULL
);


CREATE INDEX idx_products_category ON products(category);
CREATE INDEX idx_order_items_order ON order_items(order_id);


INSERT INTO products (name, description, category, price, stock)
SELECT
    'Product ' || g,
    'Description for product ' || g,
    (ARRAY['electronics','books','clothing','home','sports'])[1 + (g % 5)],
    round((random() * 500 + 1)::numeric, 2),
    (random() * 1000)::int
FROM generate_series(1, 10000) g;