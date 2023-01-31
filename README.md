# simple-wallet


## About The Project

A simple wallet system

Please write a simple wallet system to implement the following features:
- As a user they can register a new account in the wallet system:
  - It’s important for each user to have these details associated to them
  - Login Name (must be unique)
  - Account Number (must be a unique and random 12 digits number generated upon registration)
  - Password 
  - Balance (value of 0 upon creation)
  - Register Date
- As user they can deposit or withdraw form the wallet system
  - Just simply update the user balance in the database, don’t need to integrate with the real bank.
- As user they can transfer funds to other existing users
- As user they can view all their transaction history with the following details
  - Transaction Type
  - Amount
  - Account Number (From/To)
  - Date Of Transaction
  - End Balance 

### Notes:
- UI is NOT required.
- It can be a Web Api application or Console application.

### Requirements
- [x] Use DotNet Core
- [x] Use Ado.Net for database access only
- [x] Store data to MSSQL
- [x] Unit testing is required (IMPORTANT), xunit, nunit or msunit, any tools is ok.
- [ ] Handle concurrency issues (IMPORTANT)
- [x] DON’T use AspNet Core Identity library.
- [x] Make sure login name is not duplicated in database
- [x] Handle exception is very important, please handle all exceptions properly
- [x] Make sure there is not any loophole
- [x] Use Github to share the source code


## Getting Started

### Prerequisites

- MSSQL
- .NET Core 6

### Installation

- Need to create schema: `SimpleWallet`, after that need to execute SQL script `~/SimpleWallet/InitDatabase.sql` for creating tables.
- Need to update your connection string in `appsettings.json` and `appsettings.Tests.json`


## Usage

List of endpoints: `https://localhost:7168/swagger/` or `http://localhost:5168/swagger/`.
