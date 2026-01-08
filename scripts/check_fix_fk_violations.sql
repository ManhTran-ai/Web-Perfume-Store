-- SQL script to FIND and PREPARE fixes for FK violations in `dbperfume` schema.
-- Usage:
-- 1) Run the SELECT queries to see orphaned child rows.
-- 2) Review the generated cleanup statements (they are produced by SELECT CONCAT queries).
-- 3) Either run the commented-safe fixes or adapt them manually.
--
-- This script intentionally DOES NOT automatically DELETE data. It produces reports
-- and generates DELETE/UPDATE statements which are printed for review.

USE `dbperfume`;

-- 1) Report orphaned references (child rows whose parent does not exist)
-- Orders referencing missing accounts
SELECT 'orders.account_id_orphans' AS check_name, o.order_id, o.account_id
FROM orders o
LEFT JOIN account a ON o.account_id = a.account_id
WHERE a.account_id IS NULL;

-- Orders referencing missing delivery
SELECT 'orders.delivery_id_orphans' AS check_name, o.order_id, o.delivery_id
FROM orders o
LEFT JOIN delivery d ON o.delivery_id = d.delivery_id
WHERE d.delivery_id IS NULL;

-- Customers referencing missing accounts
SELECT 'customer.account_id_orphans' AS check_name, c.customer_id, c.account_id
FROM customer c
LEFT JOIN account a ON c.account_id = a.account_id
WHERE a.account_id IS NULL;

-- Delivery referencing missing accounts
SELECT 'delivery.account_id_orphans' AS check_name, d.delivery_id, d.account_id
FROM delivery d
LEFT JOIN account a ON d.account_id = a.account_id
WHERE a.account_id IS NULL;

-- Inventory referencing missing accounts
SELECT 'inventory.account_id_orphans' AS check_name, i.inventory_id, i.account_id
FROM inventory i
LEFT JOIN account a ON i.account_id = a.account_id
WHERE a.account_id IS NULL;

-- Inventory detail referencing missing products
SELECT 'inventory_detail.product_id_orphans' AS check_name, id.inventory_detail_id, id.product_id
FROM inventory_detail id
LEFT JOIN product p ON id.product_id = p.product_id
WHERE p.product_id IS NULL;

-- Order_detail referencing missing products (note: order_detail uses order_code, not order_id)
SELECT 'order_detail.product_id_orphans' AS check_name, od.order_detail_id, od.product_id
FROM order_detail od
LEFT JOIN product p ON od.product_id = p.product_id
WHERE p.product_id IS NULL;

-- Evaluate referencing missing accounts and products
SELECT 'evaluate.account_id_orphans' AS check_name, e.evaluate_id, e.account_id
FROM evaluate e
LEFT JOIN account a ON e.account_id = a.account_id
WHERE a.account_id IS NULL;

SELECT 'evaluate.product_id_orphans' AS check_name, e.evaluate_id, e.product_id
FROM evaluate e
LEFT JOIN product p ON e.product_id = p.product_id
WHERE p.product_id IS NULL;

-- Comment referencing missing article
SELECT 'comment.article_id_orphans' AS check_name, c.comment_id, c.article_id
FROM comment c
LEFT JOIN article a ON c.article_id = a.article_id
WHERE a.article_id IS NULL;

-- Product referencing capacity/category/brand
SELECT 'product.capacity_id_orphans' AS check_name, p.product_id, p.capacity_id
FROM product p
LEFT JOIN capacity c ON p.capacity_id = c.capacity_id
WHERE c.capacity_id IS NULL;

SELECT 'product.category_id_orphans' AS check_name, p.product_id, p.product_category
FROM product p
LEFT JOIN category cat ON p.product_category = cat.category_id
WHERE cat.category_id IS NULL;

SELECT 'product.brand_id_orphans' AS check_name, p.product_id, p.product_brand
FROM product p
LEFT JOIN brand b ON p.product_brand = b.brand_id
WHERE b.brand_id IS NULL;

