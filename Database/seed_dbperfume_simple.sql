-- Small seed data for `dbperfume_simple`
-- Run after creating/importing dbperfume_simple schema.

USE dbperfume_simple;

-- Categories
INSERT INTO category (category_id, category_name, category_image) VALUES
(1, 'Nước hoa', 'perfume.jpg'),
(2, 'Giày thể thao', 'shoes.jpg');

-- Brands
INSERT INTO brand (brand_id, brand_name) VALUES
(1, 'Chanel'),
(2, 'Nike');

-- Capacities (sizes / dung tích)
INSERT INTO capacity (capacity_id, capacity_name) VALUES
(1, '50ml'),
(2, '100ml');

-- Accounts (admin + customers)
INSERT INTO account (account_id, account_name, account_password, account_email, account_phone, account_type, account_status) VALUES
(1, 'Admin', '21232f297a57a5a743894a0e4a801fc3', 'admin@example.com', '0123456789', 2, 1),
(2, 'Nguyen Van A', '5f4dcc3b5aa765d61d8327deb882cf99', 'user1@example.com', '0987654321', 0, 1);

-- Products
INSERT INTO product (product_id, product_name, product_category, product_brand, capacity_id, price, quantity, description, image, status, created_at) VALUES
(1, 'Nước hoa Floral 50ml', 1, 1, 1, 1200000, 10, 'Hương hoa nhẹ nhàng', 'perfume1.jpg', 1, NOW()),
(2, 'Giày Nike Air 42', 2, 2, NULL, 2500000, 5, 'Giày thể thao thoải mái', 'nike1.jpg', 1, NOW());

-- Product variants (optional)
INSERT INTO product_variants (product_id, capacity_id, price, quantity, status) VALUES
(1, 2, 2000000, 3, 1);

-- Orders (one sample)
INSERT INTO orders (order_id, order_code, account_id, order_date, total_amount, status, delivery_name, delivery_phone, delivery_address) VALUES
(1, 100001, 2, NOW(), 1200000, 0, 'Nguyen Van A', '0987654321', '123 Le Loi, Hanoi');

-- Order details (linked by order_code)
INSERT INTO order_detail (order_detail_id, order_code, product_id, variant_id, product_quantity, product_price, product_sale) VALUES
(1, 100001, 1, NULL, 1, 1200000, 0),
(2, 100001, 2, NULL, 1, 2500000, 0);

-- One sample payment
INSERT INTO payments (order_id, method, amount, trans_id, payment_date, status) VALUES
(100001, 'COD', 1200000, '', NOW(), 0);

-- One sample review
INSERT INTO evaluate (account_id, product_id, account_name, evaluate_rate, evaluate_content, evaluate_date, evaluate_status) VALUES
(2, 1, 'Nguyen Van A', 5, 'Rất thơm, ưng.', NOW(), 1);


