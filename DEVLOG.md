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

### Added
- Added an in-window HUD for score, best score, speed, and game status.
- Added pause/resume support through a button and the spacebar.
- Added Enter-to-start support when the game is ready or over.
- Reserved vertical space for the HUD so gameplay does not draw underneath it.
- Updated README controls to include pause and keyboard start/restart.

### Fixed
- Buffered direction changes so rapid key presses cannot reverse the snake within a single tick.
- Reworked food placement to choose from available grid cells and avoid infinite retries.
- Added a win state when the snake fills the entire playable grid.

### Changed
- Stopped tracking generated Visual Studio state and build output files now covered by `.gitignore`.
