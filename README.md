# Agent Chase

# WIP
This README and project is a work in progress. We are still editing and polishing large parts of our project, so stay tuned!


Welcome to Agent Chase! This is a two player game meant to be played by two computer agents. One agent is the "attacker" and the other is the "defender". 
The defender aims to tag the attacker before it can reach the goal. This is meant to be a project demonstrating reinforcement learning with multiple, dissimilar agents.

![Agent_Chase_Poster (18)-compressed-1](https://user-images.githubusercontent.com/43019257/235407717-a676a935-fc65-4ef1-a492-645d132f67c6.png)


### Attacker
The attacker's objective is to touch the goal while avoiding the goalie in the shortest timeframe. The goal will spawn randomly in the map, and the attacker will attempt to reach it by moving in the 2D plane. When a goal is touched, the episode ends and the attacker wins.

### Defender
The defender's objective is to stop the attacker from reaching the goal. This is done by "tagging" the attacker, which is whenever the hitboxes of the defender and attacker collide. To prevent "goal-camping", where the defender just sits by the goal and moves very little, the defender is incentivized to actively chase the attacker, attempting to end the episode as quickly as possible.

Here is some additional information about the project that is not in the poster:

## Environment

### Construction
The environment is a 3D area enclosed by a perimeter. This environment has walls, which are impenetrable but can be walked around. The environment was designed in Unity with the help of the ProBuilder package.

### Obstacles
The map has walls that break lines-of-vision and physically obstruct progress. It's up to the attacker and the defender to use these walls to their advantage.

### Goal
The goal object spawns in at the start of the episode. The attacker must reach the goal as quickly as possible, while the defender must prevent the attacker from reaching the goal.

## Training the Agent

### PPO
Both agents used slightly different rewards systems with the algorithm of Proximal Policy Optimization, or PPO. The learning rate starts off at 0.0003, but decays linearly over time as the maximum amount of steps (around 5 million) is reached, eventually becoming 0. The policy model would update every 10,240 steps, with a batch size of 64 steps. The neural network used was 2 layers deep, with 9 observations: the position of the attacker, defender, and goal, each of which are observations in 3D space.

### Saving the Model
Models for the attacker and the defender were saved every 500,000 steps so that the team could observe the development of each model. This gave us 11 models for each training session, as the model progressed from 0 to 5,000,000 steps. The last 12,000 steps would be accounted for in the model.

### Training Info
The environment was replicated 16 times to speed up the process of training. Training proceeded for 5,000,000 steps, which would be around 5,000 episodes and 5 hours long. The attacker and defender had distinct policies and rewards that were optimized for their situations. Here are a list of rewards for the attacker:

- Walking towards goal: 0.01
-Walking away from goal: -0.003
- Winning (touch goal): 1
- Losing (caught by defender): -1
- Hit wall: -0.0005
- Frame passes: -0.01

Here are a list of rewards for the defender:
- Walking towards attacker: 0.005
- Walking away from defender: -0.003
- Winning (catch attacker): 1
- Losing (attacker reaches goal): -1
- Hit wall: -0.0001
- Frame passes: -0.005

The results of the training are in the poster.

## Challenges
Originally, objects would occasionally respawn into other obstacles. This is a problem, as these should be invalid locations. Resolving this was as easy as respawning the object until no collisions were detected. 

Additionally, the ML-Agents repository's installation guide was hilariously outdated, with many, many bugs, incompatible Python packages, and broken environments. Resolving this took about a month, but we wrote our own version of the installation guide for Unity ML-Agents that should work for version 2.3.0+.


## Team Members
Michael Figueroa
Vaishnavi Josyula
Nivedha Sreenivasan
Ahmed Siddiqui
Naveen Mukkatt (Research Lead)
