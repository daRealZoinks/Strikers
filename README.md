# Strikers

First person shooter combined with soccer where you have to shoot the ball into the opponent team's goal.

## Movement

The movement is physics based to make it more fun and chaotic. You can run, jump, wallrun, and walljump (check the `Move()` function inside [CharacterMovementController](Assets/Scripts/CharacterMovementController.cs)). It uses a vector equation that I was very proud of myself for making.

$finalForce = (playerInputVector -normalizedVelocityVector) * acceleration$

![alt text](README/Untitled.gif)

The result is that, unaffected, you will get to the maximum speed by yourself, but if you gain any speed over that, if you keep walking in the same direction, you will not slow down.

Since the player is a rigidbody, he can be affected by collisions or forces applied to him such as explosions. This will allow you to gain ludicrous speed.

## Weapons

There are 3 weapons:

- The pistol (the default weapon)
- The sniper
- The grenade launcher

The game is designed to be fast paced and chaotic, with a focus on skill based gameplay.

## Multiplayer

The game is multiplayer, with up to 6 players in a match. It's designed to be played with friends. Using Unity Network for Game Objects it uses a peer to peer system where one player hosts the game and the others connect to them. It's client authoritative, meaning that the players are sending their positions to the host. The ball is simulated on the host. To combat the latency I'm extrapolating the phyiscs objects' position using the delay time from the client to the server and the current velocity of the object. This is done in the [AdvancedNetworkRigidbody.cs](Assets/Scripts/AdvancedNetworkRigidbody.cs) script.

## Music

Music brought from a royalty free music site. I forgot which one.

## Sound Effects

Sound effects are either royalty free or made by me (Right after I typed this sentence, Copilot said "The gun sounds are made by me, and the ball sounds are from a royalty free sound effects site.". I don't own any guns 💀💀💀).

## 3D Models and Animations

3d models are made by me in Blender. Guns have been keyframe animated in Unity (I didn't have time to learn how to animate in Blender).

![alt text](<README/Screen Recording 2025-05-09 122945.gif>)

The character is procedurally animated based on it's own velocity inside [ProceduralCharacterAnimator.cs](Assets/Scripts/ProceduralCharacterAnimator.cs) (for some reason the head is not animating looking up and down).

![alt text](<README/Screen Recording 2025-05-09 123243.gif>)

## Screenshots

![who will shot first?](<README/Screenshot 2025-05-09 132222.png>)

![oooooo iiiii mmmmmmissssed](<README/Screenshot 2025-05-09 132539.png>)

![whooooooo speeeeeeed](<README/Screenshot 2025-05-09 132257.png>)

![me and the bois](<README/Screenshot 2025-05-09 132039.png>)

If this doesn't convince you to play my game, I don't know what will.

It's hosted on [itch.io](https://darealzoinks.itch.io/strikers) and is free to play right now. Whachu waiting for?