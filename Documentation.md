# Cyber Lockdown Documentation
###### Miles DeBoer
## Table of Contents - Code Structure
- DAO
    - Wrappers
        - AttackWrapper.cs
        - DataCenter.cs
        - MalwareWrapper.cs
        - NotificationWrapper.cs
        - PlayerWrapper.cs
    - AttackDAO.cs
    - DataCenterDAO.cs
    - GameDAO.cs
    - IDAO.cs
    - MalwareDAO.cs
    - NotificationDAO.cs
    - PlayerDAO.cs
- Objects
    - Attack.cs
    - DataCenter.cs
    - Email.cs
    - Goal.cs
    - IWorkable.cs
    - Malware.cs
    - Notification.cs
    - Player.cs
- Scene Managers
    - BetweenManager.cs
    - GameManager.cs
    - NewGameManager.cs
    - TitleManager.cs
- Util
    - MouseOverTool.cs
    - RadioButton.cs
- Window Managers
    - AttackManager.cs
    - DataCenterManager.cs
    - GoalManager.cs
    - ISavable.cs
    - MalwareController.cs
    - NotificationManager.cs
    - TaskbarController.cs
- ConflictManager.cs
- PlayerManager.cs

## DAOs
Data Access Objects (DAO) is the name for a set of objects handling access between the software and the save data. The data is saved to *Application.persistentDataPath* on the host's device

### Wrappers
Wrappers are simplified versions of existing objects involved in the saving process. The purpose is to allow saving through **JsonUtility** without giving up unneccessary access to these objecs.

#### AttackWrapper.cs
**AttackWrapper** is the wrapper object equivalent for the **Attack** object. It holds all neccessary raw information for the attack object as public variables as to satisfy the **JsonUtility** requirements. The Object requires the saved fields to exist and be valid or the wrapper will throw a **WrapperException**.

```cs
DataCenter dataCenter = new DataCenter(1001);

attack.SetObjective("money");
attack.SetDelivery("phishing");

try {
    AttackWrapper wrapper = AttackWrapper(attack);
    Attack unwrapped = wrapper.Unwrap();
    Debug.Log(wrapper.delivery + ", " + unwrapped.GetDelivery());
} catch (WrapperException e) {
    Debug.Log(e.ToString());
}
//output: phishing, phishing

```

The **AttackWrapper** can be converted back to an **Attack** object through the **Unwrap** function. If the fields of the Wrapper have been improperly set, the wrapper will throw a **WrapperException**.

---

#### DataCenterWrapper.cs
**DataCentertWrapper** is the wrapper object equivalent for the **DataCenter** object. It holds all neccessary raw information for the **DataCenter** object as public variables as to satisfy teh **JsonUtility** requirements. The object requires the saved fields to be unequal to null and be valid or the wrapper will throw a **WrapperException**.

```cs
DataCenter dataCenter = new DataCenter(0);

dataCenter.SetWorkTarget("ids");

try {
    DataCenterWrapper wrapper = DataCenterWrapper(dataCenter);
    DataCenter unwrapped = wrapper.Unwrap();
    Debug.Log(wrapper.target + ", " + unwrapped.GetWorkTarget());
} catch (WrapperException e) {
    Debug.Log(e.ToString());
}
// output: ids, ids
```

The **DataCenterWrapper** can be converted back to a **DataCenter** object through the **Unwrap** function. If teh fields of the Wrapper have been improperly set, the wrapper will throw a **WrapperException**.

---

#### MalwareWrapper.cs
**MalwareWrapper** is the wrapper object equivalent for the **Malware** object. It holds all neccessary raw information for the **Malware** object as public variables as to satisfy teh **JsonUtility** requirements. The object requires the saved fields to be unequal to null and be valid or the wrapper will throw. 

```cs
Malware malware = new Malware(101);

malware.SetMalwareType("virus");
malware.SetTime(DateTime.UtcNow);

try {
    MalwareWrapper wrapper = MalwareWrapper(malware);
    Malware unwrapped = wrapper.Unwrap();
    Debug.Log(wrapper.type + ", " + unwrapped.GetMalwareType());
} catch(WrapperException e) {
    Debug.Log(e.ToString());
}
//output: virus, virus
```

The **MalwareWrapper** can be converted back to a **Malware** object through the **Unwrap** function. If teh fields of the Wrapper have been improperly set, the wrapper will throw a **WrapperException**.

---

#### NotificationWrapper
**NotificationWrapper** is the wrapper object equivalent for the **Notification** object. It holds all neccessary raw information for the **Notification** object as public variables as to satisfy the **JsonUtility** requirements. The object requires the saved fields to be unequal to null and be valid or the wrapper will throw. 

