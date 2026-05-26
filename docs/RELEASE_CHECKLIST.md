# Release Checklist

Use this checklist before tagging or sharing a Windows build.

## Build

1. Build `SnakeGame.sln` in `Release` mode on Windows.
2. Run `SnakeGame.Tests`.
3. Confirm the `Windows Build` GitHub Action passes.
4. Download and inspect the versioned `SnakeGame-Windows-Release-<version>-<sha>` artifact.
5. Confirm the zip contains `SnakeGame.exe`, config, README, docs, and `BUILD_INFO.txt`.

## Gameplay Smoke Test

1. Start a default wrap-wall run.
2. Move with arrow keys and `WASD`.
3. Pause and resume with `Space`.
4. Adjust menu speed with `-`, `+`, Left, and Right.
5. Confirm each mode preset updates speed, wall, progressive, and obstacle options.
6. Confirm each board-size preset resizes the window before starting.
7. Confirm challenge seeds replay the same initial food and obstacle layout.
8. Confirm a scoring run updates the per-mode leaderboard.
9. Confirm progressive speed shows a `+` next to speed.
10. Confirm solid-wall mode shows the red border and ends the run on wall collision.
11. Confirm obstacle mode draws blocked cells and ends the run on obstacle collision.
12. Confirm the sound toggle mutes start, eat, pause/resume, and finish sounds.
13. Confirm game-over and win states return to the restart panel.
14. Confirm the window cannot be resized during an active run and can be resized again after game-over or win.
15. Close and reopen the app, then confirm saved options and best score are restored.

## UI Review

1. Check the start panel at the minimum window size.
2. Check the HUD labels at the minimum window size.
3. Check button hover and pressed states.
4. Check solid-wall border visibility with and without overlays.
5. Check that no text overlaps in the start panel or HUD.
6. Check that the start panel still covers every menu control at normal Windows scaling.
7. Check that the animated title-screen preview and eat feedback are smooth at normal Windows scaling.

## Release Notes

Mention these player-facing changes:

- Themed board, HUD, and start panel
- Persistent best score and player preferences
- Optional wrap-wall or solid-wall play
- Optional progressive speed
- Optional obstacles
- Mode presets and fixed board-size presets
- Per-mode local leaderboards
- Deterministic challenge seeds
- Optional sound effects with a mute toggle
- Keyboard support for movement, pause, restart, and speed setup
