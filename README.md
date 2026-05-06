# Adaptive Hypertrophy

## 1. Project Overview

**Adaptive Hypertrophy** is a C# console application for lifters who want structured, adapting workout targets. Users create a profile (experience, frequency, split, goal), enter max lifts for a small built-in catalog, review an auto-generated first-week plan with a **too easy / too hard / accept** feedback loop, and can log sessions. A **numerical exertion scale** (1–10) feeds **progression strategies** that recalculate suggested loads and reps for the next session.

**Who it is for:** Beginners needing starting loads, intermediates tuning volume, and advanced lifters adjusting after fatigue feedback.

**Core features:**

- SQLite-backed user profile and exercise catalog persistence  
- Initial plan generation from max lifts and goals  
- Console I/O for setup, plan review, optional workout logging  
- Pluggable progression (**hypertrophy** vs **strength** rules) applied to the accepted plan  

**Assumptions and constraints**

- **.NET 9 SDK** is required (see section 2).  
- **SQLite** file `adaptive_hypertrophy.db` is created beside the built executable (`AppContext.BaseDirectory`). If you previously ran an older build that created an incompatible `WorkoutLogs` table, delete `adaptive_hypertrophy.db` in the output folder and run again for a clean schema.  
- Max lifts use a **fixed catalog** of exercise keys (`ExerciseCatalog`); only those lifts appear in generated plans.  
- The app demonstrates progression on a **sample** performance slice in `Program.cs` (first two plan entries) in addition to interactive logging.  

---

## 2. Build & Run Instructions

**Tools and versions**

| Tool | Version |
|------|---------|
| .NET SDK | **9.0** or compatible (project targets `net9.0`) |
| NuGet | `Microsoft.Data.Sqlite` (see `src/AdaptiveHypertrophy/AdaptiveHypertrophy.csproj`) |

**Clean checkout — exact steps**

1. Open a terminal at the **repository root** (folder containing `readme.md` and `src/`).  
2. Restore and build:  
   ```bash
   dotnet restore src/AdaptiveHypertrophy/AdaptiveHypertrophy.csproj
   dotnet build src/AdaptiveHypertrophy/AdaptiveHypertrophy.csproj
   ```  
3. Run:  
   ```bash
   dotnet run --project src/AdaptiveHypertrophy/AdaptiveHypertrophy.csproj
   ```  

**Optional:** Open `AdaptiveHypertrophy.slnx` in Visual Studio / Rider and run the executable project.

**Configuration:** No environment variables or CLI arguments are required. Follow prompts; for optional workout logging, enter exercises until you submit a **blank exercise name** to finish.

---

## 3. Required OOP Features (with File & Line References)

| OOP feature | File | Line(s) | Reasoning / purpose |
|-------------|------|---------|----------------------|
| **Inheritance (1)** — base + derived | `Exercise.cs` | 5–33 | Abstract base `Exercise` defines shared state and abstract hooks. |
| **Inheritance (2)** — derived | `CompoundExercise.cs` | 3–40 | `CompoundExercise` inherits `Exercise` and specializes compound-lift behavior and volume. |
| **Inheritance (3)** — derived | `IsolationExercise.cs` | 3–40 | `IsolationExercise` inherits `Exercise` for accessory/isolation behavior. |
| **Interface — `IProgressionStrategy`** | `HypertrophyProgressionStrategy.cs` | 6–71 | Implements progression contract for hypertrophy-style double progression. |
| **Interface — `IProgressionStrategy`** | `StrengthProgressionStrategy.cs` | 6–73 | Second implementation for strength-biased load/rep rules. |
| **Interface — `IRepository<T>`** | `ExerciseRepository.cs` | 6–88 | `IRepository<Exercise>` for SQLite exercise catalog CRUD-style access. |
| **Interface — `IRepository<T>`** | `WorkoutRepository.cs` | 6–88 | `IRepository<WorkoutLog>` for persisting and loading log rows. |
| **Interface — `IWorkoutDisplay`** | `ConsoleWorkoutDisplay.cs` | 5–105 | Implements console UI behind `IWorkoutDisplay` for testability and polymorphism. |
| **Polymorphism (1)** — override dispatch | `CompoundExercise.cs` | 24–38 | Overrides `GetDescription`, `EstimateVolume`, `GetExerciseType` from `Exercise`. |
| **Polymorphism (2)** — override dispatch | `IsolationExercise.cs` | 24–38 | Same abstract methods, different implementations. |
| **Polymorphism (3)** — interface dispatch | `Program.cs` | 79–85 | `IProgressionStrategy` reference bound at runtime to `StrengthProgressionStrategy` or `HypertrophyProgressionStrategy` by `WorkoutGoal`. |
| **Polymorphism (4)** — interface dispatch | `WorkoutPlan.cs` | 18–49 | `ApplyProgression` calls `progression.CalculateNextWeight` / `CalculateNextReps` without knowing concrete strategy type. |
| **Access modifiers** | `Exercise.cs` | 5–14 | `abstract class` with **`protected`** constructor so only subclasses construct; public surface for domain use. |
| **Access modifiers** | `DatabaseConnectionManager.cs` | 9–24 | **`sealed`**, **`private`** constructor; **`public static`** `GetInstance()` — Singleton construction control. |
| **Access modifiers** | `User.cs` | 5–15 | **`sealed`** profile aggregate; **`private`** constructor; **`private`** `Lazy<User>`; public **`static`** `Instance` accessor. |
| **Struct** | `SetPerformance.cs` | 3–17 | **`readonly struct`** models an immutable set outcome (reps, weight, exertion) on the stack-friendly value type. |
| **Enum (1)** | `ExperienceLevel.cs` | 3–8 | User experience tier for scaling and UX. |
| **Enum (2)** | `WorkoutGoal.cs` | 3–8 | Selects goal-sensitive rep targets and progression strategy. |
| **Data structure — `List<T>`** | `WorkoutLog.cs` | 5–21 | Mutable collection of `ExerciseEntry` for a session. |
| **Data structure — `Dictionary<K,V>`** | `User.cs` | 9 | `_maxLiftsByKey` stores max weights by catalog key. |
| **Data structure — `Dictionary` / `HashSet`** | `Program.cs` | 38–40, 60–73 | `HashSet` for deduplicating exercise names; `Dictionary` for last performance by exercise. |
| **I/O** | `ConsoleWorkoutDisplay.cs` | 97–101 | `GetInput` uses `Console.ReadLine()`; other methods use `Console.WriteLine` for output. |
| **I/O** | `Program.cs` | 94–114 | Interactive workout logging loop reading numeric and text input. |

