public class Person
{
    private int age;

    private bool isAlive;

    private string name;

    public Culture culture;

    public Person(string name)
    {
        this.name = name;
        this.age = 0; // Default age
        this.isAlive = true; // Default state
        this.culture = new Culture("Goth"); // Initialize with a default culture
    }
}