```cs
Notification notification = new Notification(
    "You successfully did a thing :D", 
    "You did that one thing successfully against the one player", 
    0);

try {
    NotificationWrapper wrapper = NotificationWrapper(notification);
    Notification unwrapped = wrapper.Unwrap();
    Debug.Log(wrapper.title + ", " + unwrapped.GetTitle());
} catch(WrapperException e) {
    Debug.Log(e.ToString());
}
//output: You successfully did a thing :D, You successfully did a thing :D
```


The **NotificationWrapper** can be converted back to a **Notification** object through the **Unwrap** function. If teh fields of the Wrapper have been improperly set, the wrapper will throw a **WrapperException**.

---

#### PlayerWrapper.cs
**PlayerWrapper** is the wrapper object equivalent for the **Player** object. It holds all neccessary raw information for the **Player** object as public variables as to satisfy the **JsonUtility** requirements. The object requires the saved fields to be unequal to null and be valid or the wrapper will throw. 

```cs
Player player = new Player(0);

player.SetName("bob");

try {
    PlayerWrapper wrapper = PlayerWrapper(player);
    Player unwrapped = wrapper.Unwrap();
    Debug.Log(wrapper.name + ", " + unwrapped.GetName());
} catch(WrapperException e) {
    Debug.Log(e.ToString());
}
//output: bob, bob
```

The **PlayerWrapper** can be converted back to a **Player** object through the **Unwrap** function. If the fields of the Wrapper have been improperly set, the wrapper will throw a **WrapperException**.

---

### AttackDAO.cs
**AttackDAO** is the DAO object involved in saving information about the attacks. It holds an array of **AttackWrapper** objects as a public variable to satisfy the **JsonUtility** requirement. This object is used to both save and load information from an **AttackManager**. In order to save attack data, the following code must be run from **AttackManager**. The Save function will return true if the data has been successfully saved and false otherwise.

```cs
AttackDAO dao = new AttackDAO();
bool success = dao.Save(this);
```
Similarly, attack data is loaded using the following code which must be run from the **AttackManager**.  The Load function will return true if the data has been successfully loaded and false otherwise.
```cs
AttackDAO dao = new AttackDAO();
bool success = dao.Load(this);
```

---

### DataCenterDAO.cs
**DataCenterDAO** is the DAO object involved in saving information about the data centers. It holds an array of **DataCenterWrapper** objects as a public variable to satisfy the **JsonUtility** requirement. This object is used to both save and load information from an **DataCenterManager**. In order to save data center data, the following code must be run from **DataCenterManager**. The Save function will return true if the data has been successfully saved and false otherwise.

```cs
DataCenterDAO dao = new DataCenterDAO();
bool success = dao.Save(this);
```
Similarly, data center data is loaded using the following code which must be run from the **DataCenterManager**.  The Load function will return true if the data has been successfully loaded and false otherwise.
```cs
DataCenterDAO dao = new DataCenterDAO();
bool success = dao.Load(this);
```

---

### GameDAO
**GameDAO** is teh DAO object involved in saving infromation about the game. It holds fields for the number of players, the turn player and the turn number. These fields are public to satisfy the **JsonUtility** requirement. The following two functions do not take a parameter as it is saving and loading from static data stored in the **GameManager**. The Save function will return true if the data has been successfully saved and false otherwise.

```cs
GameDAO dao = new GameDAO();
bool success = dao.Save();
```

Similarly, game data is loaded using the following code. The Load function will return true if the data has been successfully loaded and false otherwise.

```cs
GameDAO dao = new GameDAO();
bool success = dao.Load();
```

---

### MalwareDAO
**MalwareDAO** is the DAO object involved in saving information about the malware. It holds an array of **MalwareWrapper** objects as a public variable to satisfy the **JsonUtility** requirement. This object is used to both save and load information from an **MalwareManager**. In order to save malware data, the following code must be run from **MalwareManager**. The Save function will return true if the data has been successfully saved and false otherwise.

```cs
MalwareDAO dao = new MalwareDAO();
bool success = dao.Save(this);
```

Similarly, malware data is loaded using the following code which must be run from the **MalwareManager**.  The Load function will return true if the data has been successfully loaded and false otherwise.

```cs
MalwareDAO dao = new MalwareDAO();
bool success = dao.Load(this);
```

---

### NotificationDAO
**NotificationDAO** is the DAO object involved in saving information about the notification. It holds an array of **NotificationWrapper** objects as a public variable to satisfy the **JsonUtility** requirement. This object is used to both save and load information from an **NotificationManager**. In order to save notification data, the following code must be run from **NotificationManager**. The Save function will return true if the data has been successfully saved and false otherwise.

```cs
NotificationDAO dao = new NotificationDAO();
bool success = dao.Save(this);
```

