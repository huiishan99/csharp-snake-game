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

## Engine Checks

The solution includes a small `SnakeGame.Tests` console project for rule-level checks. Run it from Visual Studio, or build the solution and run `SnakeGame.Tests.exe`.

## GitHub Actions

The `Windows Build` workflow builds the solution and runs the engine checks on push, pull request, or manual runs from the GitHub Actions tab.

## Controls

- Arrow keys or `WASD`: move the snake
- Space: pause or resume
- Enter: start or restart when the game is ready
- Speed slider: choose the starting speed before the game begins

## Features

- Themed board with grid, styled snake, and highlighted food
- In-board start, pause, game-over, and win overlays
- Persistent best score
- Rule-level engine checks in `SnakeGame.Tests`

## Project Notes

- The game currently targets .NET Framework 4.7.2.
- Development history is tracked in `DEVLOG.md`.
- Future work will focus on richer gameplay options and cleaner separation between game rules and UI code.
