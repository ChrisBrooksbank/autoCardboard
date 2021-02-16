# autoCardboard
models board games played by bots.

Developers can write logic which plays turns on any supported board game. ( Currently Pandemic and For Sale )

> For Sale is a quick, fun game nominally about buying and selling real estate. During the game's two distinct phases, players first bid for several buildings then, after all buildings have been bought, sell the buildings for the greatest profit possible.
[ForSale rules](https://www.ultraboardgames.com/for-sale/game-rules.php )

> Pandemic is a cooperative board game designed by Matt Leacock and first published by Z-Man Games in the United States in 2008.[1] Pandemic is based on the premise that four diseases have broken out in the world, each threatening to wipe out a region. The game accommodates two to four players, each playing one of seven possible roles: dispatcher, medic, scientist, researcher, operations expert, contingency planner, or quarantine specialist. Through the combined effort of all the players, the goal is to discover all four cures before any of several game-losing conditions are reached.
[Pandemic rules](https://www.ultraboardgames.com/pandemic/game-rules.php)

An *API* is provided which enables supported games to be played.
* End gamestate is returned as JSON.
* MQTT messages are broadcast during play e.g. if realtime display of moves is required.

The *Bot Library* provides helper methods.
Designed to be a toolkit for developers tweaking the existing bots, or building new ones.
e.g. the Pandemic bot library provides shortest path analysis for optimal routes between any two Pandemic cities, using Dijkstra's algorithm.

MQTT messaging is used as a way for interested code to monitor game progress in realtime.

*GamesRoom* project provides a ( Angular 8 SPA ) application enabling games to be run, and game state to be rendered. GamesRoom calls the API to play games.
