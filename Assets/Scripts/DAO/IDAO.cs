using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDAO
{
    public bool Save(ISavable manager);
    public bool Load(ISavable manager);

    public bool Erase();
}
