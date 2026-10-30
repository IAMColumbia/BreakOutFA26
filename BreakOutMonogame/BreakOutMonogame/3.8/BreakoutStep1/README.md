# Breakout (MonoGame 3.8)

Breakout built on top of `MonoGameLibrary`. Bounce the ball off the paddle, break the
wall, don't drop the ball.

## What you need

- Windows. The game project uses `MonoGame.Framework.WindowsDX` and WinForms, so it
  only runs on Windows.
- The .NET 9 SDK. The project targets `net9.0-windows7.0`, a newer SDK builds it fine
  as long as the .NET 9 targeting pack is installed.
- Nothing else to install. Content is built by `MonoGame.Content.Builder.Task` as part
  of the normal build, and the NuGet packages restore on the first build.

## Start it

From this folder (the one with `BreakoutStep1.sln`):

```
dotnet run --project BreakoutStep1
```

Or open `BreakoutStep1.sln` in Visual Studio, make sure `BreakoutStep1` is the startup
project, and press F5.

First build takes a minute while NuGet restores and the content pipeline builds the
textures and the font. After that it's quick.

## Controls

| Do this | To |
| --- | --- |
| Left / Right arrow | Move the paddle |
| Move the mouse left or right | Move the paddle |
| Up arrow, or left click | Launch the ball off the paddle |
| Esc, or gamepad Back | Quit |

## How a game goes

- The ball rides the paddle until you launch it. Each launch leans a random amount left
  or right so you don't get the same path every time.
- Blocks take two hits. The first hit cracks the block and changes its texture and is
  worth 10 points, the second breaks it and is worth 100.
- Break the whole wall and you get 500 bonus points and a fresh wall to keep going.
- Miss the ball and you lose a life. The ball goes back on the paddle and waits for you
  to launch it again.
- You start with 3 lives. Run out and it's GAME OVER, the score resets and a new wall
  goes up. Score and lives are in the top left corner the whole time.

## What's in here

| File | What it does |
| --- | --- |
| `BreakoutStep1/Game1.cs` | Builds the components in dependency order and watches for a cleared wall or a game over |
| `BreakoutStep1/Ball.cs` | Ball movement, wall bounces, and the miss that costs a life |
| `BreakoutStep1/Paddle.cs` | Paddle movement, keeps the ball on the paddle before launch, bounces the ball |
| `BreakoutStep1/PaddleController.cs` | Reads the keyboard and the mouse for the paddle |
| `BreakoutStep1/Block.cs` | A block's hit count and state, no drawing |
| `BreakoutStep1/MonogameBlock.cs` | The drawable side of a block, swaps textures as it gets hit |
| `BreakoutStep1/BlockWall.cs` | Builds and lays out the wall, and checks the ball against every block |
| `BreakoutStep1/ScoreManager.cs` | Score and lives, draws the HUD |
| `BreakoutStep1/FloatingText.cs` | One line of pop up text, how long it lives and how it drifts |
| `BreakoutStep1/FloatingTextManager.cs` | Draws the pop up text in the middle of the screen |
| `MonoGameLibrary/` | The shared library the game is built on, sprites, input, console, sound |

The managers are game services. Anything that needs one calls `GetOrAddService` and gets
the one in the game, or a new one added to the game if there isn't one yet.

## Content

Assets live in `BreakoutStep1/Content` and are listed in `Content.mgcb`, which you can
open with the MGCB Editor. The pop up text and the HUD use `Content/FunFont.spritefont`,
which builds from Comic Sans MS. That font ships with Windows so there's no `.ttf` in the
folder. If the content build ever complains it can't find the font, change `FontName` in
that file to a font you do have.

## Also in this folder

`BreakoutStep1.Web` is a separate Blazor/KNI build of the same game and is a work in
progress. These instructions are for the Windows build.
