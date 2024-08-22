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

## Developer Guide
### UI
#### Title Scene
The TitleScene is the initial scene loaded by the game. It has three buttons *New Game*, *Load Game*, and *Exit Game*.

- The New Game button brings the player to the New Game Scene.
- The Load Game button brings the player to the Between Scene to resume the game.
- The Exit Game exits the game.

![Alt text](/Screenshots/main-menu.png?raw=true "Main Menu")


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

![Notification Window](/Screenshots/notification-window.png)

##### Data Center Window
The data center window displays a map of the data centers in the game as buttons, each labeled with the player controlling it or blank for an unpurchased data center. 
the button hides the data center map and opens a customization subwindow.

![Data Center Selection Window](/Screenshots/data-center-selection-window.png)

The customization window has a list of buttons which increase a respective attribute of the selected data center, a slider which represents the strength of the firewall at this data center, a box filled with traffic entries that each have a delete button, a resources section with an increase and decrease button to control the allocated resources and a back button which takes the player back to the data center map. Any of the attributes specific to this data center can not be visible to the player if not unlocked.

![Data Center Customization Window](/Screenshots/data-center-customization-window.png)

##### Research Window
The Research Window has a graph of goal buttons that can be selected by the player. depending on the status of the goal, each button will be displayed as a different color. There is also a resources section with an increase and decrease button to control the allocated resources.

![Research Window](/Screenshots/goal-window.png)

##### Malware Window
The Malware Window has a list of malware controlled by the turn player that they can select from. When one is selected, it brings the player to a customization window where they can customize that malware.

![Malware Selection Window](/Screenshots/malware-selection-window.png)

The customization window has a list of radio buttons representing the type of malware, a list of sliders representing the attributes of the malware and a list of checkboxes representing a feature for the malware. There is also a resources section with an increase and decrease button to control the allocated resources.

This window contains a screen lock which is located behind the resource window and back button. This is only active when work has started on a piece of malware to prevent the user from interacting with customization.

![Malware Customization Window](/Screenshots/malware-customization-window.png)

##### Attack Window
The Attack Window has a list of attacks controlled by the turn player that they can select from. When one is selected, it brings the player to a customization window where they can customize that attack.

![Attack Selection Window](/Screenshots/attack-selection-window.png)

The customization window has a list of radio buttons representing the target data center, selected malware, delivery method and objective of the attack. There is also a resources section with an increase and decrease button to control the allocated resources.

This window contains a screen lock which is located behind the resource window and back button. This is only active when work has started on an attack to prevent the user from interacting with customization.

![Attack Customization Window](/Screenshots/attack-customization-window.png)

##### Pause Window
The pause window has a list of buttons for ending th eplayers turn which leads to the Between Scene, A button returning to the Title Scene, and a button to exit the game.

![Pause Window](/Screenshots/pause-menu.png)

#### End Game Scene
The end game scene is the scene visible once the game is over. It displays the winner of the game and has a button leading back to the Title Scene and another to exit the game.

---

### Malware
A **Malware** Object represents the information associated with one piece of malware. The Malware class implements the interface, Workable which allows work to be done on the object.

The Malware Objects are managed by the **MalwareManager** which also handles user interaction in the malware window of the game. Any interaction between a Malware object and the player needs to come through this object.

---

### Attacks
An **Attack** Object represents the information associated with one attack. The Attack class implements the interface, Workable which allows work to be done on the object.

The Attack Objects are managed by the **AttackManager** which also handles user interaction in the attack window of the game. Any interaction between an Attack object and the player needs to come through this object.

---

### DataCenters
A **DataCenter** Object represents the information associated with one data center. The DataCenter class implements the interface, Workable which allows work to be done on the object.

The DataCenter Objects are managed by the **DataCenterManager** which also handles user interaction in the data center window of the game. Any interaction between a DataCenter object and the player needs to come through this object.

---

### Notifications
A **Notification** Object represents the information associated with one notification. The Notification class implements the interface, Workable which allows work to be done on the object.

The Notification Objects are managed by the **NotificationManager** which also handles user interaction in the notification window of the game. Any interaction between a Notification object and the player needs to come through this object.

