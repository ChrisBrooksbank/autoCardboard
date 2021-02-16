# autoCardboard
models board games played by bots.

Developers can write logic which plays turns on any supported board game. ( Currently Pandemic and For Sale )

API is provided which enables supported games to be played.
* End gamestate is returned as JSON.
* MQTT messages are broadcast during play e.g. if realtime display of moves is required.

Initial implentation of two games : 
* For Sale ( https://www.ultraboardgames.com/for-sale/game-rules.php ) 
* and Pandemic (https://www.ultraboardgames.com/pandemic/game-rules.php). 
For Sale is competitive, Pandemic is co-operative.

> Pandemic is a cooperative board game designed by Matt Leacock and first published by Z-Man Games in the United States in 2008.[1] Pandemic is based on the premise that four diseases have broken out in the world, each threatening to wipe out a region. The game accommodates two to four players, each playing one of seven possible roles: dispatcher, medic, scientist, researcher, operations expert, contingency planner, or quarantine specialist. Through the combined effort of all the players, the goal is to discover all four cures before any of several game-losing conditions are reached.

Developers will be able to initiate a game of a supported type (For Sale or Pandemic) using a API call.

Bot Library provides helper methods. For example Pandemic library provides shortest path analysis for optimal routes between any two Pandemic cities, using Dijkstra's algorithm.

MQTT messaging is used as a way for interested code to monitor game progress in realtime.

GamesRoom project provides a ( Angular 8 SPA ) application enabling games to be run, and game state to be rendered. GamesRoom calls the API to play games.
