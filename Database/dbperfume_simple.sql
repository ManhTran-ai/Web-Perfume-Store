-- Simplified schema for GuhaStore (dbperfume_simple)
-- Minimal tables with FK and indexes suitable for a C# (.NET) student project.

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

CREATE DATABASE IF NOT EXISTS dbperfume_simple
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;
USE dbperfume_simple;

-- Accounts (users)
CREATE TABLE account (
  account_id INT NOT NULL AUTO_INCREMENT,
  account_name VARCHAR(255) NOT NULL,
  account_password VARCHAR(255) NOT NULL,
  account_email VARCHAR(255) NOT NULL,
  account_phone VARCHAR(50),
  account_type TINYINT NOT NULL DEFAULT 0,
  account_status TINYINT NOT NULL DEFAULT 1,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (account_id),
  UNIQUE KEY ux_account_email (account_email)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Category
CREATE TABLE category (
  category_id INT NOT NULL AUTO_INCREMENT,
  category_name VARCHAR(100) NOT NULL,
  category_image VARCHAR(255),
  PRIMARY KEY (category_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Brand
CREATE TABLE brand (
  brand_id INT NOT NULL AUTO_INCREMENT,
  brand_name VARCHAR(100) NOT NULL,
  PRIMARY KEY (brand_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Capacity (size/dung tích) optional
CREATE TABLE capacity (
  capacity_id INT NOT NULL AUTO_INCREMENT,
  capacity_name VARCHAR(50) NOT NULL,
  PRIMARY KEY (capacity_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Product (flattened simple model)
CREATE TABLE product (
  product_id INT NOT NULL AUTO_INCREMENT,
  product_name VARCHAR(150) NOT NULL,
  product_category INT NOT NULL,
  product_brand INT,
  capacity_id INT,
  price INT NOT NULL,
  quantity INT NOT NULL DEFAULT 0,
  description TEXT,
  image VARCHAR(255),
  status TINYINT NOT NULL DEFAULT 1,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (product_id),
  KEY idx_product_category (product_category),
  KEY idx_product_brand (product_brand),
  KEY idx_product_capacity (capacity_id),
  CONSTRAINT fk_product_category FOREIGN KEY (product_category) REFERENCES category(category_id),
  CONSTRAINT fk_product_brand FOREIGN KEY (product_brand) REFERENCES brand(brand_id),
  CONSTRAINT fk_product_capacity FOREIGN KEY (capacity_id) REFERENCES capacity(capacity_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Optional product variants table (if you later need sizes per product)
CREATE TABLE product_variants (
  variant_id INT NOT NULL AUTO_INCREMENT,
  product_id INT NOT NULL,
  capacity_id INT,
  price INT NOT NULL,
  quantity INT NOT NULL DEFAULT 0,
  status TINYINT NOT NULL DEFAULT 1,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (variant_id),
  UNIQUE KEY ux_product_capacity (product_id, capacity_id),
  KEY idx_variant_product (product_id),
  CONSTRAINT fk_variant_product FOREIGN KEY (product_id) REFERENCES product(product_id) ON DELETE CASCADE,
  CONSTRAINT fk_variant_capacity FOREIGN KEY (capacity_id) REFERENCES capacity(capacity_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Orders (delivery info embedded)
CREATE TABLE orders (
  order_id INT NOT NULL AUTO_INCREMENT,
  order_code INT NOT NULL,
  account_id INT NOT NULL,
  order_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  total_amount INT NOT NULL,
  status TINYINT NOT NULL DEFAULT 0,
  delivery_name VARCHAR(150),
  delivery_phone VARCHAR(50),
  delivery_address TEXT,
  PRIMARY KEY (order_id),
  UNIQUE KEY ux_orders_code (order_code),
  KEY idx_orders_account (account_id),
  CONSTRAINT fk_orders_account FOREIGN KEY (account_id) REFERENCES account(account_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Order items (detail) referencing order_id
CREATE TABLE order_detail (
  order_detail_id INT NOT NULL AUTO_INCREMENT,
  order_code INT NOT NULL,
  product_id INT,
  variant_id INT,
  product_quantity INT NOT NULL,
  product_price INT NOT NULL,
  product_sale INT DEFAULT 0,
  PRIMARY KEY (order_detail_id),
  KEY idx_order_items_order (order_code),
  CONSTRAINT fk_order_items_order FOREIGN KEY (order_code) REFERENCES orders(order_code) ON DELETE CASCADE,
  CONSTRAINT fk_order_items_product FOREIGN KEY (product_id) REFERENCES product(product_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Inventory log (simple)
CREATE TABLE inventory_log (
  inventory_id INT NOT NULL AUTO_INCREMENT,
  product_id INT NOT NULL,
  account_id INT,
  change_qty INT NOT NULL,
  note VARCHAR(255),
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (inventory_id),
  KEY idx_inv_product (product_id),
  CONSTRAINT fk_inv_product FOREIGN KEY (product_id) REFERENCES product(product_id),
  CONSTRAINT fk_inv_account FOREIGN KEY (account_id) REFERENCES account(account_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Reviews
CREATE TABLE evaluate (
  evaluate_id INT NOT NULL AUTO_INCREMENT,
  account_id INT NOT NULL,
  product_id INT NOT NULL,
  account_name VARCHAR(100),
  evaluate_rate TINYINT NOT NULL,
  evaluate_content TEXT,
  evaluate_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  evaluate_status TINYINT NOT NULL DEFAULT 1,
  PRIMARY KEY (evaluate_id),
  KEY idx_eval_product (product_id),
  CONSTRAINT fk_evaluate_account FOREIGN KEY (account_id) REFERENCES account(account_id),
  CONSTRAINT fk_evaluate_product FOREIGN KEY (product_id) REFERENCES product(product_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Articles + comments (simplified)
CREATE TABLE article (
  article_id INT NOT NULL AUTO_INCREMENT,
  article_author VARCHAR(100),
  article_title VARCHAR(255),
  article_summary TEXT,
  article_content TEXT,
  article_image VARCHAR(255),
  article_date DATE,
  article_status TINYINT DEFAULT 1,
  PRIMARY KEY (article_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE comment (
  comment_id INT NOT NULL AUTO_INCREMENT,
  article_id INT NOT NULL,
  comment_name VARCHAR(100),
  comment_email VARCHAR(100),
  comment_content TEXT,
  comment_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  comment_status TINYINT DEFAULT 1,
  PRIMARY KEY (comment_id),
  CONSTRAINT fk_comment_article FOREIGN KEY (article_id) REFERENCES article(article_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Payments (single table for different providers)
CREATE TABLE payments (
  payment_id INT NOT NULL AUTO_INCREMENT,
  order_id INT NOT NULL,
  method VARCHAR(50),
  amount INT NOT NULL,
  trans_id VARCHAR(100),
  payment_date TIMESTAMP,
  status TINYINT DEFAULT 0,
  PRIMARY KEY (payment_id),
  KEY idx_payments_order (order_id),
  CONSTRAINT fk_payments_order FOREIGN KEY (order_id) REFERENCES orders(order_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;


