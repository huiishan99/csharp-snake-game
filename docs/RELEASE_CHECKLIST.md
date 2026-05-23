# Release Checklist

Use this checklist before tagging or sharing a Windows build.

## Build

1. Build `SnakeGame.sln` in `Release` mode on Windows.
2. Run `SnakeGame.Tests`.
3. Confirm the `Windows Build` GitHub Action passes.
4. Download and inspect the `SnakeGame-Windows-Release` artifact.

## Gameplay Smoke Test

1. Start a default wrap-wall run.
2. Move with arrow keys and `WASD`.
3. Pause and resume with `Space`.
4. Adjust menu speed with `-`, `+`, Left, and Right.
5. Confirm progressive speed shows a `+` next to speed.
6. Confirm solid-wall mode shows the red border and ends the run on wall collision.
7. Confirm obstacle mode draws blocked cells and ends the run on obstacle collision.
8. Confirm game-over and win states return to the restart panel.
9. Close and reopen the app, then confirm saved options and best score are restored.

## UI Review

1. Check the start panel at the minimum window size.
2. Check the HUD labels at the minimum window size.
3. Check button hover and pressed states.
4. Check solid-wall border visibility with and without overlays.
5. Check that no text overlaps in the start panel or HUD.

## Release Notes

Mention these player-facing changes:

- Themed board, HUD, and start panel
- Persistent best score and player preferences
- Optional wrap-wall or solid-wall play
- Optional progressive speed
- Optional obstacles
- Keyboard support for movement, pause, restart, and speed setup
