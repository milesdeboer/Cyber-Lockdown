# Cyber-Lockdown

![Unity](https://img.shields.io/badge/unity-%23000000.svg?style=for-the-badge&logo=unity&logoColor=white)
![Visual Studio Code](https://img.shields.io/badge/Visual%20Studio%20Code-0078d7.svg?style=for-the-badge&logo=visual-studio-code&logoColor=white)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)

[![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/milesdeboer/Cyber-Lockdown/blob/master/LICENSE)

Welcome to Cyber Lockdown, an immersive and interactive game developed using Unity 2022.3.21f1. In this multiplayer game, players can create and defend against cyber threats.

## Table of Contents
1. [Installation](#installation)
2. [Usage](#usage)
3. [User Guide](#User-Guide)
4. [Features](#features)
5. [Additional Documentation](#additional-documentation)

## Installation

To get a local copy up and running, follow these steps:
### Prerequisites
- Ensure you have **Git** installed. You can download it from [Git's official website](https://git-scm.com/).

Note: The following are only prerequisites for opening the project in unity and are not required for simply running the project.
- Ensure you have **Unity Hub** installed. You can download it from [Unity's official website](https://unity3d.com/get-unity/download).
- Install **Unity Editor version 2022.3.21f1** through Unity Hub. This specific version is required for compatibility with the project.

### Cloning the Repository
1. Clone the repository to your local machine usinf the following command:

    ```sh
    git clone https://github.com/milesdeboer/Cyber-Lockdown.git
    ```
2. Navigate to the project directory:

    ```sh
    cd "Cyber Lockdown"
    ```

### Opening the Project in Unity

1. Open **Unity Hub**
2. Click on the **Add** button and navigate to the directory where you cloned the repository.
3. Select the project folder and click **Open**.

### Running the Project

1. Once the project has been cloned, open the **Builds** folder to access the build files.
2. Double-click on the appropriate build file for your operating system:
    - For Windows: `Cyber LockDown.exe
3. The project should now run on your local machine.

## Usage
This game offers an immersive experience where players can develop and defend against cyber threats. Once in the game, instructions will be provided to guide the player through gameplay.

### Basic Usage

1. **Launching the Game**
    - Please refer to [how to run the project](#running-the-project) when trying to launch the game.
2. **Main Menu Options**
    - **New Game**: Start a new game from the beginning.
    - **Load Game**: Load a previously saved game.
    - **Exit Game**: Exit the game.

### Tips

- **Saving Progress**: The game auto saves at the end of every turn therefore if you exit the game mid turn, moves taken during your turn will be lost.

## User Guide
### Overview
In this multiplayer game, players can create and defend against cyber threats. Your goal is to complete your research into cyber security whiel preventing your opponeents from doing the same as the first player to complete their research wins.

### Starting a Game
Once the game has been installed and launched, the player will be prompted with the option to create a new game or to load an existing game. To start a new game, press the new game button. 

This will take you to a game customization window where you can customize the game settings. Once the settings are set to your preference, press start game.

### Player's Turn
During a player's turn, you have the option to create malware, launch attacks on other players, build up your defences, or research new cyber security techniques. The player's money and available resources are shown in the taskbar. To end the player's turn, press the pause window at the bottom right of the taskbar and press end turn.

In between turns, the player's data centers will produce money and your resources will be used to work on where they were allocated.

### Malware Building
In the malware window, the player can choose from one of their malware slots to customize and allocate resources to build malware. The player can unlock more features by continuing research in the goals tab.

### Attacking
Launching an attack on another player is simple. make your way to the attack window and press one of the player's attack slots. The player can then customize how the attack is performed and on which data center it is performed on. The player can unlock more attack features by continuing research in the goals tab.

### Defence
In the Data Center tab, the player can purchase new data centers or manage the data centers they already own. Here the player can increase the defences of the data center to decrease the likelihood of being infected by someone else's malware. You can also increase the rate at which money is earned and the amount of resources produced by this data center.

## Features
## Additional Documentation
For further details about the development information, please refer to the [Documentation](https://github.com/milesdeboer/Cyber-Lockdown/blob/master/Documentation.md)
