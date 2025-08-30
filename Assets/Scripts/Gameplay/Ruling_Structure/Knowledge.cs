
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

[JsonObject(MemberSerialization.Fields)]
public class Knowledge
{
    public readonly string name;
    public bool isMastered;

    public int dateOfDiscovery;
    public string returnDescription(Culture culture)
    {
        switch (name)
        {
            case "Fire":
                return "The " + culture.nominativPlural + " know how to master fire for warmth and protection, they teach this knowledge to their young.";
            case "Pflanzenanbau":
                return "The " + culture.nominativPlural + " know that planting seeds allows plants to grow, what use this practice has is yet to be determinted.";
            case "Werkzeuge":
                return "The " + culture.nominativPlural + " have developed basic tools to assist with various tasks.";
            case "Cooking":
                return "The " + culture.nominativPlural + " have learned to prepare food through cooking, improving nutrient absorption.";
            case "Gathering":
                return "The " + culture.nominativPlural + " have the ability to gather edible plants and other resources from their surroundings.";
        }
        return "Error: Description for " + name + " of " + culture.name + " is not available.";
    }

    public bool isKnown;

    public int proficiency = 0;

    public void effect()
    {
        // Implement the effect of the technology here
    }

    public Knowledge(string name)
    {
        this.name = name;
        this.isMastered = false;
        this.isKnown = false;
        this.proficiency = 0;
        this.dateOfDiscovery = -1; // -1 indicates not yet discovered
    }
}

//chariots? military invention