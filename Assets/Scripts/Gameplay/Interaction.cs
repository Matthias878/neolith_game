public class Interaction
{
    public Interactions interaction;
    public int id;
    public static int nextId = 0;

    public Person sender;
    public Person intendedRecipient;

    public Interaction(Person sender, Person intendedRecipient)
    {
        this.sender = sender;
        this.intendedRecipient = intendedRecipient;
        this.id = nextId++;
    }
}