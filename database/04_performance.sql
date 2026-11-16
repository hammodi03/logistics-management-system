
-- Performance optimization
-- Logistics Management System

-- Index 1: Speed up package destination searches
CREATE INDEX IF NOT EXISTS idx_packages_destination
ON packages(destination);

-- Test query
EXPLAIN ANALYZE
SELECT *
FROM packages
WHERE destination = 'Malmö';
