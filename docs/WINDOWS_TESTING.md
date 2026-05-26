# Windows Testing Guide

Use this checklist when validating the game on Windows before sharing a build.

## Visual Studio

1. Install Visual Studio 2022 with the `.NET desktop development` workload.
2. Open `SnakeGame.sln`.
3. Set `SnakeGame` as the startup project.
4. Build the solution in `Release` mode.
5. Run `SnakeGame.Tests` once to verify the rule checks.
6. Start `SnakeGame` and complete the smoke test below.

## VS Code Or Terminal

VS Code can run the project if Windows has MSBuild for .NET Framework installed. The easiest route is Visual Studio 2022 Build Tools or full Visual Studio 2022.

Open a Developer PowerShell or Developer Command Prompt in the repository root:

```powershell
msbuild SnakeGame.sln /p:Configuration=Release /m
.\SnakeGame.Tests\bin\Release\SnakeGame.Tests.exe
.\bin\Release\SnakeGame.exe
```

If `msbuild` is not found, open the terminal from Visual Studio's Developer PowerShell entry or install the Visual Studio Build Tools.

## GitHub Actions

The `Windows Build` workflow builds the Release solution, runs `SnakeGame.Tests`, writes a build summary, and uploads a versioned zip artifact.

1. Push the branch to GitHub.
2. Open the repository's `Actions` tab.
3. Choose `Windows Build`.
4. Use `Run workflow` for a manual build, or wait for the push build.
5. Open the finished run and download the `SnakeGame-Windows-Release-<version>-<sha>` artifact.
6. Unzip it and check `BUILD_INFO.txt` before running the executable.

## Smoke Test

1. Start a run with default settings.
2. Confirm the start countdown appears before the snake moves.
3. Move with arrow keys and `WASD`.
4. Pause and resume with `Space`, then confirm the resume countdown appears.
5. Restart from the game-over overlay with `Enter`.
6. Change menu speed with `-`, `+`, Left, and Right.
7. Choose each `Mode` preset and confirm speed, wall, progressive, and obstacle options update.
8. Choose each `Board` preset and confirm the window resizes before the run starts.
9. Type a `Challenge` seed, restart with the same seed, and confirm obstacle/food placement is repeatable.
10. Finish a scoring run and confirm the mode leaderboard updates on the start panel.
11. Turn off `Wrap walls` and confirm the board shows a red border and wall collision ends the run.
12. Turn on `Progressive speed` and confirm the HUD shows a `+` next to speed.
13. Turn on `Obstacles` and confirm blocked cells appear and collision ends the run.
14. Toggle `Sound` and confirm start, eat, pause/resume, and finish sounds respect the setting.
15. Start a run and confirm the window cannot be resized until the game returns to the restart panel.
16. Confirm the start panel covers all controls at the minimum window size and normal Windows scaling.
17. Confirm the start panel preview snake animates smoothly and the eat feedback is visible without feeling distracting.
18. Close and reopen the app to confirm speed, mode, board, challenge, leaderboard, sound, and best-score settings are restored.
