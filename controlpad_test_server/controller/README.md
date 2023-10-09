# Texas Hold'em Game Readme

This readme provides an overview of the different components and utility functions that make up the Texas Hold'em game controller. The game is divided into multiple JavaScript files, each responsible for specific functionalities. Below, you will find individual sections for each JavaScript file, explaining their roles and functions.

## `app.js`

### Description

`app.js` serves as the main entry point for the Texas Hold'em game. It initializes the game, sets up event listeners, and manages the game's core logic, including rounds, player actions, and communication with other players.

### Functions and Features

- **`init()`**: Initializes the game, sets up event listeners, and prepares the game for the first round.
- **`startRound()`**: Starts a new game round, dealing cards, updating the UI, and managing player actions.
- **`handleMessage(message)`**: Handles incoming messages from other players and updates the game state accordingly.
- **`send(message)`**: Sends a message to other players in the game.
- **Player Actions**: Manages player actions like betting, folding, and checking.

## `mechanics.js`

### Description

`mechanics.js` contains various game mechanics related to player actions, card handling, and UI updates. It manages the internal workings of the game, ensuring that actions are executed correctly.

### Functions and Features

- **`chipStack()`**: Provides chips for the UI element as betting increases.
- **`addCard(cardArray)`**: Instantiates card elements based on the given array.
- **`UpdateMoney(amount)`**: Runs every time a user increments their bet to ensure actions and amounts line up.
- **`startGame(event)`**: Starts the game, taking input for starting money, game type, and minimum bet.
- **`playingRound(value)`**: Used when a user makes a decision on anteing.
- **`addFunds()`**: Used when a user requests funds.

## `messaging.js`

### Description

`messaging.js` handles the messaging system within the game, both for sending messages to other players and processing incoming messages. It plays a crucial role in coordinating game actions and communication.

### Functions and Features

- **`handleMessage(message)`**: Processes incoming messages and triggers actions based on their content.
- **`sendResponse()`**: Sends a player response like Call, Fold, Raise.
- **`moneyChoice(msg)`**: Sends a yes or no response to a player's request for money.

## `settings.js`

### Description

`settings.js` is responsible for managing user settings and preferences within the game. It provides functions to toggle options, update settings, and customize the player experience.

### Functions and Features

- **Settings Toggle Functions**: Functions to toggle sound and auto-sitout settings.
- **`changeName()`**: Allows the player to change their username.
- **`updateAutoSitOut()`**: Updates the UI to reflect the auto-sitout setting.
- **`updateSound()`**: Updates the UI to reflect the sound setting.
- **`sendSetting(setting, variable)`**: Sends a setting change message to other players.
- **`updateSettings(sections)`**: Updates settings based on incoming messages.

## `stateRequest.js`

### Description

`stateRequest.js` is responsible for setting a user's state with the given message and updating variables upon a state request. It helps maintain the game's state and ensures that players are synchronized.

### Functions and Features

- **`setState(sections)`**: Sets a user's state based on incoming messages and triggers a screen redraw.
- **`drawScreen(sections)`**: Manages the screen's display based on the current game state.
- **`stateRequest()`**: Requests an updated game state.

## `ui.js`

### Description

`ui.js` handles the game's user interface and screen drawing functions. It defines various screens and elements used in the game, making it visually interactive for players.

### Functions and Features

- Screen Drawing Functions: Functions for drawing different game screens like loading, waiting, playing, and more.
- Top Menu Functions: Functions to display the top menu.
- Action Handling Functions: Functions for handling player actions like folding, calling, and raising.
- Timer Functions: Functions for managing timers and animations.

## `utils.js`

### Description

`utils.js` contains utility functions used throughout the game. These functions perform various tasks, such as generating random player names, updating available colors, handling screen orientation changes, and more.

### Utility Functions

- **`generateName()`**: Generates random player names.
- **`updateAvailableColors(sections)`**: Updates available colors in the UI.
- **`onFlip(width, height)`**: Handles screen orientation changes.
- **`outgoingMessages()`**: Retrieves and clears outgoing messages.
- **`getDrawables()`**: Returns a list of drawables for the game screen.
- **`foldTimer()`**: Manages the fold timer.
- **`sizeFont(text, area)`**: Provides an auto-sized font for text.
- **`convertToBool(value)`**: Converts a string to a boolean.
- **`formatter`**: Defines a number formatter for currency formatting.

The combination of these JavaScript files and utility functions forms the core of the Texas Hold'em game, providing a comprehensive and engaging gaming experience for players.