A subclass of Notification exists named Email which holds additional information including the associated data center id, and an optional field of an attack id if malicious.

---

### Utility
#### Buttons
There are two scripts associated with buttons, those being **Checkbox** and **Radio Button**.

Checkbox is applied to a container with a series of buttons as children. It makes it so that when one of the buttons is pressed, it changes the image of the button and any graphic objects it may have as a child object. When the button is pressed again, the color is returned to normal.

Radio Button is also applied to a container with a series of buttons as children. The difference between checkbox and radio button is that for radio button, the player can only have one button selected. When the player presses one button, it turns all buttons in the group back to the unselected state and then changes the selected button to the selected state.

#### Popup Windows
The **MouseOverTool** is used to handle events that have the player moving their mouse over an object. When the mouse is over the object, the MouseOverTool instantiates a new game object of a prefab at a specified location and when the mouse leaves the object, the instantiated object is destroyed. 

If the new game object is placed over the object which has the mouse event, the new object will be destroyed as the mouse is no longer inside the object. The new object will then continuously be destroyed and created over and over again until the mouse leaves the area of the mouse event object. This is to be avoided.

#### Content Generation
The text associated with emails and traffic is generated through the **ContentGenerator**. This can be used to create the information held in a valid email with no negative side effects, a phishing email with negative side effects and network traffic. The text generated is random and not always the same.

```cs
ContentGenerator content = new ContentGenerator();

// Generating non malicious email text
string[] email = content.GenerateEmail("google");
Debug.Log(email[0] + " : " + email[1]);
// Example Output: william.johnson@account.google.com : The backup for application server completed on 2024-02-05. No errors were reported. A detailed backup log is attached.

// Generating malicious phishing email text
string[] phish = content.GeneratePhish("google", 50);
Debug.Log(phish[0] + " :" + phish[1]);
// Example Output: susan.jones@support.g0ogle.com : The uptime report for the VM cluster for this month is attached. The system achieved 49% uptime.

// Generating non malicious traffic text
Debug.Log(content.GenerateTraffic(false));
// Example Output: Source IP: 67.51.145.42 - Destination IP: 103.118.5.119 - Protocol: VCDT - Source Port: 7660 - Destination Port: 5132 - User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) - Packet Length: 1879 bytes - Flags: SYN CWR - Timestamp: 2024-02-22 04:35:34

// Generating malicious traffic text
Debug.Log(content.GenerateTraffic(true));
// Example Output: Source IP: 72.124.32.253 - Destination IP: 66.178.248.233 - Protocol: QTED - Source Port: 3453 - Destination Port: 1771 - User-Agent: Python-urllib/2.7 - Packet Length: 63941 bytes - Flags: SYN FIN - Timestamp: 2024-03-20 11:33:46
```

---
### Research
In the game, most features are blocked from access using the **Unlockable** component which only shows the object to the player if they have unlocked that feature. 

Research is managed in the GoalManager where goals can be completed and looked at.

---

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
### Mouse Over Tool Bug 
Date: 2024-07-02

The mouse over tool is placing the info slide on top of the button which is making the info slide destroy and create itself repeatedly.

Solution: Changed it so the info slide is placed in a specific position instead of following the mouse.

---

### Line Renderer Bug
Date: 2024-07-12

The Line renderer componenet displays a line outside of the canvas which is where all visual elements of the game are. since the canvas ui elements are displayed on top of the line renderer, none of the lines would be able to be displayed.

Solution: Created a series of images in the canvas and created the edges to the goal graph individually to get a rough picture and display neccessary information. In the future, I am going to create an image as the background for that window and make the buttons invisible on top of the buttons on the image.

---

### Checkbox Bug
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

---

### Phish Generation Error
Date: 2024-08-09

Content generator would run indefinetly when generating a phish email, crashing the game. 

The problem was caused by the adjustment of the base email. The structure of the email has a series of numbers which correspond to a tag that needs to be filled. When the system interacted with a date tag, it would change the string to add the date which includes numbers so the system would then treat the date as a series of tags and try to replace them creating an infinite loop.

Solution: This was fixed by moving the iterator to the end of the adjustment in the newly produced string.