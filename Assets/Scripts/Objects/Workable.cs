using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Workable {
    public int GetWorkResources();//50/
    public void SetWorkResources(int resources);
    public void AddWorkResources(int resources);

    public int GetWorkRequirement();// /100
    public void SetWorkRequirement(int requirement);

    public string GetWorkTarget();// self
    public void SetWorkTarget(string target);

    public int GetWorkRate();
    public void SetWorkRate(int rate);

    public bool IsComplete();
}
