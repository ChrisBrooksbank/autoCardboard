# autoCardboard
models board games played by bots.

Initial implention of two games : 
* For Sale ( https://www.ultraboardgames.com/for-sale/game-rules.php ) 
* and Pandemic (https://www.ultraboardgames.com/pandemic/game-rules.php). 
For Sale is competitive, Pandemic is co-operative.

Developers will be able to initiate a game of a supported type (For Sale or Pandemic) likely using a API call.

They can decide to leave the game to make all moves itself (standard player will use very basic decision making).
Or provide a endpoint for a player, that when asked, and given gamestate and turn type, will decide on its move.
This allows developers to create their own decision making to take player turns.

Developer can also provide a endpoint which will get called, after each move, which could be used to show the gamestate, last moves made, in some way.
And/or to provide logging of games.

Will provide sample endpoints to show progess and/or log for For-Sale and Pandemic. ( Possibly using latest Angular )

## Current Solution Structure

### autoCardboard.Common.Domain.csproj
Contains types common to all games such as Game, Player, Card and CardDeck.

### autoCardboard.Common.Test.csproj
Unit tests for above

### autoCardboard.ForSale.Domain.csproj
Implements ForSale game. ( https://www.ultraboardgames.com/for-sale/game-rules.php )
Including a simple IPlayer implementation

### autoCardboard.ForSale.Test.csproj
Unit tests for above

### autoCardboard.Pandemic.Domain.csproj
Implements Pandemic game. (https://www.ultraboardgames.com/pandemic/game-rules.php )
Including a simple IPlayer implementation

### autoCardboard.Pandemic.Test.csproj
Unit tests for above

### autoCardboard.ConsoleApp.csproj
Console application to run games.