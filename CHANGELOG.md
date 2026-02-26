# Changelog

All notable changes to the Finance Manager project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [7.0.0] - Unreleased

### Added
- **Month-Specific Monthly Allowance System**
  - Monthly allowance now tracked per calendar month (e.g., "January", "February")
  - New `[month]` column (NVARCHAR(20)) in `dbo.monthly_allowance` table
  - Automatic month detection using `DateTime.Now.ToString("MMMM", CultureInfo.GetCultureInfo("en-GB"))`
  - Each month can have its own unique allowance amount
  - Historical allowance tracking across different months

- **Enhanced Remaining Fund Calculation**
  - Remaining fund now includes savings in calculation
  - Formula: `(Monthly Allowance - Grand Total) + Savings`
  - Provides more accurate picture of available funds
  - Automatically updates when savings data changes

- **Multi-Select Grid Support**
  - Enabled multi-row selection across all DataGridView grids
  - Users can select multiple rows using Ctrl+Click for individual selection
  - Shift+Click support for range selection
  - Multi-delete functionality - delete multiple records in one operation
  - Edit functionality restricted to single selection with validation message

### Changed
- **Monthly Allowance Management**
  - `EditMonthlyAllowance_click()` now uses month-specific upsert logic
  - Query pattern: `WHERE [month] = @month` for filtering by month name
  - Success message displays current month name (e.g., "Monthly Allowance for January updated")
  - `EditMonthlyAllowance()` loads allowance for current month only
  - Default value of £0.00 for months with no allowance set

- **Grid Selection Behavior**
  - Changed from single-select to multi-select mode
  - `AttachSingleSelectionBehavior()` now allows multiple selections within same grid
  - Cross-grid selection clearing maintained (selecting in one grid clears others)
  - `EditSelected()` validates selection count and shows warning if multiple rows selected

- **Database Schema Updates**
  - `dbo.monthly_allowance` table restructured with `[month]` column
  - Upsert queries check for existing month before insert/update
  - Month stored as full month name in British English format

### Fixed
- **Removed Unused Code**
  - Deleted unused `monthlyAllowanceQuery` variable from `FilterGridByMonth()` method
  - Removed invalid column references (`month_id`, `[date]`) that caused compilation warnings
  - Cleaned up unreachable code paths

### Technical Improvements
- **Cultural Localization**
  - Month names formatted using en-GB culture for consistency
  - Ensures British English month names throughout application
  - Consistent with existing en-GB currency formatting

- **SQL Query Enhancements**
  - Month-based filtering using NVARCHAR string comparison
  - Parameter type: `SqlDbType.NVarChar, 20` for month column
  - TOP 1 query for retrieving current month's allowance
  - Proper parameterization prevents SQL injection

- **User Experience Improvements**
  - Clear feedback when editing monthly allowance with month name in confirmation
  - Multi-select provides faster bulk operations
  - Edit restriction prevents accidental multi-record edits

### Breaking Changes
- **Monthly Allowance Data Migration Required**
  - Existing single global allowance will need manual migration
  - Users must set allowance for each month individually
  - Previous global allowance value not automatically carried forward

---

## [6.0.0] - 24/02/2026

### Added
- **Advanced Filtering and Date Management**
  - Month/year filtering system with InputBox dialogs for user-friendly date selection
  - "Filter by Date" button to filter all grids by specific month and year
  - "Clear Filter" button to reset to current month view
  - Persistent filter state tracking with `isFiltered`, `currentMonth`, and `currentYear` fields
  - Dynamic "Viewing" label displaying current filtered period in bold Montserrat 9pt font
  - Filter state persists across reload operations
  - British date format (dd/MM/yyyy) used throughout all date displays

- **Bill Roll-Over Automation**
  - "Add Previous Month's Bills" button to automate recurring bill entry
  - Copies all bills from previous month with updated dates to current month
  - Respects currently displayed month context (works with filtered views)
  - Confirmation messages showing number of bills copied
  - Handles empty result sets with informative user feedback

- **Remaining Fund Calculation**
  - Real-time calculation of available funds (Monthly Allowance - Grand Total)
  - `RemainingFund()` method with currency parsing and validation
  - Displays "N/A" when calculation cannot be performed
  - Updates automatically on data reload and filtering operations
  - Right-aligned display in bottom panel financial metrics

### Changed
- **UI Layout and Spacing Improvements**
  - Increased main window width from 1200px to 1400px for better data visibility
  - Repositioned "Viewing" label to Y=105 to prevent overlap with action buttons
  - Changed "Viewing" label font from Italic to Bold for better visibility
  - Added `BringToFront()` call to ensure label visibility over other controls
  - Improved button spacing across top action bar (5 buttons spanning 1000px)

