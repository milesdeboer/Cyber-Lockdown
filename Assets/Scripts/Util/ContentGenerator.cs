using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ContentGenerator
{
    //SAMPLE ADDRESS: johnsmith@support.google.com
    private string[] subDomains = {
        "supportteam",
        "careteam",
        "helpdesk",
        "feedback",
        "complaints",
        "solutions",
        "assist",
        "techsupport",
        "secure",
        "admin",

        "assistance",
        "notifications",
        "tech",
        "service",
        "care",
        "account",
        "support",
        "contact",
        "info",
        "help"
    };

    private string[][] names = new string[][]{
        new string[]{
            "james",
            "michael",
            "robert",
            "john",
            "david",
            "william",
            "richard",
            "joseph",
            "charles",
            "alan",
            "mary",
            "patricia",
            "jennifer",
            "linda",
            "elizabeth",
            "barbara",
            "susan",
            "jessica",
            "karen",
            "ada"
        },
        new string[]{
            "smith",
            "johnson",
            "williams",
            "jones",
            "brown",
            "davis",
            "miller",
            "wilson",
            "moore",
            "taylor",
            "anderson",
            "thomas",
            "jackson",
            "white",
            "harris",
            "martin",
            "thompson",
            "turing",
            "babbage",
            "lovelace"
        }
    };

    private string[] suffix = new string[] {
        "@aol",
        "@hotmail",
        "@yahoo",
        "@gmail",
        "-official",
        "-email",
        "-com",
        "-secure",
        "-portal",
        "-web"
    };

    private string[] TLD = new string[] {
        ".online",
        ".space",
        ".xyz",
        ".info",
        ".biz",
        ".site",
        ".top",
        ".net",
        ".tech",
        ".club",
        ".org",
        ".co"
    };

    //not included letters: qrypdghkxcb
    private Dictionary<char, char> swapDictionary = new Dictionary<char, char>{
        {'a', '4'},
        {'o', '0'},
        {'e', '3'},
        {'l', '1'},
        {'s', '5'},
        {'b', '6'},
        {'i', 'l'},
        {'u', 'v'},
        {'v', 'u'},
        {'t', 'f'},
        {'f', 't'},
        {'m', 'w'},
        {'w', 'm'},
        {'j', 'i'},
        {'z', 's'},
        {'n', 'u'}
    };

    /// <summary>
    /// Generates the text values for a phish email.
    /// </summary>
    /// <param name="companyName">The name of the company the email is addressed to.</param>
    /// <param name="stealth">The stealth attribute of the associated attack [0, 100].</param>
    /// <returns>
    /// A string array containing:
    /// - Value 0: email address
    /// - Value 1: email body
    /// </returns>
    public string[] GeneratePhish(string companyName, int stealth) {
        int rand = UnityEngine.Random.Range(0,5);
        string[] email = GenerateEmail(companyName);

        switch(rand) {
            // Character Substitution - johnsmith@support.g0ogle.com
            case 0:
                email[0] = SubstituteCharacter(email[0], stealth);
                break;

            // Character Swap - johnsmith@support.googel.com
            case 1:
                email[0] = SwapCharacter(email[0], stealth);
                break;

            // Suffix Addition - johnsmith@support.googleemail.com
            case 2:
                email[0] = AddSuffix(email[0], stealth);
                break;

            // Sub Domain - johnsmith@google.help.com
            case 3:
                email[0] = email[0].Split("@", 2)[0] + "@" + companyName + "." + GetInvalidSubDomain(stealth) + ".com";
                break;

            // TLD Change - johnsmith@google.help.org
            case 4:
                email[0] = ChangeTLD(email[0], stealth);
                break;

            // Default
            default:
                break;
        }

        return email;
    } 

    /// <summary>
    /// Generates the text values for a valid email.
    /// </summary>
    /// <param name="companyName">The name of the company the email is addressed to.</param>
    /// <returns>
    /// A string array containing:
    /// - Value 0: email address
    /// - Value 1: email body
    /// </returns>
    public string[] GenerateEmail(string companyName) {
        string[] email = new string[2];

        email[0] = GenerateName() + "@" + GetValidSubDomain() + "." + companyName + ".com";

        email[1] = GenerateEmailBody();

        return email;
    }

    /// <summary>
    /// Generates a random email body
    /// </summary>
    /// <returns>String holding the text to the body of an email.</returns>
    private string GenerateEmailBody() {
        return null;
    }

    /// <summary>
    /// Generates a random name
    /// </summary>
    /// <returns>String holding the text to a name</returns>
    public string GenerateName() {
        string first = names[0][UnityEngine.Random.Range(0,20)];
        string last = names[1][UnityEngine.Random.Range(0,20)];
        return first + "." + last;
    }

    /// <summary>
    /// gets a random valid sub domain
    /// </summary>
    /// <returns>The sub domain (string) at an index in [10, 20]</returns>
    private string GetValidSubDomain() {
        int idx = UnityEngine.Random.Range(10,20);
        return subDomains[idx];
    }

    /// <summary>
    /// gets a random sub domain based on the stealth of the attack.
    /// </summary>
    /// <param name="stealth">The stealth attribute of the attack.</param>
    /// <returns>The sub domain (string) at an index in [0, 20]</returns>
    private string GetInvalidSubDomain(int stealth) {
        int rand = UnityEngine.Random.Range(0, 20);
        int idx = (rand + stealth / 5) / 2;
        return subDomains[idx];
    }

    /// <summary>
    /// Substitutes one character in the domain of an email with another.
    /// </summary>
    /// <param name="address">The original email address.</param>
    /// <param name="stealth">The stealth attribute of the attack.</param>
    /// <returns>The altered email address (string).</returns>
    private string SubstituteCharacter(string address, int stealth) {
        char[] domain = address.Split("@", 2)[1].Split(".", 3)[1].ToCharArray();
        int limit = stealth / 25 + 1;

        int i = 0;
        for (int l = stealth / 25 + 1; l > 0; i++) {
            if (swapDictionary.ContainsKey(domain[i])) {
                domain[i] = swapDictionary[domain[i]];
                    l--;
            }
        }

        //      johnsmith                   @       support                                     .       g0ogle                  .com
        return  address.Split("@", 2)[0] +  "@" +   address.Split("@", 2)[1].Split(".", 3)[0] + "." +   new string(domain) +    ".com";
    }

    /// <summary>
    /// Swaps one character in the domain of an email with another.
    /// </summary>
    /// <param name="address">The original email address.</param>
    /// <param name="stealth">The stealth attribute of the attack.</param>
    /// <returns>The altered email address (string).</returns>
    private string SwapCharacter(string address, int stealth) {
        string domain = "." + address.Split("@", 2)[1].Split(".", 3)[1];
        string original = domain;

        for (int i = 0; i < stealth / 25 + 1 || domain == original; i++) {
            int idx = UnityEngine.Random.Range(0, domain.Length-2);
            char[] chars = domain.ToCharArray();
            (chars[idx], chars[idx+1]) = (chars[idx+1], chars[idx]);
            domain = new string(chars);
        }
        //      johnsmith                   @       support                                     .googel     .com
        return  address.Split("@", 2)[0] +  "@" +   address.Split("@", 2)[1].Split(".", 3)[0] + domain +    ".com";
    }

    /// <summary>
    /// Adds a random suffix to the given email address.
    /// </summary>
    /// <param name="address">The original email address.</param>
    /// <param name="stealth">The stealth attribute of the attack.</param>
    /// <returns>The altered email address (string).</returns>
    private string AddSuffix(string address, int stealth) {
        int idx = (UnityEngine.Random.Range(0, 10) + stealth / 10) / 2;
        string suffix = this.suffix[idx];
        if (suffix[0] == '@') {
            return address.Split("@", 2)[1].Split(".", 3)[0] + "." + address.Split("@", 2)[1].Split(".", 3)[1] + suffix + ".com";
        } else {
            return address.Split(".", 4)[0] + address.Split(".", 4)[1] + "." + address.Split(".", 4)[2] + suffix + ".com";
        }
    }

    /// <summary>
    /// Changes the TLD of the given email address.
    /// </summary>
    /// <param name="address">The original email address.</param>
    /// <param name="stealth">The stealth attribute of the attack.</param>
    /// <returns>The altered email address (string).</returns>
    private string ChangeTLD(string address, int stealth) {
        int idx = (stealth * 6 / 100) + UnityEngine.Random.Range(0, 12) / 2;
        return address.Remove(address.Length-5) + TLD[idx];
    }
}
