# Controller Boilerplate

app.js is provided as an example and should be replaced by the game developer

The game developer's app.js should implement the following functions

### onFlip(width, height)
* called when phone is turned (or when PC browser is resized)

### handleMessage(message)
* called when a message is recveived from the game

### outgoingMessages() -> string[]
* called periodically, returned messages are esent to the game

### drawables() -> drawable
* called periodically, returned drawables are drawn to the browser view
* where drawable has {type={'image','circle','rect','line'}, (x,y), [(scalex, scaley)], (radius), (rotation), (color)}

### handleTouchStart(id, x, y)
* called when a finger first hits the screen
* warning: seems like there's abug where this isn't called for the second touch of a double-touch

### handleTouchMove(id, x, y)
* called when a finger that is already touching the screen is dragged across the screen

### handleTouchEnd(id, x, y)
* called when a finger that was touching the screen is lifted away from the screen

### handleTouchCancel(id, x, y)
* called when a touch event stream ends for some reason other than the finger being lifted
* don't really know the specifics on this one

### controlpadStart()
* called once at page load

### controlpadUpdate()
* called 30 times per second
