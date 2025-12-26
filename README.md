# Finance Manager (C# WinForms)

A Windows Forms finance manager that tracks bills, savings, investments and extra expenses. The app provides dedicated forms for adding entries, viewing records and rolling everything up into an "All Payments" dashboard.

---

## Table of Contents
- [Overview](#overview)
- [High Level Architecture](#high-level-architecture)
- [Data Model](#data-model)
- [Control Flow](#control-flow)
- [Randomization Logic](#randomization-logic)
- [Console Interface](#console-interface)
- [How to Run](#how-to-run)
- [Potential Improvements](#potential-improvements)
- [Personal Development](#personal-development)

---

## Overview

Finance Manager is a Windows Forms project focused on personal finance tracking. Users can add and review bills, savings, investments and extra expenses, then see aggregated totals in a combined view.

The UI emphasizes quick entry and clear summaries, while data is persisted to local databases for reuse between sessions.

---

## High Level Architecture

The program is implemented as a single WinForms application with one entry point.

The logic is divided conceptually into:

UI Layer  
WinForms screens for the main menu, add forms and record views  

Data Access Layer  
SQL Server access via `Microsoft.Data.SqlClient` plus a separate investments summary screen using `Npgsql`  

Model Layer  
Simple record classes for bills, savings, investments and extra expenses  

Presentation Layer  
Data grids and totals views that present stored records and calculated aggregates  

---

## Data Model

The application uses simple in-app models that map to database tables:

BillRecord  
BillId, Name, Amount, Type, Length, DueDate, Description  

SavingsRecord  
Savings_ID, Name, Amount, Length, Date, Notes  

InvestmentRecord  
Investment_ID, Name, Amount, Category, Length, Date, Description  

ExtraExpenseRecord  
Expense_ID, Name, Amount, Category, Type, Frequency, DateIncurred, Notes  

---

## Control Flow

Program Start  
Launches the main menu (`MainMenu`)  

Navigation  
User selects to manage bills, savings, investments, extra expenses or all payments  

Data Entry  
Add forms collect values and save to the database  

Viewing  
Record screens and the "All Payments" view load data into grids and compute totals  

Program End  
User closes forms or exits the application  

---

## Randomization Logic

This project does not use randomization. All outputs are driven by stored records and deterministic totals computed from the database.

---

## User Interface

This project uses a Windows Forms interface:

- Main menu with a dropdown for individual payment types  
- Forms for bills, savings, investments, and extra expenses  
- An "All Payments" screen with aggregated totals  
- A notification pop-up to display alerts  

---

## How to Run

Requirements  
.NET SDK (Windows required for WinForms)  
SQL Server with a `Finance_Manager` database  
`DB_PASSWORD` environment variable for SQL Server connections  

Run the program  
```
dotnet run --project FM/FM.csproj
```

If you use the Investments calculation screen, update the `Npgsql` connection string in `FM/CalculateInvestments.cs` to match your local Postgres setup.

---

## Potential Improvements

- Move connection strings into `app.config` or user secrets  
- Add validation and clearer error messages for invalid inputs  
- Add search and filters in the record screens  
- Add export options (CSV/PDF)  
- Add tests for calculations and data access  

---

## Personal Development

This project helped reinforce:

- Windows Forms layout and event-driven logic  
- Data binding with `DataGridView` and `BindingList`  
- SQL data access and basic CRUD workflows  
- Designing multi screen desktop apps  
- Presenting aggregated financial data clearly  
