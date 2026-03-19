-- Create Database
CREATE DATABASE IF NOT EXISTS UserManagementDB;
USE UserManagementDB;

-- Create Users Table
CREATE TABLE IF NOT EXISTS Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL
);

-- (Optional) Seed Data
INSERT INTO Users (Name, Email) VALUES ('Admin User', 'admin@example.com');