- **Date Format Standardization**
  - Changed date format from "d" (short date) to explicit "dd/MM/yyyy" format
  - Ensures consistent British date formatting across all grids
  - Applies to all date columns: Bills, Extra Expenses, Investments, Savings

- **Data Loading Enhancements**
  - Enhanced `ReloadAll()` method to respect filter state
  - Added `RemainingFund()` call to all reload and filter operations
  - Improved `FilterGridByMonth()` to handle all four grid types consistently
  - Updated view label dynamically based on current filter state

### Fixed
- **Database Column Name Mismatches**
  - Fixed "Invalid column name 'notes'" error when editing bills records
  - Fixed "Invalid column name 'notes'" error when editing extra expenses records
  - Fixed "Invalid column name 'date'" error when editing extra expenses records
  - Added intelligent column mapping in `EditSelected()` method:
    - Maps 'notes' → 'description' for bills and extra_expenses tables
    - Maps 'date' → 'duedate' for extra_expenses table
  - Ensures cross-table compatibility with generic EditRecordDialog

### Technical Improvements
- **State Management**
  - Added three private fields for filter tracking: `currentMonth`, `currentYear`, `isFiltered`
  - Filter state initializes to current date (DateTime.Today)
  - State preserved across all data operations and form reloads

- **SQL Query Enhancements**
  - Parameterized queries for date filtering with `@month` and `@year` parameters
  - Consistent WHERE clause pattern: `MONTH([column]) = @month AND YEAR([column]) = @year`
  - Proper handling of `duedate` vs `date` column names across tables

- **Code Organization**
  - Refactored `AddPreviousBills()` to use separate SELECT and INSERT operations
  - Enhanced error handling with specific exception messages
  - Improved method naming consistency (e.g., `RemainingFund()` follows pattern)

---

## [5.0.0] - 10/12/2026

### Added
- **Enhanced Edit Dialog System**
  - `EditRecordDialog` form with dynamic field detection
  - Support for editing: name, amount, date, category, type, length, notes/description
  - Fixed-size dialog (420x380) with proper tab order
  - Save and Cancel buttons with validation
  - Amount validation supporting both CurrentCulture and InvariantCulture parsing
  - Date picker integration for date fields with British date format
  - Multiline text box for notes (80px height)

- **Cross-Table Record Management**
  - Unified edit functionality across all four data types
  - Delete functionality with confirmation dialog and transaction support
  - `GetActiveGridWithSelection()` method to identify selected grid
  - `GetTableNameForGrid()` for table name resolution
  - `GetPrimaryKeyColumnForGrid()` for primary key detection with fallback logic
  - Batch delete support with parameterized queries

- **Grid Selection Behavior**
  - `AttachSingleSelectionBehavior()` method for mutual exclusion
  - Single-row selection enforced across all grids
  - Automatic selection clearing when switching between grids
  - ReferenceEquals checking to prevent self-clearing

### Changed
- **Edit and Delete Button Placement**
  - Moved to top action bar alongside Reload button
  - "Edit Selected" button at position (168, 150)
  - "Delete Selected" button at position (320, 150)
  - Both buttons 140x40 with secondary color scheme

- **Data Binding Events**
  - Added `DataBindingComplete` event handler to trigger formatting and total updates
  - Added `CellValueChanged` event handler for real-time total recalculation
  - Added `RowsRemoved` event handler to update totals after deletion

### Technical Details
- **EditRecordDialog Implementation**
  - Dynamic control generation based on field dictionary
  - Y-position tracking with `IncY()` helper method
  - Support for merged notes/description fields
  - DBNull handling for optional fields
  - Proper disposal with `using` statement in calling code

- **Transaction Management**
  - SQL transactions for delete operations
  - Rollback support on exceptions
  - Commit confirmation before completion

---

## [4.0.0] - 24/05/2025

### Added
- **Comprehensive Financial Tracking Dashboard**
  - Grand Total calculation displaying sum of Bills, Extra Expenses, and Investments
  - Emergency Fund tracking with persistent database storage
  - Monthly Allowance tracking with dedicated database table
  - Bottom panel (200px height) with organized financial metrics display
  - FlowLayoutPanel with RightToLeft direction for metric alignment

- **Emergency Fund Management**
  - `dbo.emergency_fund` table with columns:
    - `amount` (DECIMAL(12,2)) for fund value
    - `updated_at` (DATETIME) for change tracking
  - `LoadEmergencyFund()` method retrieves latest value using TOP 1 and ORDER BY
  - `EditEmergencyFund_Click()` handler with InputBox interface
  - Upsert pattern: UPDATE if exists, INSERT if new
  - Currency validation using en-GB culture (£ symbol support)

