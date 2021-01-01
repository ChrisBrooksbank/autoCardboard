# autoCardboard
models board games played by bots.

## autoCardboard.Common.Domain.csproj
Contains types common to all games such as Game, Player, Card and CardDeck.

## autoCardboard.ForSale.Domain.csproj
Implements ForSale game. ( https://www.ultraboardgames.com/for-sale/game-rules.php )
Including a simple IMPlayer implementation

The intention is for players to be defined as endpoints which, given a ForSaleGameTurn, will make a legal turn.


## autoCardboard.ConsoleApp.csproj
Console application to run games.