---

## 4. Design Patterns (with File & Line References)

This project documents **three standard patterns** spanning **three categories** (Creational, Behavioral, Structural), satisfying the “≥2 different categories” rule. These patterns are *Singleton*, *Strategy*, and *Repository*.

| Pattern | Category | File | Line(s) | Rationale |
|---------|----------|------|---------|-----------|
| **Singleton** | Creational (GoF) | `DatabaseConnectionManager.cs` | 9–30 | A **`Lazy<T>`**-backed **`sealed`** type with a **private** constructor ensures a **single** shared SQLite connection-string factory for the process. Call sites use **`GetInstance()`** instead of ad hoc `new`, so repositories share one database configuration. This matches Singleton intent: controlled global access to one resource coordinator. |
| **Strategy** | Behavioral (GoF) | `IProgressionStrategy.cs` | 6–11 | Defines interchangeable algorithms (`CalculateNextWeight`, `CalculateNextReps`). |
| | | `HypertrophyProgressionStrategy.cs` | 6–71 | Concrete strategy for hypertrophy-style rep-range and load progression. |
| | | `StrengthProgressionStrategy.cs` | 6–73 | Concrete strategy with strength-oriented rules (e.g. `CompoundExercise` main-lift handling at 21). |
| | | `WorkoutPlan.cs` | 18–49 | **`ApplyProgression`** accepts **`IProgressionStrategy`** and delegates all next-weight/rep math without `switch` on concrete types. Callers pick the strategy at runtime (e.g. `Program.cs` 79–85 from `WorkoutGoal`). |
| **Repository** | Structural (data-access) | `IRepository.cs` | 5–10 | Mediates between domain objects and persistence with a minimal **`Save`** / **`GetAll`** contract, hiding SQL and ADO details from the rest of the system. |
| | | `ExerciseRepository.cs` | 6–88 | Implements persistence for **`Exercise`** rows (parameterized SQLite in **`Save`**, mapping in **`GetAll`**). |
| | | `WorkoutRepository.cs` | 6–88 | Same abstraction for **`WorkoutLog`** aggregates and the **`WorkoutLogs`** table. |

**Why these fit:** **Singleton** centralizes one DB manager instance. **Strategy** isolates progression algorithms so new goals/rules can be added by implementing **`IProgressionStrategy`** rather than editing **`WorkoutPlan`**. **Repository** keeps persistence concerns in thin, testable classes behind **`IRepository<T>`**, so **`Program`** works mostly with domain types.

---

## 5. Design Decisions

- **Layering:** `Domain` holds aggregates and value types; `Exercises` models lift taxonomy; `Progression` encapsulates algorithms; `Data` implements persistence; `Display` isolates console rendering; `Setup` and `Planning` orchestrate onboarding and plan building.  
- **`User` as lazy single session instance:** One in-memory profile (`User.Instance`) matches the single-user console scope and avoids passing a `User` through every static entry point while keeping construction **`private`**. Persistence is still explicit via `UserRepository`.  
- **`IWorkoutDisplay`:** Decouples `Program` and `InitialSetupFlow` from raw `Console` calls where `IWorkoutDisplay` is injected, improving testability and clarity of boundaries.  
- **`WorkoutPlan.ApplyProgression`:** Keeps progression math out of `Program` and makes the **Strategy** attachment point obvious.  
- **SQLite:** Local file DB avoids external services and satisfies assignment I/O and persistence demonstration; schema is created on first run (see section 1 for upgrade note).  
- **Tradeoff:** `ExerciseCatalog` is static and small by design (MVP scope); expanding lifts requires code/catalog changes rather than arbitrary user-defined movements.  

---

## 6. UML Diagrams

Final diagram(s) live under **`uml/`**:

| File | Contents |
|------|----------|
| `uml/class-diagram.puml` | PlantUML class diagram — classes, key members, relationships, Strategy / Repository / Singleton collaborators. |
| `uml/class-diagram.png` | Rendered png of above PlantUML class diagram.|

---

## 7. Folder Structure (Milestone 6)

```
/src     — C# source (`src/AdaptiveHypertrophy` project)
/uml     — UML diagram(s); see section 6
/docs    — Optional supporting files (`docs/` may contain only placeholders)
/readme.md — This file (required name for the team ZIP)
```

The solution file `AdaptiveHypertrophy.slnx` at the repo root is optional tooling; **`dotnet`** commands in section 2 use the **`.csproj`** under `src/`, sufficient for a clean build from only `/src` sources.

---
