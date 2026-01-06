-- Chọn database trước khi chạy
USE `dbperfume`;

-- Xóa bảng product_variants cũ nếu có
DROP TABLE IF EXISTS `product_variants`;

-- Tạo bảng product_variants để lưu trữ nhiều kích cỡ cho mỗi sản phẩm
CREATE TABLE `product_variants` (
  `variant_id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `capacity_id` int NOT NULL,
  `variant_price` int DEFAULT NULL,
  `variant_quantity` int DEFAULT 0,
  `variant_status` tinyint(1) DEFAULT 1,
  `created_at` timestamp DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`variant_id`),
  UNIQUE KEY `unique_product_capacity` (`product_id`, `capacity_id`),
  KEY `idx_product_id` (`product_id`),
  KEY `idx_capacity_id` (`capacity_id`),
  FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`) ON DELETE CASCADE,
  FOREIGN KEY (`capacity_id`) REFERENCES `capacity` (`capacity_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Migrate dữ liệu hiện tại từ bảng product sang product_variants
INSERT INTO `product_variants` (`product_id`, `capacity_id`, `variant_quantity`, `variant_status`)
SELECT `product_id`, `capacity_id`, `product_quantity`, `product_status`
FROM `product`
WHERE `capacity_id` > 0;

-- Thêm cột để đánh dấu sản phẩm có nhiều variants (nếu chưa có)
ALTER TABLE `product` 
ADD COLUMN IF NOT EXISTS `has_variants` tinyint(1) DEFAULT 0 AFTER `product_status`;

-- Cập nhật flag has_variants cho các sản phẩm có variants
UPDATE `product` SET `has_variants` = 1 WHERE `product_id` IN (
    SELECT DISTINCT `product_id` FROM `product_variants`
);

-- Index để tối ưu truy vấn
CREATE INDEX idx_product_variants_active ON product_variants(product_id, variant_status);
CREATE INDEX idx_capacity_variants ON product_variants(capacity_id, variant_status);