- **Monthly Allowance Management**
  - `dbo.monthly_allowance` table with `amount` column (DECIMAL(12,2))
  - `EditMonthlyAllowance()` method for loading allowance value
  - `EditMonthlyAllowance_click()` handler for user updates
  - Automatic `ReloadAll()` call after successful update
  - Zero default value (£0.00) when no record exists

- **Grid Formatting Standards**
  - `FormatColumnHeaders()` method with TitleCase transformation
  - `FormatGrid()` method applying:
    - Currency format ("C") with en-GB CultureInfo for amount columns
    - British date format (dd/MM/yyyy) for date columns
    - Right alignment for amounts
    - Center alignment for dates
  - Automatic column hiding for primary key fields (billid, extra_expense_id, etc.)

- **Total Calculation System**
  - `UpdateTotal()` method summing three grid types (excludes Savings)
  - `SumAmountColumn()` helper with robust parsing:
    - Direct decimal type support
    - InvariantCulture string parsing fallback
    - CurrentCulture string parsing as final fallback
  - Real-time updates on data binding, cell changes, and row removal

### Changed
- **All Payments Form Layout**
  - Form size: 1200x1260 (from previous 1200x800)
  - Bottom panel docked with proper padding (12, 12, 12, 20)
  - Financial metrics in right-aligned flow layout
  - Transparent backgrounds for seamless gradient integration
  - Read-only textboxes for all calculated values (150px width)

- **Button Styling Consistency**
  - Primary button: RGB(255, 120, 120) with 2px black border
  - Secondary button: RGB(255, 150, 150) with 2px black border
  - Edit buttons: Auto-sized with 12,6,12,6 padding
  - FlatStyle.Flat for all buttons with border customization

- **Data Grid Setup**
  - Grid positioning: Bills(200), Expenses(420), Investments(640), Savings(800)
  - Consistent heights: Bills(200), Expenses(200), Investments(160), Savings(200)
  - White background color for all grids
  - Color-coded headers: RGB(255, 150, 150)
  - Bold Montserrat font for headers
  - FixedSingle border style

### Technical Implementation
- **Database Schema Management**
  - Automatic table creation with IF NOT EXISTS checks
  - Decimal precision set to (12,2) for all monetary columns
  - Proper SqlDbType.Decimal parameter configuration with Precision and Scale properties
  - Environment variable-based password management (DB_PASSWORD)

- **Connection String Building**
  - SqlConnectionStringBuilder with properties:
    - DataSource: "STONEY,1433"
    - InitialCatalog: "Finance_Manager"
    - UserID: "josh"
    - Encrypt: true
    - TrustServerCertificate: true

- **Error Handling**
  - Try-catch blocks in all database operations
  - User-friendly error messages with exception details
  - Graceful handling of missing environment variables
  - Validation before database updates

---

## [3.0.0] - 20/02/2025

### Added
- **Full CRUD Operations**
  - Edit functionality for all record types via DataGridView selection
  - Delete functionality with multi-record support
  - Confirmation dialog for delete operations ("Are you sure you want to permanently delete N record(s)?")
  - Transaction-based deletion for data integrity
  - Single-row selection enforcement across all four grids

- **EditRecordDialog Component**
  - Modal dialog form (420x380) with centered positioning
  - Dynamic field generation based on available columns
  - Support for fields: name, amount, date, category, type, length, notes
  - DateTimePicker for date fields (British format dd/MM/yyyy)
  - Multiline TextBox for notes (300x80)
  - Input validation for amount field (supports multiple cultures)
  - Save/Cancel buttons at bottom (80x30 each)

- **Grid Selection Management**
  - `AttachSingleSelectionBehavior()` method for grid coordination
  - Automatic clearing of other grids when selecting in one grid
  - Exception handling in selection event to prevent cascade errors
  - MultiSelect disabled on all grids

### Changed
- **Data Grid Configuration**
  - AllowUserToAddRows: false (prevents accidental row creation)
  - AllowUserToDeleteRows: false (controlled through Delete button)
  - SelectionMode: FullRowSelect for better UX
  - AutoSizeColumnsMode: Fill for responsive layout
  - RowHeadersVisible: false for cleaner appearance
  - EnableHeadersVisualStyles: false for custom header colors

- **Record Identification**
  - Primary key detection with Contains check + fallback
  - Support for all four primary keys: billid, extra_expense_id, investments_id, savings_id
  - Table name resolution based on grid reference

