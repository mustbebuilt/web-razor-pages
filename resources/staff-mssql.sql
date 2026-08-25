-- MSSQL (Microsoft SQL Server) Schema and Seed Data for Staff Table

-- 1. Create Staff Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'staff')
BEGIN
    CREATE TABLE [dbo].[staff] (
        [staff_id]   INT IDENTITY(1,1) PRIMARY KEY,
        [first_name] NVARCHAR(50) NOT NULL,
        [last_name]  NVARCHAR(50) NOT NULL,
        [email]      NVARCHAR(100) NOT NULL UNIQUE,
        [department] NVARCHAR(50) NOT NULL DEFAULT 'General',
        [job_title]  NVARCHAR(50) NOT NULL,
        [hire_date]  NVARCHAR(10) NOT NULL DEFAULT (CONVERT(VARCHAR(10), GETDATE(), 120)),
        [salary]     DECIMAL(18, 2) NOT NULL,
        [is_active]  INT NOT NULL DEFAULT 1 CHECK ([is_active] IN (0, 1))
    );
END;

-- 2. Populate Sample Staff Data
INSERT INTO [dbo].[staff] ([first_name], [last_name], [email], [department], [job_title], [hire_date], [salary], [is_active]) VALUES
('Alice', 'Johnson', 'alice.johnson@company.com', 'Engineering', 'Engineering Manager', '2021-03-15', 85000.00, 1),
('Brian', 'Lee', 'brian.lee@company.com', 'Engineering', 'Senior Developer', '2022-06-01', 72000.00, 1),
('Carla', 'Gomez', 'carla.gomez@company.com', 'Human Resources', 'HR Specialist', '2020-09-12', 58000.00, 0),
('David', 'Smith', 'david.smith@company.com', 'Finance', 'Financial Analyst', '2019-11-20', 65000.00, 1),
('Emma', 'Watson', 'emma.watson@company.com', 'Marketing', 'Marketing Director', '2018-04-10', 90000.00, 1),
('Frank', 'Wright', 'frank.wright@company.com', 'Sales', 'Account Executive', '2023-01-15', 55000.00, 1);
