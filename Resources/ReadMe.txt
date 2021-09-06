Code written by Albert liu and Alex Hudson. Last updated 12/5/2019
Skeleton code provided by Professor Kopta.

Server Settings file is under server resources.
Specific Detail:
	powerups - You may only have one powerup at a time. if you pick up another power up you can still only shoot a beam once.
	-This is implemented so a player doesnt collect as many powerups as possible (sneaky) then go on a rampage.

The general idea of how this project works is as follows:
	A) We set up our data base/add to it before our server starts
	B) We read in the server settings and create our world.
	C) When a client connects, we assign them an Id and use this for the tank and socket state.
	D) When we receive data we added it to a list of all the things we want to be updated called
		data_awaiting_updates.
	E) In a different thread from receiving the data we do our calculation after a set number of 
		Milliseconds called update.
	F) When update is called it checks each object for collisions with another object.
	G) After update does all the game mechanics and collision detection it sends a string of all the data that was updated to all the clients.
 	H) This process loops until the server closes.
	I) When a client disconnects, we add the player data to the data base.
	J) When input is sent to the Server console it will close and create a game for the data base.

The most difficult problem was collision detection and async bugs / hisenbugs.
With Collisions, we tried to implement many ways to solve this problem and it took a good chunk of our time before finally figuring out how to easily do it. 
	Also, we struggled with when to update collisions and tank location.
Heisenbugs are difficult for obvious reasons. They are hard to replicate and happen occasionally. We managed these with locks but its still hard to tell the reliability. 



Current known problems:

	1) Bugs 2 and 3 are likely related and likely solved. We had a bug where we were not properly locking the state. We have been unable to replicate since but its hard to tell explicitly if its solved.
	2)	The client will occasionally not load in the world fully and will not allow for input or center the tank. This is likely due to a bad handshake but is extremely difficult to replicate.
	3)	The server will sometimes say running but when a client connects the client remains black. Also likely due to a bad handshake

