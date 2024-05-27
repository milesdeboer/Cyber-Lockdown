using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    private int id = -1;

    private int target = -1;

    private Malware malware;
    private string objective;
    private string delivery;
    private int exploit;

    public Attack(int id) {
        this.id = id;
    }

    /**
     *  Returns the Attack Identification Number.
     *  @returns {int} - The identification number of the attack.
     */
    public int GetId() {
        return id;
    }

    /**
     *  Returns the target data center number.
     *  @returns {int} - The identification number of the data center
     */
    public int GetTarget() {
        return target;
    }

    /**
     *  Sets the target data center number
     *  @param {int} target - The identification number of the data center.
     */
    public void SetTarget(int target) {
        this.target = target;
    }

    /**
     *  Returns the Malware associated with the attack.
     *  @returns {Malware} - The Malware associated with the attack.
     */
    public Malware GetMalware() {
        return malware;
    }

    /**
     *  Sets the Malware associated with the attack.
     *  @param {Malware} - The Malware associated with the attack.
     */
    public void SetMalware(Malware malware) {
        this.malware = malware;
    }

    /**
     *  Returns the Objective for the attack.
     *  @returns {string} - The name of the objective for the attack.
     */
    public string GetObjective() {
        return objective;
    }

    /**
     *  Sets the Objective for the attack.
     *  @param {string} objective - The name o fthe objective for the attack.
     */
    public void SetObjective(string objective) {
        this.objective = objective;
    }

    /**
     *  Returns the Delivery method for the attack.
     *  @returns {string} - The name of the delivery method for the attack.
     */
    public string GetDelivery() {
        return delivery;
    }

    /**
     *  Sets the Delivery method for the attack.
     *  @param {string} delivery - The name of the delivery method for the attack.
     */
    public void SetDelivery(string delivery) {
        this.delivery = delivery;
    }

    /**
     *  Returns the quality of exploit used in the attack. 
     *  (0 - not used, 1 - recon used, 2 - vulnerability used).
     *  @returns {int} - The quality of exploit used
     */
    public int GetExploit() {
        return exploit;
    }

    /**
     *  Sets the quality of exploit used in the attack.
     *  (0 - not used, 1 - recon used, 2 - vulnerability used).
     *  @param {int} - The quality of exploit used.
     */
    public void SetExploit(int exploit) {
        this.exploit = exploit;
    }
}
