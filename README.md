# Adaptive Hypertrophy

## Overview
Adaptive Hypertrophy is a console-based workout tracking and progression application built in C# (.NET 9.0). It allows users to set up a profile, log max lifts, and dynamically generate and scale workout plans based on different progression strategies. The application utilizes a local SQLite database to persist user data, workout logs, and exercise data.

This project was built to demonstrate object-oriented programming concepts and adhered strictly to SOLID principles, featuring:
- Abstract base classes and inheritance
- Interfaces and polymorphism
- Design Patterns (Strategy, Repository, Singleton)

## Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download) installed on your machine.

## How to Compile and Run

1. **Open a terminal/command prompt** in the root of the workspace directory.
2. **Build the project** to ensure all dependencies and the SQLite database configuration are restored:
   ```bash
   dotnet build src/AdaptiveHypertrophy/AdaptiveHypertrophy.csproj
   ```
3. **Run the application**:
   ```bash
   dotnet run --project src/AdaptiveHypertrophy/AdaptiveHypertrophy.csproj
   ```

Follow the on-screen console prompts to set up your initial user profile, generate workout plans, and track your exercises!

## Application Structure
- **Domain:** Contains core data models (User, SetPerformance) and Enums.
- **Exercises:** Defines exercise behavior and types (Compound vs Isolation).
- **Progression:** Implements the Strategy Pattern to alter how workout intensity is calculated.
- **Planning:** Generates plans and scales weights based on past performance.
- **Data:** Implements the Repository Pattern, managing data cleanly with a Singleton `DatabaseConnectionManager`.
- **Display:** Decoupled UI logic for the console interaction.
