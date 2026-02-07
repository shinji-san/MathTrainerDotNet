# MathTrainerDotNet

[![Build and Test](https://github.com/shinji-san/MathTrainerDotNet/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/shinji-san/MathTrainerDotNet/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

🇩🇪 **[Deutsche Version](#deutsch)** | 🇬🇧 **[English Version](#english)**

---

## Deutsch

Eine Blazor-Webanwendung für Grundschüler zum Üben aller vier Grundrechenarten.

### Features

- 🎲 **Zufällige Aufgabengenerierung** mit einstellbarem Zahlenraum
- ➕ **Alle Rechenarten**: Addition, Subtraktion, Multiplikation, Division
- 🔀 **Gemischte Modi**: 
  - Multiplikation & Division
  - Addition & Subtraktion
  - Alle Rechenarten kombiniert
- 🔢 **Variable Operanden**: 2-5 Operanden pro Aufgabe (z.B. `3 + 5 + 2 = `)
- 👤 **Schülerverwaltung**: Namen speichern und wiederverwenden
- ✅ **Automatische Prüfung** ohne Ergebnis preiszugeben
- 👁️ **Ergebnisse anzeigen** per Button
- 📄 **PDF-Export** mit Seitenzahlen zum Ausdrucken
- ✅ **Lösungs-PDF** separat herunterladbar
- 🔑 **Eindeutige ID** zum späteren Abrufen
- 💾 **SQLite-Datenbank** für persistente Speicherung
- 🌙 **Dark/Light Theme**
- 🌍 **Deutsch/Englisch** umschaltbar

### Voraussetzungen

- .NET 10.0 SDK oder höher

### Installation & Start

```bash
cd MathTrainerDotNet
dotnet restore
dotnet run --project MathTrainerDotNet
```

Die Anwendung ist unter `http://localhost:5000` erreichbar.

### Tests ausführen

```bash
dotnet test
```

---

## English

A Blazor web application for elementary school students to practice all four basic arithmetic operations.

### Features

- 🎲 **Random exercise generation** with customizable number range
- ➕ **All operations**: Addition, Subtraction, Multiplication, Division
- 🔀 **Mixed modes**: 
  - Multiplication & Division
  - Addition & Subtraction
  - All operations combined
- 🔢 **Variable operands**: 2-5 operands per exercise (e.g., `3 + 5 + 2 = `)
- 👤 **Student management**: Save and reuse student names
- ✅ **Automatic checking** without revealing the answer
- 👁️ **Show results** with a button
- 📄 **PDF export** with page numbers for printing
- ✅ **Solution PDF** downloadable separately
- 🔑 **Unique ID** for later retrieval
- 💾 **SQLite database** for persistent storage
- 🌙 **Dark/Light theme**
- 🌍 **German/English** switchable

### Prerequisites

- .NET 10.0 SDK or higher

### Installation & Start

```bash
cd MathTrainerDotNet
dotnet restore
dotnet run --project MathTrainerDotNet
```

The application is available at `http://localhost:5000`.

### Run Tests

```bash
dotnet test
```

---

## Solution Structure / Projektstruktur

```
MathTrainerDotNet/
├── MathTrainerDotNet.slnx              # Solution file (SLNX format)
├── Dockerfile                          # Docker configuration
├── docker-compose.yml                  # Docker Compose setup
├── README.md
│
├── MathTrainerDotNet/                  # Main application project
│   ├── Components/
│   │   ├── Pages/
│   │   │   ├── Home.razor
│   │   │   └── HomeComponents/         # Sub-components for Home page
│   │   ├── App.razor
│   │   └── Routes.razor
│   ├── Data/
│   │   ├── Helper/
│   │   └── AppDbContext.cs
│   ├── Models/
│   │   ├── Exercise.cs
│   │   ├── ExerciseSet.cs
│   │   ├── OperationType.cs
│   │   └── Student.cs
│   ├── Resources/
│   │   ├── Strings.resx
│   │   ├── Strings.de.resx
│   │   └── Strings.en.resx
│   ├── Services/
│   │   ├── Backup/                     # SQLite Backup & Restore
│   │   ├── Format/                     # Date formatting
│   │   ├── Id/                         # Public ID generation
│   │   ├── Localization/               # Multi-language support
│   │   ├── Pdf/                        # PDF generation (QuestPDF)
│   │   ├── Repository/                 # Data access layer
│   │   └── ExerciseGeneratorService.cs # Core business logic
│   ├── ViewModels/
│   │   ├── ExerciseSetViewModel.cs
│   │   └── ExerciseViewModel.cs
│   └── wwwroot/css/
│       └── app.css
│
└── MathTrainerDotNetTest/              # Unit test project
    ├── Models/
    ├── Services/
    └── ViewModels/
```

## Technologies / Technologien

- **ASP.NET Core 10.0** with Blazor Server
- **Entity Framework Core 10.0** with SQLite
- **QuestPDF** for PDF generation
- **xUnit** for unit testing
- **Moq** for mocking

## License / Lizenz

MIT License - Free to use and modify.
MIT-Lizenz - Frei zur Verwendung und Modifikation.
