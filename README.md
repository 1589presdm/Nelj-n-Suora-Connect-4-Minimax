# Neljän Suora — Connect-4 Minimax

A desktop Connect-4 game with AI opponent using the Minimax algorithm, developed as a student project at Karelia University of Applied Sciences (AMK Karelia).

This project demonstrates game logic, WPF UI, and AI decision-making using Minimax with alpha-beta pruning.

---

## 🎯 Project Overview

The application allows players to compete against an AI opponent that uses Minimax logic to simulate intelligent moves. It supports:

- Interactive WPF-based UI
- Human vs AI gameplay
- Role-based visual feedback (player and AI colors)
- Win detection and draw detection

---

## 🏫 Project Details

- **Institution:** Karelia University of Applied Sciences (AMK Karelia)
- **Course:** Tekoäly ja robotiikka (Artificial Intelligence and Robotics)
- **Type:** Student project
- **Team:** 5 students
- **Platform:** Desktop (WPF)
- **Language:** C# (.NET 8)
- **AI:** Minimax algorithm with alpha-beta pruning

---

## ⚙️ Features

### Game Logic

- Connect-4 logic with a 6x7 grid
- Turn-based game flow
- Win, draw, and invalid move detection
- Board cloning for AI calculations

### AI (Computer.cs)

- **Minimax algorithm with alpha-beta pruning** for optimal move selection
- Board evaluation function to estimate the current state
- Dynamic depth weighting to favor quicker wins

### UI (MainWindow)

- Hover effect for possible player moves
- Dynamic feedback (turn indicators and game status)
- New game button to reset

### Documentation

- Comprehensive XML comments on all public methods
- Board management (`Board.cs`)
- AI logic (`Computer.cs`)
- UI interactions (`MainWindow.xaml.cs`)

---

## 🎨 Design Details

- Player token color: **Coral**
- AI token color: **Yellow**

---

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or newer (with WPF support)

### Setup

1. Clone or download this repository.

2. Open `Connect4Minimax.sln` in Visual Studio.

3. Build and run the solution.

4. Play against the AI and test different strategies!

---

## 🧾 License

This is a student project and not intended for production use without further development.

---

## 👥 Authors

- [Kulmala Katja](https://github.com/Boustaaja)
- Varkoi Patrick
- Heiskanen Saku
- Mälkönen Kasper Petja
- [Presniakov Dmitrii](https://github.com/1589presdm)

---