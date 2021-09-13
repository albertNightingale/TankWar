# TankWar

## Credit: 
This project is designed by the curriculum of CS3500 from School of Computing, University of Utah. 
This project is implemented by Albert Liu. 

## Project Description
A multi-player networked tankwar game built in C#. 

The server is started in separately from the client. 
The client will start a GUI form where the user can enter the name of the player and click start to join the game. 
The player will take the form of a tank who will be fighting other tanks available on the map, which is other players playing. 
The player can shoot projectiles that will deal some damage to the opponent and despawn upon collision (with player and the edge of the map). 
The player can laser beams that can penetrate all objects in game and one shot all players it passed. 
The player can only shoot laser beam when he collects food that spawn on the ground overtime. 
A player can at most collect three food. 
Food on the ground will despawn after a certain time. 

## Performance
The code is inoptimized. 

## Reference: 
Utilizes the Networking Library built in the past: 
https://github.com/albertNightingale/NetworkingLibrary
