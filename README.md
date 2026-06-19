# autoCardboard

A toolkit for developing bots that play supported board games.

The repository contains game logic, bot helpers, an API for running games, and an Angular GamesRoom application for displaying game state.

## Supported games

- [For Sale](https://www.ultraboardgames.com/for-sale/game-rules.php)
- [Pandemic](https://www.ultraboardgames.com/pandemic/game-rules.php)

## Main components

- Bot libraries that reduce the amount of code needed to write or tune bots.
- Pandemic helper logic, including shortest-path analysis between cities using Dijkstra's algorithm.
- API projects for running supported games and returning final game state as JSON.
- MQTT messages during play, which can be used by real-time displays.
- Swagger documentation for the API.
- `autoCardboard.GamesRoom`, an Angular 8 SPA for running games and rendering game state.

## Build

Open `autoCardboard.sln` in Visual Studio or build with the .NET SDK from the repository root.

```bash
dotnet build autoCardboard.sln
```

## Presentation

There is a [presentation about the project](https://docs.google.com/presentation/d/1nDATDgEdOwdXd4_0JmDBBXhuMUGC-TO7n47whI2fgt8/edit?usp=sharing).
