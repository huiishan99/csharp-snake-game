# SnakeGame

A small classic Snake game built with C# WinForms.

![屏幕截图 2024-04-23 231232](https://github.com/huiishan99/CSharp_SnakeGame/assets/61934115/8e19b685-2f5c-45f3-9512-430365c9d2e6)

## Requirements

- Windows
- Visual Studio 2022, or another environment that can build .NET Framework 4.7.2 WinForms projects

## Run From Source

1. Open `SnakeGame.sln` in Visual Studio.
2. Restore/build the solution.
3. Start the `SnakeGame` project.

For detailed local, VS Code, and GitHub Actions validation steps, see [Windows Testing Guide](docs/WINDOWS_TESTING.md).

For release readiness checks, see [Release Checklist](docs/RELEASE_CHECKLIST.md).

## Checks

The solution includes a small `SnakeGame.Tests` console project for rule-level engine and speed checks. Run it from Visual Studio, or build the solution and run `SnakeGame.Tests.exe`.

## GitHub Actions

The `Windows Build` workflow builds the solution and runs the engine checks on push, pull request, or manual runs from the GitHub Actions tab.

When the workflow passes, download the `SnakeGame-Windows-Release` artifact from the workflow run to get the packaged executable and config file.

## Controls

- Arrow keys or `WASD`: move the snake
- Space: pause or resume
- Enter: start or restart when the game is ready
- Speed `-` / `+`, Left / Right: choose the starting speed before the game begins
- Wrap walls: turn classic edge wrapping on or off before starting
- Progressive speed: make the snake gradually speed up as the score rises
- Obstacles: add blocked cells for a harder run

## Features

- Layered board background with subtle cell texture, major-grid rhythm, styled snake, and highlighted food
- Richer start panel with display typography, accent line, and a subtle snake/food motif
- Custom rounded gradient buttons and toggles with restrained proportions and focus states
- Theme-matched speed stepper instead of the default Windows slider
- Compact self-painted HUD pills with status-aware colors
- Solid-wall mode shows a visible board border before and during play
- Optional obstacle mode with collision checks and safe food placement
- In-board start, pause, game-over, and win overlays
- Persistent best score, speed, wall-mode, progressive-speed, and obstacle preferences
- Optional classic wrap-wall or solid-wall play
- Optional progressive speed for longer, more intense runs
- Rule-level engine and speed checks in `SnakeGame.Tests`

## Project Notes

- The game currently targets .NET Framework 4.7.2.
- Development history is tracked in `DEVLOG.md`.
- The current focus is Windows verification and release polish.