-- Additional checks for "code" style links (no FK currently)
-- inventory_detail.inventory_code should exist in inventory.inventory_code
SELECT 'inventory_detail.inventory_code_orphans' AS check_name, id.inventory_detail_id, id.inventory_code
FROM inventory_detail id
LEFT JOIN inventory i ON id.inventory_code = i.inventory_code
WHERE i.inventory_code IS NULL;

-- order_detail.order_code should exist in orders.order_code
SELECT 'order_detail.order_code_orphans' AS check_name, od.order_detail_id, od.order_code
FROM order_detail od
LEFT JOIN orders o ON od.order_code = o.order_code
WHERE o.order_code IS NULL;

-- vnpay.order_code should exist in orders.order_code
SELECT 'vnpay.order_code_orphans' AS check_name, v.vnp_id, v.order_code
FROM vnpay v
LEFT JOIN orders o ON v.order_code = o.order_code
WHERE o.order_code IS NULL;

-- momo.order_code should exist in orders.order_code
SELECT 'momo.order_code_orphans' AS check_name, m.momo_id, m.order_code
FROM momo m
LEFT JOIN orders o ON m.order_code = o.order_code
WHERE o.order_code IS NULL;


-- 2) Generate cleanup statements (as text) so you can review then run them.
-- Example: produce DELETE statements for orphaned order rows (review before executing)
SELECT CONCAT('-- DELETE orphan orders: order_id=', o.order_id, CHAR(10),
              'DELETE FROM orders WHERE order_id=', o.order_id, ';') AS stmt
FROM orders o
LEFT JOIN account a ON o.account_id = a.account_id
WHERE a.account_id IS NULL;

-- Generate DELETE for order rows with missing delivery
SELECT CONCAT('-- DELETE orphan orders (missing delivery): order_id=', o.order_id, CHAR(10),
              'DELETE FROM orders WHERE order_id=', o.order_id, ';') AS stmt
FROM orders o
LEFT JOIN delivery d ON o.delivery_id = d.delivery_id
WHERE d.delivery_id IS NULL;

-- Generate DELETE for customers with missing account
SELECT CONCAT('-- DELETE orphan customer: customer_id=', c.customer_id, CHAR(10),
              'DELETE FROM customer WHERE customer_id=', c.customer_id, ';') AS stmt
FROM customer c
LEFT JOIN account a ON c.account_id = a.account_id
WHERE a.account_id IS NULL;

-- Generate DELETE for delivery with missing account
SELECT CONCAT('-- DELETE orphan delivery: delivery_id=', d.delivery_id, CHAR(10),
              'DELETE FROM delivery WHERE delivery_id=', d.delivery_id, ';') AS stmt
FROM delivery d
LEFT JOIN account a ON d.account_id = a.account_id
WHERE a.account_id IS NULL;

-- Generate DELETE for inventory_detail referencing missing product
SELECT CONCAT('-- DELETE orphan inventory_detail: id=', id.inventory_detail_id, CHAR(10),
              'DELETE FROM inventory_detail WHERE inventory_detail_id=', id.inventory_detail_id, ';') AS stmt
FROM inventory_detail id
LEFT JOIN product p ON id.product_id = p.product_id
WHERE p.product_id IS NULL;

-- Generate DELETE for order_detail referencing missing product
SELECT CONCAT('-- DELETE orphan order_detail: id=', od.order_detail_id, CHAR(10),
              'DELETE FROM order_detail WHERE order_detail_id=', od.order_detail_id, ';') AS stmt
FROM order_detail od
LEFT JOIN product p ON od.product_id = p.product_id
WHERE p.product_id IS NULL;

-- Generate DELETE for evaluate referencing missing product or account
SELECT CONCAT('-- DELETE orphan evaluate (missing product): id=', e.evaluate_id, CHAR(10),
              'DELETE FROM evaluate WHERE evaluate_id=', e.evaluate_id, ';') AS stmt
