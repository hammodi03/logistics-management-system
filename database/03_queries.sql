
-- Query 1: All packages with customer information
SELECT 
    p.package_id,
    c.name AS customer_name,
    c.email,
    p.destination,
    p.status
FROM packages p 
JOIN customers c 
    ON p.customer_id = c.customer_id;

-- Query 2: Delivery overview with customers and drivers
SELECT
    d.delivery_id,
    d.delivery_status,
    c.name AS customer_name,
    dr.name AS driver_name,
    p.destination
FROM deliveries d 
JOIN packages p 
    ON d.package_id = p.package_id
JOIN customers c 
    ON p.customer_id = c.customer_id
JOIN drivers dr 
    ON dr.driver_id = d.driver_id
ORDER BY d.delivery_id;

-- Query 3: Total packages per customer
SELECT
    c.name AS customer_name,
    COUNT(*) AS total_packages
FROM customers c
JOIN packages p
    ON c.customer_id = p.customer_id
GROUP BY c.customer_id, c.name
ORDER BY total_packages DESC;


-- Query 4: Customers with more than two packages
SELECT
    c.name AS customer_name,
    COUNT(*) AS total_packages
FROM customers c
JOIN packages p
    ON c.customer_id = p.customer_id
GROUP BY c.customer_id, c.name
HAVING COUNT(*) > 2
ORDER BY total_packages DESC;

-- Query 5: Completed deliveries per driver
SELECT
    dr.name AS driver_name,
    COUNT(*) FILTER (WHERE d.delivery_status = 'Delivered') AS completed_deliveries
FROM drivers dr
LEFT JOIN deliveries d
    ON dr.driver_id = d.driver_id
GROUP BY dr.driver_id, dr.name
ORDER BY completed_deliveries DESC;

-- Query 6: Delivery status history
SELECT
    d.delivery_id,
    c.name AS customer_name,
    de.status,
    de.event_time
FROM delivery_events de
JOIN deliveries d
    ON de.delivery_id = d.delivery_id
JOIN packages p
    ON p.package_id = d.package_id
JOIN customers c
    ON c.customer_id = p.customer_id
WHERE d.delivery_id = 2
ORDER BY de.event_time ASC, de.event_id ASC;


-- Query 7: Customers with more than two packages using CTE
WITH customer_packages AS (
    SELECT
        c.customer_id,
        c.name AS customer_name,
        COUNT(*) AS total_packages
    FROM customers c
    JOIN packages p
        ON c.customer_id = p.customer_id
    GROUP BY c.customer_id, c.name
)
SELECT
    customer_name,
    total_packages
FROM customer_packages
WHERE COUNT(total_packages) > 2
ORDER BY total_packages DESC;


-- Query 8: Number delivery events per delivery
SELECT
    de.delivery_id,
    ROW_NUMBER() OVER (
        PARTITION BY de.delivery_id
        ORDER BY de.event_time, de.event_id
    ) AS event_number,
    de.status,
    de.event_time
FROM delivery_events de
ORDER BY de.delivery_id, de.event_time, de.event_id;


-- Query 9: Latest status event per delivery
WITH ranked_events AS (
    SELECT
        de.delivery_id,
        de.status,
        de.event_time,
        ROW_NUMBER() OVER (
            PARTITION BY de.delivery_id
            ORDER BY de.event_time DESC, de.event_id DESC
        ) AS rn
    FROM delivery_events de
)
SELECT
    delivery_id,
    status,
    event_time
FROM ranked_events
WHERE rn = 1;
