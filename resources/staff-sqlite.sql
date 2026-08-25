-- SQLite Schema and Seed Data for Staff Table

-- 1. Create Staff Table
CREATE TABLE IF NOT EXISTS "staff" (
    "staff_id"   INTEGER PRIMARY KEY AUTOINCREMENT,
    "first_name" TEXT NOT NULL,
    "last_name"  TEXT NOT NULL,
    "email"      TEXT NOT NULL UNIQUE,
    "department" TEXT NOT NULL DEFAULT 'General',
    "job_title"  TEXT NOT NULL,
    "hire_date"  TEXT NOT NULL DEFAULT (DATE('now')),
    "salary"     REAL NOT NULL,
    "is_active"  INTEGER NOT NULL DEFAULT 1 CHECK("is_active" IN (0, 1))
);

-- 2. Populate Sample Staff Data
INSERT INTO "staff" ("first_name", "last_name", "email", "department", "job_title", "hire_date", "salary", "is_active") VALUES
('Alice', 'Johnson', 'alice.johnson@company.com', 'Engineering', 'Engineering Manager', '2021-03-15', 55000.00, 1),
('Brian', 'Lee', 'brian.lee@company.com', 'Engineering', 'Senior Developer', '2022-06-01', 52000.00, 1),
('Carla', 'Gomez', 'carla.gomez@company.com', 'Human Resources', 'HR Specialist', '2020-09-12', 48000.00, 0),
('David', 'Smith', 'david.smith@company.com', 'Finance', 'Financial Analyst', '2019-11-20', 55000.00, 1),
('Emma', 'Watson', 'emma.watson@company.com', 'Marketing', 'Marketing Director', '2018-04-10', 60000.00, 1),
('Frank', 'Wright', 'frank.wright@company.com', 'Sales', 'Account Executive', '2023-01-15', 45000.00, 1);
