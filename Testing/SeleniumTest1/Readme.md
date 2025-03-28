# Selenium Feature-Based Tests

This folder contains **End-to-End Selenium tests** for the COMP 7071 ASP.NET project.

## 📂 Structure
SeleniumTest1/
├── SeleniumTest1.sln                 → Solution file
├── README.md                         → Test instructions
└── SeleniumTest1/
    ├── SeleniumTest1.csproj          → Project file
    ├── drivers/                      → ChromeDriver
    ├── Tests/                        → All Selenium test files
    │   └── ShiftManagementTests.cs   → Tests for Employee Shift feature
    └── Utils/                        → (Optional) Helper functions



## 🚀 How to Run the Tests

### Pre-requisites:
- Chrome browser installed
- ChromeDriver is already included in `/drivers` folder

### Run Command:
cd Testing/SeleniumTest1
dotnet test SeleniumTest1.sln


## Add More Tests
For new features:

Create a new .cs file in Tests/ folder.

Follow the feature-based naming pattern.

Example:

Tests/
├── ShiftManagementTests.cs
├── LeaveRequestTests.cs      (future)