Similarly, notification data is loaded using the following code which must be run from the **NotificationManager**.  The Load function will return true if the data has been successfully loaded and false otherwise.

```cs
MalwareDAO dao = new MalwareDAO();
bool success = dao.Load(this);
```

---

### PlayerDAO
**PlayerDAO** is the DAO object involved in saving information about the notification. It holds an array of **PlayerWrapper** objects as a public variable to satisfy the **JsonUtility** requirement. This object is used to both save and load information from an **NotificationManager**. In order to save player data, the following code must be run from **PlayerManager**. The Save function will return true if the data has been successfully saved and false otherwise.

```cs
PlayerDAO dao = new PlayerDAO();
bool success = dao.Save(this);
```

Similarly, player data is loaded using the following code which must be run from the **PlayerManager**.  The Load function will return true if the data has been successfully loaded and false otherwise.

```cs
PlayerDAO dao = new PlayerDAO();
bool success = dao.Load(this);
```

---

## Objects
The following objects mainly contain a collection of fields as a way to store data and does not perform an extensive amount of work on these values.
### Attack.cs
The Attack Object represents an attack made by a player against a data center. 
Attack implements Workable.

An attack is complete when the work resources is greater than or equal to the work required.

```cs
Attack attack = new Attack(101);
Debug.Log("Work Done: " + attack.GetWorkResources());
Debug.Log("Work Required: " + attack.GetWorkRequired());
Debug.Log("The Attack is Complete? " + attack.IsComplete());
/* output:
    WorkDone: 0
    WorkRequired: 100
    The Attack is Complete? false
*/
```

To clear all information from an attack object, run the Reset command which will set the malware id, work resources and exploit to zero and remove the objective and delivery method

```cs
Attack attack = new Attack(101);
attack.SetDelivery("manual");
attack.Reset();
Debug.Log("Delivery Method Exists: " + (attack.GetDelivery() == "").ToString());
//output: Delivery Method Exist: false
```

---

### DataCenter.cs
The DataCenter Object represents the collection of data associated with a specific data center. \
DataCenter implements Workable.

To Increment one of the attributes of a data center, run the IncrementAttribute function with the name of the attribute as the parameter.'
The following are the valid attributes for incrementation:
- *"emailFiltering"* : Email Filtering Level [0, 5]
- *"dlp"* : Data Loss Prevension Level [0, 5]
- *"hiddenStructure"* : Hidden Structure Level [0, 5]
- *"encryption"* : Data Encryption Level [0, 5]
- *"ids"* : Intrusion Detection System Level [0, 5]
- *"ips"* : Intrusion Prevention System Level [0, 5]

```cs
DataCenter dc = new DataCenter();
dc.IncrementAttribute("encryption");
dc.IncrementAttribute("encryption");
Debug.Log(dc.GetEncryption());
//output: 2
```

---

### Email.cs
The Email Object represents the data associated with a specific email. This is a subclass of **Notification**. \
The Email class adds an associated DataCenter and Attack object. This class is used for handling phishing attacks. \
Normally each player will receive one email per data center they control.

---

### Goal.cs
The Goal object represents one specific goal of the game. Goals make up a directional acyclic graph where the player must complete previous goals before moving onto the next one. \
Each goal holds a list of strings representing what is unlocked when the goal is completed by the player.

Goal implements Workable.

---

### Malware.cs
The Malware object represents one specific piece of malware in the game. 

Each piece of malware holds a number of different attributes which combine to determine the success rate of an attack. These attributes are:
- Stealth: Determines how stealthy the attack will be where more stealthy malware will make for an attack with a higher success rate.
- Speed: Decreases the time spent in the attack process and slightly increases the success rate for the attack
- Size: Malware with a greater size will be more easily spotted and stopped by the target data center's defences, but decreases the work required to complete the malware
- Intrusion: Increases the amount of spoils earned through the attack using this malware, but slightly decreases the success rate of the attack.

Malware implements Workable.

---

### Notification.cs
The Notification Object represents a notification given to a player. This obejct is simply to notify the player of the events that took place regarding their attacks, malware, and data centers. Each notification has a title, a body, and a player it is associated with.

An example of this would be to notify the player that their attack against another player was successful and list the amount of money stolen.

---

### Player.cs
The Player Object represents a specific player in the game. This object holds:
- Money: the amount of money available to the player.
- Available Resources: The amount of resources that the player can draw from to perform work on any Workable object.
- Total amount of resources: The Total amount of resources that the player can draw from including the resources that are currently being used on the production of a Workable object.

---

### Workable.cs
The Workable Interface represents any object that work can be performed on. A Workable object requires a way to perform work and has to hold information on the work done and what is being worked on.

### Scene Managers



## Util
## Window Managers


