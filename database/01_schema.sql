
-- Logistics Management System
-- Database Schema

-- Table 1: Customers
CREATE TABLE customers (
    customer_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE
);

-- Table 2: Drivers
CREATE TABLE drivers (
    driver_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    phone VARCHAR(20) UNIQUE
);

-- Table 3: Packages
CREATE TABLE packages (
    package_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    customer_id INTEGER NOT NULL,
    destination VARCHAR(100) NOT NULL,
    status VARCHAR(30) NOT NULL,

    CONSTRAINT fk_packages_customer
        FOREIGN KEY (customer_id)
        REFERENCES customers(customer_id),
    CONSTRAINT chk_packages_status
    CHECK (status IN ('Registered', 'In transit', 'Delivered'))
);

-- Table 4: Deliveries
CREATE TABLE deliveries (
    delivery_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    package_id INTEGER NOT NULL,
    driver_id INTEGER NOT NULL,

    delivery_status VARCHAR(30) NOT NULL DEFAULT 'Assigned',
    assigned_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_delivery_package
        FOREIGN KEY (package_id)
        REFERENCES packages(package_id),

    CONSTRAINT fk_delivery_driver
        FOREIGN KEY (driver_id)
        REFERENCES drivers(driver_id)
);

-- Table 5: Delivery Events
CREATE TABLE delivery_events (
    event_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    delivery_id INTEGER NOT NULL,
    status VARCHAR(30) NOT NULL,
    event_time TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_event_delivery
        FOREIGN KEY (delivery_id)
        REFERENCES deliveries(delivery_id)
);

