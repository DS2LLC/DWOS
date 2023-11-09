# DWOS

# Getting Started

This is a quick guide to DWOS development.

## Development

1. Install the following on your system:
   - Visual Studio
   - SQL Server Management Studio (database development & occasional debugging)
   - Microsoft .NET Framework 4.5.2
   - Infragistics Ultimate - Windows Forms, WPF, and ASP .NET are used
   - /n software IP*Works! for server/email development
   - /n software QuickBooks Integrator for accounting development
   - Dynamic .NET TWAIN with Webcam Capture component
   - Xamarin (only for mobile development)
        - Install through Visual Studio installer
   - [Node.js](https://nodejs.org/en/) - Portal site development
        - Using the latest ("Current") release is recommended.
2. Clone the DWOS repository
    - This is the main repository for the DWOS product.
3. (Optional) Clone the DevelopmentUtility repository
   - This repository contains the helpful DWOS.DevelopmentUtility project that automates a number of important tasks.
   - You will need to generate an Azure DevOps access key and set it in the options for the DevelopmentUtility program.
4. (Optional) Setup local development server
   1. You need a Customer Key and License key - see [Licensing](/Licensing)
   2. Open the DWOS.Server solution and run  DWOS.Server.Admin and go through the initial setup
   3. From the DWOS.Server solution, run the DWOS.Server project - it should stay open
5. Open the DWOS.Client solution and run DWOS.UI
   - If you did not setup a local server in Step 4, you can use **ds2-dwos-dev-1** for development.

### Development Tips

- Do not commit changes to .licx files unless you know what the changes do
  - These files contain licensing information that, when removed, prevents end-users from using certain parts of DWOS
- For Windows Forms controls, check tab order and tooltips
- For WPF controls, check tooltips and styling
- Depending on the location of your cloned repository, NuGit packages required as part of the projects may need to be reinstalled.
  This can be accomplished by running the following cammond at the Nuget package manager commandline. "Update-Package –reinstall".

## General Overview of the DWOS Product Suite

**DWOS Server** acts the central server for a DWOS installation. It is a Windows Service that typically runs 24/7 on a server somewhere on-site. It is responsible for licensing/login, regularly scheduled tasks (sending emails, backups, etc.), hosting services used by the mobile application, and SQL Server maintenance.

**DWOS Server Administration Utility** is a utility program for DWOS Server. It is responsible for initial server setup, server upgrades, and status reporting for the server, database, and Portal (if available).

Several DWOS products (including Server and Client) connect to an **SQL Server**.  This contains data and application settings for the application.

**DWOS Client** is the desktop client application for DWOS. It connects to both the Server (for licensing/login) and the SQL Server (for just about everything else). Client is the primary place for user interaction with DWOS, as it provides an interface for all workflow phases, reporting, and configuration.

**DWOS Mobile** is the Android client application for DWOS that uses Xamarin. Unlike Client, it interacts exclusively with DWOS Server and does not directly access the SQL Server. This mobile application has limited functionality and focuses on processes and inspections.

**Customer Portal** is a website that a company's customers can go to for information on Work Orders. It interacts exclusively with the SQL Server and does not interact with DWOS Server for licensing. There are currently two different versions of this available: Portal I is an ASP .NET Web Pages application, and Portal II is a mobile-friendly application using Angular and ASP .NET Web API. Note: I is not the first version of the web site, it just has that name.

### Other Products

**Order Check-in Tool** is bundled with DWOS Client and provides additional barcode functionality.

**Data Migration Tool** allows users to import/export data. It's typically used during initial implementation to bring a company's customers and parts into DWOS.

**Data Archival Tool** extracts media for old, closed Work Orders from DWOS. It's typically used to free database space.

**AWOT** is the Automatic Work Order Tool. It was build specifically for one customer and allows for quick creation of parts and Work Orders.

## Important Database Tables

DWOS has numerous tables, and here are what I think are the most important tables:

**[Order]** is the Work Order table. All details about an order are not in this table, but you can retrieve it from this table's `OrderID` column. Since "Order" is a keyword in T-SQL, it should always be referenced as [Order].

**OrderProcesses** contains information about processes for Work Orders. Each process for an order is associated with a Process in `Process` and an alias in `ProcessAlias`. You can see information about processes/aliases in Process Manager.

**Customer** is the primary table for customer information and contains the customer's name, address, billing information, and status. Contacts (which can be Portal site users) are stored in the `d_Contact` table.

Every row in **Part** represents a part in DWOS. Every part is associated with a customer and contains pricing information specific to that customer.

**Batch** contains information about batches. It has its own processes table, `BatchProcesses`.

**User** contains information about DWOS users.

**ApplicationSettings** acts as a key-value store for application-wide settings used throughout a DWOS installation. One of the most important settings is **DatabaseVersion** - it contains the database schema version. DWOS Server will not run if the database has a different schema version than it expects.

**UserEventLog** contains information about deletions and other activities performed by users in DWOS.

**Audit** is a 'hidden' table in DWOS. The code itself does not interact with it, but multiple database triggers insert rows into it that represent changes to data. This table represents changes made to multiple tables, so be sure to check the `TableName` column before searching for data.
