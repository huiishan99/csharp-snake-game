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

### Fixed
- Removed the trailing blank line from `.gitignore` so repository-wide whitespace checks pass.

### Fixed
- Centered the initial snake position within the current playable grid instead of using a hard-coded cell.
- Added a minimum window size to prevent the board from collapsing into unusable dimensions.
- Repositioned menu and pause controls when the form is resized.
- Kept existing snake and food coordinates inside the playable grid after resizing.

### Added
- Persisted the best score with user-scoped application settings so it survives app restarts.

### Changed
- Renamed the custom game settings type to `GameSettings` to avoid confusion with `Properties.Settings`.
- Removed unused imports from the main form code.

### Changed
- Extracted snake movement, food placement, scoring, pause, and win/loss state into `SnakeGameEngine`.
- Kept `Form1` focused on WinForms input, layout, persistence, and rendering.
- Clarified engine direction naming and prevented repeated finish dialogs after resizing a completed round.

### Added
- Added a lightweight `SnakeGame.Tests` console project for rule-level engine checks.
- Documented how to run the engine checks from Visual Studio or the built executable.

### Added
- Added a Windows GitHub Actions workflow to build the solution and run engine checks on push and pull requests.

### Changed
- Split reusable game types (`Direction`, `GameStatus`, and `GridCell`) out of `GameEngine.cs`.

### Changed
- Reworked HUD label layout so score, best score, speed, and status resize with the window.
- Enabled ellipsis behavior for HUD labels to avoid overlap when text grows.

### Added
- Added deterministic engine checks for food growth and self-collision behavior.
- Exposed engine internals to the test project without making test setup APIs public.

### Added
- Enabled manual runs for the Windows GitHub Actions build workflow.

### Changed
- Added a cohesive dark visual theme for the window, HUD, board, snake, and food.
- Added anti-aliased drawing, board grid lines, rounded snake segments, and food highlights.
- Styled the start and pause buttons to match the game theme.

### Changed
- Replaced finish message boxes with in-board start, pause, game-over, and win overlays.
- Added overlay typography and hints for keyboard controls.

### Changed
- Updated README with current UI features and GitHub Actions usage.

### Changed
- Added a short direction input queue so fast corner turns are preserved across ticks.
- Added an engine check for buffered corner input behavior.

### Added
- Added a boundary mode option for classic wrap-wall play or solid-wall game-over behavior.
- Added UI and engine checks for solid-wall mode.

### Added
- Packaged the Release build as a downloadable `SnakeGame-Windows-Release` GitHub Actions artifact.
- Documented how to download the packaged executable from a workflow run.

### Added
- Persisted the selected speed and wrap-wall preference with user-scoped application settings.
- Restored saved player preferences when the game window opens.

### Added
- Added an optional progressive-speed mode that increases game pace as score rises.
- Added a start-menu checkbox and saved preference for progressive speed.
- Updated the HUD speed label to show when progressive speed is active.

### Changed
- Extracted speed clamping, timer interval calculation, and speed labels into a reusable `GameSpeed` rule class.
- Added checks for fixed speed, progressive speed, interval caps, and speed display labels.

### Changed
- Added a themed starting-speed label above the speed slider so menu changes give immediate feedback.
- Added a solid-wall board border so the selected boundary mode is visually clear before and during play.

### Added
- Added an optional obstacle mode with generated blocked cells and themed obstacle rendering.
- Prevented food from spawning on obstacles and ended the round when the snake hits one.
- Persisted the obstacle preference and added obstacle generation/collision checks.

### Added
- Added a Windows testing guide covering Visual Studio, VS Code/terminal, GitHub Actions artifacts, and gameplay smoke tests.
- Linked the testing guide from the README.
- Included project docs in the packaged GitHub Actions artifact.

### Changed
- Reworked the start and restart UI into a centered themed settings panel.
- Switched HUD, overlay, button, and menu text to a Segoe UI font stack.
- Restyled game options as compact toggle buttons instead of default WinForms checkboxes.

### Changed
- Replaced the default Windows speed slider with themed `-` and `+` speed step buttons.
- Kept speed persistence and progressive-speed labels working with the new stepper control.

### Changed
- Restyled the top HUD as compact pill labels with status-aware colors.
- Added a HUD divider and stronger hover/press colors for themed buttons.

### Added
- Added keyboard shortcuts for changing start-menu speed with Left/Right or `-`/`+`.
