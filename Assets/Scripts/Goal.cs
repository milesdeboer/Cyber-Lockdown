using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal
{
    private List<Goal> parents;
    private List<Goal> children;

    private int workDone = 0;
    private int workRequired;

    public Goal(int workRequired) {
        this.workRequired = workRequired;
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
}
