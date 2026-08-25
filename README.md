# Strikers

A unique first-person shooter that merges competitive soccer with fast-paced FPS action.
First person shooter combined with soccer where you have to shoot the ball into the opponent team's goal.

## Movement

The game features a custom-built, physics-driven movement system enabling dynamic maneuvers such as wall-running and wall-jumping, made to be fun and chaotic. The movement uses a satisfying acceleration and momentum keeping system that feels very responsive when changing direction very fast, using the following physics equation: 

$finalForce = (playerInputVector - normalizedVelocityVector) * acceleration$

![alt text](README/Untitled.gif)

Since the player object is a Rigidbody3D, it can be affected by collisions or forces applied to him such as explosions, which allow you to build up speed over the maximum speed, and not losing it if going the same direction.

## Weapons

There are 3 weapons:

- The pistol (the default weapon)
- The sniper
- The grenade launcher

The game is designed to be fast paced and chaotic, with a focus on accuracy, positioning and using the environment to move fast.

## Multiplayer

The game features multiplayer, with up to 6 players in a 3v3 match, designed to be played with friends.

Using Unity's Network for Game Objects, it uses a peer to peer system where one player hosts the game and the others connect to them.

The game features a client authoritative architecture, meaning that the players are processing their movement on their machines and sending their positions to the host. The ball is simulated on the host, alongside with weapon dispensers and goal detection. This ensures player's inputs are processed instantly with the tradeoff of latency. To combat that, the game features a physics objects' position extrapolation system using the delay time from the client to the server and the current velocity of the object. This is done in the [AdvancedNetworkRigidbody.cs](Assets/Scripts/AdvancedNetworkRigidbody.cs) script.

## 3D Models and Animations

The 3d models were made in Blender. Guns use keyframe animations made in Unity.

![alt text](<README/Screen Recording 2025-05-09 122945.gif>)

The player characters are procedurally animated using their own velocity inside [ProceduralCharacterAnimator.cs](Assets/Scripts/ProceduralCharacterAnimator.cs).

![alt text](<README/Screen Recording 2025-05-09 123243.gif>)

## Screenshots

![who will shot first?](<README/Screenshot 2025-05-09 132222.png>)

![oooooo iiiii mmmmmmissssed](<README/Screenshot 2025-05-09 132539.png>)

![whooooooo speeeeeeed](<README/Screenshot 2025-05-09 132257.png>)

![me and the bois](<README/Screenshot 2025-05-09 132039.png>)

It's hosted on [itch.io](https://darealzoinks.itch.io/strikers) and is free to play.
