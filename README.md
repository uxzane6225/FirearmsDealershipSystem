# ITCP Project
## Title
 - Firearms Dealership System
## Made by
 - Uriel Laurence M. Mendoza
## About
 - The Firearms Dealership System is a WinForms (.NET Framework) Desktop Application that is connected to a local MySQL Database, it utilizes basic CRUD Operations, Sotred Procedure for Transactions, and BCrypt for password hashing.
## Requirements
### 1. Project Architecture
  To keep the code clean, do not put everything in the main form. Divide your logic into these categories:
  Data Models: Classes that represent your objects (e.g., User.cs, Product.cs).
  Logic/Services: Classes that handle calculations, validation, and database interactions.
  UI Layer: Your Windows Forms.

### 2. Security: Implementing BCrypt
  Storing passwords in plain text is a major "no-go." You will use the BCrypt.Net-Next library.
  The Logic:
  Registration: Hash the password before saving it to the database. (optional)
  Login: Retrieve the stored hash and use BCrypt to verify it against the user's input.
  Key Rule: Never try to "decrypt" a BCrypt hash. You can only "verify" if a new input matches the existing hash.

### 3. Core Requirements & Logic
  The CRUD Functionality
  Your program should manage a specific entity (like Employees, Inventory, or Books).
  Create: Form to input new data.
  Read: Display data in a DataGridView or List.
  Update: Ability to select an item and modify its details.
  Delete: Remove data with a "Are you sure?" confirmation prompt.
  Search & Filtering
  Implement a TextBox that filters your list in real-time.
  Input Validation
  Prevent the program from crashing by checking data before it’s processed:
  Presence Check: Ensure no fields are empty.
  Type Check: Ensure "Price" or "Age" fields only contain numbers.
  Length Check: Passwords should be at least 8 characters.

### 4. Class Structure Example
  You must use classes to stay organized.

### 5. Development Steps
  Set up the Database: Create your tables (Users and Items).
  Install BCrypt & MySQL: Open NuGet Package Manager and search for BCrypt.Net-Next and MySQL.
  Build the Login Form: Focus on the Verify() method first.
  Build the Main Dashboard: Create the UI for your CRUD operations.
  Apply Validations: Add if-else blocks or Try-Catch to handle user errors gracefully.