### Fixed
- Selection state synchronization across grids
- Data refresh after edit/delete operations
- Null handling in primary key extraction
- DBNull conversion in edit dialog value assignment

### Technical Details
- **Edit Operation Flow**
  1. Detect active grid with selection
  2. Extract primary key and table name
  3. Load current values into dictionary
  4. Show EditRecordDialog with values
  5. Build UPDATE statement with parameterized clauses
  6. Execute update and reload data

- **Delete Operation Flow**
  1. Extract primary key IDs from selected rows
  2. Filter null/invalid IDs
  3. Show confirmation dialog
  4. Execute parameterized DELETE in transaction
  5. Commit transaction on success
  6. Reload all data

---

## [2.0.0] - 15/02/2025

### Added
- **All Payments Consolidated Dashboard**
  - Single form displaying all four payment types in separate grids
  - Logo display (120x120) centered at top with transparent background
  - Title label "All Payments" in Montserrat 14pt Bold
  - Four DataGridView controls with read-only configuration
  - Reload button (140x40) at position (16, 150)
  - "View All Payments" navigation buttons in all add forms

- **Data Loading Methods**
  - `BillsDataGrid()`: Loads from dbo.bills with ORDER BY [date] DESC
  - `ExtraExpenseDataGrid()`: Loads from dbo.extra_expenses with duedate AS [date] alias
  - `InvestmentsDataGrid()`: Loads from dbo.investments
  - `SavingsDataGrid()`: Loads from dbo.savings
  - `ReloadAll()`: Orchestrates loading of all four grids

- **Grid Setup Infrastructure**
  - `SetupGrid()` method with caption label creation
  - Label positioned 28px above grid with bold font
  - Consistent left margin (16px) and width calculation (ClientSize - 32)
  - Vertical spacing: 20px between grids
  - Custom grid styling with color-coded headers

### Changed
- **Form Dimensions and Layout**
  - ClientSize: 1200x1260 (increased from smaller forms)
  - StartPosition: CenterScreen for better visibility
  - AutoScroll enabled with 20px margin for overflow
  - DoubleBuffered: true for smoother rendering
  - Gradient background: LightCoral to White (vertical)

- **Navigation Integration**
  - Added "View All Payments" button to AddBill form (180, 70, 200x40)
  - Added "View All Payments" button to AddExtraExpense form
  - Added "View All Payments" button to AddInvestment form
  - Added "View All Payments" button to AddSavings form
  - All buttons use secondary color scheme

### Technical Implementation
- **DataAdapter Pattern**
  - SqlDataAdapter for query execution
  - DataTable for in-memory storage
  - DataSource binding to DataGridView
  - Automatic column generation

- **Grid Positioning**
  - Bills: Y=200, Height=200
  - Extra Expenses: Y=420, Height=200
  - Investments: Y=640, Height=160
  - Savings: Y=800, Height=200
  - Total vertical span: 800px

---

## [1.0.0] - 01/02/2025 - Initial Release

### Added
- **Core Application Framework**
  - Windows Forms application with custom gradient backgrounds (LightCoral → White)
  - Montserrat font family used consistently at 10pt Regular
  - SQL Server integration using Microsoft.Data.SqlClient
  - Environment variable-based authentication (DB_PASSWORD)
  - Main menu with dropdown navigation to all payment types
  - Logo integration (FM_Logo_Main_Menu.png) with StretchImage mode

- **Bills Management Module**
  - `AddBill.cs` form for bill entry (560x620)
  - Fields: Name, Amount (£), Date, Type, Length, Description
  - `BillsRecord.cs` view for displaying all bills
  - Database table: `dbo.bills`
  - Primary key: `billid` (INT IDENTITY)
  - Automatic schema creation on first run

- **Extra Expenses Module**
  - `AddExtraExpense.cs` form with category selection (560x620)
  - Fields: Name, Amount (£), Category, Type, Frequency, Date Incurred, Notes
  - Dynamic frequency field (visible only for recurring expenses)
  - `ExtraExpensesRecord.cs` view
  - Database tables: `dbo.extra_expenses`, `dbo.categories`
  - Foreign key relationship: `extra_expenses.category_id` → `categories.id`
  - 12 predefined categories: Groceries, Transport, Entertainment, Dining Out, Utilities, Health, Education, Gifts, Home, Personal, Travel, Other

- **Investments Module**
  - `AddInvestment.cs` form for investment tracking (560x620)
  - Fields: Name, Amount (£), Date, Category, Length, Notes
  - `InvestmentRecord.cs` view
  - Database table: `dbo.investments`
  - Primary key: `investments_id` (INT IDENTITY)

