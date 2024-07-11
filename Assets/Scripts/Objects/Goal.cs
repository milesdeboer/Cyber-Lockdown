using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal
{
    private int gid;
    private List<Goal> parents;
    private List<Goal> children;

    private int workDone = 0;
    private int workRequired;

    private List<string> unlockables;

    public Goal(int gid, int workRequired) {
        this.gid = gid;
        this.workRequired = workRequired;
        parents = new List<Goal>();
        children = new List<Goal>();
    }
    public Goal(int gid, int workRequired, List<string> unlockables) {
        this.gid = gid;
        this.workRequired = workRequired;
        this.unlockables = unlockables;
        parents = new List<Goal>();
        children = new List<Goal>();
    }

    public int GetId() {
        return gid;
    }
    public void SetId(int gid) {
        this.gid = gid;
    }

    public List<Goal> GetParents() {
        return parents;
    }
    public void AddParent(Goal parent) {
        parents.Add(parent);
        if (!parent.GetChildren().Contains(this)) {
            parent.AddChild(this);
        }
    }

    public List<Goal> GetChildren() {
        return children;
    }
    public void AddChild(Goal child) {
        children.Add(child);
        if (!child.GetParents().Contains(this)) {
            child.AddParent(this);
        }
    }

    public int GetWorkDone() {
        return workDone;
    }
    public void DoWork(int work) {
        workDone += work;
    }

    public int GetWorkRequired() {
        return workRequired;
    }

    public bool IsDone() {
        return workDone >= workRequired;
    }

    public List<string> GetUnlockables() {
        return unlockables;
    }

    public void AddUnlockable(string unlockable) {
        this.unlockables.Add(unlockable);
    }

    public bool HasUnlockable(string unlockable) {
        return unlockables.Contains(unlockable);
    }
}
