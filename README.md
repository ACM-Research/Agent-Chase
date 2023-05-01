# Agent Chase

Welcome to Agent Chase! This is a two player game meant to be played by two computer agents. One agent is the "runner" and the other is the "goalie". 
A game where one player tries to stop another from reaching the goals. This is meant to be a project demonstrating a genetic algorithm that succeeds for both players.

![Agent_Chase_Poster (18)-compressed-1](https://user-images.githubusercontent.com/43019257/235407717-a676a935-fc65-4ef1-a492-645d132f67c6.png)


## Environment
The environment is a 3D area enclosed by a perimeter. Walls that generate will be insurmountable, but can be walked around. 

### Runner
The runner's objective is to touch the goal while avoiding the goalie in the shortest timeframe. The goal will spawn in, and the runner will attempt to reach it by moving in one of 8 directions (up, down, left, right, and in between those four). When a goal is touched, the runner receives some points (depending on how quickly the goal was reached) and a new goal is spawned in. 

### Goalie
The goalie's objective is to stop the runner from reaching the goal. This is done by "tagging" the runner, which is whenever the hitboxes of the goalie and runner collide. To prevent "goal-camping", where the goalie just sits by the goal and moves very little, the goalie will be forced to stay some distance away from the goal, creating an exclusion zone for the goalie.

### Goals
Goals spawn in randomly throughout the environment. 

### Obstacles
When a goal spawns in, walls appear throughout the map that obstruct the path to the goal. It's up to the runner and the goalie to use these walls to their advantage.

## Restrictions
The goalie will always have a shorter path to the goal than the runner when the runner and goalie are outside the exclusion zone for the goalie. To make up for this, the goalie does not have a perfect read on the runner: it will have its inputs on movement delayed by some time (subject to change, but maybe 250-500 ms might be good.) 