- **Savings Module**
  - `AddSavings.cs` form for savings entries (560x620)
  - Fields: Name, Amount (£), Length, Date, Notes
  - Length options: Daily, Weekly, Monthly, Quarterly, Yearly
  - Database table: `dbo.savings`
  - Primary key: `savings_id` (INT IDENTITY)

- **Common UI Components**
  - Primary button style: RGB(255, 120, 120), FlatStyle.Flat, 2px black border
  - Secondary button style: RGB(255, 150, 150), FlatStyle.Flat, 2px black border
  - Input validation with visual feedback (MistyRose background on error)
  - DateTimePicker controls with British date format (dd/MM/yyyy)
  - ComboBox controls with DropDownList style (prevents manual entry)
  - Multiline TextBox with vertical scrollbars for notes fields
  - Pound symbol (£) label prefix for all amount fields

### Database Features
- **Schema Management**
  - Automatic table creation with IF NOT EXISTS checks
  - Column addition with IF COL_LENGTH checks for backward compatibility
  - Proper data type definitions: DECIMAL(12,2) for monetary values
  - NVARCHAR(200) for names, NVARCHAR(MAX) for notes
  - DATE type for date fields
  - INT IDENTITY(1,1) for primary keys

- **Data Integrity**
  - Foreign key constraints with named constraints (FK_extra_expenses_categories)
  - NOT NULL constraints on required fields
  - UNIQUE constraint on category names
  - Transaction support for batch operations

- **Category Seeding**
  - Automatic population of categories table on first run
  - Upsert pattern: INSERT only if not exists
  - Category synchronization for existing records
  - Lowercase comparison for matching

### Input Validation
- **Common Validation Rules**
  - Name: Required, non-whitespace
  - Amount: Required, positive decimal, parsed using CurrentCulture
  - Date: Cannot be in future (DateTimePicker.Value > DateTime.Today)
  - Category: Required selection (SelectedValue must be int)
  - Frequency: Required only for recurring expenses

- **Visual Feedback**
  - Invalid fields: MistyRose background color
  - Valid fields: SystemColors.Window (white)
  - Reset all field colors before validation
  - Focus set to first invalid field

### Technical Specifications
- **Framework and Languages**
  - .NET 8.0 (C# 12.0)
  - Windows Forms (System.Windows.Forms)
  - Target OS: Windows (required for WinForms)

- **Database Configuration**
  - Server: STONEY,1433 (SQL Server instance)
  - Database: Finance_Manager
  - Authentication: SQL Server (username: josh)
  - Encryption: Enabled with TrustServerCertificate
  - Connection pooling: Enabled by default

- **Localization**
  - Currency culture: en-GB (British Pound £)
  - Date format: British format (dd/MM/yyyy)
  - Number parsing: Supports both CurrentCulture and InvariantCulture

- **Form Layout Standards**
  - Data entry forms: 560x620
  - Button heights: 40px (standard), 30px (dialog buttons)
  - Bottom panel height: 120px
  - Logo size: 120x120
  - Control spacing: 40px vertical between major sections
  - Left margin: 20px, Right margin: 30px (for 560px forms)
  - Tab indexing: Sequential from top to bottom

### Architecture Patterns
- **Data Access Layer**
  - Direct SQL connection per operation
  - Using statements for proper disposal
  - Parameterized queries for SQL injection prevention
  - SqlDataAdapter for read operations
  - SqlCommand for write operations

- **UI Layer**
  - Event-driven architecture
  - Button click handlers for all user actions
  - Form Paint event for custom backgrounds
  - Control validation before database operations

- **Data Storage**
  - In-memory stores: BillStore, ExtraExpenseStore, InvestmentStore, SavingsStore
  - BindingList for data-bindable collections
  - Synchronization between database and in-memory stores

---

## Project Information

**Repository**: https://github.com/JoshuaStone9/Finance-Manager

**Author**: Joshua Stone

**License**: Not specified

**Platform**: Windows (WinForms)

**Database**: SQL Server

**Primary Language**: C# 12.0 (.NET 8.0)

---

## Development Notes

### Known Limitations
- Requires Windows operating system (WinForms dependency)
- SQL Server must be accessible at STONEY,1433
- DB_PASSWORD environment variable must be set per user
- No export functionality (CSV/PDF) in current version
- No search/filter capabilities within individual record views
- No data backup/restore functionality

### Future Considerations
- Move connection strings to app.config or user secrets
- Implement comprehensive unit testing
- Add data validation rules at database level
- Consider migration to cross-platform UI framework
- Add audit logging for financial transactions
- Implement data export and reporting features
- Add budget tracking and forecasting capabilities