FROM evaluate e
LEFT JOIN product p ON e.product_id = p.product_id
WHERE p.product_id IS NULL;

SELECT CONCAT('-- DELETE orphan evaluate (missing account): id=', e.evaluate_id, CHAR(10),
              'DELETE FROM evaluate WHERE evaluate_id=', e.evaluate_id, ';') AS stmt
FROM evaluate e
LEFT JOIN account a ON e.account_id = a.account_id
WHERE a.account_id IS NULL;

-- Generate DELETE for comment referencing missing article
SELECT CONCAT('-- DELETE orphan comment: id=', c.comment_id, CHAR(10),
              'DELETE FROM comment WHERE comment_id=', c.comment_id, ';') AS stmt
FROM comment c
LEFT JOIN article a ON c.article_id = a.article_id
WHERE a.article_id IS NULL;

-- Generate DELETE for product invalid references (capacity/category/brand)
SELECT CONCAT('-- CHECK product missing capacity: product_id=', p.product_id, CHAR(10),
              '/* consider deleting or updating product ', p.product_id, ' */') AS stmt
FROM product p
LEFT JOIN capacity c ON p.capacity_id = c.capacity_id
WHERE c.capacity_id IS NULL;

SELECT CONCAT('-- CHECK product missing category: product_id=', p.product_id, CHAR(10),
              '/* consider deleting or updating product ', p.product_id, ' */') AS stmt
FROM product p
LEFT JOIN category cat ON p.product_category = cat.category_id
WHERE cat.category_id IS NULL;

SELECT CONCAT('-- CHECK product missing brand: product_id=', p.product_id, CHAR(10),
              '/* consider deleting or updating product ', p.product_id, ' */') AS stmt
FROM product p
LEFT JOIN brand b ON p.product_brand = b.brand_id
WHERE b.brand_id IS NULL;

-- Generate checks for code-link orphans (inventory_code, order_code)
SELECT CONCAT('-- DELETE orphan inventory_detail by inventory_code: id=', id.inventory_detail_id, CHAR(10),
              'DELETE FROM inventory_detail WHERE inventory_detail_id=', id.inventory_detail_id, ';') AS stmt
FROM inventory_detail id
LEFT JOIN inventory i ON id.inventory_code = i.inventory_code
WHERE i.inventory_code IS NULL;

SELECT CONCAT('-- DELETE orphan order_detail by order_code: id=', od.order_detail_id, CHAR(10),
              'DELETE FROM order_detail WHERE order_detail_id=', od.order_detail_id, ';') AS stmt
FROM order_detail od
LEFT JOIN orders o ON od.order_code = o.order_code
WHERE o.order_code IS NULL;

SELECT CONCAT('-- DELETE orphan vnpay by order_code: vnp_id=', v.vnp_id, CHAR(10),
              'DELETE FROM vnpay WHERE vnp_id=', v.vnp_id, ';') AS stmt
FROM vnpay v
LEFT JOIN orders o ON v.order_code = o.order_code
WHERE o.order_code IS NULL;

SELECT CONCAT('-- DELETE orphan momo by order_code: momo_id=', m.momo_id, CHAR(10),
              'DELETE FROM momo WHERE momo_id=', m.momo_id, ';') AS stmt
FROM momo m
LEFT JOIN orders o ON m.order_code = o.order_code
WHERE o.order_code IS NULL;


-- 3) Helpful optional fixes (commented). Review BEFORE running.
-- (A) Create a placeholder account with id=0 to map rows that use account_id=0:
SELECT 'To create a placeholder account with account_id = 0 (optional):' AS info;
SELECT 'INSERT INTO account (account_id, account_name, account_password, account_email, account_type, account_status) VALUES (0, ''SYSTEM_PLACEHOLDER'', '''', '''', 0, 0);' AS stmt;

-- (B) Orphan cleanup example (commented) - run only after review:
SELECT 'Example cleanup (review) - these statements are generated above. Run them manually after review.' AS info;

-- End of script


