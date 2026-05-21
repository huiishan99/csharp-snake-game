# Devlog

## 2026-05-21

### Added
- Created this devlog so each project improvement has a visible history.
- Added `.gitignore` entries for Visual Studio state, build outputs, and temporary files.
- Expanded the README with clearer build, run, and control instructions.

### Fixed
- Kept the game timer stopped until the player starts a round.
- Generated food when a new game starts instead of relying on the default `(0, 0)` position.
- Prevented the board from drawing food before a round has started.
- Removed a duplicate paint call that could cause unnecessary redraw work.
- Added safe default speed slider values and clamped timer intervals.
