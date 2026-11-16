
-- Logistics Management System
-- Seed Data

-- Customers
INSERT INTO customers (name, email)
VALUES
    ('Erik', 'erik@example.com'),
    ('Sara', 'sara@example.com'),
    ('Ali', 'ali@example.com');

-- Drivers
INSERT INTO drivers (name, phone)
VALUES
    ('Johan', '0701112233'),
    ('Emma', '0702223344'),
    ('Ahmed', '0703334455');

-- Packages
INSERT INTO packages (customer_id, destination, status)
VALUES
    (1, 'Malmö', 'In transit'),
    (2, 'Lund', 'In transit'),
    (3, 'Eslöv', 'Delivered'),
    (1, 'Stockholm', 'Registered'),
    (1, 'Göteborg', 'In transit'),
    (2, 'Helsingborg', 'Registered');

-- Deliveries
INSERT INTO deliveries (package_id, driver_id, delivery_status)
VALUES
    (1, 1, 'Assigned'),
    (2, 2, 'In transit'),
    (3, 3, 'Delivered');


-- Delivery Events
INSERT INTO delivery_events (delivery_id, status)
VALUES
    (1, 'Assigned'),
    (2, 'Assigned'),
    (2, 'In transit'),
    (3, 'Assigned'),
    (3, 'In transit'),
    (3, 'Delivered');
