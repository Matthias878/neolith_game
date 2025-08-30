
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

[JsonObject(MemberSerialization.Fields)]

public class Person
{

    public static Controller controller;

    public static void SetController(Controller newController)
    {
        controller = newController;
    }

    public int x; public int y;
    //private int age;

    //public Person Spouse;

    public Person swornChief; //Should not loop

    //public Person engaged_or_betrothed_promised;

    //public List<Person> Siblings;

    //public List<Person> Children;

    //public Person Father;

    //public Person Mother;

    public bool isAlive;

    public string name;

    public Culture culture;

    public String information;

    public Person getClanLeader()
    {
        if (swornChief == null)
        {
            return this;
        }
        else
        {
            return swornChief;
        }
    }

    public Person(string name, Culture culture, Person swornChief = null)
    {
        this.name = name;
        //this.age = 0; // Default age
        this.isAlive = true; // Default state
        this.culture = culture;
        if (swornChief != null && swornChief.swornChief != null)
        {
            throw new ArgumentException("The sworn chief cannot have their own sworn chief.");
        }
        this.swornChief = swornChief;
        this.information = "";
    }

    public void dies()
    {
        isAlive = false;
        information += "Died of natural causes.\n";

    }

}