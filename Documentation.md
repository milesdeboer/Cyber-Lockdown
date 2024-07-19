# Cyber Lockdown Documentation
###### Miles DeBoer

## Table of Contents
1. [Introduction](#Introduction)
2. [User Guide](#User-Guide)
3. [Developer Guide](#Developer-Guide) \
    3.1. [Malware](#Malware) \
    3.2. [Attacks](#Attacks) \
    3.3. [DataCenters](#DataCenters) \
    3.4. [Notifications](#Notifications)
4. [Data Storage](#Data-Storage)
4. [Troubleshooting](#Troubleshooting)


## Introduction
Welcome to Cyber Lockdown, an immersive and interactive game developed using Unity 2022.3.21f1. In this multiplayer game, players can create and defend against cyber threats.

## User Guide

## Developer Guide
### UI
#### Title Scene
The TitleScene is the initial scene loaded by the game. It has three buttons *New Game*, *Load Game*, and *Exit Game*.

The New Game button brings the player to the New Game Scene. \
The Load Game button brings the player to the Between Scene to resume the game. \
The Exit Game exits the game.

#### New Game Scene
The New Game Scene is for customizing game parameters.

Currently, this scene only has a list of player entries where they can enter their name and add or remove other players.
There is also a button to start the game which leads the player to the Between Scene.

#### Between Scene
The BetweenScene is the scene that takes place between player turns. It says the turn number, the turn player and a button to start the player's turn.

#### Player Scene
At the bottom of this scene is a taskbar meant to resemble that of a computer OS. This lists the turn player, their money/resources and a list of buttons which open a different window.
The Player scene is split into several windows:

##### Notification Window
The notification window displays a list of notifications and emails sent to the turn player. each notification has a button to remove it from the inbox and emails have two buttons, one to accept the email and another to decline. There is also a button to clear all notifications.

##### Data Center Window
The data center window displays a map of the data centers in the game as buttons, each labeled with the player controlling it or blank for an unpurchased data center. 
the button hides the data center map and opens a customization subwindow.

The customization window has a list of buttons which increase a respective attribute of the selected data center, a slider which represents the strength of the firewall at this data center, a box filled with traffic entries that each have a delete button, a resources section with an increase and decrease button to control the allocated resources and a back button which takes the player back to the data center map. Any of the attributes specific to this data center can not be visible to the player if not unlocked.

##### Goal Window
The Goal Window has a graph of goal buttons that can be selected by the player. depending on the status of the goal, each button will be displayed as a different color. There is also a resources section with an increase and decrease button to control the allocated resources.

##### Malware Window
The Malware Window has a list of malware controlled by the turn player that they can select from. When one is selected, it brings the player to a customization window where they can customize that malware.

The customization window has a list of radio buttons representing the type of malware, a list of sliders representing the attributes of the malware and a list of checkboxes representing a feature for the malware. There is also a resources section with an increase and decrease button to control the allocated resources.

##### Attack Window
The Attack Window has a list of attacks controlled by the turn player that they can select from. When one is selected, it brings the player to a customization window where they can customize that attack.

The customization window has a list of radio buttons representing the target data center, selected malware, delivery method and objective of the attack. There is also a resources section with an increase and decrease button to control the allocated resources.
##### Pause Window
The pause window has a list of buttons for ending th eplayers turn which leads to the Between Scene, A button returning to the Title Scene, and a button to exit the game.

#### End Game Scene
The end game scene is the scene visible once the game is over. It displays the winner of the game and has a button leading back to the Title Scene and another to exit the game.

### Malware
A **Malware** Object represents the information associated with one piece of malware. The Malware class implements the interface, Workable which allows work to be done on the object.

The Malware Objects are managed by the **MalwareManager** which also handles user interaction in the malware window of the game. Any interaction between a Malware object and the player needs to come through this object.

### Attacks
An **Attack** Object represents the information associated with one attack. The Attack class implements the interface, Workable which allows work to be done on the object.

The Attack Objects are managed by the **AttackManager** which also handles user interaction in the attack window of the game. Any interaction between an Attack object and the player needs to come through this object.

### DataCenters
A **DataCenter** Object represents the information associated with one data center. The DataCenter class implements the interface, Workable which allows work to be done on the object.

The DataCenter Objects are managed by the **DataCenterManager** which also handles user interaction in the data center window of the game. Any interaction between a DataCenter object and the player needs to come through this object.

### Notifications
A **Notification** Object represents the information associated with one notification. The Malware class implements the interface, Workable which allows work to be done on the object.

The Notification Objects are managed by the **NotificationManager** which also handles user interaction in the notification window of the game. Any interaction between a Notification object and the player needs to come through this object.

## Data Storage
### Wrappers
**Wrappers** are simplified versions of existing objects involved in the saving process. The purpose is to allow saving through **JsonUtility** without giving unneccessary access to these objects. Wrappers exist for the following objects:
- *Malware* → *MalwareWrapper*
- *Attack* → *AttackWrapper*
- *DataCenter* → *DataCenterWrapper*
- *Notification* → *NotificationWrapper*
- *Player* → *PlayerWrapper*

Each of the base object have a function called **Wrap** which returns the wrapper equivalent object. Similarly, a wrapper object holds a function called **Unwrap** which returns the base object type.
```cs
DataCenter dataCenter = new DataCenter(1001);

attack.SetDelivery("phishing");

try {
    // Wrap Object
    AttackWrapper wrapper = AttackWrapper(attack);
    //Unwrap Object
    Attack unwrapped = wrapper.Unwrap();

    Debug.Log(dataCenter.GetDelivery() + ", " + wrapper.delivery + ", " + unwrapped.GetDelivery());
} catch (WrapperException e) {
    Debug.Log(e.ToString());
}
//output: phishing, phishing, phishing

```
### DAO

**Data Access Objects (DAO)** is the name for a set of objects handling access between the software and the save data. The data is saved to *Application.persistentDataPath* on the host's device. DAO objects exist for the following objects:
- *MalwareManager* → *MalwareDAO*
- *AttackManager* → *AttackDAO*
- *DataCenterManager* → *DataCenterDAO
- *NotificationManager* → *NotificationDAO*
- *PlayerManager* → *PlayerDAO*
- *GameManager* → *GameDAO*

All DAO object except *GameDAO* holds a list of their respective wrapper (e.g. MalwareDAO has List<MalwareManager>) as a public field as to satisfy the conditions of the **JsonUtility** saving method.

Each DAO object has a Save and Load function which takes a parameter of the appropiate type (e.g. *MalwareDAO.Save(MalwareManager manager)*) and returns ture if the data was successfully saved/loaded. These functions are only called by the manager themselves for simplicity.

Saving
```cs
MalwareDAO dao = new MalwareDAO();
bool success = dao.Save(this);
```
Loading

```cs
MalwareDAO dao = new MalwareDAO();
bool success = dao.Load(this);
```

## Bug Report
### Line Renderer Bug - Resolved
Date: 2024-07-12

The Line renderer componenet displays a line outside of the canvas which is where all visual elements of the game are. since the canvas ui elements are displayed on top of the line renderer, none of the lines would be able to be displayed.

Solution: Created a series of images in the canvas and created the edges to the goal graph individually to get a rough picture and display neccessary information. In the future, I am going to create an image as the background for that window and make the buttons invisible on top of the buttons on the image.

---

### Checkbox Bug - Resolved
Date: 2024-07-16

The checkboxes were holding information from window to window so when the player opened a malware object, it displayed the incorrect information associated with that object. This was happening specifically when opening new malware and had no effect on opening those already at least partially created.

Solution: updated the function that runs when the player selects an object to edit so that it goes through each button to make them deselected and then loads information if there is an object with that id.

---

### Conflict Record Bug
Date: 2024-07-18

The conflict manager added the malware involved in the attack to the target data center's record before checking if it was already there. Since if the malware is in the data center's record, it halved the attack score, the attack score will always be halved making most attacks fail.

Solution: I moved the line that adds the attack to the target data center's record.

---

### Notification Bug
Date: 2024-07-18

Notifications were not being sent to both players when needed to.

Solution: When resolving conflict record bug, I removed a line which notifies the notification manager to send a notification to both players instead of just one.

---

### Capturing Error
Date: 2024-07-19

When adding onClick listeners with a parameter to a list of objects in a loop, it would set the parameter to id of the last element in the list instead of their own respective id.

Solution: Create a local variable before delegation of listener so that it is not changed on next iteration.